using System.ComponentModel.DataAnnotations;

namespace Delux.Models
{
    public class Brand
    {
        public int BrandId { get; set; }
        [Display(Name = "Бренд")]
        [Required(ErrorMessage = "Поле 'Название' обязательно для заполнения.")]
        [StringLength(15, ErrorMessage = "Название должно быть не более 15 символов")]
        public string? BrandName { get; set; }
        [Display(Name = "Почта")]
        [Required(ErrorMessage = "Поле 'Email' обязательно для заполнения.")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
        public string Email { get; set; }
        [Display(Name = "Дата основания")]
        [Required(ErrorMessage = "Поле 'Дата основания' обязательно для заполнения.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FoundationDate { get; set; }
    }
}

