using Microsoft.Extensions.Configuration;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using LocalGovImsApiClient.Model;
using System;
using System.Linq;
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

        private readonly ImportFileCommandResult _result = new();
        private readonly ImportFileModel _importFileModel = new();
        private readonly TransactionImportModel _transactionImportModel = new();
        private string file;

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
            GetConfigurationDetails();

            GetFile();

            try
            {
                await ProcessFile(cancellationToken);
            }
            catch (Exception exception)
            {
                _result.Errors.Add("Error processing file, " + exception.Message);
            }

            return _result;
        }

        private void GetConfigurationDetails()
        {
            _importFileModel.GetConfigValues(_configuration);
        }

        private void GetFile()
        {
            var _files = _fileSystem.Directory.GetFiles(_importFileModel.Path, _importFileModel.SearchPattern);
            file = _files.FirstOrDefault();
        }

        private async Task ProcessFile(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(file))
            {
                Prepare();

                ReadFile(file);

                DeleteFile();

                if (_result.Success)
                {
                    await PostFileAsync();
                }
            }
        }

        private void Prepare()
        {
            _transactionImportModel.Initialise(_importFileModel);
            var archiveFileName = _importFileModel.ArchivePath + DateTime.Now.ToString("yyyyMMddhhmm") + _fileSystem.Path.GetFileName(file);
            _fileSystem.File.Copy(file, archiveFileName, true);
        }

        private void ReadFile(string file)
        {
            int lineCount = 0;
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                using var package = new ExcelPackage(new FileInfo(file));
                var myWorksheet = package.Workbook.Worksheets.First(); //select sheet here
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;

                for (int rowNum = 2; rowNum <= totalRows; rowNum++) //selet starting row here
                {
                    lineCount++;
                    var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns].Select(c => c.Value == null ? string.Empty : c.Value.ToString());
                    var transaction = new ProcessedTransactionModel();
                    transaction.PopulateFields(_importFileModel, lineCount, row, _transactionImportModel);
                }
            }
            catch (Exception exception)
            {
                _result.Errors.Add("Threw an error on line " + lineCount + " - " + exception.Message);
            }
        }

        private void DeleteFile()
        {
            try
            {
                _fileSystem.File.Delete(file);
            }
            catch (Exception exception)
            {
                _result.Errors.Add("Error deleting the file " + exception.Message);
            }
        }

        private async Task PostFileAsync()
        {
            _transactionImportModel.NumberOfRows = _transactionImportModel.Rows?.Count() ?? 0;
            try
            {
                var result = await _transactionImportApi.TransactionImportPostAsync(_transactionImportModel);
                if (result == null)
                {
                    throw new InvalidOperationException("IMSApi not found to post the transactions");
                }
            }
            catch (Exception exception)
            {
                _result.Errors = _transactionImportModel.Errors;
                _result.Errors.Add(exception.Message);
            }
        }
    }
}
