using ECommerce.DataAccess.Repository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using System.Security.Claims;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = await _unitOfWork.OrderHeader.GetAsync(u => u.ID == orderId, include: "ApplicationUser"),
                OrderDetail = await _unitOfWork.OrderDetail.GetAllAsync(u => u.OrderHeaderId == orderId, include: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> UpdateOrderDetail()
        {
            var orderHeaderFromDb = await _unitOfWork.OrderHeader.GetAsync(u => u.ID == OrderVM.OrderHeader.ID);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.ID });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> StartProcessing()
        {
            await _unitOfWork.OrderHeader.UpdateStatusAsync(OrderVM.OrderHeader.ID, SD.StatusInProcess);
            await _unitOfWork.SaveAsync();
            TempData["Success"] = "Order Status Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.ID });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> ShipOrder()
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(u => u.ID == OrderVM.OrderHeader.ID);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            await _unitOfWork.SaveAsync();
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.ID });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> CancelOrder()
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(u => u.ID == OrderVM.OrderHeader.ID);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                await _unitOfWork.OrderHeader.UpdateStatusAsync(orderHeader.ID, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                await _unitOfWork.OrderHeader.UpdateStatusAsync(orderHeader.ID, SD.StatusCancelled, SD.StatusCancelled);
            }

            await _unitOfWork.SaveAsync();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.ID });
        }

        [ActionName("Details")]
        [HttpPost]
        public async Task<IActionResult> Details_PAY_NOW()
        {
            OrderVM.OrderHeader = await _unitOfWork.OrderHeader
                .GetAsync(u => u.ID == OrderVM.OrderHeader.ID, include: "ApplicationUser");
            OrderVM.OrderDetail = await _unitOfWork.OrderDetail
                .GetAllAsync(u => u.OrderHeaderId == OrderVM.OrderHeader.ID, include: "Product");

            //stripe logic
            var domain = $"{Request.Scheme}://{Request.Host.Value}/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.ID}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.ID}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            await _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.ID, session.Id, session.PaymentIntentId);
            await _unitOfWork.SaveAsync();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = await _unitOfWork.OrderHeader.GetAsync(u => u.ID == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                   await _unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                    await _unitOfWork.OrderHeader.UpdateStatusAsync(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    await _unitOfWork.SaveAsync();
                }
            }

            return View(orderHeaderId);
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = await _unitOfWork.OrderHeader.GetAllAsync(include: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeaders = await _unitOfWork.OrderHeader
                    .GetAllAsync(u => u.ApplicationUserID == userId, include: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
            }

            return Json(new { data = objOrderHeaders });
        }

        #endregion
    }
}
