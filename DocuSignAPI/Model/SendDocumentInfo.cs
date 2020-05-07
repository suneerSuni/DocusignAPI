using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillTheDoc.Model
{
    public class SendDocumentInfo
    {
        public string File_Sign { get; set; }
        public string FileType { get; set; }
        public List<SignerInfo> SignerDetails { get; set; }

    }

    public class SignerInfo
    {
        public string ReciName { get; set; }
        public string ReciEmail { get; set; }
        public string ReciId { get; set; }
        public List<SignatureDetails> SignaturePosition { get; set; }
    }
    public class SignatureDetails
    {
        public string SignaturePos_X { get; set; }
        public string SignaturePos_Y { get; set; }
        public string SignaturePage { get; set; }
    }

}
