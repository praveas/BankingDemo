using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BankingDemo.DB;
using BankingDemo.Models;

namespace BankingDemo.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly MyBankingDbContext _dbContext;// Calling database detail

        public AccountService(MyBankingDbContext dbContext) // contructor 
        {
            _dbContext = dbContext;
        }

        public Account Authenticate(string AccountNumber, string Pin)
        {
            // Authentication
            if (string.IsNullOrEmpty(AccountNumber) || string.IsNullOrEmpty(Pin))
                return null;
            // check if account already exist
            var account = _dbContext.Accounts.SingleOrDefault(x => x.UniqueGeneratedAccountNumber == AccountNumber);
            if (account == null)
                return null;
            // we have a match
            // verify pinHash
            if (!VerifyPinHash(Pin, account.PinHash, account.PinSalt))
                return null;

            // After Aunthentication is passed
            return account;
        }

        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentNullException("Pin");
            // now verifying pin
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for(int i =0; i< computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }
        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            // this will create new accoount
            if (_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("An account already exists with this email");
            // validate pin
            if (!Pin.Equals(ConfirmPin)) throw new ArgumentException("Pins doesnot match", "Pin");

            // After all the validation, creating account
            // encrupting pin first
            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt); // crypto method

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            // adding new account to db
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();

            return account;
        }

        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }


        public void Delete(int Id)
        {
            var account = _dbContext.Accounts.Find(Id);
            if(account != null)
            {
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _dbContext.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _dbContext.Accounts.Where(x => x.UniqueGeneratedAccountNumber == AccountNumber).FirstOrDefault();
            if (account == null) return null;

            return account;
        }

        public Account GetById(int Id)
        {
            var account = _dbContext.Accounts.Where(x => x.ID == Id).FirstOrDefault();
            if (account == null) return null;

            return account;
        }

        public void Update(Account account, string Pin = null)
        {
            //Update

            var accountToBeUpdated = _dbContext.Accounts.Where(x => x.ID == account.ID).SingleOrDefault();
            if (accountToBeUpdated == null) throw new ApplicationException("Account doesnot exist");
            // if it exists, listen for user and change the properties
            if(!string.IsNullOrWhiteSpace(account.Email))
            {
                // changing email
                // check if email is already taken
                if (_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("This email " + account.Email + " already exists");
                // else change email
                accountToBeUpdated.Email = account.Email;
            }

            // actually we want to allow the user to be able to change only Email, phone and Pin
            
            // if it exists, listen for user and change the properties
            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                // changing Phone Number
                // check if Phone is already taken
                if (_dbContext.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber)) throw new ApplicationException("This PhoneNumber " + account.PhoneNumber + " already exists");
                // else change phone number
                accountToBeUpdated.PhoneNumber = account.PhoneNumber;
            }

            
            // if it exists, listen for user and change the properties
            if (!string.IsNullOrWhiteSpace(Pin))
            {
                // changing pin

                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);

                accountToBeUpdated.PinHash = pinHash;
                accountToBeUpdated.PinSalt = pinSalt;

            }
            accountToBeUpdated.DateModified = DateTime.Now;

            // now persist this update to db
            _dbContext.Accounts.Update(accountToBeUpdated);
            _dbContext.SaveChanges();
        }
    }
}
