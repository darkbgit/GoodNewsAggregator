using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.Users
{
    public class EditUserViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public int Year { get; set; }
    }
}
