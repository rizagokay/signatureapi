using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mechsoft.ESign.Library.Validation
{
    public class ConfigFactory
    {
        public static SignatureConfig GetConfig(string PolicyPath = "certval-policy.xml", string LicensePath = "license.xml", string CertificateRepositoryPath = "SertifikaDeposu.xml")
        {
            return new SignatureConfig() { LisansXmlPath = LicensePath, PolicyXmlPath = PolicyPath, SertifikaDeposuPath = CertificateRepositoryPath };
        }
    }
}
