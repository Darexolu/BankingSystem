namespace BankingSystem.Models.ViewModel
{
    public class TransferViewModel
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public decimal TransferAmount { get; set; }
    }
}
