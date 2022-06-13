using System.Collections.Generic;

namespace Application.Commands
{
    public class ImportFileCommandResult
    {
        public List<string> ErrorString { get; set; } = new List<string>();
        public bool Success { get; set; }

        public int FilesProcessed { get; set; }
    }
}
