using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WebApi.Authentication.Basic.Handler
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock):base(options,logger,encoder,clock)
        {

        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                AuthenticateResult.Fail("No Authentication found");
            try
            {
                var header = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(header.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(':');
                if(credentials[0]=="admin" && credentials[1] == "pass")
                {
                    var claim = new[] { new Claim(ClaimTypes.Role, "admin") , new Claim(ClaimTypes.Name, "Naveen") };
                    var identity = new ClaimsIdentity(claim,Scheme.Name);
                    var principle = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principle, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("Invalid Username and Password");
                }
            }
            catch
            {
                return AuthenticateResult.Fail("An Error Occured");
            }
            
        }
    }
}
