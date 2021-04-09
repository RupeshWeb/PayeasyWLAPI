using AttributeRouting.Helpers;
using AttributeRouting.Web.Http;
using BusinessEntities;
using BusinessServices;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebPlatApi.ActionFilters;
using ActionNameAttribute = System.Web.Mvc.ActionNameAttribute;
using HttpGetAttribute = System.Web.Mvc.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace WebPlatApi.Controllers
{
    public class PanCardController : Controller
    {
        #region Private Variables
        private readonly IPancardServices _apiServices;
        #endregion

        #region Constructor
        public PanCardController(IPancardServices apiServices)
        {
            _apiServices = apiServices;
        }
        #endregion

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Pancard()
        {
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult PancardRequest()
        {
            PanRedirectResponseEntity _response = new PanRedirectResponseEntity();
            var result = _apiServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
            if (result != null && result.UserID > 0)
            {
                _response = _apiServices.PancardRedirect(result.UserID, result.AgentID, result.MerchantID);
                if (_response.StatusCode == clsVariables.APIStatus.Success)
                    return Redirect(_response.URL);
            }
            else
            {
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = "Session is expired.";
            }
            TempData["ErrorMessage"] = _response.Message;
            return RedirectToAction("Pancard");
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Reports()
        {
            var result = _apiServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
            if (result != null && result.UserID > 0)
            {
                ViewBag.transaction = _apiServices.TransactionReport(result.UserID, result.AgentID, DateTime.Now.Date.ToString(), DateTime.Now.Date.ToString(), string.Empty);
            }
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Reports(string mobilenumber, string fromdate, string todate)
        {
            var result = _apiServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
            if (result != null && result.UserID > 0)
            {
                ViewBag.transaction = _apiServices.TransactionReport(result.UserID, result.AgentID, fromdate, todate, mobilenumber);
            }
            return View();
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Receipt(string Id)
        {
            PANReceiptReponseEntity _response = new PANReceiptReponseEntity();
            try
            {
                if (Request.UrlReferrer != null)
                {
                    if (!string.IsNullOrEmpty(Id))
                    {
                        var result = _apiServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                        if (result != null && result.UserID > 0)
                        {
                            _response = _apiServices.Receipt(result.UserID, result.AgentID, Id);
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Session is expired.";
                        }
                    }
                    else
                    {
                        return RedirectToAction("Reports");
                    }
                }
                else
                {
                    return RedirectToAction("Reports");
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return View(_response);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult SubLastTransaction()
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _apiServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _apiServices.LastTransaction(result.UserID, result.AgentID);
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Session is expired.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        #region Payment Method
        [HttpPost]
        [ActionName("OnlinePaymentHome")]
        public ActionResult OnlinePaymentHome()
        {
            UtiPaymentDeductionEntity _response = new UtiPaymentDeductionEntity();
            string request = Request.Url.AbsoluteUri;
            string url = Request.Url.AbsolutePath;
            string data = Request.Form.ToString();
            ClsMethods.ResponseLogs(1, "HMPANDeduction", clsVariables.ServiceType.PanCard, url, request, data, 0, "", "");
            string applicationNo = Request.Form["applicationNo"];
            string userID = Request.Form["userID"];
            string UTITSLTransID = Request.Form["UTITSLTransID"];
            string transAmt = Request.Form["transAmt"];
            string transactionNo = Request.Form["transactionNo"];
            string appName = Request.Form["appName"];
            string PANCardType = Request.Form["PANCardType"];
            string payment = Request.Form["payment"];
            string responseStr = "";
            try
            {
                if (!string.IsNullOrEmpty(applicationNo) && !string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(transAmt))
                {
                    var results = _apiServices.PaymentGateway(applicationNo, userID, UTITSLTransID, transAmt, transactionNo, appName, PANCardType, payment);
                    responseStr = new JavaScriptSerializer().Serialize(results);
                    if (results.StatusCode == clsVariables.APIStatus.Success)
                    {
                        _response.transID = _apiServices.EncryptionData(results.TxnID);
                        _response.UTITSLTransID = _apiServices.EncryptionData(results.UTITxnID);
                        _response.transStatus = _apiServices.EncryptionData("S");
                        _response.applicationNo = _apiServices.EncryptionData(results.ApplicationNo);
                        _response.transAmt = _apiServices.EncryptionData(results.Amount);
                    }
                    else
                    {
                        _response.transID = _apiServices.EncryptionData(results.TxnID);
                        _response.UTITSLTransID = _apiServices.EncryptionData(results.UTITxnID);
                        _response.transStatus = _apiServices.EncryptionData("F");
                        _response.applicationNo = _apiServices.EncryptionData(results.ApplicationNo);
                        _response.transAmt = _apiServices.EncryptionData(results.Amount);
                    }
                }
                else
                {
                    _response.transID = transactionNo;
                    _response.UTITSLTransID = UTITSLTransID;
                    _response.transStatus = _apiServices.EncryptionData("F");
                    _response.applicationNo = applicationNo;
                    _response.transAmt = transAmt;
                }
            }
            catch (Exception)
            {
                _response.transID = transactionNo;
                _response.UTITSLTransID = UTITSLTransID;
                _response.transStatus = _apiServices.EncryptionData("F");
                _response.applicationNo = applicationNo;
                _response.transAmt = transAmt;
            }
            responseStr += " / " + new JavaScriptSerializer().Serialize(_response);
            ClsMethods.ResponseLogs(1, "HMPANDeduction", clsVariables.ServiceType.PanCard, url, data, responseStr, 0, "Response", "");
            return View(_response);
        }

        [GET("OnlinePaymentProccess")]
        public ActionResult OnlinePaymentProccess(string EncData)
        {
            UTIPancardResponseEntity _response = new UTIPancardResponseEntity();
            try
            {
                string request = Request.Url.AbsoluteUri;
                string url = Request.Url.AbsolutePath;
                ClsMethods.ResponseLogs(1, "PANProccess", clsVariables.ServiceType.PanCard, url, request, EncData, 0, "pan", "");
                if (!string.IsNullOrEmpty(EncData))
                {
                    _response = _apiServices.PaymentGatewayVerification(EncData);
                    string txnStatus = _response.StatusCode == 1 ? "S" : "F";
                    string XML = "<response><transID>" + _response.TxnID + "</transID><UTITSLTransID>" + _response.UTITxnID + "</UTITSLTransID><transStatus>" + txnStatus + "</transStatus><transAmt>" + _response.Amount + "</transAmt></response>";
                    ClsMethods.ResponseLogs(1, "PANProccess", clsVariables.ServiceType.PanCard, url, EncData, XML, 0, "", "");
                    return this.Content(XML, "application/xml", Encoding.UTF8);
                    //return new HttpResponseMessage()
                    //{
                    //    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    //};
                }
                else
                {
                    string XML = "<response><transID>empty</transID><UTITSLTransID>response</UTITSLTransID><transStatus>F</transStatus><transAmt>107.00</transAmt></response>";
                    ClsMethods.ResponseLogs(1, "PANProccess", clsVariables.ServiceType.PanCard, url, EncData, XML, 0, "", "");
                    return this.Content(XML, "application/xml", Encoding.UTF8);
                    //return new HttpResponseMessage()
                    //{
                    //    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    //};
                }
            }
            catch (Exception ex)
            {
                string XML = "<response><transID>exception</transID><UTITSLTransID>1234</UTITSLTransID><transStatus>F</transStatus><transAmt>107.00</transAmt></response>";
                ClsMethods.ResponseLogs(1, "PANProccess", clsVariables.ServiceType.PanCard, Request.Url.AbsolutePath, EncData, XML + " / " + ex.Message, 0, "", "");
                return this.Content(XML, "application/xml", Encoding.UTF8);
                //return new HttpResponseMessage()
                //{
                //    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                //};
            }
        }


        public ActionResult TestingCall([FromBody] UtiPaymentDeductionEntity model)
        {
            string transID = _apiServices.DecryptionData(model.transID);
            string UTITSLTransID = _apiServices.DecryptionData(model.UTITSLTransID);
            string transStatus = _apiServices.DecryptionData(model.transStatus);
            string applicationNo = _apiServices.DecryptionData(model.applicationNo);
            string transAmt = _apiServices.DecryptionData(model.transAmt);

            return View();
        }
        #endregion

    }
}
