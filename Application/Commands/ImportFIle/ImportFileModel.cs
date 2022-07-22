using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class ImportFileModel
    {
        public string Path { get; set; }

        public string SearchPattern { get; set; }

        public int MaximumProcessableLineLength { get; set; }

        public int TransactionImportTypeId { get; set; }

        public string TransactionImportTypeDescription { get; set; }

        public string PSPReferencePrefix { get; set; }

        public string OfficeCode { get; set; }

        public string MopCode { get; set; }

        public string BenefitOverpaymentFundCode { get; set; }

        public string BusinessRatesFundCode { get; set; }

        public string CouncilTaxFundCode { get; set; }

        public string HousingRentsFundCode { get; set; }

        public string OldCouncilTaxFundCode { get; set; }

        public string OldDebtorFundCode { get; set; }

        public string OldNonDomesticRatesFundCode { get; set; }

        public string ParkingFinesFundCode { get; set; }

        public string SapInvoicesFundCode { get; set; }

        public string SuspenseFundCode { get; set; }

        public string SuspenseVatCode { get; set; }

        public string VatCode { get; set; }

        public string SapDebtorVatCode { get; set; }
    }
    
}