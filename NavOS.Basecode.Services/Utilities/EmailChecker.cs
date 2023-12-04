using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using NavOS.Basecode.Services.Interfaces;

namespace NavOS.Basecode.Services.Utilities
{
    public class EmailChecker : IEmailChecker
    {
        private readonly HttpClient client;
        public EmailChecker() 
        { 
            client = new HttpClient();
        }
        /// <summary>
        /// Determines whether a certain email is valid.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>
        ///   <c>true</c> if email is valid; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsEmailValidAsync(string email)
        {
            //iQdBTaJNRZCeLA7inmIHv
            var accessKey = "nPcRjWkbOlMk7KbzYtjRX";
            var request = new HttpRequestMessage
            {
	            Method = HttpMethod.Get,
	            RequestUri = new Uri("https://api.emails-checker.net/check?access_key=" + accessKey + "&email=" + email)
            };
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(body);
            if (responseObject.response.result == "deliverable")
            {
                return true;
            }
            return false;
        }
    }
}