using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Contract;
using CoreWCF;

namespace NetCoreServer
{
    public class EchoService : IEchoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EchoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Echo(string text)
        {
            System.Console.WriteLine($"Received {text} from client!");
            var context = _httpContextAccessor.HttpContext;
        

            System.Console.WriteLine($"Request Path: {context.Request.Path}");
            System.Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
            return text;
        }

        public string ComplexEcho(EchoMessage text)
        {
            System.Console.WriteLine($"Received {text.Text} from client!");
            return text.Text;
        }

        public string FailEcho(string text)
            => throw new FaultException<EchoFault>(new EchoFault() { Text = "WCF Fault OK" }, new FaultReason("FailReason"));

        [AuthorizeRole("CoreWCFGroupAdmin")]
        public string EchoForPermission(string echo)
        {
            return echo;
        }
    }
}
