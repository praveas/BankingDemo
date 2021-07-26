using System;
using System.Collections.Generic;
using BankingDemo.Models;

namespace BankingDemo.Services
{
    public interface IAccountService
    {
        Account Authenticate(String AccountNumber, string Pin);
        IEnumerable<Account> GetAllAccounts();
        Account Create(Account account, string Pin, string ConfirmPin);
        void Update(Account account, string Pin = null);
        void Delete(int Id);
        Account GetById(int id);

        Account GetByAccountNumber(string AccountNumber);

    }
}
