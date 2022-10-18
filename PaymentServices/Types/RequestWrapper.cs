using System;
namespace PaymentServices.Types
{
    public class RequestWrapper
    {
        public RequestWrapper(Account account, MakePaymentRequest request)
        {
            Account = account;
            Request = request;
        }

        public Account Account { get; private set; }

        public MakePaymentRequest Request { get; set; }
    }
}