using System.ComponentModel.DataAnnotations;

namespace Delux.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Display(Name = "Название")]
        [StringLength(15, ErrorMessage = "Название должно быть не более 10 букв")]
        public string? CategoryName { get; set; }
    }
}
