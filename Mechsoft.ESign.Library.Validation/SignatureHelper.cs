﻿using Mechsoft.ESign.Library.Validation.Exceptions;
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
using tr.gov.tubitak.uekae.esya.api.crypto.alg;

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

                BaseSignedData bs = new BaseSignedData(input);
                Dictionary<string, object> params_ = new Dictionary<string, object>();
                params_[EParameters.P_CERT_VALIDATION_POLICY] = _policy;
                params_[EParameters.P_FORCE_STRICT_REFERENCE_USE] = true;

                SignedDataValidation sdv = new SignedDataValidation();
                SignedDataValidationResult sdvr = sdv.verify(input, params_);

                List<SignatureInfo> signInfo = new List<SignatureInfo>();

                for (int i = 0; i < sdvr.getSDValidationResults().Count; i++)
                {

                    var item = sdvr.getSDValidationResults()[i];
                    var signatureType = bs.getSignerList()[i].getType().name();
                    var certificate = item.getSignerCertificate();
                    var name = certificate.getSubject().getCommonNameAttribute();
                    var identity = certificate.getSubject().getSerialNumberAttribute();
                    var serialnumber = certificate.getSerialNumber().ToString();
                    var issuer = certificate.getIssuer().getCommonNameAttribute();



                    bool isvalid = false;

                    if (item.getSignatureStatus() == Types.Signature_Status.VALID)
                    {
                        isvalid = true;
                    }

                    var info = new SignatureInfo() { Identity = identity, Name = name, IsValid = isvalid, Issuer = issuer, SerialNumber = serialnumber, SignatureType = signatureType };

                    if (certificate.getNotAfter().HasValue)
                    {
                        info.ValidUntil = certificate.getNotAfter().Value;
                    }

                    if (certificate.getNotBefore().HasValue)
                    {
                        info.ValidFrom = certificate.getNotBefore().Value;
                    }

                    var signaturealgorithm = SignatureAlg.fromAlgorithmIdentifier(certificate.getSignatureAlgorithm()).first().getName();
                    var publickeyalgorithm = SignatureAlg.fromAlgorithmIdentifier(certificate.getPublicKeyAlgorithm()).first().getName();

                    var publicKey = certificate.asX509Certificate2().GetPublicKeyString();

                    info.PublicKey = publicKey;
                    info.SignatureAlgorithm = signaturealgorithm;
                    info.PublicKeyAlgorithm = publickeyalgorithm;

                    info.IsTimeStampedCertificate = certificate.isTimeStampingCertificate();
                    info.IsQualifiedCertificate = certificate.isQualifiedCertificate();

                    if (item.getSigningTime().HasValue)
                    {
                        info.SignedOn = item.getSigningTime().Value;
                    }

                    signInfo.Add(info);
                }

                return signInfo;


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
