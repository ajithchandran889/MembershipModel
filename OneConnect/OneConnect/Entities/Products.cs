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
        public static List<ProductPriceDetails> GetProductsPriceDetails(string getProductsPriceUrl, string token)
        {
            using (var client = new HttpClient())
            {
                dynamic myModel = new ExpandoObject();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                List<ProductPriceDetails> productsPrice = null;
                using (var response = client.GetAsync(getProductsPriceUrl).Result)
                {

                    if (response.IsSuccessStatusCode)
                    {

                        productsPrice = response.Content.ReadAsAsync<List<ProductPriceDetails>>().Result;

                    }

                }

                return productsPrice;
            }
        }
        
    }
}