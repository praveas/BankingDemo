using System;
using BankingDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingDemo.DB
{
    public class MyBankingDbContext : DbContext
    {
        public MyBankingDbContext(DbContextOptions<MyBankingDbContext> options) : base(options)
        {
        }

        // dbset
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
