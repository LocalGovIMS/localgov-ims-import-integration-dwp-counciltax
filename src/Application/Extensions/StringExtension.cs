using System.Linq;
using System.Text.RegularExpressions;

namespace Application.Extensions
{
    public static class StringExtension
    {
        private const string CouncilTaxPattern = "^7{2}[0-9]{7}$";
        private const string OldDebtorRefPattern = "^[mMpP]{1}[0-9]{10}$";
        private const string BenefitOverpaymentPattern = "^7{1}[0-9]{7}$";
        private const string BusinessRatesPattern = "^56[0-9]{7}$";
        private const string OldNonDomesticRatePattern = "^5{2}[0-9]{8}$";
        private const string OldCouncilTaxPattern = "^70[0-9]{8}$";
        private const string SapInvoicePattern = "^[93]{1}[0-9]{9}$";
        private const string HousingRentsPattern = "^6{1}[0-9]{10}$";
        private const string ParkingFinesPattern = "^[Bb]{1}[Jj]{1}[0-9]{7}[0-9Aa]$";

        public static bool EqualsAnyOf(this string value, params string[] targets)
        {
            return targets.Any(target => target.Equals(value));
        }

        public static bool IsOldDebtorRef(this string value)
        {
            return Regex.IsMatch(value, OldDebtorRefPattern);
        }

        public static bool IsBenefitOverpayment(this string value)
        {
            return Regex.IsMatch(value, BenefitOverpaymentPattern);
        }
        public static bool IsCouncilTax(this string value)
        {
            return Regex.IsMatch(value, CouncilTaxPattern);
        }
        public static bool IsBusinessRates(this string value)
        {
            return Regex.IsMatch(value, BusinessRatesPattern);
        }
        public static bool IsOldNonDomesticRates(this string value)
        {
            return Regex.IsMatch(value, OldNonDomesticRatePattern);
        }
        public static bool IsOldCouncilTax(this string value)
        {
            return Regex.IsMatch(value, OldCouncilTaxPattern);
        }
        public static bool IsSapInvoice(this string value)
        {
            return Regex.IsMatch(value, SapInvoicePattern);
        }
        public static bool IsHousingRents(this string value)
        {
            return Regex.IsMatch(value, HousingRentsPattern);
        }

        public static bool IsParkingFine(this string value)
        {
            return Regex.IsMatch(value, ParkingFinesPattern);
        }

    }
}
