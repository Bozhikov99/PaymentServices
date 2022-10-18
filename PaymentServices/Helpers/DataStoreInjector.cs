using System;
using System.Configuration;
using PaymentServices.Common;
using PaymentServices.Data;
using PaymentServices.Data.Contracts;

namespace PaymentServices.Helpers
{
    public static class DataStoreInjector
    {
        public static IDataStore GetDataStore()
        {
            var dataStoreType = ConfigurationManager.AppSettings[Constants.DATA_STORE_TYPE];
            IDataStore dataStore = dataStoreType == Constants.DATA_STORE_BACKUP ?
                new BackupAccountDataStore() :
                new AccountDataStore();

            return dataStore;
        }
    }
}

