using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;

namespace WebApi.Helpers
{
   
    public static class HttpHelper
    {
        // Create a request using a URL that can receive a post. 

        public  static string PostWebApiTokenAuthor(string postData)
        {
            var request = (System.Net.HttpWebRequest)WebRequest.Create("http://104.248.152.173:8081/api/v1/users/authenticate");
            request.Method = "POST";
            //postData = "This is a test that posts this string to a Web server.";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;

            var Stream =(request.GetRequestStream());

            var responseFromServer = Stream.ToString();

            //var reader = Stream;
            //var responseFromServer = reader.ReadToEnd();

            //request.GetResponse();

            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            //Console.WriteLine(responseFromServer);
            //reader.Close();
            //Stream.Close();
            //response.Close();

            return responseFromServer.ToString();
        }


        //var request = (System.Net.HttpWebRequest)WebRequest.Create("");
        // Set the Method property of the request to POST.
        // Create POST data and convert it to a byte array.
        // Set the ContentType property of the WebRequest.
        // Set the ContentLength property of the WebRequest.
        // Get the request stream.
        // Write the data to the request stream.
        // Close the Stream object.
        // Get the response.
        // Display the status.
        // Get the stream containing content returned by the server.
        // Open the stream using a StreamReader for easy access.
        // Read the content.

        // Display the content.
        // Clean up the streams.


    }

   
}
