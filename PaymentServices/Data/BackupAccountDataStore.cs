using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Data
{
    public class BackupAccountDataStore : IDataStore
    {
        private List<Account> accounts;
        public BackupAccountDataStore()
        {
            accounts = new();
        }

        public Account GetAccount(string accountNumber)
        {
            Account account = accounts
                .FirstOrDefault(a => a.AccountNumber == accountNumber);

            return account;
        }

        public void AddAccount(Account account)
        {
            accounts.Add(account);
        }

        public bool RemoveAccount(Account account)
        {
            return accounts.Remove(account);
        }

        public void AddAccountRange(ICollection<Account> accounts)
        {
            this.accounts.AddRange(accounts);
        }

        public void AddAccountRange(params Account[] accounts)
        {
            this.accounts.AddRange(accounts);
        }
        
        public void RemoveAccountRange(ICollection<Account> accounts)
        {
            foreach (Account a in accounts)
            {
                this.accounts.Remove(a);
            }
        }

        public void UpdateAccount(Account account)
        {
        }

        //public List<Account> Accounts { get; set; }
    }
}