namespace BankingSystem.Models.ViewModel
{
    public class IndexViewModel
    {
        public string FirstName { get; set; }           // User's name
        public string LastName { get; set; }           // User's name
        public decimal Balance { get; set; }            // Account balance
        public string AccountNumber { get; set; }
        public List<Transaction> RecentTransactions { get; set; }
    }
}
