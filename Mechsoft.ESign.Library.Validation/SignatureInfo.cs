using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mechsoft.ESign.Library.Validation
{
    public class SignatureInfo
    {
        public string Name { get; set; }
        public string Identity { get; set; }
        public bool IsValid { get; set; }
        public string Issuer { get; set; }
        public string SerialNumber { get; set; }
        public string SignatureType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string PublicKeyAlgorithm { get; set; }
        public string SignatureAlgorithm { get; set; }
        public string PublicKey { get; set; }
        public bool IsTimeStampedCertificate { get; set; }
        public bool IsQualifiedCertificate { get; set; }
        public DateTime? SignedOn { get; set; }
    }
}
