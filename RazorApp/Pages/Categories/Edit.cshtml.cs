using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorApp.Data;
using RazorApp.Models;

namespace RazorApp.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        [BindProperty]
        public Category Category { get; set; }
        public EditModel(AppDbContext context)
        {
            _context = context;
        }
        public void OnGet(int id)
        {
            Category = _context.Categories.Find( id);
            
        }
        public IActionResult OnPost() {
        
            _context.Categories.Update(Category);
            _context.SaveChanges();
            TempData["Success"] = "Category Updated Successfully";

            return RedirectToPage("index");
        }
    }
}
