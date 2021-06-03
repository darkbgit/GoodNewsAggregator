using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.Users
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Please fill email field")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please fill password field")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage ="Введите год рождения")]
        [Range(1980, 2100, ErrorMessage = "Недопустимый год рождения")]
        [Display(Name = "Год рождения")]
        public int Year { get; set; }
    }
}
