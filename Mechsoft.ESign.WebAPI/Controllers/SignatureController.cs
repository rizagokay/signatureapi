using Mechsoft.ESign.Library.Validation;
using Mechsoft.ESign.Library.Validation.Exceptions;
using Sharpbrake.Client;
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

            var airbrake = new AirbrakeNotifier(new AirbrakeConfig
            {
                ProjectId = "146734",
                ProjectKey = "6b2293ec486cbbea517b945202e7c7fc"
            });

            try
            {
                List<SignatureInfo> infos = await signHelper.CheckSignaturesAsync(data);
                return Ok(infos);
            }
            catch (SignatureNotFoundException Ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, Ex.Message));
            }
            catch (FileNotFoundException Ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, Ex.Message));
            }
            catch (Exception Ex)
            {
                await airbrake.NotifyAsync(Ex);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, Ex.Message));
            }
        }

    }
}
