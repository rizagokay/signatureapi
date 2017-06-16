using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mechsoft.ESign.Library.Validation.Exceptions
{
    public class SignatureNotFoundException : Exception
    {
        public SignatureNotFoundException(string message) : base(message)
        {
        }

    }
}
