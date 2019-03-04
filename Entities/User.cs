using System;

namespace WebApi.Entities
{
    public class User
    {
        public Guid rowid { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string authorization_code { get; set; }
        public string authorization_exp_date { get; set; }
        public string access_token { get; set; }
        public string accesstoken_exp_date { get;set;}
        public string grant_type { get; set; }

    }
}