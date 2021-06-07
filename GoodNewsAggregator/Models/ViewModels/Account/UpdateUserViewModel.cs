using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.Account
{
    public class UpdateUserViewModel
    {
        public Guid UserId { get; set; }
        public int Year { get; set; }
        public string PhoneNumber { get; set; }
        public double MinimalRating { get; set; }
    }
}
