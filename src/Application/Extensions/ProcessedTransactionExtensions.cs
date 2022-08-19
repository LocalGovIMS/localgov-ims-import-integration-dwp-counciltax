using Application.Commands;
using LocalGovImsApiClient.Model;
using System;

namespace Application.Extensions
{
    public static class ProcessedTransactionExtensions
    {
        public static void SetStaticValues(this ProcessedTransactionModel item, ImportFileModel importFileModel)
        {
            item.OfficeCode = importFileModel.OfficeCode;
            item.EntryDate = DateTime.Now;
            item.VatAmount = 0;
            item.VatRate = 0;
            item.UserCode = 0;
            item.Narrative = importFileModel.PSPReferencePrefix;
            item.MopCode = importFileModel.MopCode;
        }
    }
}

