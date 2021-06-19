using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.Users
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [Display(Name = "Год рождения")]
        public int Year { get; set; }

        [Display(Name = "Минимальный рейтинг")]
        public double MinimalRating { get; set; }

        [Display(Name = "Роли")]
        public IEnumerable<string> Roles { get; set; }
    }
}
