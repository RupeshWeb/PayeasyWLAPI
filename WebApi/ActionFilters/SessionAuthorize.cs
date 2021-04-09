using System.Web;
using System.Web.Mvc;

namespace WebPlatApi.ActionFilters
{
    public class SessionAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //if (httpContext.Session["RGsecurity1tokenid3"] != null)
            //{
            //    clsrgauthorization obj = new clsrgauthorization();
            //    bool dostatus = obj.ValidateToken(httpContext.Session["RGsecurity1tokenid3"].ToString());
            //    if (!dostatus) { httpContext.Session["RGsecurity1tokenid3"] = null; }
            //}
            return (httpContext.Session["RGMGDDATAINFO"] != null);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Errors/SessionExpire");
        }
    }

    public class MCTSessionAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return (httpContext.Session["MCHTRGMGDDATAINFO"] != null);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Errors/SessionExpire");
        }
    }
}