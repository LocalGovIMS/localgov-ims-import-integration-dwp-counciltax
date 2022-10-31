using Application.Commands;
using LocalGovImsApiClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Extensions
{
    public static class ProcessedTransactionExtensions
    {
        public static void PopulateFields(this ProcessedTransactionModel item, ImportFileModel importFileModel, int lineCount, IEnumerable<string> row, TransactionImportModel transactionImportModel)
        {
            item.OfficeCode = importFileModel.OfficeCode;
            item.EntryDate = DateTime.Now;
            item.VatAmount = 0;
            item.VatRate = 0;
            item.UserCode = 0;
            item.Narrative = importFileModel.PSPReferencePrefix;
            item.MopCode = importFileModel.MopCode;
            item.PspReference = importFileModel.PSPReferencePrefix + DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            item.TransactionDate = DateTime.Parse(row.ElementAt(10).Trim());
            item.AccountReference = row.SetAccountReference();
            item.FundCode = item.AccountReference.SetFundCode(importFileModel);
            item.Amount = row.SetAmount();
            item.VatCode = item.FundCode.SetVatCode(importFileModel);
            transactionImportModel.Rows.Add(item);
        }
    }
}

