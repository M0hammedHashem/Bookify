using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorApp.Data;
using RazorApp.Models;

namespace RazorApp.Pages.Categories
{
    [BindProperties]

    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        [BindProperty]

        public Category Category { get; set; }
        public CreateModel(AppDbContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost() { 
        
            _context.Categories.Add(Category);
            _context.SaveChanges();
            TempData["Success"] = "Category Created Successfully";

            return RedirectToPage("Index");
        }
        
    }
}
