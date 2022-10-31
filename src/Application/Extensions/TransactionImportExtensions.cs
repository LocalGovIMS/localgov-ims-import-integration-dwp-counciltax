using Application.Commands;
using LocalGovImsApiClient.Model;
using System.Collections.Generic;

namespace Application.Extensions
{
    public static class TransactionImportExtensions
    {
        public static void Initialise(this TransactionImportModel item, ImportFileModel importFileModel)
        {
            item.ImportTypeId = importFileModel.TransactionImportTypeId;
            item.Rows = new List<ProcessedTransactionModel>();
            item.Errors = new List<string>();
        }
    }
}
