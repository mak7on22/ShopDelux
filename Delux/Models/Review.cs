using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Delux.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        [Display(Name = "Отценка")]
        [Required(ErrorMessage = "Поле 'Оценка' обязательно для заполнения.")]
        [Range(0, 5, ErrorMessage = "Оценка должна быть от 0 до 5.")]
        public int Rating { get; set; }
        [Display(Name = "Комментарий")]
        [Required(ErrorMessage = "Поле 'Отзыв' обязательно для заполнения.")]
        [StringLength(3000, ErrorMessage = "Попоробуйте описать немного короче товар!")]
        public string Comment { get; set; }
        [Display(Name = "Автор")]
        [StringLength(10, ErrorMessage = "Название должно быть не более 10 букв")]
        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения.")]
        public string Author { get; set; }
        public int ProductId { get; set; }
        public Product? Products { get; set; }
    }
}
