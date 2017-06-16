using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mechsoft.ESign.Library.Validation
{
    public interface ISignatureHelper
    {
        bool IsSignedData(byte[] input);
        Task<List<SignatureInfo>> CheckSignaturesAsync(byte[] input);
        List<SignatureInfo> CheckSignature(byte[] input);
    }
}
