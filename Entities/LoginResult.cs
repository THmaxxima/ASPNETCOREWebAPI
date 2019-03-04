using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    public class LoginResult
    {
        public string AccessToken { get; set; }

        public LoginResult(string accessToken)
        {
            AccessToken = accessToken;
        }
    }
}
