using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public decimal Balance { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Address { get; set; }
        /*public string? UserName { get; set; }*/ /*{ get => base.UserName; set => base.UserName = value; }*/
        public ApplicationUser() : base()
        {
            // Initialize any custom properties or set default values
            Balance = 0;
            Transactions = new List<Transaction>();
        }

    }
}
