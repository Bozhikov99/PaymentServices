using System;
using PaymentServices.Types;

namespace PaymentServices.Data.Contracts
{
    public interface IDataStore
    {
        void UpdateAccount(Account account);

        Account GetAccount(string accountNumber);
    }
}

