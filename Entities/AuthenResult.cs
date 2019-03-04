using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    //Class สร้างมาเพื่อรองรับการ convert string to json object  
    public class AuthenResult
{
        //รับค่า httpresponse message จาก api ใช้คลาส รับค่า สำหรับ convert string to json object.
        public string authorization_code { get; set; }
        public string access_token { get; set; }
                    
    }
}
