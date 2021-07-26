using System;
using AutoMapper;
using BankingDemo.Models;

namespace BankingDemo.Profiles
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<RegisterNewAccountModel, Account>();

            CreateMap<UpdateAccountModel, Account>();
            CreateMap<Account, GetAccountModel>();
            CreateMap<TransactionRequestDto, Transaction>();
            // Creating these dto classes, just give me momemt
        }
    }
}
