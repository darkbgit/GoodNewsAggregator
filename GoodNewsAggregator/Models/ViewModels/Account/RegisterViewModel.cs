using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please fill email field")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage ="Please fill year field")]
        [Range(1980, 2100, ErrorMessage = "Недопустимый год рождения")]
        [Display(Name ="Год рождения")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Please fill password field")]
        [DataType(DataType.Password)]
        [Display(Name ="Пароль")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords doesn't match. Please re entry passwords")]
        [Display(Name = "Подтвердите пароль")]
        public string PasswordConfirmation { get; set; }
    }
}
