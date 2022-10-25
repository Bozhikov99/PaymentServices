using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Commands.PaymentCommands;

public class MakeFasterPaymentCommand: PayCommand
{
    public MakeFasterPaymentCommand(IDataStore dataStore, Account account, decimal amount) : 
        base(dataStore, account, amount)
    {
    }

    public override bool IsExecutable()
    {
        bool isExecutable = account.Balance >= amount;
        
        return isExecutable;
    }
}