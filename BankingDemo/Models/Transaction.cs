using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingDemo.Models
{
    [Table("Transaction")]
    public class Transaction
    {
       [Key]
       public int Id { get; set; }
        public string TransactionRefID { get; set; } //this will generate in every instance of this class
        public decimal TransactionAmount { get; set; }
        public TranStatus TransactionStatus { get; set; }// enum 
        public bool IsSuccessful => TransactionStatus.Equals(TranStatus.Success); // this will depends upon the Transaction Status
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public string TransactionParticulars { get; set; }
        public TranType TransactionType { get; set; } // Enum for various Transaction type
        public DateTime TransactionDate { get; set; }

        public Transaction()
        {
            TransactionRefID = $"{Guid.NewGuid().ToString().Replace("-","").Substring(1,27)}"; // use GUID to generate Globally Unique Identifier 
        }
    }

    public enum TranStatus
    {
        Failed,
        Success,
        Error
    }

    public enum TranType
    {
        Deposit,
        Withdrawal,
        Transfer
    }
    
}
