using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Commands.PaymentCommands;

public class MakeBacsPaymentCommand: PayCommand
{
    public MakeBacsPaymentCommand(IDataStore dataStore, Account account, decimal amount) : 
        base(dataStore, account, amount)
    {
    }
}