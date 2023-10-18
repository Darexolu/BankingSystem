namespace BankingSystem.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
    }
    public enum TransactionType
    {
        Deposit,
        Withdraw
    }
}
