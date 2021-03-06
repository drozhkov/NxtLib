namespace NxtLib.ServerInfo
{
    public class TransactionType
    {
        public bool CanHaveRecipient { get; set; }
        public bool IsPhasingSafe { get; set; }
        public bool MustHaveRecipient { get; set; }
        public string Name { get; set; }
        public int SubType { get; set; }
        public int Type { get; set; }
    }
}