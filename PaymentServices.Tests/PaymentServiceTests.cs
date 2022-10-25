using System;
using PaymentServices.Commands;
using PaymentServices.Data;
using PaymentServices.Services;
using PaymentServices.Types;

namespace PaymentServices.Tests
{
    public class PaymentServiceTests
    {
        private const string ACCOUNT_NUMBER_FIRST = "123456789";
        private const string ACCOUNT_NUMBER_SECOND = "113456789";
        private const string ACCOUNT_NUMBER_THIRD= "223456789";
        private const string ACCOUNT_NUMBER_CREDITOR= "223456789";

        private const decimal AMMOUNT = 5.5m;

        private const AllowedPaymentSchemes CHAPS = AllowedPaymentSchemes.Chaps;
        private const AllowedPaymentSchemes BACS = AllowedPaymentSchemes.Bacs;
        private const AllowedPaymentSchemes FASTER = AllowedPaymentSchemes.FasterPayments;

        private const PaymentScheme FASTER_SCHEME = PaymentScheme.FasterPayments;
        private const PaymentScheme BACS_SCHEME = PaymentScheme.Bacs;
        private const PaymentScheme CHAPS_SCHEME = PaymentScheme.Chaps;

        private const AccountStatus LIVE_STATUS = AccountStatus.Live;
        private const AccountStatus DISABLED_STATUS = AccountStatus.Disabled;
        private const AccountStatus INBOUND_STATUS = AccountStatus.InboundPaymentsOnly;

        private Account firstAccount;
        private Account secondAccount;
        private Account thirdAccount;

        private AccountDataStore accountDataStore;
        private PaymentService service;

        private CommandInvoker invoker;
        
        [SetUp]
        public void Setup()
        {
            invoker = new CommandInvoker();
            
            firstAccount = new Account()
            {
                AccountNumber = ACCOUNT_NUMBER_FIRST,
                Balance = AMMOUNT,
                AllowedPaymentSchemes = CHAPS,
                Status = LIVE_STATUS
            };

            secondAccount = new Account()
            {
                AccountNumber = ACCOUNT_NUMBER_SECOND,
                Balance = AMMOUNT,
                AllowedPaymentSchemes = FASTER,
                Status = LIVE_STATUS
            };

            thirdAccount = new Account()
            {
                AccountNumber = ACCOUNT_NUMBER_THIRD,
                Balance = AMMOUNT,
                AllowedPaymentSchemes = BACS,
                Status = LIVE_STATUS
            };

            accountDataStore = new AccountDataStore();

            service = new PaymentService(accountDataStore);

            accountDataStore.AddAccountRange(firstAccount, secondAccount, thirdAccount);
        }

        [Test]
        public void Payment_Fails_WhenUserIsNull()
        {
            string dummyNumber = "Dummy";

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = ACCOUNT_NUMBER_CREDITOR,
                DebtorAccountNumber = dummyNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = CHAPS_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void Payment_Fails_WhenPaymentIsNotAllowed()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = ACCOUNT_NUMBER_CREDITOR,
                DebtorAccountNumber = firstAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = FASTER_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        [TestCase(PaymentScheme.Chaps, 5.1d, AccountStatus.Disabled)]
        [TestCase(PaymentScheme.FasterPayments, 55.2d, AccountStatus.Live)]
        public void Payment_Fails_WhenExtraConditionsAreFalse(PaymentScheme scheme, decimal amount, AccountStatus status)
        {
            firstAccount.Status = status;

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = amount,
                CreditorAccountNumber = ACCOUNT_NUMBER_CREDITOR,
                DebtorAccountNumber = firstAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = scheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void Payment_Successful_Bacs()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = ACCOUNT_NUMBER_CREDITOR,
                DebtorAccountNumber = thirdAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = BACS_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void Payment_Successful_Chaps()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = ACCOUNT_NUMBER_CREDITOR,
                DebtorAccountNumber = firstAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = CHAPS_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void Payment_Successful_Faster()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = ACCOUNT_NUMBER_CREDITOR,
                DebtorAccountNumber = secondAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = FASTER_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsTrue(result.Success);
        }

        [Test]
        [TestCase(ACCOUNT_NUMBER_FIRST, CHAPS_SCHEME)]
        [TestCase(ACCOUNT_NUMBER_SECOND, FASTER_SCHEME)]
        [TestCase(ACCOUNT_NUMBER_THIRD, BACS_SCHEME)]
        public void Payment_Withdraws_Correctly(string accountNumber, PaymentScheme scheme)
        {
            decimal expected = 0.5m;

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = 5m,
                CreditorAccountNumber = ACCOUNT_NUMBER_CREDITOR,
                DebtorAccountNumber = accountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = scheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Account account = accountDataStore.GetAccount(accountNumber);

            Assert.AreEqual(expected, account.Balance);
        }
    }
}

