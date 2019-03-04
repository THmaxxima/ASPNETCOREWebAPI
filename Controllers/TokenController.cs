using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using WebApi.Dtos;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Flurl;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Dynamic;
using WebApi.Entities;

namespace WebApi.Controllers
{
    //[Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _config;
        private DataContext _context;
        public string ResToken;

        public TokenController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IConfiguration config,
            DataContext context)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _config = config;
            _context = context;
        }
            
        public string HeaderAuthorization { get; private set; }
        public string _accesstoken { get; set; }
        public string _refresh_token { get; set; }
        public JwtSecurityToken _APIresToken { get; set; }
        public string APIRES { get; set; }
        public HttpResponseMessage resapidata;
        public HttpResponseMessage response;
        public StringContent contentData;
        //*********************************************************************************
        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/refreshtoken")]
        public IActionResult ManageTokenAsync([FromBody] UserDto users)
        {
            if (users.grant_type != "verify_access_token" && (string.IsNullOrWhiteSpace(users.authorization_code)))
                return BadRequest("Authorization code not found.");
            if (users.grant_type == "verify_access_token" && (string.IsNullOrWhiteSpace(users.access_token)))
                return BadRequest("access_token is missing.");

            try
            {
                //******************** Section for call API authorization *********************
               HttpResponseMessage resapidata =  (PostWebApiToken(users.login, users.password,
                "refresh_access_token", users.authorization_code, users.access_token).Result);
                //******************************************************************************
                //*************** แปลง ข้อมูล Plan/text ให้เป็น json object แล้ว return กลับ *****************
                if(resapidata.StatusCode != HttpStatusCode.OK)
                {
                    //return BadRequest(resapidata.Content.ReadAsStringAsync().Result);
                    return BadRequest("access_token invalid or expired.");

                }

                AuthenResult result = JsonConvert.DeserializeObject<AuthenResult>(resapidata.Content.ReadAsStringAsync().Result);
                return Ok(new
                {
                    //authorization_code = result.authorization_code,
                    access_token = result.access_token
                });
                //return Ok(resapidata);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //*********************************************************************************
        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/revoketoken")]
        public IActionResult revoketoken([FromBody] UserDto users)
        {
            if (users.grant_type != "verify_access_token" && (string.IsNullOrWhiteSpace(users.authorization_code)))
                return BadRequest("Authorization code not found.");
            if (users.grant_type == "verify_access_token" && (string.IsNullOrWhiteSpace(users.access_token)))
                return BadRequest("Token not found");

            try
            {
                //******************** Section for call API authorization *********************
                HttpResponseMessage resapidata = (PostWebApiToken(users.login, users.password,
                 "revoke_access_token", users.authorization_code, users.access_token).Result);
                //******************************************************************************
                if (resapidata.StatusCode != HttpStatusCode.OK)
                {
                    return BadRequest(resapidata.Content.ReadAsStringAsync().Result);
                }
                return Ok("access_token revoked.");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //*********************************************************************************
        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/revokeauth")]
        public IActionResult Revokeauth([FromBody] UserDto users)
        {
            if (users.grant_type != "verify_access_token" && (string.IsNullOrWhiteSpace(users.authorization_code)))
                return BadRequest("Authorization code not found.");
            if (users.grant_type == "verify_access_token" && (string.IsNullOrWhiteSpace(users.access_token)))
                return BadRequest("access_token is missing.");

            try
            {
                //******************** Section for call API authorization *********************
                HttpResponseMessage resapidata = (PostWebApiToken(users.login, users.password,
                 "revoke_authorization_code", users.authorization_code, users.access_token).Result);
                //******************************************************************************
                if (resapidata.StatusCode != HttpStatusCode.OK)
                {
                    return BadRequest(resapidata.Content.ReadAsStringAsync().Result);
                }
                return Ok("authorization_code revoked.");

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //*********************************************************************************
        public async Task<HttpResponseMessage> PostWebApiToken(string loginname, string loginPwd,
            string grant_type, string refresh_token, string access_token)
        {
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
                if (string.IsNullOrWhiteSpace(loginname)==false || string.IsNullOrWhiteSpace(loginPwd)==false)//((loginname != "" && loginname != null && loginPwd != "" || loginPwd != null) && grant_type == "" || grant_type == null)
                {
                    client.BaseAddress = new Uri (X10APIURL + "authenticate");
                    string stringData = JsonConvert.SerializeObject(userdynamic);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    response = await client.PostAsync
                  (X10APIURL + "authenticate", contentData);

                    return response;
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
                    return (response);
                }//
                else if (grant_type == "revoke_access_token")
                {
                    client.BaseAddress = new Uri(X10APIURL + "dispatchtoken");
                    string stringData = JsonConvert.SerializeObject(revokeaccesstoken);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    var response = client.PostAsync
                   (X10APIURL + "dispatchtoken", contentData).Result;
                    return (response);
                }
                else if (grant_type == "revoke_authorization_code")
                {
                    client.BaseAddress = new Uri(X10APIURL + "dispatchtoken");
                    string stringData = JsonConvert.SerializeObject(revokeauthorization);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    var response = client.PostAsync
                   (X10APIURL + "dispatchtoken", contentData).Result;
                    return (response);
                }
                else if (grant_type == "verify_authorizaton_code")
                {
                    client.BaseAddress = new Uri(X10APIURL + "dispatchtoken");
                    string stringData = JsonConvert.SerializeObject(verifyauthorizaton);
                    var contentData = new StringContent
                    (stringData, System.Text.Encoding.UTF8,
                    "application/json");
                    var response = client.PostAsync
                   (X10APIURL + "dispatchtoken", contentData).Result;
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
                    return response;
                }

                return (response);
            }
        }
        //*****************************************************************************
        //[Produces("application/json")]
        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/login")]
        public async Task<IActionResult> CreateTokenAsync([FromBody] UserDto users)
        {
            //input validation
            string loginErr = string.Empty;
            if(string.IsNullOrWhiteSpace(users.login))
            {
                loginErr += "login is missing. " + Environment.NewLine;
            }
            if(string.IsNullOrWhiteSpace(users.password))
            {
                loginErr += "password is missing. " + Environment.NewLine;
            }
            if(loginErr!="")
                return BadRequest(loginErr);
            //***************************************************************************
            try
            {
                //******************** Section for call API authorization *********************
                HttpResponseMessage ResMsg = await (PostWebApiToken(users.login, users.password,
                users.grant_type, users.authorization_code, users.access_token));
                //******************************************************************************
                
                //*************** แปลง ข้อมูล Plan/text ให้เป็น json object แล้ว return กลับ *****************
                AuthenResult result = JsonConvert.DeserializeObject<AuthenResult>(ResMsg.Content.ReadAsStringAsync().Result);
                if (result.access_token == null)
                    return BadRequest("login or password invalid.");

                return Ok(new
                {
                    authorization_code = result.authorization_code,
                    access_token = result.access_token
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
