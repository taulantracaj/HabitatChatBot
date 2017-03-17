using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Alexa.SkillsKit;

namespace Alexa.Endpoint.Controllers
{
    public class AlexaController : ApiController
    {
        // GET: Alexa
        [Route("alexa/endava")]
        [HttpPost]
        public HttpResponseMessage SampleSession()
        {
            var speechlet = new EndavaSessionSpeechlet();
            return speechlet.GetResponse(Request);
        }
    }
}