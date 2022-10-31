using Application.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application.Extensions
{
    public static class StringExtension
    {
        private const string CouncilTaxPattern = "^7{2}[0-9]{7}$";

 
        public static bool IsCouncilTax(this string value)
        {
            return Regex.IsMatch(value, CouncilTaxPattern);
        }

        public static string SetFundCode(this string accountReference, ImportFileModel importFileModel)
        {
            if (accountReference.IsCouncilTax())
            {
                return importFileModel.CouncilTaxFundCode;
            }
            return importFileModel.SuspenseFundCode;
        }

        public static string SetVatCode(this string fundcode, ImportFileModel importFileModel)
        {
            if (fundcode == importFileModel.SuspenseFundCode)
            {
                return importFileModel.SuspenseVatCode;
            }
            return importFileModel.VatCode;
        }

        public static string SetAccountReference(this IEnumerable<string> row)
        {
            var accountReference = row.ElementAt(5).Trim();
            accountReference = accountReference.Replace(" ", "");
            return accountReference;
        }

        public static decimal SetAmount(this IEnumerable<string> row)
        {
            decimal amount;
            if (row.ElementAt(12).Trim() == "-")
            {
                amount = decimal.Parse(row.ElementAt(11).Trim()) / 100 * -1;
            }
            else
            {
                amount = decimal.Parse(row.ElementAt(11).Trim()) / 100;
            }
            return amount;
        }
    }
}
