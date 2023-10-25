namespace BankingSystem.Models.ViewModel
{
    public class ConfirmTransferViewModel
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public string SourceName { get; set; }
        public string ReceiverName { get; set; }
        public decimal TransferAmount { get; set; }

    }
}
