using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_App
{
    public class UpdateData
    {
        public MicrosoftUpdatePackageIdentity UpdateID { get; set; }
        public int RevisionNumber { get; set; }
        public string? DefaultPropertiesLanguage { get; set; }
        public string? UpdateType { get; set; }
        public long MaxDownloadSize { get; set; }
        public long MinDownloadSize { get; set; }
        public string? PublicationState { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid PublisherID { get; set; }
        public string? Title { get; set; }

    }
}
