﻿using System.ComponentModel.DataAnnotations;

namespace GoodNewsAggregator.Core.WebApi.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}