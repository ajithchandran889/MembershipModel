using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OneConnect.ViewModels;
using System.Net.Http;
using System.Dynamic;

namespace OneConnect.Entities
{
    public static class Products
    {
        public static List<ProductInfo> GetProducts(string getProductsUrl, string token)
        {
            using (var client = new HttpClient())
            {
                dynamic myModel = new ExpandoObject();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                List<ProductInfo> products = null;
                using (var response = client.GetAsync(getProductsUrl).Result)
                {

                    if (response.IsSuccessStatusCode)
                    {

                        products = response.Content.ReadAsAsync<List<ProductInfo>>().Result;

                    }

                }

                return products;
            }
        }
    }
}