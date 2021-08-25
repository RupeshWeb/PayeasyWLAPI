using BusinessEntities;
using BusinessServices;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace WebPlatApi.Controllers
{
    public class LoginController : Controller
    {
        #region Private Variables
        private readonly ITokenServices _tokenServices;
        #endregion

        #region Constructor
        public LoginController(ITokenServices tokenServices)
        {
            _tokenServices = tokenServices;
        }
        #endregion

        [HttpPost]
        public ActionResult Authenticate(FormCollection form)
        {
            string referrerUrl = Request.UrlReferrer.AbsoluteUri;
            if (!string.IsNullOrEmpty(referrerUrl))
            {
                Session["RGMGDDATAINFO"] = null;
                Session["RGRTRequestLogo"] = null;
                Session["RGRTRequestCopy"] = null;
                Session["RGRTReferrerUrl"] = null;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                Response.Cache.SetNoStore();
                string authentication = form["authentication"];
                if (!string.IsNullOrEmpty(authentication))
                {
                    var result = AuthenticateValidate(authentication);
                    if (result.StatusCode == clsVariables.APIStatus.Success && result.Status == clsVariables.APIStatus.Success)
                    {
                        Session["RGRTReferrerUrl"] = referrerUrl;
                        if (Session["RGRTRequestServices"].ToString() == "151")
                            return RedirectToAction("AEPS", "AEPS");
                        else
                            return RedirectToAction("Error", "Errors");
                    }
                    else if (result.Status == 2)
                    {
                        return RedirectToAction("MerchantRegistration", "Services");
                    }
                    else if (new[] { 3, 4 }.Contains(result.Status))
                    {
                        return RedirectToAction("MerchantRegistrationStatus", "Services");
                    }
                    else if (result.Status == 5)
                    {
                        return RedirectToAction("MerchantRevision", "Services");
                    }
                    else
                        return RedirectToAction("Error", "Errors");
                }
                else
                    return RedirectToAction("Error", "Errors");
            }
            else
                return RedirectToAction("Error", "Errors");
        }

        private TokenRequestResponseEntity AuthenticateValidate(string credentials)
        {
            TokenRequestResponseEntity result = _tokenServices.ValidateRequest(credentials);
            if (result.StatusCode == clsVariables.APIStatus.Success && result.Status == clsVariables.APIStatus.Success)
            {
                Session["RGMGDDATAINFO"] = result.Value;
                Session["RGRTRequestLogo"] = result.Logourl;
                Session["RGRTRequestCopy"] = result.Copyright;
                Session["RGRTRequestCompany"] = result.FirmName;
                Session["RGRTRequestServices"] = result.ServiceId;
                return result;
            }
            else if (new[] { 2, 3, 4, 5 }.Contains(result.Status))
            {
                Session["MCHTRGMGDDATAINFO"] = result.Value;
                Session["RGRTRequestLogo"] = result.Logourl;
                Session["RGRTRequestCopy"] = result.Copyright;
                Session["RGRTRequestCompany"] = result.FirmName;
                Session["RGRTRequestServices"] = result.ServiceId;
                Session["RGRTRequestStatus"] = result.Status;
                return result;
            }
            else
                return result;
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));

            Response.Cache.SetNoStore();
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-30);
            FormsAuthentication.SignOut();
            //if (Session["RGRTReferrerUrl"] != null)
            //{
            //    return Redirect(Session["RGRTReferrerUrl"].ToString());
            //}
            return RedirectToAction("SessionExpire", "Errors");
        }

    }
}
