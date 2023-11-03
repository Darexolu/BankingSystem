﻿using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Models.ViewModel
{
    public class WithdrawViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public decimal Balance { get; set; }
        public decimal WithdrawAmount { get; set; }
        [Required]
        [Display(Name = "PIN")]
        public string PIN { get; set; }


    }
}
