using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebApi.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        //UserDto PostWebApiToken(string loginname, string loginPwd,string grant_type, string refresh_token, string access_token);
        //IEnumerable<User> GetAll();
        //User GetById(int id);
        //*********************************************
        //IEnumerable<Queue> GetAllQueue(string chanelId);
        //Queue GetAllQueueByChanelId([FromHeader] string channelId);
        //*********************************************
    }

     class UserService : IUserService
    {
        private DataContext _context;
        
        public UserService(DataContext context)
        {
            _context = context;
        }
        
        public User Authenticate(string username, string password)
        //IActionResult
        {
            if (username != "User1" || password != "User1")
            {
                return null;
            }
            //authentication successful
            return new User();
        }
        //***********************************************************************
        //public IEnumerable<Queue> GetAllQueue(string chanelId)
        //{
        //    return _context.Queues_API;
        //}

        public Queue GetAllQueueByChanelId( string channelId)
        {
            return new Queue();
        }
      
        //public IEnumerable<User> GetAll()
        //{
        //    return _context.Users_API;
        //}

        public User GetById(int id)
        {
            return new User();
            //return _context.Users.Find(id);
        }     

        // private helper methods
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

       
    }
}