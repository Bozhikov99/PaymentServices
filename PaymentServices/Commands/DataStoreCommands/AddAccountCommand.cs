using PaymentServices.Commands.Contracts;
using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Commands.DataStoreCommands;

public class AddAccountCommand: ICommand
{
    private readonly IDataStore dataStore;
    private readonly Account account;
    
    public AddAccountCommand(IDataStore dataStore, Account account)
    {
        this.dataStore = dataStore;
        this.account = account;
    }
    
    public void Execute()
    {
        dataStore.AddAccount(account);
    }

    public bool IsExecutable()
    {
        string accountNumber = account.AccountNumber;
        bool isExecutable = dataStore.GetAccount(accountNumber) == null;
        
        return isExecutable;
    }

    public void Undo()
    {
        dataStore.RemoveAccount(account);
    }
}