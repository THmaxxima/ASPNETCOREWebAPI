using System;

namespace WebApi.Entities
{
    public class Tokens
    {
        //[KEY]
        public int ID { get; set; }
        //public Guid ID { get; set; } = Guid.NewGuid();
        public int UserID { get; set; }
        public int TokenTypeID { get; set; }
        public string Token { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}