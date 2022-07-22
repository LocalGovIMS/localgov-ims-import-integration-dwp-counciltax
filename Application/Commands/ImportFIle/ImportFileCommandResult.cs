using System.Collections.Generic;
using System.Linq;

namespace Application.Commands
{
    public class ImportFileCommandResult
    {
        public List<string> Errors { get; set; } = new List<string>();
        public bool Success => !Errors.Any();
    }
}
