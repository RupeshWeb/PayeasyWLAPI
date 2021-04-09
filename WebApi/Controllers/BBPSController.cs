using BusinessEntities;
using BusinessServices;
using System;
using System.Linq;
using System.Web.Mvc;
using WebPlatApi.ActionFilters;

namespace WebPlatApi.Controllers
{
    public class BBPSController : Controller
    {
        #region Private variable.
        private readonly IBBPSServices _bbpsServices;
        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public BBPSController(IBBPSServices bbpsServices)
        {
            _bbpsServices = bbpsServices;
        }

        #endregion

        [HttpGet]
        [SessionAuthorize]
        public ActionResult BBPS()
        {
            return View();
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Reports()
        {
            var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
            if (result != null && result.UserID > 0)
            {
                ViewBag.transaction = _bbpsServices.TransactionReport(result.UserID, result.AgentID, DateTime.Now.Date.ToString(), DateTime.Now.Date.ToString(), string.Empty, string.Empty);
            }
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Reports(string mobilenumber, string fromdate, string todate, string txnRefId)
        {
            var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
            if (result != null && result.UserID > 0)
            {
                ViewBag.transaction = _bbpsServices.TransactionReport(result.UserID, result.AgentID, fromdate, todate, mobilenumber, txnRefId);
            }
            return View();
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Receipt(string Id)
        {
            BBPSReceiptReponseEntity _response = new BBPSReceiptReponseEntity();
            try
            {
                if (Request.UrlReferrer != null)
                {
                    if (!string.IsNullOrEmpty(Id))
                    {
                        var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                        if (result != null && result.UserID > 0)
                        {
                            _response = _bbpsServices.Receipt(result.UserID, result.AgentID, Id);
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
        public ActionResult BillPayments(BBPSPaymentRequestEntity model)
        {
            APIBBPSReponseEntity _response = new APIBBPSReponseEntity();
            try
            {
                if (Request.UrlReferrer != null)
                {
                    try
                    {
                        var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                        if (result != null && result.UserID > 0)
                        {
                            _response = _bbpsServices.BillPayment(result.UserID, result.AgentID, result.MerchantID, Convert.ToInt32(model.BillerCode), Convert.ToDecimal(model.BillAmount), model.BillNumber, model.MobileNumber, model.AccountNumber, model.Authenticator, model.Payment, model.DueDate, model.BillDate, model.ConsumerName, model.BillerNumber, model.RefNumber);
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
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return RedirectToAction("BBPS");
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Complain()
        {
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        public ActionResult Complain(TransactionComplainRequestEntity model)
        {
            APIBBPSComplaintReponseEntity _response = new APIBBPSComplaintReponseEntity();
            if (ModelState.IsValid)
            {
                var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _bbpsServices.RaiseComplaint(result.UserID, result.AgentID, model);
                    //if (results.StatusCode == clsVariables.APIStatus.Success)
                    //    TempData["SuccessMessage"] = results.Message;
                    //else
                    //    TempData["ErrorMessage"] = results.Message;
                    //ModelState.Clear();
                }
            }
            else
            {
                //TempData["ErrorMessage"] = string.Join(" | ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = string.Join(" | ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }
            //return View();
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult ComplainReport()
        {
            var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
            if (result != null && result.UserID > 0)
            {
                ViewBag.transaction = _bbpsServices.ComplaintReport(result.UserID, result.AgentID, DateTime.Now.Date.ToString(), DateTime.Now.Date.ToString(), string.Empty, string.Empty);
            }
            return View();
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult ComplainReport(string mobilenumber, string fromdate, string todate, string txnRefId)
        {
            var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
            if (result != null && result.UserID > 0)
            {
                ViewBag.transaction = _bbpsServices.ComplaintReport(result.UserID, result.AgentID, fromdate, todate, txnRefId, mobilenumber);
            }
            return View();
        }

        #region Private method
        [HttpPost]
        [SessionAuthorize]
        public JsonResult BillerValidate(string billercode, string customerNo, string billnumber, string billeraccount, string billercycle)
        {
            APIBBPSFetchReponseEntity _response = new APIBBPSFetchReponseEntity();
            try
            {
                var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _bbpsServices.BillFetch(result.UserID, result.AgentID, Convert.ToInt32(billercode), customerNo, billnumber, billeraccount, billercycle, string.Empty, string.Empty, string.Empty);
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
        public JsonResult BillPayment(string billnumber, string billercode, decimal Amount, string custmobileno, string billeraccount, string billercycle, string payment, string duedate, string billdate, string consumername, string billnumbers, string referenceNo)
        {
            APIBBPSReponseEntity _response = new APIBBPSReponseEntity();
            try
            {
                var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _bbpsServices.BillPayment(result.UserID, result.AgentID, result.MerchantID, Convert.ToInt32(billercode), Amount, billnumber, custmobileno, billeraccount, billercycle, payment, duedate, billdate, consumername, billnumbers, referenceNo);
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
        public JsonResult TransactionStatus(string referenceNo)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _bbpsServices.TransactionStatus(result.UserID, result.AgentID, referenceNo);
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
        #endregion

    }
}
