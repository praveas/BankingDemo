using System;
namespace BankingDemo.Models
{
    public class TransactionRequestDto
    {
        public decimal TransactionAmount { get; set; }
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public TranType TransactionType { get; set; } // Enum for various Transaction type
        public DateTime TransactionDate { get; set; }

    }
}
