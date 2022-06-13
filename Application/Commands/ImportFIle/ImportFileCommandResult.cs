using System.Collections.Generic;

namespace Application.Commands
{
    public class ImportFileCommandResult
    {
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public bool ErrorProcessingFile { get; set; }
    }
}
