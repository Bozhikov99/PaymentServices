using PaymentServices.Commands.Contracts;
using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Commands.PaymentCommands;

public class MakeChapsPaymentCommand: PayCommand
{
    public MakeChapsPaymentCommand(IDataStore dataStore, Account account, decimal amount) :
        base(dataStore, account, amount)
    {
    }
    
    public override bool IsExecutable()
    {
        bool isExecutable = account.Status == AccountStatus.Live;

        return isExecutable;
    }
}