namespace BankingSystem.Models.ViewModel
{
    public class IndexViewModel
    {
        //public string UserName { get; set; }           // User's name
        public decimal Balance { get; set; }            // Account balance
        public List<Transaction> RecentTransactions { get; set; }
    }
}
