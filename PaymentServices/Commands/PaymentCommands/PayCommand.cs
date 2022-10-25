using PaymentServices.Commands.Contracts;
using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Commands.PaymentCommands;

public abstract class PayCommand: ICommand
{
    protected readonly IDataStore dataStore;
    protected readonly Account account;
    protected readonly decimal amount;
    
    public PayCommand(IDataStore dataStore, Account account, decimal amount)
    {
        this.dataStore = dataStore;
        this.account = account;
        this.amount = amount;
    }

    public void Execute()
    {
        account.Balance -= amount;
        dataStore.UpdateAccount(account);
    }

    public virtual bool IsExecutable()
    {
        bool isExecutable = false;
        isExecutable = dataStore.GetAccount(account.AccountNumber) != null;
        
        return isExecutable;
    }

    public void Undo()
    {
        account.Balance+=amount;
        dataStore.UpdateAccount(account);
    }
}