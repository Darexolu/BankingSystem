using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Models.ViewModel
{
    public class DepositViewModel
    {
        [ValidateNever]
        public string UserId { get; set; }
        [Display(Name = "Account Number")]
        [Required(ErrorMessage = "Please enter an account number")]
        public string AccountNumber { get; set; }
        [Display(Name = "User Name")]
        [ValidateNever]
        public string Username { get; set; }

        [Display(Name = "Deposit Amount")]
        [Required(ErrorMessage = "Please enter a deposit amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Deposit amount must be greater than 0")]
        public decimal DepositAmount { get; set; }
        [ValidateNever]
        public List<ApplicationUser> Users { get; set; }
    }
}
