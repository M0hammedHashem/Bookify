using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorApp.Data;
using RazorApp.Models;

namespace RazorApp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        public List<Category> Categories { get; set; }
        public IndexModel(AppDbContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
            Categories = _context.Categories.ToList();
        }
    }
}
