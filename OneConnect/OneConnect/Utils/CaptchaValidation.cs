using Newtonsoft.Json;
using OneConnect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace OneConnect.Utils
{
    public class CaptchaValidation
    {
        public static CaptchaResponse CaptchaValidate(string secret, string response, string remoteIP)
        {
            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            CaptchaResponse captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            return captchaResponse;
        }
    }
}