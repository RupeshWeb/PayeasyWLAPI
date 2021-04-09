using BusinessEntities;
using BusinessServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPlatApi.ActionFilters;

namespace WebPlatApi.Controllers
{
    public class ServicesController : Controller
    {
        #region Private Variables
        private readonly ISupportServices _apiServices;
        #endregion

        #region Constructor
        public ServicesController(ISupportServices apiServices)
        {
            _apiServices = apiServices;
        }
        #endregion

        [HttpGet]
        public ActionResult BBPS()
        {
            return View();
        }

        [HttpGet]
        [MCTSessionAuthorize]
        public ActionResult MerchantRegistration()
        {
            if (Session["RGRTRequestStatus"] == null)
                return RedirectToAction("SessionExpire", "Errors");
            else if (Session["RGRTRequestStatus"].ToString() == "3")
                return RedirectToAction("MerchantRegistrationStatus", "Services");
            else if (Session["RGRTRequestStatus"].ToString() == "4")
                return RedirectToAction("MerchantRegistrationStatus", "Services");
            else if (Session["RGRTRequestStatus"].ToString() == "5")
                return RedirectToAction("MerchantRevision", "Services");
            return View();
        }

        [HttpPost]
        [MCTSessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult MerchantRegistration(MerchantActivationEntity model, HttpPostedFileBase Document1, HttpPostedFileBase Document2, HttpPostedFileBase Document3)
        {
            if (ModelState.IsValid)
            {
                var result = _apiServices.ValidateUser(Session["MCHTRGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    string myDomain = ConfigurationManager.AppSettings["Mydomain"];
                    #region aadhar F Page
                    if (Document2 != null)
                    {
                        string ext = Path.GetExtension(Document2.FileName);
                        decimal size = Math.Round(((decimal)Document2.ContentLength / (decimal)1024), 2);
                        if (ext != null && new[] { ".JPEG", ".JPG", ".PNG", ".PDF" }.Contains(ext.ToUpper()) && size < 500)
                        {
                            string MyPath = model.MobileNo + "_POA_Front" + ext;
                            var path = Path.Combine(Server.MapPath("~/Content/Images/Merchant/POA"), MyPath);
                            Document2.SaveAs(path);
                            model.Document2 = myDomain + "Content/Images/Merchant/POA/" + MyPath;
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Please use JPG/PNG/PDF format only with file less than 500 KB.";
                            return View();
                        }
                    }
                    #endregion
                    #region aadhar S Page
                    if (Document3 != null)
                    {
                        string ext = Path.GetExtension(Document3.FileName);
                        decimal size = Math.Round(((decimal)Document3.ContentLength / (decimal)1024), 2);
                        if (ext != null && new[] { ".JPEG", ".JPG", ".PNG", ".PDF" }.Contains(ext.ToUpper()) && size < 500)
                        {
                            string MyPath = model.MobileNo + "_POA_Back" + ext;
                            var path = Path.Combine(Server.MapPath("~/Content/Images/Merchant/POA"), MyPath);
                            Document3.SaveAs(path);
                            model.Document3 = myDomain + "Content/Images/Merchant/POA/" + MyPath;
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Please use JPG/PNG/PDF format only with file less than 500 KB.";
                            return View();
                        }
                    }
                    #endregion
                    #region pancard Page
                    if (Document1 != null)
                    {
                        string ext = Path.GetExtension(Document1.FileName);
                        decimal size = Math.Round(((decimal)Document1.ContentLength / (decimal)1024), 2);
                        if (ext != null && new[] { ".JPEG", ".JPG", ".PNG", ".PDF" }.Contains(ext.ToUpper()) && size < 500)
                        {
                            string MyPath = model.MobileNo + "_POI" + ext;
                            var path = Path.Combine(Server.MapPath("~/Content/Images/Merchant/POI"), MyPath);
                            Document1.SaveAs(path);
                            model.Document1 = myDomain + "Content/Images/Merchant/POI/" + MyPath;
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Please use JPG/PNG/PDF format only with file less than 500 KB.";
                            return View();
                        }
                    }
                    #endregion

                    var results = _apiServices.AEPSActivation(result.UserID, model);
                    if (results.StatusCode == clsVariables.APIStatus.Success)
                        ViewBag.ErrorMessage = results.Message;
                    else
                        ViewBag.ErrorMessage = results.Message;
                }
                else
                {
                    ViewBag.ErrorMessage = "Retry again.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = string.Join(" | ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }
            return View();
        }

        [HttpGet]
        [MCTSessionAuthorize]
        public ActionResult MerchantRegistrationStatus()
        {
            MerchantActivationResponseEntity _response = new MerchantActivationResponseEntity();
            try
            {
                if (Session["RGRTRequestStatus"] == null)
                    return RedirectToAction("SessionExpire", "Errors");
                else if (Session["RGRTRequestStatus"].ToString() == "2")
                    return RedirectToAction("MerchantRegistration", "Services");
                else if (Session["RGRTRequestStatus"].ToString() == "5")
                    return RedirectToAction("MerchantRevision", "Services");

                var result = _apiServices.ValidateUser(Session["MCHTRGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    _response = _apiServices.GetMerchantActivation(result.MerchantID);
                }
                else
                {
                    return RedirectToAction("Error", "Errors");
                }
                return View(_response);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Errors");
            }
        }

        [HttpGet]
        [MCTSessionAuthorize]
        public ActionResult MerchantRevision()
        {
            MerchantActivationEntity _response = new MerchantActivationEntity();
            try
            {
                if (Session["RGRTRequestStatus"] == null)
                    return RedirectToAction("SessionExpire", "Errors");
                else if (Session["RGRTRequestStatus"].ToString() == "2")
                    return RedirectToAction("MerchantRegistration", "Services");
                else if (Session["RGRTRequestStatus"].ToString() == "3")
                    return RedirectToAction("MerchantRegistrationStatus", "Services");
                else if (Session["RGRTRequestStatus"].ToString() == "4")
                    return RedirectToAction("MerchantRegistrationStatus", "Services");

                var result = _apiServices.ValidateUser(Session["MCHTRGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    var results = _apiServices.GetMerchantActivation(result.MerchantID);
                    if (results.StatusCode == clsVariables.APIStatus.Success)
                    {
                        _response.MobileNo = results.Data.MobileNo;
                        _response.FirstName = results.Data.FirstName;
                        _response.MiddleName = results.Data.MiddleName;
                        _response.LastName = results.Data.LastName;
                        _response.PanNumber = results.Data.PanNumber;
                        _response.AadharNo = results.Data.AadharNo;
                        _response.PinCode = results.Data.PinCode.ToString();
                        _response.StateName = results.Data.StateName;
                        _response.District = results.Data.District;
                        _response.Document1 = results.Data.Document1;
                        _response.Document2 = results.Data.Document2;
                        _response.Document3 = results.Data.Document3;
                        _response.Status = results.Data.Status;
                        _response.Remarks = results.Data.Remarks;
                    }
                }
                else
                {
                    return RedirectToAction("Error", "Errors");
                }
                return View(_response);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Errors");
            }
        }

        [HttpPost]
        [MCTSessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult MerchantRevision(MerchantActivationEntity model, HttpPostedFileBase Document1, HttpPostedFileBase Document2, HttpPostedFileBase Document3)
        {
            if (ModelState.IsValid)
            {
                var result = _apiServices.ValidateUser(Session["MCHTRGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    string myDomain = ConfigurationManager.AppSettings["Mydomain"];
                    #region aadhar F Page
                    if (Document2 != null)
                    {
                        string ext = Path.GetExtension(Document2.FileName);
                        decimal size = Math.Round(((decimal)Document2.ContentLength / (decimal)1024), 2);
                        if (ext != null && new[] { ".JPEG", ".JPG", ".PNG", ".PDF" }.Contains(ext.ToUpper()) && size < 500)
                        {
                            string MyPath = model.MobileNo + "_POA_Front" + ext;
                            var path = Path.Combine(Server.MapPath("~/Content/Images/Merchant/POA"), MyPath);
                            Document2.SaveAs(path);
                            model.Document2 = myDomain + "Content/Images/Merchant/POA/" + MyPath;
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Please use JPG/PNG/PDF format only with file less than 500 KB.";
                            return View();
                        }
                    }
                    #endregion
                    #region aadhar S Page
                    if (Document3 != null)
                    {
                        string ext = Path.GetExtension(Document3.FileName);
                        decimal size = Math.Round(((decimal)Document3.ContentLength / (decimal)1024), 2);
                        if (ext != null && new[] { ".JPEG", ".JPG", ".PNG", ".PDF" }.Contains(ext.ToUpper()) && size < 500)
                        {
                            string MyPath = model.MobileNo + "_POA_Back" + ext;
                            var path = Path.Combine(Server.MapPath("~/Content/Images/Merchant/POA"), MyPath);
                            Document3.SaveAs(path);
                            model.Document3 = myDomain + "Content/Images/Merchant/POA/" + MyPath;
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Please use JPG/PNG/PDF format only with file less than 500 KB.";
                            return View();
                        }
                    }
                    #endregion
                    #region pancard Page
                    if (Document1 != null)
                    {
                        string ext = Path.GetExtension(Document1.FileName);
                        decimal size = Math.Round(((decimal)Document1.ContentLength / (decimal)1024), 2);
                        if (ext != null && new[] { ".JPEG", ".JPG", ".PNG", ".PDF" }.Contains(ext.ToUpper()) && size < 500)
                        {
                            string MyPath = model.MobileNo + "_POI" + ext;
                            var path = Path.Combine(Server.MapPath("~/Content/Images/Merchant/POI"), MyPath);
                            Document1.SaveAs(path);
                            model.Document1 = myDomain + "Content/Images/Merchant/POI/" + MyPath;
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Please use JPG/PNG/PDF format only with file less than 500 KB.";
                            return View();
                        }
                    }
                    #endregion
                    var results = _apiServices.AEPSActivation(result.UserID, model);
                    if (results.StatusCode == clsVariables.APIStatus.Success)
                        ViewBag.ErrorMessage = results.Message;
                    else
                        ViewBag.ErrorMessage = results.Message;
                }
                else
                {
                    ViewBag.ErrorMessage = "Retry again.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = string.Join(" | ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }
            return RedirectToAction("MerchantRevision");
        }

        [HttpPost]
        public JsonResult StatenameList()
        {
            var result = _apiServices.StateNameList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CitynameList(string stateName)
        {
            var result = _apiServices.CityNameList(stateName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FindPinCodeDetails(string pinCode)
        {
            var result = _apiServices.FindStateByPinCode(pinCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
