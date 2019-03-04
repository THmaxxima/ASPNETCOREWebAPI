using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Dtos;
using WebApi.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using AutoMapper.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Dapper;
using System.Linq;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System.Net;
using System.Runtime.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SqlConnectionStringBuilder : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _config;
        public string _userId;
        private DataContext _context;
        private IFormatProvider provider;
        public JwtSecurityToken _APIresToken { get; set; }
        public string _accesstoken { get; set; }
        public Guid user_Id { get; set; }
        DateTime dateValue;
        HttpResponseMessage response;
        public HttpResponseMessage Unauthorized_Msg;
        public DataTable queueResult;

        public SqlConnectionStringBuilder(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            DataContext context,
           IConfiguration config
           )
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _config = config;
            _context = context;
        }


        public static TData ExtecuteProcedureReturnData<TData>(string connString, string procName,
           Func<SqlDataReader, TData> translator,
           params SqlParameter[] parameters)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;
                    if (parameters != null)
                    {
                        sqlCommand.Parameters.AddRange(parameters);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public async Task<HttpResponseMessage> PostWebApiToken(string loginname, string loginPwd,
           string grant_type, string refresh_token, string access_token)
        {
            //
            string X10APIURL = MyHelper.GetX10APIURL();

            dynamic userdynamic = new ExpandoObject();
            userdynamic.username = loginname;
            userdynamic.password = loginPwd;

            dynamic refreshToken = new ExpandoObject();
            refreshToken.authorization_code = refresh_token;
            refreshToken.grant_type = "refresh_access_token";
            dynamic revokeauthorization = new ExpandoObject();
            revokeauthorization.authorization_code = refresh_token;
            revokeauthorization.grant_type = "revoke_authorization_code";
            dynamic revokeaccesstoken = new ExpandoObject();
            revokeaccesstoken.authorization_code = refresh_token;
            revokeaccesstoken.grant_type = "revoke_access_token";
            dynamic verifyauthorizaton = new ExpandoObject();
            verifyauthorizaton.authorization_code = refresh_token;
            verifyauthorizaton.grant_type = "verify_authorizaton_code";
            dynamic verifyaccesstoken = new ExpandoObject();
            verifyaccesstoken.access_token = access_token;
            verifyaccesstoken.grant_type = "verify_access_token";

            using (var client = new HttpClient())
            {
                //**********************************************************************
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("ContentType", "application/json");
                //**********************************************************************
                if (string.IsNullOrWhiteSpace(loginname) == false && string.IsNullOrWhiteSpace(loginPwd) == false)
                {
                    client.BaseAddress = new Uri(X10APIURL + "authenticate");
                    string stringData = JsonConvert.SerializeObject(userdynamic);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    response = client.PostAsync
                  (X10APIURL + "authenticate", contentData).Result;
                    var postTask = response.Content.ReadAsStringAsync().Result;
                    return (response);
                }
                else if (grant_type == "refresh_access_token")
                {
                    client.BaseAddress = new Uri(X10APIURL + "dispatchtoken");
                    string stringData = JsonConvert.SerializeObject(refreshToken);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    var response = client.PostAsync
                   (X10APIURL + "dispatchtoken", contentData).Result;
                    var postTask = response.Content.ReadAsStringAsync().Result;
                    return (response);
                }//
                else if (grant_type == "revoke_access_token")
                {
                    client.BaseAddress = new Uri(X10APIURL + "dispatchtoken");
                    string stringData = JsonConvert.SerializeObject(revokeaccesstoken);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    response = client.PostAsync
                  (X10APIURL + "dispatchtoken", contentData).Result;
                    var postTask = response.Content.ReadAsStringAsync().Result;
                    return (response);
                }
                else if (grant_type == "revoke_authorization_code")
                {
                    client.BaseAddress = new Uri(X10APIURL + "dispatchtoken");
                    string stringData = JsonConvert.SerializeObject(revokeauthorization);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    response = client.PostAsync
                  (X10APIURL + "dispatchtoken", contentData).Result;
                    var postTask = response.Content.ReadAsStringAsync().Result;
                    return (response);
                }
                else if (grant_type == "verify_authorizaton_code")
                {
                    client.BaseAddress = new Uri(X10APIURL + "dispatchtoken");
                    string stringData = JsonConvert.SerializeObject(verifyauthorizaton);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    response = client.PostAsync
                  (X10APIURL + "dispatchtoken", contentData).Result;
                    var postTask = response.Content.ReadAsStringAsync().Result;
                    return (response);
                }
                else if (grant_type == "verify_access_token")
                {
                    client.BaseAddress = new Uri(X10APIURL + "dispatchtoken");
                    string stringData = JsonConvert.SerializeObject(verifyaccesstoken);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    var response = client.PostAsync
                   (X10APIURL + "dispatchtoken", contentData).Result;
                    var postTask = response.Content.ReadAsStringAsync().Result;
                    return response;
                }

            }

            return response;
        }
        //***************************************************************************
        public DateTime convertDatetime(string d)
        {
            string[] dateStrings = { d };
            foreach (string dateString in dateStrings)
            {
                if (DateTime.TryParse(dateString, out dateValue))
                    Console.WriteLine("  Converted '{0}' to {1} ({2}).", dateString,
                                      dateValue, dateValue.Kind);
                else
                    //Console.WriteLine("  Unable to parse '{0}'.", dateString);
                    dateValue = DateTime.MinValue;
                //return BadRequest("Invalid datetime param : {0}", d);

            }
            return dateValue;
        }

        //***************+++ 2.2 Interface No.2 – เรียกดู slot คิวทั้งหมดที่สามารถจองคิวได้ ++**************

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/v1/queues")]
        public async Task<IActionResult> RequestQueuesAsync([FromHeader]string channelId, [FromHeader]string etaDate
            , [FromHeader] string maxDate, [FromHeader]string truckTypeId, [FromHeader] string licensePlate
            , [FromHeader] string provinceId, [FromHeader] string phoneNo, [FromHeader] string remark
            , [FromHeader] string dnNo)
        {
            var strDateError = string.Empty;

            try
            {

                string token = string.Empty;
                JwtSecurityToken jwtSecurityToken;

                string accessToken = MyHelper.GetAuthorizationHeader(Request);
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    return BadRequest("access_token is missing.");
                }
                else
                {
                    _accesstoken = accessToken;
                }

                try
                {
                    //ป้องการสวมรอย Token มั่วมาว่างั้น
                    token = _accesstoken.Replace("Bearer ", " ").Trim().ToString();
                    jwtSecurityToken = new JwtSecurityToken(token);
                    if (DateTime.UtcNow > jwtSecurityToken.ValidTo || jwtSecurityToken.ValidTo == DateTime.MinValue)
                    {
                        return StatusCode(401, "access_token invalid or expired.");
                    }
                    user_Id = new Guid(jwtSecurityToken.Payload["userId"].ToString());
                }
                catch (Exception)
                {
                    return StatusCode(401, "access_token invalid or expired.");
                }
                HttpResponseMessage ResMsg = await PostWebApiToken("", "", "verify_access_token", "", token);

                if (ResMsg.StatusCode != HttpStatusCode.OK)//.ReadAsStringAsync().Result.Contains("access_token invalid or expired."))
                {
                    return StatusCode(401, ResMsg.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            // validate mandatory fields
            DateTime resultEtaDate = DateTime.MinValue;
            DateTime resultMaxDate = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(channelId))
                strDateError += "Missing channelId. " + Environment.NewLine;
            if (string.IsNullOrWhiteSpace(truckTypeId))
                strDateError += "Missing truckTypeId. " + Environment.NewLine;
            if (string.IsNullOrWhiteSpace(licensePlate))
                strDateError += "Missing licensePlate. " + Environment.NewLine;
            if (string.IsNullOrWhiteSpace(etaDate))
            {
                strDateError += "Missing etaDate. " + Environment.NewLine;
            }
            else
            {
                if (!DateTime.TryParse(etaDate, out resultEtaDate))
                {
                    strDateError += "Invalid etaDate. " + Environment.NewLine;
                }
            }

            // validate format maxdate
            if (!string.IsNullOrWhiteSpace(maxDate))
            {
                if (!DateTime.TryParse(maxDate, out resultMaxDate))
                {
                    strDateError += "Invalid maxDate. " + Environment.NewLine;
                }
            }

            // throw error if not valide
            if (strDateError != "")
                return BadRequest(strDateError);

            //++++++++++++++++++++++ +++++Call Proc to get data ++++++++++++++++++++++++++++++
            try
            {
                string connectionString = MyHelper.GetDBConn();

                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    string sQuery = "EXEC [dbo].[proc_SmartQWeb_API_RequestQueues] ";
                    sQuery += "@UserRowID = '" + user_Id + "',";
                    sQuery += "@ChannelID = '" + channelId + "',";
                    sQuery += "@TruckTypeID = '" + truckTypeId + "',";
                    sQuery += "@LicensePlate = '" + licensePlate + "',";
                    sQuery += "@ETADate = '" + resultEtaDate.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    if (resultMaxDate == DateTime.MinValue)
                    {
                        sQuery += "@MaxDate = null, ";
                    }
                    else
                    {
                        sQuery += "@MaxDate = '" + resultMaxDate.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    }
                    sQuery += "@ProvinceID = " + (provinceId.Trim() == "0" || provinceId.Trim() == "" ? "null" : $"{provinceId}") + ",";
                    sQuery += "@PhoneNo = " + (phoneNo.Trim() == "" ? "null" : $"'{phoneNo}'") + ",";
                    sQuery += "@Remark = " + (remark.Trim() == "" ? "null" : $"'{remark}'") + ",";
                    sQuery += "@AllDNNo = " + (dnNo.Trim() == "" ? "null" : $"'{dnNo}'");

                    SqlCommand cmd = new SqlCommand(sQuery, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds, "Data");
                } // end using

                /////******
                // check error from store result
                string errorFromStore = MyHelper.GetErrorFromStoreToApiError(ds);
                if (!string.IsNullOrWhiteSpace(errorFromStore))
                {
                    return BadRequest(errorFromStore);
                }

                // remove message column if success
                DataSet dsResult = MyHelper.RemoveColumnFromStore(ds, new string[] { "Message"});

                // change column name to sencesitive
                Dictionary<string, string> dicOfColumnMapping = new Dictionary<string, string>()
                {
                  { "slotID", "slotId" }
                };
                dsResult = MyHelper.ChangeColumnNameFromMapping(dsResult, dicOfColumnMapping);

                return Ok(dsResult.Tables["Data"]);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //********************** 2.3 Interface No.3 - ยืนยันการจองคิว *************************
        [AllowAnonymous]
        [HttpPost("{slotId}")]
        [Route("/api/v1/queues/{slotId}")]
        public async Task<IActionResult> ConfirmRequestQueues(string slotId)
        {
            string token = string.Empty;

            // check requied Authorization Header
            string accessToken = MyHelper.GetAuthorizationHeader(Request);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return BadRequest("access_token is missing.");
            }
            else
            {
                _accesstoken = accessToken;
            }

            JwtSecurityToken jwtSecurityToken;
            try
            {
                token = _accesstoken.Replace("Bearer ", " ").Trim().ToString();
                jwtSecurityToken = new JwtSecurityToken(token);
                //ป้องการสวมรอย Token มั่วมาว่างั้น
                if (DateTime.UtcNow > jwtSecurityToken.ValidTo || jwtSecurityToken.ValidTo == DateTime.MinValue)
                {
                    return StatusCode(401, "access_token invalid or expired.");
                }
                user_Id = new Guid(jwtSecurityToken.Payload["userId"].ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(401, "access_token invalid or expired.");
            }

            HttpResponseMessage ResMsg = await PostWebApiToken("", "", "verify_access_token", "", token);

            if (ResMsg.StatusCode != HttpStatusCode.OK)//.ReadAsStringAsync().Result.Contains("access_token invalid or expired."))
            {
                return StatusCode(401, "access_token invalid or expired.");
            }

            if (string.IsNullOrWhiteSpace(slotId.ToString()))
                return BadRequest("Slot Id not found");

            //++++++++++++++++++++++ +++++Call Proc to get data ++++++++++++++++++++++++++++++
            DataSet ds = new DataSet();
            string connectionString = MyHelper.GetDBConn();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sQuery = "EXEC [dbo].[proc_SmartQWeb_API_ConfirmRequestQueues] ";
                sQuery += "@UserRowID = '" + user_Id + "',";
                sQuery += "@SlotRowID = '" + slotId + "'";

                SqlCommand cmd = new SqlCommand(sQuery, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds, "Data");
            }

            /////******
            // check error from store result
            string errorFromStore = MyHelper.GetErrorFromStoreToApiError(ds);
            if (!string.IsNullOrWhiteSpace(errorFromStore))
            {
                return BadRequest(errorFromStore);
            }

            // remove message column if success
            DataSet dsResult = MyHelper.RemoveColumnFromStore(ds, new string[] { "Message" });

            // change column name to sencesitive
            Dictionary<string, string> dicOfColumnMapping = new Dictionary<string, string>()
                {
                  { "queueID", "queueId" },
                  { "channelID", "channelId" },
                  { "provinceID", "provinceId" },
                  { "truckTypeID", "truckTypeId" }

                };
            dsResult = MyHelper.ChangeColumnNameFromMapping(dsResult, dicOfColumnMapping);

            return Ok(dsResult.Tables["Data"]);

        }

        //********************** 2.3 Interface No.3 - การจองคิว *************************
        [AllowAnonymous]
        [HttpDelete]
        [Route("/api/v1/queues/{queueId}")]
        public async Task<IActionResult> CancelQueue(string queueId)
        {
            // check requied Authorization Header
            string accessToken = MyHelper.GetAuthorizationHeader(Request);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return BadRequest("access_token is missing.");
            }
            else
            {
                _accesstoken = accessToken;
            }

            string token = string.Empty;
            JwtSecurityToken jwtSecurityToken;
            try
            {
                token = _accesstoken.Replace("Bearer ", " ").Trim().ToString();
                //ป้องการสวมรอย Token มั่วมาว่างั้น
                jwtSecurityToken = new JwtSecurityToken(token);
                if (DateTime.UtcNow > jwtSecurityToken.ValidTo || jwtSecurityToken.ValidTo == DateTime.MinValue)
                {
                    return StatusCode(401, "access_token invalid or expired.");
                }
                user_Id = new Guid(jwtSecurityToken.Payload["userId"].ToString());
            }
            catch (Exception)
            {
                return StatusCode(401, "access_token invalid or expired.");
            }

            HttpResponseMessage ResMsg = await PostWebApiToken("", "", "verify_access_token", "", token);
            if (ResMsg.StatusCode != HttpStatusCode.OK)//.ToString().Contains("access_token invalid or expired."))
            {
                return StatusCode(401, "access_token invalid or expired.");
            }

            if (string.IsNullOrWhiteSpace(queueId.ToString()))
                return BadRequest("QueueId not found.");

            //++++++++++++++++++++++ +++++Call Proc to get data ++++++++++++++++++++++++++++++
            DataSet ds = new DataSet();
            string connectionString = MyHelper.GetDBConn();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sQuery = "EXEC [dbo].[proc_SmartQWeb_API_CancelQueue] ";
                sQuery += "@UserRowID = '" + user_Id + "',";
                sQuery += "@QueueRowID = '" + queueId + "'";

                SqlCommand cmd = new SqlCommand(sQuery, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds, "Data");
            }// end using

            /////******
            // check error from store result
            string errorFromStore = MyHelper.GetErrorFromStoreToApiError(ds);
            if (!string.IsNullOrWhiteSpace(errorFromStore))
            {
                return BadRequest(errorFromStore);
            }

            // remove message column if success
            DataSet dsResult = MyHelper.RemoveColumnFromStore(ds, new string[] { "Message" });

            // change column name to sencesitive
            Dictionary<string, string> dicOfColumnMapping = new Dictionary<string, string>()
                {
                  { "queueID", "queueId" },
                  { "channelID", "channelId" },
                  { "provinceID", "provinceId" },
                  { "truckTypeID", "truckTypeId" },
                  { "bookingID", "bookingId" }
                };
            dsResult = MyHelper.ChangeColumnNameFromMapping(dsResult, dicOfColumnMapping);

            return Ok(dsResult.Tables["Data"]);

        }
        //********************** 2.4 Interface No.4 - ยกเลิกการจองคิว *************************

        //************ 2.5 Interface No.5 - ดูสถานะคิวทั้งหมดที่อยู่ในโรงงานและออกโรงงานไม่เกิน 1 วัน *********
        [AllowAnonymous]
        [HttpGet]
        [Route("/api/v1/queues/status")]
        public async Task<IActionResult> QueueStatus()
        {
            // check requied Authorization Header
            string accessToken = MyHelper.GetAuthorizationHeader(Request);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return BadRequest("access_token is missing.");
            }
            else
            {
                _accesstoken = accessToken;
            }

            string token = string.Empty;
            JwtSecurityToken jwtSecurityToken;
            try
            {
                token = _accesstoken.Replace("Bearer ", " ").Trim();
                //ป้องการสวมรอย Token มั่วมาว่างั้น
                jwtSecurityToken = new JwtSecurityToken(token);
                if (DateTime.UtcNow > jwtSecurityToken.ValidTo || jwtSecurityToken.ValidTo == DateTime.MinValue)
                {
                    return StatusCode(401, "access_token invalid or expired.");
                }
                user_Id = new Guid(jwtSecurityToken.Payload["userId"].ToString());
            }
            catch (Exception)
            {
                return StatusCode(401, "access_token invalid or expired.");// invalid token format.
            }

            HttpResponseMessage ResMsg = await PostWebApiToken("", "", "verify_access_token", "", token);
            if (ResMsg.StatusCode != HttpStatusCode.OK)// Contains("access_token invalid or expired."))
            {
                return StatusCode(401, "access_token invalid or expired.");
            }



            DataSet ds = new DataSet();
            string connectionString = MyHelper.GetDBConn();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sQuery = "select* from [dbo].[view_QueueStatus_SCGL]";
                SqlCommand cmd = new SqlCommand(sQuery, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds, "Data");
            }// end using

            /////******
            // check error from store result
            string errorFromStore = MyHelper.GetErrorFromStoreToApiError(ds);
            if (!string.IsNullOrWhiteSpace(errorFromStore))
            {
                return BadRequest(errorFromStore);
            }

            // remove message column if success
            DataSet dsResult = MyHelper.RemoveColumnFromStore(ds, new string[] { "WarehouseRemark" });

            // change column name to sencesitive
            Dictionary<string, string> dicOfColumnMapping = new Dictionary<string, string>()
                {
                  { "queueID", "queueId" },
                  { "channelID", "channelId" },
                  { "provinceID", "provinceId" },
                  { "truckTypeID", "truckTypeId" },
                  { "bookingID", "bookingId" },
                  { "checkinID", "checkinId" },
                
                };
            dsResult = MyHelper.ChangeColumnNameFromMapping(dsResult, dicOfColumnMapping);

            return Ok(dsResult.Tables["Data"]);
        }

    }

    [Serializable]
    internal class HttpResponseException : Exception
    {
        private HttpResponseMessage resp;


        public HttpResponseException(HttpStatusCode notFound)
        {
        }

        public HttpResponseException(HttpResponseMessage resp)
        {
            this.resp = resp;
        }

        public HttpResponseException(string message) : base(message)
        {
        }

        public HttpResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }


    }
}