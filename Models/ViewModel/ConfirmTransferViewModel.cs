namespace BankingSystem.Models.ViewModel
{
    public class ConfirmTransferViewModel
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public decimal TransferAmount { get; set; }
    }
}
