using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webbsäkerhet_Uppgift_2.Models
{
    public class FileFromUser
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UntrustedName { get; set; }
        public long FileSize { get; set; }

        public byte[] Content { get; set; }

    }
}
