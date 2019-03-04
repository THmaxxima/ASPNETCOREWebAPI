namespace WebApi.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public double AuthorizationTokenExpires { get;  set; }
        public double AccessTokenExpires { get; set; }

    }
}