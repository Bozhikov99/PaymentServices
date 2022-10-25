using PaymentServices.Commands.Contracts;
using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Commands.DataStoreCommands;

public class AddAccountRangeCommand: ICommand
{
    private readonly IDataStore dataStore;
    private readonly ICollection<Account> accounts;

    public AddAccountRangeCommand(IDataStore dataStore, ICollection<Account> accounts)
    {
        this.accounts = accounts;
        this.dataStore = dataStore;
    }
    
    public void Execute()
    {
        dataStore.AddAccountRange(accounts);
    }

    public bool IsExecutable()
    {
        bool isExecutable = true;
        
        foreach (Account a in accounts)
        {
            string accountNumber = a.AccountNumber;
            
            if (dataStore.GetAccount(accountNumber) == null)
            {
                isExecutable = false;
                break;
            }
        }
        
        return isExecutable;
    }

    public void Undo()
    {
        dataStore.RemoveAccountRange(accounts);
    }
}