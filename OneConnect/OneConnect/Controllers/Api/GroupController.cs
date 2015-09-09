using OneConnect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace OneConnect.Controllers.Api
{
    [RoutePrefix("api/Group")]
    public class GroupController : ApiController
    {
        OneKonnectEntities DBEntities = new OneKonnectEntities();
        //POST api/Group/GetGroups
        [HttpGet]
        [Authorize]
        [Route("GetGroups")]
        public IEnumerable<GroupDetails> GetGroups()
        {
            IEnumerable<GroupDetails> list = null;
            try
            {
                AccountController acc = new AccountController();
                string userId = acc.GetUserIdByName(User.Identity.Name);
                AccountInfo info = (from a in DBEntities.AspNetUsers
                        join ua in DBEntities.UsersAditionalInfoes on a.Id equals ua.AspNetUserId
                        where a.Id == userId
                        select new { r = a, s = ua }).Select(t => new AccountInfo { userId = t.r.Id, customUserId = t.r.UserName, email = t.r.Email, name = t.s.Name, company = t.s.CompanyName, address = t.s.Address, contact = t.s.ContactInfo, status = t.s.Status.Value, isOwner = (bool)t.s.IsOwner }).SingleOrDefault();
                if (info != null)
                {
                    list = (from g in DBEntities.Groups
                            where g.GroupOwner == (info.isOwner?userId:g.GroupOwner) && g.IsDeleted == false && g.GroupAdmin==(info.isOwner?g.GroupAdmin:userId)
                            select new { r = g }).Select(t => new GroupDetails { groupId = t.r.Id, groupName = t.r.Name, description = t.r.Description, isActive = (bool)t.r.IsActive, productCount = t.r.GroupProducts.Count(), memberCount = t.r.GroupMembers.Count() }).ToList();
                }
            }
            catch (Exception e)
            {
            }
            return list;
        }

        //POST api/Group/GetGroupsakwdksdjfhjksdhfkjdhsjk
        [HttpGet]
        [Authorize]
        [Route("GetGroupsWithStatus/{isActiveOnly:bool}")]
        public IEnumerable<GroupDetails> GetGroupsWithStatus(bool isActiveOnly)
        {
            IEnumerable<GroupDetails> list = null;
            try
            {
                AccountController acc = new AccountController();
                string userId = acc.GetUserIdByName(User.Identity.Name);

                //list = (from g in DBEntities.Groups
                //        where g.GroupOwner == userId && g.IsDeleted == false && g.IsActive == (bool?)(isActiveOnly == true ? true : g.IsActive)
                //        select new { r = g }).Select(t => new GroupDetails { groupId = t.r.Id, groupName = t.r.Name, description = t.r.Description, isActive = (bool)t.r.IsActive, productCount = t.r.GroupProducts.Count(), memberCount = t.r.GroupMembers.Count() }).ToList();
                
                AccountInfo info = (from a in DBEntities.AspNetUsers
                                    join ua in DBEntities.UsersAditionalInfoes on a.Id equals ua.AspNetUserId
                                    where a.Id == userId
                                    select new { r = a, s = ua }).Select(t => new AccountInfo { userId = t.r.Id, customUserId = t.r.UserName, email = t.r.Email, name = t.s.Name, company = t.s.CompanyName, address = t.s.Address, contact = t.s.ContactInfo, status = t.s.Status.Value, isOwner = (bool)t.s.IsOwner }).SingleOrDefault();
                if (info != null)
                {
                    list = (from g in DBEntities.Groups
                            where g.GroupOwner == (info.isOwner ? userId : g.GroupOwner) && g.IsDeleted == false && g.IsActive == (bool?)(isActiveOnly == true ? true : g.IsActive) && g.GroupAdmin == (info.isOwner ? g.GroupAdmin : userId)
                           select new { r = g }).Select(t => new GroupDetails { groupId = t.r.Id, groupName = t.r.Name, description = t.r.Description, isActive = (bool)t.r.IsActive, productCount = t.r.GroupProducts.Count(), memberCount = t.r.GroupMembers.Count() }).ToList();
                }
            }
            catch (Exception e)
            {
            }
            return list;
        }

        //GET api/Group/GetGroupInfo
        [HttpGet]
        [Authorize]
        [Route("GetGroupInfo/{id:int}", Name = "GetGroupInfoRoute")]
        public GroupInfo GetGroupInfo(int id)
        {
            //string id = "";

            GroupInfo groupInfo = new GroupInfo();

            GroupDetails groupDetails = new GroupDetails();

            IEnumerable<GroupProductDetails> groupProducts = new List<GroupProductDetails>();

            IEnumerable<GroupMemberDetails> groupMembers = new List<GroupMemberDetails>();

            IEnumerable<GroupMemberRoleDetails> groupMemberRoles = new List<GroupMemberRoleDetails>();

            int inGroupId = id;// int.Parse((id != null && id != "") ? id : "0");

            try
            {
                AccountController acc = new AccountController();
                string userId = acc.GetUserIdByName(User.Identity.Name);
                
                AccountInfo info = (from a in DBEntities.AspNetUsers
                                    join ua in DBEntities.UsersAditionalInfoes on a.Id equals ua.AspNetUserId
                                    where a.Id == userId
                                    select new { r = a, s = ua }).Select(t => new AccountInfo { userId = t.r.Id, customUserId = t.r.UserName, email = t.r.Email, name = t.s.Name, company = t.s.CompanyName, address = t.s.Address, contact = t.s.ContactInfo, status = t.s.Status.Value, isOwner = (bool)t.s.IsOwner,owner=t.s.CreatedBy }).SingleOrDefault();
                if (info != null)
                {

                    groupDetails = (from g in DBEntities.Groups
                                    where g.Id == inGroupId
                                    select new { r = g }).Select(t => new GroupDetails { groupId = t.r.Id, groupName = t.r.Name, description = t.r.Description, isActive = (bool)t.r.IsActive, groupAdmin = t.r.GroupAdmin, productCount = t.r.GroupProducts.Count(), memberCount = t.r.GroupMembers.Count() }).SingleOrDefault();

                    if (groupDetails != null)
                    {
                        groupProducts = (from p in DBEntities.Products
                                         from gp in DBEntities.GroupProducts
                                         .Where(o => (o.ProductId == p.Id && o.GroupId == inGroupId)).DefaultIfEmpty()
                                         select new { r = p, s = gp }).Select(t => new GroupProductDetails { id = (t.s.Id == null ? 0 : t.s.Id), groupId = (t.s.GroupId == null ? 0 : t.s.GroupId), productId = t.r.Id, productName = t.r.Name, productDescription = t.r.Description, isSubscribed = (t.s.Id == null ? false : true), productRole = t.r.ProductRoles.Select(pr => new ProductRoleDetails { id = pr.Id, productId = (int)pr.ProductId, roleName = pr.RoleName, roleDescription = pr.RoleDescription }).ToList() }).ToList();



                        groupMembers = (from u in DBEntities.AspNetUsers
                                        join ua in DBEntities.UsersAditionalInfoes on u.Id equals ua.AspNetUserId
                                        from gm in (DBEntities.GroupMembers.Where(o => (o.UserId == u.Id && o.GroupId == inGroupId)).DefaultIfEmpty())
                                        where ua.CreatedBy == (info.isOwner ? userId : info.owner) && ua.AspNetUserId != groupDetails.groupAdmin && ua.AspNetUserId != (info.isOwner ? userId : info.owner)
                                        select new { r = u, s = gm }).Select(t => new GroupMemberDetails { id = (t.s.Id == null ? 0 : t.s.Id), groupId = (t.s.GroupId == null ? 0 : t.s.GroupId), userId = t.r.Id, userName = t.r.UserName, userEmail = t.r.Email, isSubscribed = (t.s.Id == null ? false : true) }).ToList();


                        groupMemberRoles = (from gmr in DBEntities.GroupMemberRoles
                                            join gp in DBEntities.GroupProducts.Where(o => o.GroupId == inGroupId) on gmr.GroupProductId equals gp.Id
                                            join gm in DBEntities.GroupMembers.Where(u => u.GroupId == inGroupId) on gmr.GroupMemberId equals gm.Id
                                            select new { r = gmr, s = gp, v = gm }).Select(t => new GroupMemberRoleDetails { id = t.r.Id, groupMemberId = t.v.Id, groupProductId = t.s.Id, roleId = t.r.RoleId }).ToList();

                    }

                }

                groupInfo.groupDetails = groupDetails==null?new GroupDetails():groupDetails;
                groupInfo.groupProducts = groupProducts == null ? new List<GroupProductDetails>() : groupProducts.ToList();
                groupInfo.groupMembers = groupMembers == null ? new List<GroupMemberDetails>() : groupMembers.ToList();
                groupInfo.groupMemberRoles = groupMemberRoles == null ? new List<GroupMemberRoleDetails>() : groupMemberRoles.ToList();

            }
            catch (Exception e)
            {
            }
            return groupInfo;
        }

        //GET api/Group/GetGroupProductDetails
        [HttpGet]
        [Authorize]
        [Route("GetGroupProductDetails/{id:int}")]
        public IEnumerable<GroupProductDetails> GetGroupProductDetails(int id)
        {
            //string id = "";
            IEnumerable<GroupProductDetails> groupProducts = null;

            int inGroupId = id;// int.Parse((id != null && id != "") ? id : "0");

            try
            {

                groupProducts = (from p in DBEntities.Products
                                 from gp in DBEntities.GroupProducts
                                 .Where(o=>(o.ProductId==p.Id && o.GroupId==id)).DefaultIfEmpty()
                                 select new { r = p, s = gp }).Select(t => new GroupProductDetails { id = (t.s.Id == null ? 0 : t.s.Id), groupId = (t.s.GroupId == null ? 0 : t.s.GroupId), productId = t.r.Id, productName = t.r.Name, productDescription = t.r.Description, isSubscribed = (t.s.Id == null ? false : true) }).ToList();

            }
            catch (Exception e)
            {
            }
            return groupProducts;
        }

        //GET api/Group/GetGroupMemberDetails
        [HttpGet]
        [Authorize]
        [Route("GetGroupMemberDetails/{id:int}")]
        public IEnumerable<GroupMemberDetails> GetGroupMemberDetails(int id)
        {
            //string id = "";
            IEnumerable<GroupMemberDetails> groupMembers = null;

            int inGroupId = id;// int.Parse((id != null && id != "") ? id : "0");

            try
            {
                AccountController acc = new AccountController();
                string userId = acc.GetUserIdByName(User.Identity.Name);

                groupMembers = (from u in DBEntities.AspNetUsers 
                                join  ua in DBEntities.UsersAditionalInfoes on u.Id equals ua.AspNetUserId
                                from gm in (DBEntities.GroupMembers.Where(o => (o.UserId == u.Id && o.GroupId == id)).DefaultIfEmpty())
                                where ua.CreatedBy == userId && ua.AspNetUserId != userId
                                 select new { r = u, s = gm }).Select(t => new GroupMemberDetails { id = (t.s.Id == null ? 0 : t.s.Id), groupId = (t.s.GroupId == null ? 0 : t.s.GroupId), userId = t.r.Id, userName = t.r.UserName, userEmail = t.r.Email, isSubscribed = (t.s.Id == null ? false : true) }).ToList();

            }
            catch (Exception e)
            {
            }
            return groupMembers;
        }


        ////GET api/Group/GetGroupMemberDetails
        //[HttpGet]
        //[Authorize]
        //[Route("GetGroupMemberDetails/{id:int}")]
        //public IEnumerable<GroupMemberDetails> GetGroupMemberDetails(int id)
        //{
        //    //string id = "";
        //    IEnumerable<GroupMemberDetails> groupMembers = null;

        //    int inGroupId = id;// int.Parse((id != null && id != "") ? id : "0");

        //    try
        //    {

        //        groupMembers = (from u in DBEntities.AspNetUsers
        //                        from gm in DBEntities.GroupMembers
        //                        .Where(o => (o.UserId == u.Id && o.GroupId == id)).DefaultIfEmpty()
        //                        select new { r = u, s = gm }).Select(t => new GroupMemberDetails { id = (t.s.Id == null ? 0 : t.s.Id), groupId = (t.s.GroupId == null ? 0 : t.s.GroupId), userId = t.r.Id, userName = t.r.UserName, userEmail = t.r.Email, isSubscribed = (t.s.Id == null ? false : true) }).ToList();

        //    }
        //    catch (Exception e)
        //    {
        //    }
        //    return groupMembers;
        //}


        //POST api/Group/SaveGroup
        [HttpPost]
        [Authorize]
        [Route("SaveGroup")]
        public HttpResponseMessage SaveGroup(GroupDetails groupDetails)
        {
            try
            {


                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    AccountController acc = new AccountController();
                    string userId = acc.GetUserIdByName(User.Identity.Name);

                    Group group = DBEntities.Groups.Where(g =>  g.Id == groupDetails.groupId).SingleOrDefault();

                    if (group == null)
                    {
                        group = new Group();
                        group.Name = groupDetails.groupName;
                        group.Description = groupDetails.description;
                        group.GroupAdmin = groupDetails.groupAdmin;
                        group.IsActive = groupDetails.isActive;
                        group.IsDeleted = false;
                        group.GroupOwner = userId;
                        group.CreatedBy = userId;
                        group.CreatedAt = DateTime.Now;
                        group.LastModifiedBy = userId;
                        group.LastModifiedAt = DateTime.Now;

                        group = DBEntities.Groups.Add(group);
                        DBEntities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, group.Id.ToString());

                    }
                    else
                    {


                        group.Name = groupDetails.groupName;
                        group.Description = groupDetails.description;
                        group.IsActive = groupDetails.isActive;
                        group.GroupAdmin = groupDetails.groupAdmin;
                        group.LastModifiedBy = userId;
                        group.LastModifiedAt = DateTime.Now;

                        DBEntities.Groups.Attach(group);
                        var entry = DBEntities.Entry(group);
                        entry.Property(g => g.Name).IsModified = true;
                        entry.Property(g => g.Description).IsModified = true;
                        entry.Property(g => g.IsActive).IsModified = true;
                        entry.Property(g => g.GroupAdmin).IsModified = true;
                        entry.Property(g => g.LastModifiedBy).IsModified = true;
                        entry.Property(g => g.LastModifiedAt).IsModified = true;

                        DBEntities.SaveChanges();

                        if (groupDetails.isActive == false)
                        {

                            List<GroupMemberRole> groupMemberRoleList = DBEntities.GroupMemberRoles.Where(t => t.GroupProduct.GroupId == group.Id).ToList();

                            //var groupMemberRoles= from gmr in DBEntities.GroupMemberRoles
                            // join gp in DBEntities.GroupProducts.Where(o => o.GroupId == groupId) on gmr.GroupProductId equals gp.Id
                            // select gmr;
                            foreach (GroupMemberRole groupMemberRole in groupMemberRoleList)
                            {

                                DBEntities.GroupMemberRoles.Remove(groupMemberRole);
                            }
                            DBEntities.SaveChanges();

                            List<GroupProduct> groupProducts = DBEntities.GroupProducts.Where(gp => gp.GroupId == group.Id).ToList();

                            foreach (GroupProduct groupProduct in groupProducts)
                            {
                                GroupProductsBakup groupProductBackup = new GroupProductsBakup();
                                groupProductBackup.GroupId = groupProduct.GroupId;
                                groupProductBackup.ProductId = groupProduct.ProductId;
                                groupProductBackup.IsDeleted = groupProduct.IsDeleted;
                                groupProductBackup.CreatedBy = groupProduct.CreatedBy;
                                groupProductBackup.CreatedAt = groupProduct.CreatedAt;
                                groupProductBackup.LastModifiedBy = userId;
                                groupProductBackup.LastModifiedAt = DateTime.Now;
                                groupProductBackup = DBEntities.GroupProductsBakups.Add(groupProductBackup);

                                if (groupProductBackup != null)
                                {
                                    DBEntities.GroupProducts.Remove(groupProduct);
                                }
                                DBEntities.SaveChanges();

                            }

                            List<GroupMember> groupMembers = DBEntities.GroupMembers.Where(gm => gm.GroupId == group.Id).ToList();

                            foreach (GroupMember groupMember in groupMembers)
                            {
                                GroupMembersBackup groupMembersBackup = new GroupMembersBackup();
                                groupMembersBackup.GroupId = groupMember.GroupId;
                                groupMembersBackup.UserId = groupMember.UserId;
                                groupMembersBackup.IsDeleted = groupMember.IsDeleted;
                                groupMembersBackup.CreatedBy = groupMember.CreatedBy;
                                groupMembersBackup.CreatedAt = groupMember.CreatedAt;
                                groupMembersBackup.LastModifiedBy = userId;
                                groupMembersBackup.LastModifiedAt = DateTime.Now;
                                groupMembersBackup = DBEntities.GroupMembersBackups.Add(groupMembersBackup);

                                if (groupMembersBackup != null)
                                {
                                    DBEntities.GroupMembers.Remove(groupMember);
                                }
                                DBEntities.SaveChanges();

                            }


                        }



                        return Request.CreateResponse(HttpStatusCode.OK, group.Id.ToString());

                    }
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Exception");
            }
        }

        //POST api/Group/SaveGroup
        [HttpPost]
        [Authorize]
        [Route("DeleteGroup")]
        public HttpResponseMessage DeleteGroup(GroupDetails groupDetails)
        {
            try
            {
                int groupId = groupDetails.groupId != null ? groupDetails.groupId : 0;
                    AccountController acc = new AccountController();
                    string userId = acc.GetUserIdByName(User.Identity.Name);

                    Group group = DBEntities.Groups.Where(g => g.GroupOwner == userId && g.Id == groupId).SingleOrDefault();

                    if (group == null)
                    {

                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Exception");

                    }
                    else
                    {

                        group.IsDeleted = true;
                        group.LastModifiedBy = userId;
                        group.LastModifiedAt = DateTime.Now;

                        DBEntities.Groups.Attach(group);
                        var entry = DBEntities.Entry(group);
                        entry.Property(g => g.IsDeleted).IsModified = true;
                        entry.Property(g => g.LastModifiedBy).IsModified = true;
                        entry.Property(g => g.LastModifiedAt).IsModified = true;
                        DBEntities.SaveChanges();

                        List<GroupMemberRole> groupMemberRoleList = DBEntities.GroupMemberRoles.Where(t => t.GroupProduct.GroupId == group.Id).ToList();

                        //var groupMemberRoles= from gmr in DBEntities.GroupMemberRoles
                        // join gp in DBEntities.GroupProducts.Where(o => o.GroupId == groupId) on gmr.GroupProductId equals gp.Id
                        // select gmr;
                        foreach (GroupMemberRole groupMemberRole in groupMemberRoleList)
                        {

                            DBEntities.GroupMemberRoles.Remove(groupMemberRole);
                        }
                        DBEntities.SaveChanges();


                        List<GroupProduct> groupProducts = DBEntities.GroupProducts.Where(gp => gp.GroupId == group.Id).ToList();

                        foreach (GroupProduct groupProduct in groupProducts)
                        {
                            groupProduct.IsDeleted = true;
                            groupProduct.LastModifiedBy = userId;
                            groupProduct.LastModifiedAt = DateTime.Now;
                            DBEntities.GroupProducts.Attach(groupProduct);
                            var entryGroupProduct = DBEntities.Entry(groupProduct);
                            entryGroupProduct.Property(g => g.IsDeleted).IsModified = true;
                            entryGroupProduct.Property(g => g.LastModifiedBy).IsModified = true;
                            entryGroupProduct.Property(g => g.LastModifiedAt).IsModified = true;
                            DBEntities.SaveChanges();

                        }

                        List<GroupMember> groupMembers = DBEntities.GroupMembers.Where(gm => gm.GroupId == group.Id).ToList();

                        foreach (GroupMember groupMember in groupMembers)
                        {
                            groupMember.IsDeleted = true;
                            groupMember.LastModifiedBy = userId;
                            groupMember.LastModifiedAt = DateTime.Now;
                            DBEntities.GroupMembers.Attach(groupMember);
                            var entryGroupMember = DBEntities.Entry(groupMember);
                            entryGroupMember.Property(g => g.IsDeleted).IsModified = true;
                            entryGroupMember.Property(g => g.LastModifiedBy).IsModified = true;
                            entryGroupMember.Property(g => g.LastModifiedAt).IsModified = true;
                            DBEntities.SaveChanges();

                        }

                        return Request.CreateResponse(HttpStatusCode.OK, "deleted");

                    }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Exception");
            }
        }

        public Boolean DeleteGroupWithId(int groupId)
        {
            try
            {
                AccountController acc = new AccountController();
                string userId = acc.GetUserIdByName(User.Identity.Name);

                Group group = DBEntities.Groups.Where(g => g.GroupOwner == userId && g.Id == groupId).SingleOrDefault();

                if (group == null)
                {

                    return false;

                }
                else
                {

                    group.IsActive = false;
                    group.LastModifiedBy = userId;
                    group.LastModifiedAt = DateTime.Now;

                    DBEntities.Groups.Attach(group);
                    var entry = DBEntities.Entry(group);
                    entry.Property(g => g.IsActive).IsModified = true;
                    entry.Property(g => g.LastModifiedBy).IsModified = true;
                    entry.Property(g => g.LastModifiedAt).IsModified = true;
                    DBEntities.SaveChanges();

                    List<GroupMemberRole> groupMemberRoleList = DBEntities.GroupMemberRoles.Where(t => t.GroupProduct.GroupId == group.Id).ToList();

                    //var groupMemberRoles= from gmr in DBEntities.GroupMemberRoles
                    // join gp in DBEntities.GroupProducts.Where(o => o.GroupId == groupId) on gmr.GroupProductId equals gp.Id
                    // select gmr;
                    foreach (GroupMemberRole groupMemberRole in groupMemberRoleList)
                    {

                        DBEntities.GroupMemberRoles.Remove(groupMemberRole);
                    }
                    DBEntities.SaveChanges();


                    List<GroupProduct> groupProducts = DBEntities.GroupProducts.Where(gp => gp.GroupId == group.Id).ToList();

                    foreach (GroupProduct groupProduct in groupProducts)
                    {
                        GroupProductsBakup groupProductBackup = new GroupProductsBakup();
                        groupProductBackup.GroupId = groupProduct.GroupId;
                        groupProductBackup.ProductId = groupProduct.ProductId;
                        groupProductBackup.IsDeleted = groupProduct.IsDeleted;
                        groupProductBackup.CreatedBy = groupProduct.CreatedBy;
                        groupProductBackup.CreatedAt = groupProduct.CreatedAt;
                        groupProductBackup.LastModifiedBy = userId;
                        groupProductBackup.LastModifiedAt = DateTime.Now;
                        groupProductBackup = DBEntities.GroupProductsBakups.Add(groupProductBackup);

                        if (groupProductBackup != null)
                        {
                            DBEntities.GroupProducts.Remove(groupProduct);
                        }
                        DBEntities.SaveChanges();

                    }

                    List<GroupMember> groupMembers = DBEntities.GroupMembers.Where(gm => gm.GroupId == group.Id).ToList();

                    foreach (GroupMember groupMember in groupMembers)
                    {
                        GroupMembersBackup groupMembersBackup = new GroupMembersBackup();
                        groupMembersBackup.GroupId = groupMember.GroupId;
                        groupMembersBackup.UserId = groupMember.UserId;
                        groupMembersBackup.IsDeleted = groupMember.IsDeleted;
                        groupMembersBackup.CreatedBy = groupMember.CreatedBy;
                        groupMembersBackup.CreatedAt = groupMember.CreatedAt;
                        groupMembersBackup.LastModifiedBy = userId;
                        groupMembersBackup.LastModifiedAt = DateTime.Now;
                        groupMembersBackup = DBEntities.GroupMembersBackups.Add(groupMembersBackup);

                        if (groupMembersBackup != null)
                        {
                            DBEntities.GroupMembers.Remove(groupMember);
                        }
                        DBEntities.SaveChanges();

                    }

                    return true;

                }
            }
            catch (Exception e)
            {
                return false;
            }
        }


        //POST api/Group/GroupProductSubscription
        [HttpPost]
        [Authorize]
        [Route("GroupProductSubscription")]
        public HttpResponseMessage GroupProductSubscription(GroupProductDetails grpPrdDetails)
        {

            try
            {
                AccountController acc = new AccountController();
                string userId = acc.GetUserIdByName(User.Identity.Name);

                var groupProduct = DBEntities.GroupProducts.Where(gp => gp.ProductId == grpPrdDetails.productId && gp.GroupId == grpPrdDetails.groupId).SingleOrDefault();

                if (grpPrdDetails.isSubscribed == false && groupProduct != null)
                {
                    List<GroupMemberRole> groupMemberRoleList = DBEntities.GroupMemberRoles.Where(t => t.GroupProduct.GroupId == groupProduct.GroupId && t.GroupProductId==groupProduct.Id).ToList();

                    foreach (GroupMemberRole groupMemberRole in groupMemberRoleList)
                    {

                        DBEntities.GroupMemberRoles.Remove(groupMemberRole);
                    }
                    DBEntities.SaveChanges();
                   
                    GroupProductsBakup groupProductBackup = new GroupProductsBakup();
                    groupProductBackup.GroupId = groupProduct.GroupId;
                    groupProductBackup.ProductId = groupProduct.ProductId;
                    groupProductBackup.IsDeleted = !grpPrdDetails.isSubscribed;
                    groupProductBackup.CreatedBy = groupProduct.CreatedBy;
                    groupProductBackup.CreatedAt = groupProduct.CreatedAt;
                    groupProductBackup.LastModifiedBy = userId;
                    groupProductBackup.LastModifiedAt = DateTime.Now;
                    groupProductBackup = DBEntities.GroupProductsBakups.Add(groupProductBackup);

                    if (groupProductBackup != null)
                    {
                        DBEntities.GroupProducts.Remove(groupProduct);
                    }
                    DBEntities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "unsubscribed");
                }
                else if (grpPrdDetails.isSubscribed == false)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "unsubscribed");
                }
                else if (grpPrdDetails.isSubscribed == true)
                {
                    if (groupProduct != null)
                    {
                        groupProduct.IsDeleted = !grpPrdDetails.isSubscribed;
                        groupProduct.LastModifiedBy = userId;
                        groupProduct.LastModifiedAt = DateTime.Now;
                        DBEntities.GroupProducts.Attach(groupProduct);
                        DBEntities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "subscribed");
                    }
                    else
                    {
                        groupProduct = new GroupProduct();
                        groupProduct.ProductId = grpPrdDetails.productId;
                        groupProduct.GroupId = grpPrdDetails.groupId;
                        groupProduct.IsDeleted = !grpPrdDetails.isSubscribed;
                        groupProduct.CreatedBy = userId;
                        groupProduct.CreatedAt = DateTime.Now;
                        groupProduct.LastModifiedBy = userId;
                        groupProduct.LastModifiedAt = DateTime.Now;

                        groupProduct = DBEntities.GroupProducts.Add(groupProduct);
                        DBEntities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "subscribed");
                    }


                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Exception");
            }



        }

        //POST api/Group/GroupMemberSubscription
        [HttpPost]
        [Authorize]
        [Route("GroupMemberSubscription")]
        public HttpResponseMessage GroupMemberSubscription(GroupMemberDetails grpUsrDetails)
        {

            try
            {
                AccountController acc = new AccountController();
                string userId = acc.GetUserIdByName(User.Identity.Name);

                var groupMember = DBEntities.GroupMembers.Where(gm => gm.UserId == grpUsrDetails.userId && gm.GroupId == grpUsrDetails.groupId).SingleOrDefault();

                if (grpUsrDetails.isSubscribed == false && groupMember != null)
                {
                    List<GroupMemberRole> groupMemberRoleList = DBEntities.GroupMemberRoles.Where(t => t.GroupProduct.GroupId == groupMember.GroupId && t.GroupMemberId == groupMember.Id).ToList();

                    foreach (GroupMemberRole groupMemberRole in groupMemberRoleList)
                    {

                        DBEntities.GroupMemberRoles.Remove(groupMemberRole);
                    }
                    DBEntities.SaveChanges();

                    GroupMembersBackup groupMembersBackup = new GroupMembersBackup();
                    groupMembersBackup.GroupId = groupMember.GroupId;
                    groupMembersBackup.UserId = groupMember.UserId;
                    groupMembersBackup.IsDeleted = !grpUsrDetails.isSubscribed;
                    groupMembersBackup.CreatedBy = groupMember.CreatedBy;
                    groupMembersBackup.CreatedAt = groupMember.CreatedAt;
                    groupMembersBackup.LastModifiedBy = userId;
                    groupMembersBackup.LastModifiedAt = DateTime.Now;
                    groupMembersBackup = DBEntities.GroupMembersBackups.Add(groupMembersBackup);

                    if (groupMembersBackup != null)
                    {
                        DBEntities.GroupMembers.Remove(groupMember);
                    }
                    DBEntities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "unsubscribed");
                }
                else if (grpUsrDetails.isSubscribed == false)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "unsubscribed");
                }
                else if (grpUsrDetails.isSubscribed == true)
                {
                    if (groupMember != null)
                    {
                        groupMember.IsDeleted = !grpUsrDetails.isSubscribed;
                        groupMember.LastModifiedBy = userId;
                        groupMember.LastModifiedAt = DateTime.Now;
                        DBEntities.GroupMembers.Attach(groupMember);
                        DBEntities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "subscribed");
                    }
                    else
                    {
                        groupMember = new GroupMember();
                        groupMember.UserId = grpUsrDetails.userId;
                        groupMember.GroupId = grpUsrDetails.groupId;
                        groupMember.IsDeleted = !grpUsrDetails.isSubscribed;
                        groupMember.CreatedBy = userId;
                        groupMember.CreatedAt = DateTime.Now;
                        groupMember.LastModifiedBy = userId;
                        groupMember.LastModifiedAt = DateTime.Now;

                        groupMember = DBEntities.GroupMembers.Add(groupMember);
                        DBEntities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "subscribed");
                    }


                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Exception");
            }



        }

        //POST api/Group/SaveGroupMemberRoles
        [HttpPost]
        [Authorize]
        [Route("SaveGroupMemberRole")]
        public HttpResponseMessage SaveGroupMemberRole(GroupMemberRoleDetailsSave groupMemberRoleDetailsSave)
        {
            try
            {
                    int groupId=groupMemberRoleDetailsSave.groupId;
                    List<GroupMemberRoleDetails> groupMemberRoleDetails=groupMemberRoleDetailsSave.groupMemberRoleDetails;

                    AccountController acc = new AccountController();
                    string userId = acc.GetUserIdByName(User.Identity.Name);
                    
                    List<GroupMemberRole> groupMemberRoleList= DBEntities.GroupMemberRoles.Where(t=> t.GroupProduct.GroupId==groupId).ToList();

                    //var groupMemberRoles= from gmr in DBEntities.GroupMemberRoles
                                                           // join gp in DBEntities.GroupProducts.Where(o => o.GroupId == groupId) on gmr.GroupProductId equals gp.Id
                                                           // select gmr;
                    foreach(GroupMemberRole groupMemberRole in groupMemberRoleList){

                        DBEntities.GroupMemberRoles.Remove(groupMemberRole);
                    }
                    DBEntities.SaveChanges();

                    foreach(GroupMemberRoleDetails groupMemberRoleDetail in groupMemberRoleDetails)
                    {
                        GroupMemberRole groupMemberRole=new GroupMemberRole();
                        groupMemberRole.GroupMemberId=groupMemberRoleDetail.groupMemberId;
                        groupMemberRole.GroupProductId=groupMemberRoleDetail.groupProductId;
                        groupMemberRole.RoleId=groupMemberRoleDetail.roleId;
                        groupMemberRole.CreatedAt=DateTime.Now;
                        groupMemberRole.CreatedBy=userId;
                        groupMemberRole.LastModifiedBy=userId;
                        groupMemberRole.LastModifiedAt=DateTime.Now;

                        DBEntities.GroupMemberRoles.Add(groupMemberRole);


                    }
                    DBEntities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "subscribed");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Exception");
            }
        }

        

        
    }
}