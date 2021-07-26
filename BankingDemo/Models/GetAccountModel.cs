using System;
using System.ComponentModel.DataAnnotations;

namespace BankingDemo.Models
{
    public class GetAccountModel
    {
        [Key]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CurrentAccountBalance { get; set; }
        public AccountType AccountType { get; set; }
        // This will be an enum to show, if A/C tobe created as "current" or "savings"
        public string UniqueGeneratedAccountNumber { get; set; }
        
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
