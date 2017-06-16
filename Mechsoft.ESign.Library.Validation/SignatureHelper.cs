using Mechsoft.ESign.Library.Validation.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using tr.gov.tubitak.uekae.esya.api.common.util;

namespace Mechsoft.ESign.Library.Validation
{
    public class SignatureHelper : ISignatureHelper, IDisposable
    {
        private SignatureConfig _config;

        private ValidationPolicy _policy;

        public SignatureHelper(SignatureConfig config)
        {
            _config = config;

            SetLicense();
            SetPolicy();
        }

        private void SetLicense()
        {
            using (Stream license = new FileStream(_config.LisansXmlPath, FileMode.Open, FileAccess.Read))
            {
                LicenseUtil.setLicenseXml(license);
            }
        }

        private void SetPolicy()
        {
            using (var File = new FileStream(_config.PolicyXmlPath, FileMode.Open))
            {
                this._policy = PolicyReader.readValidationPolicy(File);
            }
          
            
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["storepath"] = _config.SertifikaDeposuPath;
            _policy.bulmaPolitikasiAl().addTrustedCertificateFinder("tr.gov.tubitak.uekae.esya.api.certificate.validation.find.certificate.trusted.TrustedCertificateFinderFromXml",
                    parameters);
            _policy.bulmaPolitikasiAl().addCertificateFinder("tr.gov.tubitak.uekae.esya.api.certificate.validation.find.certificate.CertificateFinderFromXml", parameters);

        }

        public bool IsSignedData(byte[] input)
        {
            try
            {
                new BaseSignedData(input);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public Task<List<SignatureInfo>> CheckSignaturesAsync(byte[] input)
        {

            return Task.Factory.StartNew(() =>
            {

                if (!IsSignedData(input))
                {
                    throw new SignatureNotFoundException("İmza bilgisi bulunamdı.");
                }

                try
                {
                    bool allvalid = false;

                    Dictionary<string, object> params_ = new Dictionary<string, object>();
                    params_[EParameters.P_CERT_VALIDATION_POLICY] = _policy;

                    //if (externalContent != null)
                    //    params_[EParameters.P_EXTERNAL_CONTENT] = externalContent;

                    params_[EParameters.P_FORCE_STRICT_REFERENCE_USE] = true;

                    SignedDataValidation sdv = new SignedDataValidation();
                    SignedDataValidationResult sdvr = sdv.verify(input, params_);

                    if (sdvr.getSDStatus() == SignedData_Status.ALL_VALID)
                    {

                        allvalid = true;
                    }
                    else
                    {
                        allvalid = false;
                    }

                    List<SignatureInfo> signInfo = new List<SignatureInfo>();

                    foreach (var item in sdvr.getSDValidationResults())
                    {
                        var certificate = item.getSignerCertificate();
                        var name = certificate.getSubject().getCommonNameAttribute();
                        var identity = certificate.getSubject().getSerialNumberAttribute();

                        bool isvalid = false;

                        if (item.getSignatureStatus() == Types.Signature_Status.VALID)
                        {
                            isvalid = true;
                        }

                        signInfo.Add(new SignatureInfo() { Identity = identity, Name = name, IsValid = isvalid });
                    }

                    return signInfo;

                    //  return allvalid;

                }
                catch (FileNotFoundException e)
                {
                    throw new SystemException("Policy file could not be found.", e);
                }


            });


        }

        public List<SignatureInfo> CheckSignature(byte[] input)
        {
            return
                CheckSignaturesAsync(input)
                .Result;
        }

        public void Dispose()
        {
           
        }
    }

    public class SignatureConfig
    {
        public string PolicyXmlPath { get; set; }
        public string SertifikaDeposuPath { get; set; }
        public string LisansXmlPath { get; set; }
    }
}
