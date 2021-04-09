using BusinessEntities;
using BusinessServices.Resource;
using CCA.Util;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;

namespace BusinessServices.BBPS
{
    public class AvenueAPI
    {
        #region Private variable
        private readonly string Url = ConfigurationManager.AppSettings["Avenue_BBPS_Url"];
        private readonly string Version = ConfigurationManager.AppSettings["Avenue_Version"];
        private readonly string InstitutionID = ConfigurationManager.AppSettings["Avenue_InstitutionID"];
        private readonly string InstitutionName = ConfigurationManager.AppSettings["Avenue_InstitutionName"];
        private readonly string AccessCode = ConfigurationManager.AppSettings["Avenue_AccessCode"];
        private readonly string WorkingKey = ConfigurationManager.AppSettings["Avenue_WorkingKey"];
        #endregion

        public APIReponseEntity BillerCoverage(int userID)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                string requestID = ClsMethods.GenereateUniqueNumber(35);
                string apiRequest = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><billerInfoRequest></billerInfoRequest>";
                string apiResponse = Execute(Url + "extMdmCntrl/mdmRequest/xml", apiRequest, requestID);
                apiResponse = ConvertXMLToJson(apiResponse);
                var apiResult = new JavaScriptSerializer().Deserialize<CCCoverageResponseEntity>(apiResponse);
                if (apiResult.billerInfoResponse.responseCode == "000")
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = apiResult.billerInfoResponse.biller;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = apiResult.billerInfoResponse.errorInfo.error.errorMessage;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public string BillerParameters(int userID, string billerID)
        {
            try
            {
                string requestID = ClsMethods.GenereateUniqueNumber(35);
                string apiRequest = "<?xml version =\"1.0\" encoding=\"UTF-8\"?><billerInfoRequest><billerId>" + billerID + "</billerId></billerInfoRequest>";
                string apiResponse = Execute(Url + "extMdmCntrl/mdmRequest/xml", apiRequest, requestID);
                return ConvertXMLToJson(apiResponse);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public CCBillFetchResponseEntity SendBillFetchRequest(int userID, string agentID, string billerID, string mobileNo, string consumerNo, string param, string param3, string param4, string param5, string requestID)
        {
            CCBillFetchResponseEntity _response = new CCBillFetchResponseEntity();
            try
            {
                string ParamInput = string.Empty;
                if (!string.IsNullOrEmpty(consumerNo.Split('$')[0]))
                {
                    ParamInput = "<input><paramName>" + consumerNo.Split('$')[1] + "</paramName><paramValue>" + consumerNo.Split('$')[0] + "</paramValue></input>";
                }
                if (!string.IsNullOrEmpty(param.Split('$')[0]))
                {
                    ParamInput += "<input><paramName>" + param.Split('$')[1] + "</paramName><paramValue>" + param.Split('$')[0] + "</paramValue></input>";
                }
                if (!string.IsNullOrEmpty(param3.Split('$')[0]))
                {
                    ParamInput += "<input><paramName>" + param3.Split('$')[1] + "</paramName><paramValue>" + param3.Split('$')[0] + "</paramValue></input>";
                }
                if (!string.IsNullOrEmpty(param4.Split('$')[0]))
                {
                    ParamInput += "<input><paramName>" + param4.Split('$')[1] + "</paramName><paramValue>" + param4.Split('$')[0] + "</paramValue></input>";
                }
                if (!string.IsNullOrEmpty(param5.Split('$')[0]))
                {
                    ParamInput += "<input><paramName>" + param5.Split('$')[1] + "</paramName><paramValue>" + param5.Split('$')[0] + "</paramValue></input>";
                }
                string macAddress = ClsMethods.MacAddress();
                string apiUrl = Url + "extBillCntrl/billFetchRequest/xml";
                string apiRequest = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><billFetchRequest><agentId>" + agentID + "</agentId><agentDeviceInfo><ip>" + HttpContext.Current.Request.UserHostAddress + "</ip><initChannel>AGT</initChannel><mac>" + macAddress + "</mac></agentDeviceInfo><customerInfo><customerMobile>" + mobileNo + "</customerMobile><customerEmail></customerEmail><customerAdhaar></customerAdhaar><customerPan></customerPan></customerInfo><billerId>" + billerID + "</billerId><inputParams>" + ParamInput + "</inputParams></billFetchRequest>";
                string apiResponse = Execute(apiUrl, apiRequest, requestID);

                ClsMethods.RequestLogs(userID, mobileNo, clsVariables.ServiceType.BBPS, apiUrl, apiRequest, apiResponse, 0, requestID, "");
                apiResponse = ConvertXMLToJson(apiResponse);
                _response = new JavaScriptSerializer().Deserialize<CCBillFetchResponseEntity>(apiResponse);
            }
            catch (Exception ex)
            {
                _response.billFetchResponse.responseCode = "1";
                _response.billFetchResponse.errorInfo.error.errorMessage = ex.Message;
            }
            return _response;
        }


        #region Private Method
        private string Execute(string Url, string request, string requestID)
        {
            CCACrypto _avenues = new CCACrypto();

            String encRequest = _avenues.Encrypt(request, WorkingKey);   //Encrypted XML request using working key shared by Avenues.
            WebRequest wReq = WebRequest.Create(Url);
            HttpWebRequest httpReq = (HttpWebRequest)wReq;
            httpReq.Method = "POST";
            httpReq.ContentType = "application/x-www-form-urlencoded";
            string vData = "accessCode=" + AccessCode + "&encRequest=" + encRequest + "&requestId=" + requestID + "&ver=" + Version + "&instituteId=" + InstitutionID;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Stream sendStream = httpReq.GetRequestStream();
            StreamWriter strmWrtr = new StreamWriter(sendStream);
            strmWrtr.Write(vData);
            strmWrtr.Close();
            WebResponse wResp = null;
            StreamReader strmRdr = null;
            string sResult = "";
            try
            {
                wResp = httpReq.GetResponse();
                Stream respStrm = wResp.GetResponseStream();
                strmRdr = new StreamReader(respStrm);
                sResult = strmRdr.ReadToEnd();
                sResult = _avenues.Decrypt(sResult, WorkingKey);
            }
            catch (Exception Ex)
            {
                sResult = "error: " + Ex.Message;
            }
            return sResult;
        }

        private string ConvertXMLToJson(string request)
        {
            string response = "";
            if (!string.IsNullOrEmpty(request))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(request);
                response = JsonConvert.SerializeXmlNode(doc);
            }
            return response;
        }
        #endregion

    }
}
