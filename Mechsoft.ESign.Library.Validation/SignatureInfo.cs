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
    }
}
