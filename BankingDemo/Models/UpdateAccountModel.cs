using System;
using System.ComponentModel.DataAnnotations;

namespace BankingDemo.Models
{
    public class UpdateAccountModel
    {
        [Key]
        public int ID { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        
        public DateTime DateModified { get; set; }
        // adding regular expression
        [Required]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin must not be more than 4 digits")]// it should be 4-digit string
        public string Pin { get; set; }

        [Required]
        [Compare("Pin", ErrorMessage = "Pins do not match")]
        public string ConfirmPin { get; set; }
       
    }
}
