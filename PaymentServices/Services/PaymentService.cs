using System.Configuration;
using System.Linq.Expressions;
using System.Security.Principal;
using PaymentServices.Common;
using PaymentServices.Data;
using PaymentServices.Data.Contracts;
using PaymentServices.Types;

namespace PaymentServices.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDataStore dataStore;

        private static readonly Func<Account, bool> IsChapsValid = (Account account) =>
        {
            if (account.Status != AccountStatus.Live)
            {
                return false;
            }

            return true;
        };

        private static readonly Func<RequestWrapper, bool> IsFasterValid = (RequestWrapper wrapper) =>
        {
            decimal balance = wrapper.Account
                .Balance;

            decimal required = wrapper.Request
                .Amount;

            if (balance < required)
            {
                return false;
            }

            return true;
        };

        private static readonly Dictionary<PaymentScheme, Func<RequestWrapper, bool>> operations = new()
        {
            [PaymentScheme.Chaps] = (RequestWrapper wrapper) => IsChapsValid(wrapper.Account),
            [PaymentScheme.FasterPayments] = (RequestWrapper wrapper) => IsFasterValid(wrapper),
            [PaymentScheme.Bacs] = (RequestWrapper wrapper) => { return true; }
        };

        public PaymentService(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var result = new MakePaymentResult()
            {
                Success = true
            };

            Account account = dataStore.GetAccount(request.DebtorAccountNumber);
            PaymentScheme paymentScheme = request.PaymentScheme;
            string paymentKey = Enum.GetName<PaymentScheme>(paymentScheme);
            AllowedPaymentSchemes allowedPaymentScheme = Enum.Parse<AllowedPaymentSchemes>(paymentKey);

            RequestWrapper wrapper = new RequestWrapper(account, request);

            if (account == null ||
                !account.AllowedPaymentSchemes.HasFlag(allowedPaymentScheme) ||
                !operations[paymentScheme](wrapper))
            {
                result.Success = false;
                return result;
            }

            WithdrawBalance(account, request.Amount);

            return result;
        }



        private void WithdrawBalance(Account account, decimal amount)
        {
            account.Balance -= amount;

            dataStore.UpdateAccount(account);
        }
    }
}