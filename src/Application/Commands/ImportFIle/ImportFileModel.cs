

namespace Application.Commands
{
    public class ImportFileModel
    {
        public string Path { get; set; }

        public string SearchPattern { get; set; }

        public string ArchivePath { get; set; }

        public int TransactionImportTypeId { get; set; }

        public string TransactionImportTypeDescription { get; set; }

        public string PSPReferencePrefix { get; set; }

        public string OfficeCode { get; set; }

        public string MopCode { get; set; }

        public string CouncilTaxFundCode { get; set; }

        public string SuspenseFundCode { get; set; }

        public string SuspenseVatCode { get; set; }

        public string VatCode { get; set; }
    }
}