using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Delux.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        [Display(Name = "Дата заказа")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Ф.И.О.")]
        [Required(ErrorMessage = "Обязательное поле,заполните его!")]
        [StringLength(30, ErrorMessage = "Название должно быть не более 10 букв")]
        public string? NameUser { get; set; }
        [Display(Name = "Адрес")]
        [Required(ErrorMessage = "Обязательное поле,заполните его!")]
        public string? Address { get; set; }
        [StringLength(15, ErrorMessage = "Номер телефона должен быть не более 15 цифр!")]
        [Display(Name = "Номер телефона")]
        [Required(ErrorMessage = "Обязательное поле,заполните его!")]
        public string? ContactPhone { get; set; }
        [Display(Name = "Почтовый адрес")]
        [Required(ErrorMessage = "Обязательное поле,заполните его!")]
        public string? Email { get; set; }
        //Внешний ключ на таблицу Product
        public int ProductId { get; set; }
        public Product? Products { get; set; }

        public Order() { OrderDate = DateTime.Now; }
    }
}
