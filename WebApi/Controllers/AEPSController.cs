using BusinessEntities;
using BusinessServices;
using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebPlatApi.ActionFilters;

namespace WebPlatApi.Controllers
{
    public class AEPSController : Controller
    {
        #region Private Variables
        private readonly IAEPSServices _aepsServices;
        #endregion

        #region Constructor
        public AEPSController(IAEPSServices aepsServices)
        {
            _aepsServices = aepsServices;
        }
        #endregion

        #region Public Method
        [HttpGet]
        [SessionAuthorize]
        public ActionResult AEPS()
        {
            Session["MiniStateobject"] = null;
            return View();
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Reports()
        {
            var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
            if (result != null && result.UserID > 0)
            {
                ViewBag.transaction = _aepsServices.TransactionReport(result.UserID, result.AgentID, DateTime.Now.Date.ToString(), DateTime.Now.Date.ToString(), string.Empty);
            }
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Reports(string mobilenumber, string fromdate, string todate)
        {
            var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
            if (result != null && result.UserID > 0)
            {
                ViewBag.transaction = _aepsServices.TransactionReport(result.UserID, result.AgentID, fromdate, todate, mobilenumber);
            }
            return View();
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Receipt(string Id)
        {
            ReceiptReponseEntity _response = new ReceiptReponseEntity();
            try
            {
                if (Request.UrlReferrer != null)
                {
                    if (!string.IsNullOrEmpty(Id))
                    {
                        var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                        if (result != null && result.UserID > 0)
                        {
                            _response = _aepsServices.Receipt(result.UserID, result.AgentID, Id);
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

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Download()
        {
            return View();
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Statement(string Id)
        {
            AEPSMiniAPIReponseEntity _response = new AEPSMiniAPIReponseEntity();
            try
            {
                if (Request.UrlReferrer != null)
                {
                    if (!string.IsNullOrEmpty(Id) && Session["MiniStateobject"] != null)
                    {
                        var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                        if (result != null && result.UserID > 0)
                        {
                            _response = (AEPSMiniAPIReponseEntity)Session["MiniStateobject"];
                            if (_response.RefNumber != Id)
                            {
                                return RedirectToAction("AEPS");
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Session is expired.";
                        }
                    }
                    else
                    {
                        return RedirectToAction("AEPS");
                    }
                }
                else
                {
                    return RedirectToAction("AEPS");
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return View(_response);
        }
        #endregion

        #region Private Method
        [HttpPost]
        [SessionAuthorize]
        public JsonResult BindBankList()
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                if (Session["AepsBankNameList"] == null)
                {
                    _response = _aepsServices.BankName();
                    if (_response.StatusCode == clsVariables.APIStatus.Success)
                    {
                        Session["AepsBankNameList"] = _response;
                    }
                    else
                        Session["AepsBankNameList"] = null;
                }
                else
                    _response = (APIReponseEntity)Session["AepsBankNameList"];
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public JsonResult CashWithdrawalRequest(string mobileNumber, string aadharNumber, string biometricData, decimal amount, string bankName)
        {
            AEPSAPIReponseEntity _response = new AEPSAPIReponseEntity();
            try
            {
                var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _aepsServices.CashWithdrawal(result.UserID, result.AgentID, result.MerchantID, ClsMethods.GenereateUniqueNumber(9), mobileNumber, aadharNumber, amount, bankName, biometricData, "18.4621230", "73.8582062");
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Invalid user.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public JsonResult BalanceInquiryRequest(string mobileNumber, string aadharNumber, string biometricData, string bankName)
        {
            AEPSAPIReponseEntity _response = new AEPSAPIReponseEntity();
            try
            {
                var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _aepsServices.BalanceInquiry(result.UserID, result.AgentID, result.MerchantID, "NA", mobileNumber, aadharNumber, bankName, biometricData, "18.4621230", "73.8582062");
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Invalid user.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult TransactionInquiry(string txnRef)
        {
            AEPSAPIReponseEntity _response = new AEPSAPIReponseEntity();
            try
            {
                var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _aepsServices.TransactionInquiry(result.UserID, txnRef);
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Invalid user.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public JsonResult MiniStatementRequest(string mobileNumber, string aadharNumber, string biometricData, string bankName)
        {
            AEPSMiniAPIReponseEntity _response = new AEPSMiniAPIReponseEntity();
            try
            {
                var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _aepsServices.Ministatement(result.UserID, result.AgentID, result.MerchantID, "NA", mobileNumber, aadharNumber, bankName, biometricData, "18.4621230", "73.8582062");
                    if (_response.StatusCode == clsVariables.APIStatus.Success)
                        Session["MiniStateobject"] = _response;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Invalid user.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult LastOrdersDetails()
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _aepsServices.LastTransaction(result.UserID, result.AgentID);
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

        [HttpPost]
        [SessionAuthorize]
        public JsonResult ConvertValueToWords(double value)
        {
            var response = _aepsServices.ConvertToWords(value);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult CustomerBalanceInquiry()
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _aepsServices.CustomerBalanceRequest(result.UserID, result.AgentID);
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

        [HttpPost]
        [SessionAuthorize]
        public JsonResult CashValidateRequest(string mobileNumber, string aadharNumber, decimal amount, string bankName)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _aepsServices.CustomerCommRequest(result.UserID, result.AgentID, result.MerchantID, mobileNumber, aadharNumber, amount, bankName);
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Invalid user.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult CashValidateV2Request(string mobileNumber, string aadharNumber, decimal amount, string bankName, string mode)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _aepsServices.CustomerCommV2Request(result.UserID, result.AgentID, result.MerchantID, mobileNumber, aadharNumber, amount, bankName, mode);
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Invalid user.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateInput(false)]
        public JsonResult CashWithdrawalv2Request(string mobileNumber, string aadharNumber, string biometricData, decimal amount, string bankName, string mode)
        {
            AEPSAPIReponseEntity _response = new AEPSAPIReponseEntity();
            try
            {
                Regex regx = new Regex("^[0-9]{10}");
                if (regx.Matches(mobileNumber).Count > 0)
                {
                    Regex regxaadhar = new Regex("^[0-9]{12}");
                    if (regxaadhar.Matches(aadharNumber).Count > 0)
                    {
                        Regex regxbankcode = new Regex("^[0-9]{1,9}");
                        if (regxbankcode.Matches(bankName).Count > 0)
                        {
                            var result = _aepsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                            if (result != null && result.UserID > 0)
                            {
                                if (mode == "51")
                                {
                                    _response = _aepsServices.CashWithdrawal(result.UserID, result.AgentID, result.MerchantID, ClsMethods.GenereateUniqueNumber(9), mobileNumber, aadharNumber, amount, bankName, biometricData, "18.4621230", "73.8582062");
                                }
                                else if (mode == "52")
                                    _response = _aepsServices.BalanceInquiry(result.UserID, result.AgentID, result.MerchantID, "NA", mobileNumber, aadharNumber, bankName, biometricData, "18.4621230", "73.8582062");
                                else if (mode == "53")
                                {
                                    var miniResponse = _aepsServices.Ministatement(result.UserID, result.AgentID, result.MerchantID, "NA", mobileNumber, aadharNumber, bankName, biometricData, "18.4621230", "73.8582062");
                                    if (miniResponse.StatusCode == clsVariables.APIStatus.Success)
                                        Session["MiniStateobject"] = miniResponse;
                                    return Json(miniResponse, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    _response.StatusCode = clsVariables.APIStatus.Failed;
                                    _response.Message = "Invalid request format.";
                                }
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = "Invalid user.";
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Invalid bankCode.";
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Aadhar no should be 12 digits.";
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Mobile no should be 10 digits.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult ConsentAdd(string type, bool status)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                int userID = Convert.ToInt32(Session["RTUserId"]);
                _response = _aepsServices.ConsentAdd(userID, type, status);
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
