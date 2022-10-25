using PaymentServices.Types;

namespace PaymentServices.Data.Contracts
{
    public interface IDataStore
    {
        void UpdateAccount(Account account);

        Account GetAccount(string accountNumber);

        void AddAccount(Account account);

        bool RemoveAccount(Account account);

        void AddAccountRange(ICollection<Account> accounts);
        
        void AddAccountRange(params Account[] accounts);
        
        void RemoveAccountRange(ICollection<Account> accounts);
    }
}

