// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VirtualAssistantSample.Dialogs;

namespace VirtualAssistantSample.Controllers
{
    [Route("webchat")]
    public class WebChatController : Controller
    {
        // https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-authentication?view=azure-bot-service-4.0
        private const string GenerateDirectLineTokenUrl = "https://directline.botframework.com/v3/directline/tokens/generate";
        private readonly string directLineSecret;

        public WebChatController(IConfiguration configuration)
        {
            directLineSecret = configuration.GetSection("DirectLineSecret").Value;
        }

        public IActionResult Index(string locale = "en-us", string userId = "")
        {
            var directLineToken = string.Empty;
            var directLineClient = new HttpClient();
            directLineClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", directLineSecret);

            var response = directLineClient.PostAsync(GenerateDirectLineTokenUrl, new StringContent(string.Empty, Encoding.UTF8, "application/json")).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var directLineResponse = JsonConvert.DeserializeObject<DirectLineResponse>(responseString);
                directLineToken = directLineResponse.token;
            }

            ViewData["DirectLineToken"] = directLineToken;

            ViewData["Locale"] = locale;

            ViewData["UserID"] = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;

            return View();
        }
    }
}
