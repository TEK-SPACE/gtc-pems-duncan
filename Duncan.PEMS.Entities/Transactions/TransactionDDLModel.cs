namespace Duncan.PEMS.Entities.Transactions
{
    public class TransactionDDLModel
    {
        public TransactionDDLModel(string value, string text)
        {
            DDLText = text;
            DDLValue = value;
        }

        public TransactionDDLModel(string text)
        {
            DDLText = text;
            DDLValue = text;
        }

        public string DDLValue { get; set; }
        public string DDLText { get; set; }
    }
}