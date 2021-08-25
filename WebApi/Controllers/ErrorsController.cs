using BusinessEntities;
using BusinessServices;
using System;
using System.Text;
using System.Web.Mvc;

namespace WebPlatApi.Controllers
{
    public class ErrorsController : Controller
    {
        #region Private variable.
        private readonly ITokenServices _tokenServices;
        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public ErrorsController(ITokenServices tokenServices)
        {
            _tokenServices = tokenServices;
        }

        #endregion

        public ActionResult Error()
        {
            //TokenRequestEntity _request = new TokenRequestEntity()
            //{
            //    Token = "b9368c50-6df8-4a28-8bb8-4073b40d111",
            //    RetailerID = "12",
            //    MerchantId = "9561352874",
            //    Copyright = "Payeassy",
            //    LogoUrl = "https://payeassy.co.in/assets/img/logo/logo6.png",
            //    FirmName = "Webplat in",
            //    ServiceID = "151"
            //};
            //var result = _tokenServices.ValidateToken(_request.Token, _request.ServiceID);
            //if (result.StatusCode == clsVariables.APIStatus.Success)
            //{
            //    var results = _tokenServices.TokenGenerate(_request);
            //    if (results.StatusCode == clsVariables.APIStatus.Success)
            //    {
            //        string formID = "PostForm";
            //        StringBuilder strForm = new StringBuilder();
            //        strForm.Append("<form action=\"/login/Authenticate\" id=\"" + formID + "\" name=\"" + formID + "\" method=\"post\">");
            //        strForm.Append("<input type=\"hidden\" name=\"authentication\" value=\"" + results.Message + "\" id=\"authentication\" />");
            //        strForm.Append("</form>");
            //        StringBuilder strScript = new StringBuilder();
            //        strScript.Append("<script language='javascript'>");
            //        strScript.Append("var v" + formID + " = document." + formID + ";");
            //        strScript.Append("v" + formID + ".submit();");
            //        strScript.Append("</script>");
            //        string requestForm = strForm.ToString() + strScript.ToString();
            //        Response.Clear();
            //        Response.Write(requestForm);
            //        Response.End();
            //    }
            //}

            //Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-30);
            Session.Abandon();
            return View();
        }

        public ActionResult SessionExpire()
        {
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-30);
            Session.Abandon();
            return View();
        }

    }
}
