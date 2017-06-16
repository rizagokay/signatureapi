using Mechsoft.ESign.Library.Validation;
using Mechsoft.ESign.Library.Validation.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mechsoft.ESign.WebAPI.Controllers
{
    [RoutePrefix("REST")]
    public class SignatureController : ApiController
    {
        ISignatureHelper signHelper;

        public SignatureController()
        {

            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/Config");

            SignatureConfig config = ConfigFactory.GetConfig(Path.Combine(path, "certval-policy.xml"), Path.Combine(path, "lisans.xml"), Path.Combine(path, "SertifikaDeposu.xml"));
            signHelper = new SignatureHelper(config);
        }

        [HttpPost]
        [Route("GetSignatures")]
        public async Task<IHttpActionResult> GetSignatures(byte[] data)
        {
            try
            {
                List<SignatureInfo> infos = await signHelper.CheckSignaturesAsync(data);
                return Ok(infos);
            }
            catch (SignatureNotFoundException ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
            }
        }

    }
}
