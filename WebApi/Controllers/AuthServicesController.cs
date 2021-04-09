using AttributeRouting.Web.Http;
using BusinessEntities;
using BusinessServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace WebPlatApi.Controllers
{
    public class AuthServicesController : Controller
    {
        #region Private variable.
        private readonly IBBPSServices _bbpsServices;
        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public AuthServicesController(IBBPSServices bbpsServices)
        {
            _bbpsServices = bbpsServices;
        }

        #endregion

        [POST("BillerDetails")]
        public JsonResult BillerDetails(string billerid, string mobile, string number, string param1, string param2, string param3, string param4, string param5)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _bbpsServices.ValidateUser(Session["RGMGDDATAINFO"].ToString());
                if (result != null && result.UserID > 0)
                {
                    if (!string.IsNullOrEmpty(billerid) && !string.IsNullOrEmpty(mobile) && !string.IsNullOrEmpty(number))
                    {
                        return SubBillPaymentFetch(result.UserID, result.AgentID, billerid, mobile, number, param1, param2, param3, param4, param5);
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Biller code is required.";
                    }
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }


        #region Private Method
        private JsonResult SubBillPaymentFetch(int userID, string agentID, string billerCode, string mobile, string number, string param1, string param2, string param3, string param4, string param5)
        {
            var result = _bbpsServices.BillFetch(userID, agentID, Convert.ToInt32(billerCode), mobile, number, param1, param2, param3, param4, param5);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
