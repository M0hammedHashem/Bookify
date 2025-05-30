using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RazorApp.Models
{
    public class Category
    {
        [Key]

        public int Id { get; set; }
        [Required]
        [DisplayName("Category Name")]

        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 50, ErrorMessage = "Display Order must be 1-50")]

        public int DisplayOrder { get; set; }
    }
}
