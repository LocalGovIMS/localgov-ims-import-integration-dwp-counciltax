using Microsoft.Extensions.Configuration;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using LocalGovImsApiClient.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using OfficeOpenXml;

namespace Application.Commands
{
    public class ImportFileCommand : IRequest<ImportFileCommandResult>
    {

    }

    public class ImportFileCommandHandler : IRequestHandler<ImportFileCommand, ImportFileCommandResult>
    {
        private readonly IConfiguration _configuration;

        private ImportFileCommandResult _result = new ImportFileCommandResult();
        private string _fileLocation;
        private string _fileNameFormat;
        private IEnumerable<string> _files;
        private List<ProcessedTransactionModel> _transactions = new List<ProcessedTransactionModel>();
        private Random _randomGenerator = new Random();

        public ImportFileCommandHandler(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ImportFileCommandResult> Handle(ImportFileCommand request, CancellationToken cancellationToken)
        {
            
            GetFileConfiDetails();

            GetFiles();

            // TODO: need to decide if processing more than one file at a time
            foreach (var file in _files)
            {
                ReadFile(file);

                PostFile();

                _result.FilesProcessed++;
            }

            //TODO: how is file moved so it isnt processed again

            return _result;
        }

        private void GetFileConfiDetails()
        {
            //TODO: is there any validation needed on checking have the right file eg checking name? extension?
            _fileLocation = _configuration.GetValue<string>("FileDetails:Location");
            _fileNameFormat = _configuration.GetValue<string>("FileDetails:FileNameFormat");
        }

        private void GetFiles()
        {
            _files = Directory.EnumerateFiles(_fileLocation, _fileNameFormat);
        }

        private void ReadFile(string file)
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
                    var transaction = new ProcessedTransactionModel()
                    {
                        OfficeCode = "99",
                        EntryDate = DateTime.Now,
                        VatAmount = 0,
                        VatRate = 0,
                        UserCode = 0,
                        Narrative = "DWP",
                        MopCode = "27"
                    };
                    try
                    {
                        lineCount++;
                        //        transaction.Reference = lineCount + GetNextReferenceId(_randomGenerator);
                        transaction.InternalReference = transaction.Reference;
                        transaction.PspReference = transaction.Reference;
                        transaction.TransactionDate = DateTime.Parse(row.ElementAt(10).Trim());
                        transaction.AccountReference = SetAccountReference(row);
                        transaction.FundCode = SetFundCode(transaction.AccountReference);
                        transaction.Amount = SetAmount(row);
                        transaction.VatCode = SetVatCode(transaction.FundCode);
                        //       transaction.BatchReference = batchref;  //TODO: is batch reference going

                        _transactions.Add(transaction);
                    }
                    catch (Exception exception)
                    {
                        _result.ErrorString.Add("Threw an error on line " + lineCount + " - " + exception.Message);
                        _result.Success = false;
                    }
                }
            }
        }

        private void PostFile()
        {
            _result.Success = true;
        }

        private string SetAccountReference(IEnumerable<string> row)
        {
            var accountReference = row.ElementAt(5).Trim();
            accountReference = accountReference.Replace(" ", "");
            return accountReference;
        }

        private string SetFundCode(string accountReference)
        {
            if (accountReference.StartsWith("77") && accountReference.Length == 9 && accountReference.All(char.IsDigit))
            {
                return "23";
            }
            else
            {
                return "SP";
            }
        }

        private string SetVatCode(string fundcode)
        {
            if (fundcode == "SP")
            {
                return "M0";
            }
            else
            {
                return "N0";
            } 
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

        //internal static string GetNextReferenceId(Random randomGenerator)
        //{
        //   var hash = new Hashids("BMBC", 9);
        //    return hash.Encode(DateTime.Now.Millisecond, randomGenerator.Next(999999));
        //}

    }
}
