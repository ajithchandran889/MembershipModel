using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OneConnect.ViewModels;

namespace OneConnect.Controllers.Api
{
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        OneKonnectEntities DBEntities = new OneKonnectEntities();
        //POST api/Product/GetProducts
        [HttpGet]
        [Authorize]
        [Route("GetProducts")]
        public IEnumerable<ProductInfo> GetProducts()
        {
            IEnumerable<ProductInfo> list = null;
            try
            {
                
                list = (from p in DBEntities.Products
                        where p.IsDeleted == false
                        select new { r = p }).Select(t => new ProductInfo { productId = t.r.Id, productName = t.r.Name, productDescription = t.r.Description,productImageUrl=t.r.imageUrl,
                            silverMonthly=t.r.silverMonthly,silverYearly=t.r.silverYearly,goldMonthly=t.r.goldMonthly,goldYearly=t.r.goldYearly,silverPerUserMonthly=t.r.silverPerUserMonthy,
                            goldPerUserMonthly=t.r.goldPerUserMonthy,silverPerUserYearly=t.r.silverperUserYearly,goldPerUserYearly=t.r.goldPerUserYearly }).ToList();

            }
            catch(Exception ex)
            {

            }
            return list;
            
        }
        //POST api/Product/PurchaseProductDetails
        [HttpPost]
        [Authorize]
        [Route("PurchaseProductDetails")]
        public int PurchaseProductDetails(List<PurchaseItemDeatils> list)
        {
            
            return 0;

        }
    }
}