using Application.Commands;
using Application.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Application.UnitTests.Extensions.String
{
    public class StringTests
    {
        [Fact]
        public void Check_for_a_council_tax_account()
        {
            // Arrange
            string accountReference = "771234567";

            // Act
            var result = accountReference.IsCouncilTax();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Should_fail_check_for_a_council_tax_account()
        {
            // Arrange
            string accountReference = "711234567";

            // Act
            var result = accountReference.IsCouncilTax();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Set_account_reference_to_remove_spaces()
        {
            // Arrange
            IEnumerable<string> field = new string[] { "10219", "446", "23/05/2022", "10:27:34", "28812", "7733 10328", "2", "104127", "NN111111", "15/04/2022", "12/05/2022", "835", "+", "2481467", "+", "1547", "0" };

            // Act
            var accountReference = field.SetAccountReference();

            // Assert
            accountReference.Should().Be("773310328");
        }

        [Fact]
        public void Throws_error_if_account_field_not_available()
        {
            // Arrange
            IEnumerable<string> line = new string[] { "10219", "446", "23/05/2022", "10:27:34", "28812"};

            // Act

            // Assert
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => line.SetAmount());
        }


        [Fact]
        public void Set_council_tax_fund_code()
        {
            // Arrange
            string field = "770778675";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("23");
        }


        [Fact]
        public void return_suspense_if_fund_not_known()
        {
            // Arrange
            string field = "11045382";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("SP");
        }

        [Fact]
        public void Set_vat_code_to_default_for_council_tax()
        {
            // Arrange
            string field = "23";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.SuspenseFundCode = "SP";
            importFileModel.VatCode = "N0";
            importFileModel.SuspenseVatCode = "M0";

            // Act
            var vatCode = field.SetVatCode(importFileModel);

            // Assert
            vatCode.Should().Be("N0");
        }


        [Fact]
        public void Set_suspense_vat_code()
        {
            // Arrange
            string field = "SP";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.SuspenseFundCode = "SP";
            importFileModel.VatCode = "N0";
            importFileModel.SuspenseVatCode = "M0";

            // Act
            var vatCode = field.SetVatCode(importFileModel);

            // Assert
            vatCode.Should().Be("M0");
        }

        [Fact]
        public void Set_amount_for_positive_amount()
        {
            // Arrange
            IEnumerable<string> line = new string[] { "10219", "446", "23/05/2022", "10:27:34", "28812", "7733 10328", "2", "104127", "NN111111", "15/04/2022", "12/05/2022", "835", "+", "2481467", "+", "1547", "0" };

            // Act
            var result = line.SetAmount();

            // Assert
            result.Should().Be((decimal)8.35);
        }

        [Fact]
        public void Set_amount_for_negative_amount()
        {
            // Arrange
            IEnumerable<string> line = new string[] { "10219", "446", "23/05/2022", "10:27:34", "28812", "7733 10328", "2", "104127", "NN111111", "15/04/2022", "12/05/2022", "835", "-", "2481467", "+", "1547", "0" };

            // Act
            var result = line.SetAmount();

            // Assert
            result.Should().Be((decimal)-8.35);
        }


        [Fact]
        public void Returns_error_for_bad_amount()
        {
            // Arrange
            IEnumerable<string> line = new string[] { "10219", "446", "23/05/2022", "10:27:34", "28812", "7733 10328", "2", "104127", "NN111111", "15/04/2022", "12/05/2022"};

            // Act

            // Assert
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => line.SetAmount());
        }
    }
}
