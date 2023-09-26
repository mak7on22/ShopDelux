using System.ComponentModel.DataAnnotations;

namespace Delux.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Обязательное поле,заполните его!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string? Name { get; set; }
        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Обязательное поле, заполните его!")]
        [Range(50, double.MaxValue, ErrorMessage = "Цена должна быть не менее 50 единиц")]
        public decimal Price { get; set; }
        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Обязательное поле,заполните его!")]
        [StringLength(1024, ErrorMessage = "Попоробуйте описать немного короче товар!")]
        public string? Description { get; set; }
        [Display(Name = "Изображение")]
        [Required(ErrorMessage = "Обязательное поле,заполните его!")]
        public string? ImageUrl { get; set; }
        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }
        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }
        public int CategoryId { get; set; }
        [Display(Name = "Категория")]
        public Category? Categories { get; set; }
        public int BrandId { get; set; }
        [Display(Name = "Бренд")]
        public Brand? Brands { get; set; }
        public int ReviewId { get; set; }
        [Display(Name = "Отзывы")]
        public List<Review>? Reviews { get; set; }
        [Display(Name = "")]
        public double AverageRating
        {
            get
            {
                if (Reviews != null && Reviews.Any())
                    return Reviews.Average(r => r.Rating);
                return 0;
            }
        }
    }
}

