using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.Users
{
    public class ChangePasswordViewModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string NewPassword { get; set; }
    }
}
