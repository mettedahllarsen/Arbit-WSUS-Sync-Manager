using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_App
{
    public class FileUpdateData
    {
        public string? FileName { get; set; }
        public long Size { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string? PatchingType { get; set; }
        public string? SHA1Hash { get; set; }
        public string? SHA256Hash { get; set; }
        public string? MUUrl { get; set; }
        public string? USSUrl { get; set; }
    }
}
