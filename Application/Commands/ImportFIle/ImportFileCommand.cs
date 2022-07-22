using Microsoft.Extensions.Configuration;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using LocalGovImsApiClient.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO.Abstractions;
using Application.Extensions;

namespace Application.Commands
{
    public class ImportFileCommand : IRequest<ImportFileCommandResult>
    {

    }

    public class ImportFileCommandHandler : IRequestHandler<ImportFileCommand, ImportFileCommandResult>
    {
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;
        private readonly LocalGovImsApiClient.Api.ITransactionImportApi _transactionImportApi;

        private ImportFileCommandResult _result = new();
        private ImportFileModel _importFileModel = new();
        private TransactionImportModel _transactionImportModel = new();
        private IEnumerable<string> _files;

        public ImportFileCommandHandler(
            IConfiguration configuration,
            IFileSystem file,
            LocalGovImsApiClient.Api.ITransactionImportApi transactionImportApi)
        {
            _configuration = configuration;
            _fileSystem = file;
            _transactionImportApi = transactionImportApi;
        }

        public async Task<ImportFileCommandResult> Handle(ImportFileCommand request, CancellationToken cancellationToken)
        {
            GetTransactionDefaultValues();

            GetFileConfiDetails();

            GetFiles();

            await ProcessFiles(cancellationToken);

            //TODO: how is file moved so it isnt processed again

            SetUpResult();

            return _result;
        }

        private void GetTransactionDefaultValues()
        {
            _importFileModel.CouncilTaxFundCode = _configuration.GetValue<string>("TransactionDefaultValues:CouncilTaxFundCode");
            _importFileModel.SuspenseFundCode = _configuration.GetValue<string>("TransactionDefaultValues:SuspenseFundCode");
            _importFileModel.MopCode = _configuration.GetValue<string>("TransactionDefaultValues:MopCode");
            _importFileModel.OfficeCode = _configuration.GetValue<string>("TransactionDefaultValues:OfficeCode");
            _importFileModel.SuspenseVatCode = _configuration.GetValue<string>("TransactionDefaultValues:SuspenseVatCode");
            _importFileModel.VatCode = _configuration.GetValue<string>("TransactionDefaultValues:VatCode");
            _importFileModel.TransactionImportTypeId = _configuration.GetValue<int>("TransactionDefaultValues:TransactionImportTypeId");
            _importFileModel.TransactionImportTypeDescription = _configuration.GetValue<string>("TransactionDefaultValues:TransactionImportTypeDescription");
            _importFileModel.PSPReferencePrefix = _configuration.GetValue<string>("TransactionDefaultValues:PSPReferencePrefix");
        }

        private void GetFileConfiDetails()
        {
            _importFileModel.Path = _configuration.GetValue<string>("FileDetails:Path");
            _importFileModel.SearchPattern = _configuration.GetValue<string>("FileDetails:SearchPattern");
        }

        private void GetFiles()
        {
            _files = _fileSystem.Directory.GetFiles(_importFileModel.Path, _importFileModel.SearchPattern);
        }

        private async Task ProcessFiles(CancellationToken cancellationToken)
        {
            Prepare();

            // TODO: need to decide if processing more than one file at a time
            foreach (var file in _files)
            {
                await ReadFile(file, cancellationToken);

                await PostFileAsync();
            }
            //TODO: how is file moved so it isnt processed again
        }

        private void Prepare()
        {
            _transactionImportModel.Initialise();
        }

        private Task ReadFile(string file, CancellationToken cancellationToken)
        {
            //TODO: are we loading multiple files or just one at a time?
            int lineCount = 0;
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (var package = new ExcelPackage(new FileInfo(file)))
            {
                var myWorksheet = package.Workbook.Worksheets.First(); //select sheet here
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;

                for (int rowNum = 2; rowNum <= totalRows; rowNum++) //selet starting row here
                {
                    var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns].Select(c => c.Value == null ? string.Empty : c.Value.ToString());
                    var transaction = new ProcessedTransactionModel();
                    transaction.SetStaticValues(_importFileModel);
                    try
                    {
                        lineCount++;
                        transaction.PspReference = _importFileModel.PSPReferencePrefix + DateTime.Now.ToString("yyMMddhhmm") + lineCount;
                        transaction.TransactionDate = DateTime.Parse(row.ElementAt(10).Trim());
                        transaction.AccountReference = SetAccountReference(row);
                        transaction.FundCode = SetFundCode(transaction.AccountReference);
                        transaction.Amount = SetAmount(row);
                        transaction.VatCode = SetVatCode(transaction.FundCode);
                        _transactionImportModel.Rows.Add(transaction);
                    }
                    catch (Exception exception)
                    {
                        _transactionImportModel.Errors.Add("Threw an error on line " + lineCount + " - " + exception.Message);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private async Task PostFileAsync()
        {
            _transactionImportModel.ImportTypeId = _importFileModel.TransactionImportTypeId;
            _transactionImportModel.NumberOfRows = _transactionImportModel.Rows?.Count() ?? 0;
            try
            {
                var result = await _transactionImportApi.TransactionImportPostAsync(_transactionImportModel);
                if (result == null)
                {
                    throw new Exception("IMSApi not found to post the transactions");
                }
            }
            catch (Exception exception)
            {
                _transactionImportModel.Errors.Add(exception.Message);
            }
        }


        private string SetAccountReference(IEnumerable<string> row)
        {
            var accountReference = row.ElementAt(5).Trim();
            accountReference = accountReference.Replace(" ", "");
            return accountReference;
        }

        private string SetFundCode(string accountReference)
        {
            if (accountReference.IsCouncilTax())
            {
                return _importFileModel.CouncilTaxFundCode;
            }
            return _importFileModel.SuspenseFundCode;

        }

        private string SetVatCode(string fundcode)
        {
            if (fundcode == _importFileModel.SuspenseFundCode)
            {
                return _importFileModel.SuspenseVatCode;
            }
            return _importFileModel.VatCode;

        }

        private decimal SetAmount(IEnumerable<string> row)
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

        private void SetUpResult()
        {
            _result.Errors = _transactionImportModel.Errors;
        }
    }
}
