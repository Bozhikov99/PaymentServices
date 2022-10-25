using PaymentServices.Commands.Contracts;
using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Commands.DataStoreCommands;

public class RemoveAccountCommand:ICommand
{
    private readonly IDataStore dataStore;
    private readonly Account account;
    
    public RemoveAccountCommand(IDataStore dataStore, Account account)
    {
        this.dataStore = dataStore;
        this.account = account;
    }
    
    public void Execute()
    {
        dataStore.RemoveAccount(account);
    }

    public bool IsExecutable()
    {
        string accountNumber = account.AccountNumber;
        bool isExecutable = dataStore.GetAccount(accountNumber) != null;
        
        return isExecutable;
    }

    public void Undo()
    {
        dataStore.AddAccount(account);
    }
}