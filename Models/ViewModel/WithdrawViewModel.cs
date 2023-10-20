namespace BankingSystem.Models.ViewModel
{
    public class WithdrawViewModel
    {
        public string UserId { get; set; }

        public decimal Balance { get; set; }
        public decimal WithdrawAmount { get; set; }

    }
}
