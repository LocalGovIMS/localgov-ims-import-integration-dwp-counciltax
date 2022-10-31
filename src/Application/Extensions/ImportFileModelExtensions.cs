using Application.Commands;
using Microsoft.Extensions.Configuration;

namespace Application.Extensions
{
    public static class ImportFileModelExtensions
    {
        public static void GetConfigValues(this ImportFileModel model, IConfiguration configuration)
        {
            model.CouncilTaxFundCode = configuration.GetValue<string>("TransactionDefaultValues:CouncilTaxFundCode");
            model.SuspenseFundCode = configuration.GetValue<string>("TransactionDefaultValues:SuspenseFundCode");
            model.MopCode = configuration.GetValue<string>("TransactionDefaultValues:MopCode");
            model.OfficeCode = configuration.GetValue<string>("TransactionDefaultValues:OfficeCode");
            model.SuspenseVatCode = configuration.GetValue<string>("TransactionDefaultValues:SuspenseVatCode");
            model.VatCode = configuration.GetValue<string>("TransactionDefaultValues:VatCode");
            model.TransactionImportTypeId = configuration.GetValue<int>("TransactionDefaultValues:TransactionImportTypeId");
            model.TransactionImportTypeDescription = configuration.GetValue<string>("TransactionDefaultValues:TransactionImportTypeDescription");
            model.PSPReferencePrefix = configuration.GetValue<string>("TransactionDefaultValues:PSPReferencePrefix");
            model.Path = configuration.GetValue<string>("FileDetails:Path");
            model.SearchPattern = configuration.GetValue<string>("FileDetails:SearchPattern");
            model.ArchivePath = configuration.GetValue<string>("FileDetails:ArchivePath");
        }
    }
}
