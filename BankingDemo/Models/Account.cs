using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankingDemo.Models
{
    [Table("Account")]
    public class Account
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
        // to generate Unique Account Number

        // hash & salt for transaction PIN
        [JsonIgnore]
        public byte[] PinHash { get; set; }
        [JsonIgnore]
        public byte[] PinSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        // Creating Account Number in Constructor
        Random rand = new Random();

        public Account()
        {
            UniqueGeneratedAccountNumber = Convert.ToString((long)Math.Floor(rand.NextDouble() * 9_000_000_000L + 1_000_000_000L));// To generate 10-digit random long double
            // Concatenate FirstName and LastName to get AccountName
            AccountName = $"{FirstName} {LastName}"; 
        }


    }

    public enum AccountType
    {
        Savings, //savings => 0 , current => 1, corporate => 2, goverment => 3
        Current,
        Corporate,
        Government
    }
}
