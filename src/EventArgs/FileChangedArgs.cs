using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield.EventArguments
{
    public enum FileChangeType
    {
        None,
        Added,
        Modified,
        Deleted,
    }

    public class FileChangedArgs
    {
        public FileChangeType ChangeType { get; set; }
        public Guid Id { get; set; }
        public string Path { get; set; } = string.Empty;
    }
}
