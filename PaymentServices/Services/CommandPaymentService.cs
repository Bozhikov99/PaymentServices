using PaymentServices.Commands;
using PaymentServices.Commands.PaymentCommands;
using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Services;

public class CommandPaymentService: IPaymentService
{
    private readonly IDataStore dataStore;
    private readonly CommandInvoker invoker;
    private readonly Dictionary<PaymentScheme, Func<RequestWrapper, bool>> operations;
    
    public CommandPaymentService(IDataStore dataStore, CommandInvoker invoker)
    {
        this.dataStore = dataStore;
        this.invoker = invoker;
        
        //Load the operations
        operations = new()
        {
            [PaymentScheme.Bacs] = (RequestWrapper wrapper) => this.invoker.Invoke(
                new MakeBacsPaymentCommand(dataStore, wrapper.Account, wrapper.Request.Amount)),
            [PaymentScheme.Chaps] = (RequestWrapper wrapper) => this.invoker.Invoke(
                new MakeChapsPaymentCommand(dataStore, wrapper.Account, wrapper.Request.Amount)),
            [PaymentScheme.FasterPayments] = (RequestWrapper wrapper) => this.invoker.Invoke(
                new MakeFasterPaymentCommand(dataStore, wrapper.Account, wrapper.Request.Amount)),
        };
    }
    
    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        var result = new MakePaymentResult()
        {
            Success = true
        };

        Account account = dataStore.GetAccount(request.DebtorAccountNumber);
        PaymentScheme paymentScheme = request.PaymentScheme;
        string paymentKey = Enum.GetName(paymentScheme);
        AllowedPaymentSchemes allowedPaymentScheme = Enum.Parse<AllowedPaymentSchemes>(paymentKey);

        RequestWrapper wrapper = new RequestWrapper(account, request);

        if (account == null ||
            !account.AllowedPaymentSchemes.HasFlag(allowedPaymentScheme) ||
            !operations[paymentScheme](wrapper))
        {
            result.Success = false;
            return result;
        }

        return result;
    }
}