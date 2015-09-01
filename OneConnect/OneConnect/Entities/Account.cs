using OneConnect.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace OneConnect.Entities
{
    public static class Account
    {
        public static AccountInfo GetAccountInfo(string accountInfoUrl,string token)
        {
            using (var client = new HttpClient())
            {
                dynamic myModel = new ExpandoObject();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                AccountInfo accounInfo = null;
                using (var response = client.GetAsync(accountInfoUrl).Result)
                {

                    if (response.IsSuccessStatusCode)
                    {

                        accounInfo = response.Content.ReadAsAsync<AccountInfo>().Result;

                    }

                }

                return accounInfo;
            }
        }
    }
}