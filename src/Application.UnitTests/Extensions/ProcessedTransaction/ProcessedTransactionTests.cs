using Xunit;
using FluentAssertions;
using System;
using LocalGovImsApiClient.Model;
using Application.Extensions;
using Application.Commands;
using System.Collections.Generic;

namespace Application.UnitTests.Extensions.ProcessedTransaction
{
    public class ProcessedTransactionTests
    {
        [Fact]
        public void Populate_for_good_fields()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                MopCode = "27",
                PSPReferencePrefix = "DWP",
                OfficeCode = "99",
                CouncilTaxFundCode = "23",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SuspenseVatCode = "M0",
                TransactionImportTypeId = 5
            };

            IEnumerable<string> row = new string[] { "10219", "446", "23/05/2022", "10:27:34", "28812", "773310328", "2", "104127", "NN111111", "15/04/2022", "12/05/2022", "835", "+", "2481467", "+", "1547", "0" };
            int lineCount = 5;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);

            // Act
            processedTransaction.PopulateFields(importFileModel, lineCount, row, transactionImportModel );

            // Assert
            processedTransaction.OfficeCode.Should().Be("99");
            processedTransaction.VatAmount.Should().Be(0);
            processedTransaction.VatRate.Should().Be(0);
            processedTransaction.UserCode.Should().Be(0);
            processedTransaction.MopCode.Should().Be("27");
            processedTransaction.PspReference.Should().Be("DWP" + dateTime);
            processedTransaction.TransactionDate.Should().Be(DateTime.Parse("12/05/2022"));
            processedTransaction.AccountReference.Should().Be("773310328");
            processedTransaction.Amount.Should().Be((decimal)8.35);
            processedTransaction.Narrative.Should().Be("DWP");
            processedTransaction.FundCode.Should().Be("23");
            processedTransaction.VatCode.Should().Be("N0");
            transactionImportModel.Rows.Count.Should().Be(1);
        }

        [Fact]
        public void Populate_for_bad_account_reference()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                MopCode = "27",
                PSPReferencePrefix = "DWP",
                OfficeCode = "99",
                CouncilTaxFundCode = "23",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SuspenseVatCode = "M0",
                TransactionImportTypeId = 5
            };

            IEnumerable<string> row = new string[] { "10219", "446", "23/05/2022", "10:27:34", "28812", "77331032", "2", "104127", "NN111111", "15/04/2022", "12/05/2022", "835", "+", "2481467", "+", "1547", "0" };
            int lineCount = 5;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);

            // Act
            processedTransaction.PopulateFields(importFileModel, lineCount, row, transactionImportModel);

            // Assert
            processedTransaction.OfficeCode.Should().Be("99");
            processedTransaction.VatAmount.Should().Be(0);
            processedTransaction.VatRate.Should().Be(0);
            processedTransaction.UserCode.Should().Be(0);
            processedTransaction.MopCode.Should().Be("27");
            processedTransaction.PspReference.Should().Be("DWP" + dateTime);
            processedTransaction.TransactionDate.Should().Be(DateTime.Parse("12/05/2022"));
            processedTransaction.AccountReference.Should().Be("77331032");
            processedTransaction.Amount.Should().Be((decimal)8.35);
            processedTransaction.Narrative.Should().Be("DWP");
            processedTransaction.FundCode.Should().Be("SP");
            processedTransaction.VatCode.Should().Be("M0");
            transactionImportModel.Rows.Count.Should().Be(1);
        }

        [Fact]
        public void catch_error_for_bad_date()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                MopCode = "27",
                PSPReferencePrefix = "DWP",
                OfficeCode = "99",
                CouncilTaxFundCode = "23",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SuspenseVatCode = "M0",
                TransactionImportTypeId = 5
            };
            IEnumerable<string> row = new string[] { "10219", "446", "32/05/2022", "10:27:34", "28812", "773310328", "2", "104127", "NN111111", "15/04/2022", "32/05/2022", "835", "+", "2481467", "+", "1547", "0" };
            int lineCount = 5;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);

            // Act

            // Assert
            Assert.Throws<FormatException>(() => processedTransaction.PopulateFields(importFileModel, lineCount, row, transactionImportModel));
        }

        [Fact]
        public void Catch_error_for_missing_data()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                MopCode = "27",
                PSPReferencePrefix = "DWP",
                OfficeCode = "99",
                CouncilTaxFundCode = "23",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SuspenseVatCode = "M0",
                TransactionImportTypeId = 5
            };
            IEnumerable<string> row = new string[] { "10219", "446", "32/05/2022", "10:27:34", "28812" };
            int lineCount = 5;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);

            // Act

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => processedTransaction.PopulateFields(importFileModel, lineCount, row, transactionImportModel));
        }

        [Fact]
        public void Catch_error_for_bad_amount()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                MopCode = "27",
                PSPReferencePrefix = "DWP",
                OfficeCode = "99",
                CouncilTaxFundCode = "23",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SuspenseVatCode = "M0",
                TransactionImportTypeId = 5
            };
            IEnumerable<string> row = new string[] { "10219", "446", "23/05/2022", "10:27:34", "28812", "773310328", "2", "104127", "NN111111", "15/04/2022", "12/05/2022", "8s35", "+", "2481467", "+", "1547", "0" };
            int lineCount = 5;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);

            // Act

            // Assert
            Assert.Throws<FormatException>(() => processedTransaction.PopulateFields(importFileModel, lineCount, row, transactionImportModel));
        }
    }
}
