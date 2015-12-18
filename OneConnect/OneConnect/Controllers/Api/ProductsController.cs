using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OneConnect.ViewModels;
using System.Web;
using System.IO;

namespace OneConnect.Controllers.Api
{
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
                        select new { r = p }).Select(t => new ProductInfo { productId = t.r.Id, productName = t.r.Name, productDescription = t.r.Description,productImageUrl=t.r.imageUrl }).ToList();

            }
            catch(Exception ex)
            {
                logger.Error("ProductController :GetProducts: Something went wrong");
                logger.Error(ex.StackTrace); 
            }
            return list;
            
        }
        //POST api/Product/GetProductsPrice
        [HttpGet]
        [Authorize]
        [Route("GetProductsPrice")]
        public IEnumerable<ProductPriceDetails> GetProductsPrice()
        {
            IEnumerable<ProductPriceDetails> list = null;
            try
            {
                
                list = (from st in DBEntities.SubscriptionTypes
                        join psp in DBEntities.ProductSubscriptionPriceDetails on st.id equals psp.SubsrciptionTypeId
                        select new { r = st, s = psp }).Select(t => new ProductPriceDetails { typeId = t.r.id, typeName = t.r.subscriptionType1, displayName = t.r.displayName, productId = t.s.productId, price = t.s.price, isPerUser = t.s.isPerUser }).ToList();

            }
            catch(Exception ex)
            {
                logger.Error("ProductController :GetProductPrice: Something went wrong");
                logger.Error(ex.StackTrace); 
            }
            return list;
            
        }
        //POST api/Product/GetProductsPrice
        [HttpGet]
        [Authorize]
        [Route("GetSubscribedProducts")]
        public IEnumerable<SubscribedProductDetails> GetSubscribedProducts()
        {
            IEnumerable<SubscribedProductDetails> list = null;
            try
            {
                AccountController accContoller=new AccountController();
                string ownerUserCustomId = accContoller.GetCustomUserId(accContoller.GetUserIdByName(User.Identity.Name));

                list = (from ps in DBEntities.ProductSubscriptions
                        join p in DBEntities.Products on ps.ProductId equals p.Id
                        join s in DBEntities.SubscriptionTypes on ps.ProductSubscriptionModel equals s.id
                        where ps.CreatedBy == ownerUserCustomId
                        select new { x = ps, y = p, z = s }).Select(t => new SubscribedProductDetails { productName = t.y.Name, productImageUrl = t.y.imageUrl, productDescription = t.y.Description, subscriptionType=t.z.displayName,toDate=t.x.ToDate,fromDate=t.x.FromDate}).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("ProductController :GetSubscribedProducts: Something went wrong");
                logger.Error(ex.StackTrace); 
            }
            return list;

        }
        //POST api/Product/PurchaseProductDetails
        [HttpPost]
        [Authorize]
        [Route("PurchaseProductDetails")]
        public string PurchaseProductDetails(List<PurchaseItemDeatils> list)
        {
            var tempIds = "";
            try
            {
                foreach(PurchaseItemDeatils item in list)
                {
                    DateTime toDate =DateTime.MaxValue;
                    SubscriptionType subType = DBEntities.SubscriptionTypes.Where(s => s.id == item.subsriptionType).FirstOrDefault();
                    if (subType.displayName.ToUpper().Contains("Monthly".ToUpper()))
                    {
                        toDate = item.fromDate.AddMonths(1);
                    }
                    else
                    {
                        toDate = item.fromDate.AddYears(1);
                    }
                    ProductSubscriptionBackup newItem = new ProductSubscriptionBackup();
                    newItem.productId = item.productId;
                    newItem.subscriptionType = item.subsriptionType;
                    newItem.FromDate = item.fromDate;
                    newItem.toDate = toDate;
                    newItem.userIds = item.userIds;
                    DBEntities.ProductSubscriptionBackups.Add(newItem);
                    DBEntities.SaveChanges();
                    tempIds += tempIds + "$"+newItem.id;
                }
            }
            catch(Exception e)
            {
                logger.Error("ProductController :PurchaseProductDetails: Something went wrong");
                logger.Error(e.StackTrace); 
            }
            return tempIds;

        }
        //POST api/Product/Subscribe
        [HttpPost]
        [Authorize]
        [Route("Subscribe")]
        public string Subscribe([FromBody]ProductSubscribeDetails productSubscribeDetail)
        {
            try
            {
                AccountController accContoller=new AccountController();
                string ownerUserName = accContoller.GetCustomUserId(accContoller.GetUserIdByName(User.Identity.Name));
                FinancialTransaction financialTransaction = new FinancialTransaction();
                string host = Dns.GetHostName();
                financialTransaction.IpAddress= Dns.GetHostByName(host).AddressList[0].ToString();
                financialTransaction.PaidMeduim = "Paypal";
                financialTransaction.AdditionalTransactionDetails = productSubscribeDetail.item;
                financialTransaction.Amount = productSubscribeDetail.sAmountPaid;
                financialTransaction.DateOfTransaction = DateTime.Now;
                financialTransaction.CreatedAt = DateTime.Now;
                financialTransaction.Createdby = ownerUserName;
                financialTransaction.LastModifiedAt = DateTime.Now;
                financialTransaction.LastModifiedBy = ownerUserName;
                financialTransaction.IsDeleted = false;
                DBEntities.FinancialTransactions.Add(financialTransaction);
                DBEntities.SaveChanges();
                var tempItemIds = productSubscribeDetail.tempItemIds;
                tempItemIds = HttpUtility.UrlDecode(tempItemIds);
                var itemIds = tempItemIds.Split('$');
                
                foreach(string itemId in itemIds)
                {
                    if(itemId!="" && itemId!=null)
                    {
                        ProductSubscription productSubscription = new ProductSubscription();
                        int id=Convert.ToInt32(itemId);
                        var item = DBEntities.ProductSubscriptionBackups.Where(u => u.id == id ).SingleOrDefault();
                        productSubscription.FinancialTransactionId = financialTransaction.Id;
                        productSubscription.ProductId = item.productId;
                        productSubscription.ProductSubscriptionModel = item.subscriptionType;
                        productSubscription.FromDate = item.FromDate;
                        productSubscription.ToDate = item.toDate;
                        productSubscription.CreatedBy = ownerUserName;
                        productSubscription.CreatedAt = DateTime.Now;
                        productSubscription.LastModifiedBy = ownerUserName;
                        productSubscription.LastModiifedAt = DateTime.Now;
                        productSubscription.IsDeleted = false;
                        DBEntities.ProductSubscriptions.Add(productSubscription);
                        DBEntities.SaveChanges();
                        var userItems = item.userIds;
                        var userIds = userItems.Split('$');
                        foreach (string userId in userIds)
                        {
                            if(userId!="")
                            {
                                ProductSubscribedUser productSubscribedUsers = new ProductSubscribedUser();
                                productSubscribedUsers.productSubscriptionId = productSubscription.Id;
                                productSubscribedUsers.userId = userId;
                                DBEntities.ProductSubscribedUsers.Add(productSubscribedUsers);
                                DBEntities.SaveChanges();
                            }
                            
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                logger.Error("ProductController :Subscribe: Something went wrong");
                logger.Error(ex.StackTrace); 
            }
            return "success";
        }
    }
}