using System;
using System.Linq;
using BankingDemo.DB;
using BankingDemo.Models;
using BankingDemo.Services.Interfaces;
using BankingDemo.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BankingDemo.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private MyBankingDbContext _dbContext;
        ILogger<TransactionService> _logger;
        private AppSettings _settings;
        private static string _ourBankSettlementAccount;
        private readonly IAccountService _accountService;

        public TransactionService(MyBankingDbContext dbContext, ILogger<TransactionService> logger, IOptions<AppSettings> settings,
            IAccountService accountService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _settings = settings.Value;
            _ourBankSettlementAccount = _settings.OurBankSettlementAccount;
            _accountService = accountService;

        }


        public Response CreateNewTransaction(Transaction transaction)
        {
            // Create new trasaction
            Response response = new Response();
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully!";
            response.Data = null;

            return response;
        }

        public Response FindTransactionByDate(DateTime date)
        {
            Response response = new Response();
            var transaction = _dbContext.Transactions.Where(x => x.TransactionDate == date).ToList(); // because there are many transaction in a day
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully!";
            response.Data = null;

            return response;
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            // Make deposit
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();


            // First check that user - Account owner is valid,
            // we need authenticate in UserService, so lets inject IUserService here
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null)
            {
                throw new ApplicationException("Invalid Credentials");
            }


            // If validation pass
            try
            {
                // for deposit, our bankSettlementAccount is the source given money to user's account
                sourceAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);
                destinationAccount = _accountService.GetByAccountNumber(AccountNumber);

                // nowlet's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // check if there is update
                if((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful!";
                    response.Data = null;
                }
                else
                {
                    // so transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            // set other props of transaction
            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _ourBankSettlementAccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} " +
                $"TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} " +
                $"ON DATE => {transaction.TransactionDate} " +
                $"FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} " +
                $"TRANSACTION TYPE => {transaction.TransactionType} " +
                $"TRANSACTION STATUS => {transaction.TransactionStatus}";
            // All Done. lets commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;
            // 8792385799 is settlement bank account
        }

        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            // let's implement Transfer
            // Make Transfer...
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();


            // First check that user - Account owner is valid,
            // we need authenticate in UserService, so lets inject IUserService here
            var authUser = _accountService.Authenticate(FromAccount, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid Credentials");


            // If validation pass
            try
            {
                // for deposit, our bankSettlementAccount is the destination sending money from user's account
                sourceAccount = _accountService.GetByAccountNumber(FromAccount);
                destinationAccount = _accountService.GetByAccountNumber(ToAccount);

                // nowlet's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // check if there is update
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful!";
                    response.Data = null;
                }
                else
                {
                    // so transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            // set other props of transaction
            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} " +
                $"TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} " +
                $"ON DATE => {transaction.TransactionDate} " +
                $"FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} " +
                $"TRANSACTION TYPE => {transaction.TransactionType} " +
                $"TRANSACTION STATUS => {transaction.TransactionStatus}";
            // All Done. lets commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;
        }

        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            // let's implement withdrawal
            // Make withdrawal...
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();


            // First check that user - Account owner is valid,
            // we need authenticate in UserService, so lets inject IUserService here
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid Credentials");


            // If validation pass
            try
            {
                // for deposit, our bankSettlementAccount is the destination sending money from user's account
                sourceAccount = _accountService.GetByAccountNumber(AccountNumber);
                destinationAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);

                // nowlet's update their account balances
                if(sourceAccount.CurrentAccountBalance >= Amount) { sourceAccount.CurrentAccountBalance -= Amount; }
             
                destinationAccount.CurrentAccountBalance += Amount;

                // check if there is update
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful!";
                    response.Data = null;
                }
                else
                {
                    // so transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            // set other props of transaction
            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = AccountNumber;
            transaction.TransactionDestinationAccount = _ourBankSettlementAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} " +
                $"TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} " +
                $"ON DATE => {transaction.TransactionDate} " +
                $"FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} " +
                $"TRANSACTION TYPE => {transaction.TransactionType} " +
                $"TRANSACTION STATUS => {transaction.TransactionStatus}";
            // All Done. lets commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;

        }
    }
}
