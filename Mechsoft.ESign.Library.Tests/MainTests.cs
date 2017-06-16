using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mechsoft.ESign.Library.Validation;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;

namespace Mechsoft.ESign.Library.Tests
{
    [TestClass]
    public class MainTests
    {
        [TestMethod]
        public void ValidateSignature()
        {
            var signConfig = new SignatureConfig()
            {
                LisansXmlPath = @"config\lisans.xml",
                PolicyXmlPath = @"config\certval-policy.xml",
                SertifikaDeposuPath = @"config\SertifikaDeposu.xml"

            };

            var signHelper = new SignatureHelper(signConfig);

            byte[] input =
                File.ReadAllBytes(@"c:\temp\ss.pdf");

            List<SignatureInfo> infos;

            if (signHelper.IsSignedData(input))
            {
                infos = signHelper.CheckSignature(input);
            }
        }


        [TestMethod]
        public void ValidateSignatureWithAPI()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:51978/");

            var bytes = File.ReadAllBytes(@"c:\temp\ss.pdf");

            var response = client.PostAsJsonAsync("REST/GetSignatures", bytes).Result;

        }
    }
}
