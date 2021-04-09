using BusinessEntities;
using BusinessServices.Resource;
using DataModel;
using DataModel.UnitOfWork;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;

namespace BusinessServices.BillPayment
{
    public class PaymtBillPaymentAPI
    {
        #region Private Variable
        public string Url = ConfigurationManager.AppSettings["Paytm_Bill_Payment_URL"];
        public string Paytm_Username = ConfigurationManager.AppSettings["Paytm_Bill_Payment_UserName"];
        public string Paytm_Password = ConfigurationManager.AppSettings["Paytm_Bill_Payment_Password"];
        public string Paytm_AgentId = ConfigurationManager.AppSettings["Paytm_Bill_Payment_AgentId"];
        public string Paytm_NPCIAgentId = ConfigurationManager.AppSettings["Paytm_Bill_Payment_NPCI_AgentId"];
        public string Paytm_BBPSAgentId = ConfigurationManager.AppSettings["Paytm_Bill_Payment_BBPS_AgentId"];
        #endregion

        private PTMLoginResponseEntity GenerateToken()
        {
            PTMLoginResponseEntity _response = new PTMLoginResponseEntity();
            try
            {
                string endPoint = "v2/agents/" + Paytm_AgentId + "/login";
                PTMBillLoginRequestEntity _request = new PTMBillLoginRequestEntity()
                {
                    username = Paytm_Username,
                    password = Paytm_Password
                };
                var apiResponse = GetAPIConsume(endPoint, _request, null, Method.POST);
                var apiResult = new JavaScriptSerializer().Deserialize<PTMBillLoginResponseEntity>(apiResponse.ApiResponse);
                if (!string.IsNullOrEmpty(apiResult.token))
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Token = apiResult.token;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.Unknownresponse;
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        public APIBBPSFetchReponseEntity BillFetch(UnitOfWork _unitOfWork, int billerID, string number, string billerCode, string mobileNumber, string accountNo, string authenticator)
        {
            APIBBPSFetchReponseEntity _response = new APIBBPSFetchReponseEntity();
            try
            {
                var authResult = GenerateToken();
                if (authResult.StatusCode == clsVariables.APIStatus.Success)
                {
                    string billDetails = ""; string refNumber = ClsMethods.GenereateUniqueNumber(12);
                    var billDetailsResult = _unitOfWork.ValidateParameterRepository.GetMany(x => x.OperatorID == billerID && x.IsActive).ToList();
                    foreach (ValidateParameter row in billDetailsResult)
                    {
                        if (string.IsNullOrEmpty(billDetails))
                            billDetails = "\"" + row.Name + "\": \"" + number + "\"";
                        else if (!string.IsNullOrEmpty(accountNo))
                            billDetails += ",\"" + row.Name + "\": \"" + accountNo + "\"";
                        else if (!string.IsNullOrEmpty(authenticator))
                            billDetails += ",\"" + row.Name + "\": \"" + authenticator + "\"";
                    }
                    string endPoint = "v2/agents/" + Paytm_AgentId + "/bill/fetch";
                    string request = "{\"ref_id\": \"" + refNumber + "\",\"bill_details\": {" + billDetails + "},\"biller_details\": {\"biller_id\": \"" + billerCode + "\"},\"additional_info\": {\"agent_id\": \"" + Paytm_NPCIAgentId + "\",\"initiating_channel\": \"INT\",\"ip\": \"10.10.10.10\",\"mac\":\"" + ClsMethods.MacAddress() + "\",\"imei\": \"183000006490\",\"os\": \"Android\",\"app\": \"2.0\",\"customer_mobile\": \"" + mobileNumber + "\",\"si_txn\": \"Yes\",\"bbpsAgentId\": \"" + Paytm_BBPSAgentId + "\"}}";

                    var apiResponse = APIConsume(endPoint, request, authResult.Token);
                    ClsMethods.ResponseLogs(1, number, clsVariables.ServiceType.BBPS, apiResponse.ApiUrl, apiResponse.ApiRequest, apiResponse.ApiResponse, 0, refNumber, "");
                    JObject apiResult = JObject.Parse(apiResponse.ApiResponse);
                    if (apiResult["status"].ToString() == "SUCCESS")
                    {
                        _response.RefNumber = apiResult["ref_id"].ToString();
                        BBPSFetchReponseDataEntity _subResponse = new BBPSFetchReponseDataEntity();
                        _subResponse.Amount = apiResult["data"]["bill_details"]["amount"].ToString();
                        try
                        {
                            _subResponse.BillDueDate = apiResult["data"]["bill_details"]["Due Date"].ToString();
                        }
                        catch { _subResponse.BillDueDate = "NA"; }
                        try
                        {
                            _subResponse.BillerName = apiResult["data"]["bill_details"]["Name"].ToString();
                        }
                        catch
                        {
                            try
                            {
                                _subResponse.BillerName = apiResult["data"]["bill_details"]["Consumer Name"].ToString();
                            }
                            catch
                            {
                                _subResponse.BillerName = "NA";
                            }
                        }
                        try
                        {
                            _subResponse.BillerNumber = apiResult["data"]["bill_details"]["Bill Number"].ToString();
                        }
                        catch { _subResponse.BillerNumber = "NA"; }
                        try
                        {
                            _subResponse.BillDate = apiResult["data"]["bill_details"]["Bill Date"].ToString();
                        }
                        catch { _subResponse.BillDate = "NA"; }
                        try
                        {
                            _subResponse.Partial = apiResult["data"]["bill_details"]["editable"].ToString().ToUpper() == "TRUE" ? "Y" : "N";
                        }
                        catch { _subResponse.Partial = "N"; }
                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = Message.Su;
                        _response.Data = _subResponse;
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = apiResult["message"].ToString();
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = authResult.Message;
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        public PTMAPIBBPSBillReponseEntity BillPayment(UnitOfWork _unitOfWork, int billerID, string number, string billerCode, decimal amount, string mobileNumber, string accountNo, string authenticator, long orderId)
        {
            PTMAPIBBPSBillReponseEntity _response = new PTMAPIBBPSBillReponseEntity();
            try
            {
                var authResult = GenerateToken();
                if (authResult.StatusCode == clsVariables.APIStatus.Success)
                {
                    string billDetails = "";
                    var billDetailsResult = _unitOfWork.ValidateParameterRepository.GetMany(x => x.OperatorID == billerID && x.IsActive).ToList();
                    foreach (ValidateParameter row in billDetailsResult)
                    {
                        if (string.IsNullOrEmpty(billDetails))
                            billDetails = "\"" + row.Name + "\": \"" + number + "\"";
                        else if (!string.IsNullOrEmpty(accountNo))
                            billDetails += ",\"" + row.Name + "\": \"" + accountNo + "\"";
                        else if (!string.IsNullOrEmpty(authenticator))
                            billDetails += ",\"" + row.Name + "\": \"" + authenticator + "\"";
                    }
                    string endPoint = "v2/agents/" + Paytm_AgentId + "/bill/pay";
                    string request = "{\"ref_id\": \"" + orderId + "\",\"bill_details\": {" + billDetails + ",\"amount\": \"" + amount + "\"},\"biller_details\": {\"biller_id\": \"" + billerCode + "\"},\"additional_info\": {\"agent_id\": \"" + Paytm_NPCIAgentId + "\",\"initiating_channel\": \"INT\",\"ip\": \"10.10.10.10\",\"mac\":\"" + ClsMethods.MacAddress() + "\",\"imei\": \"183000006490\",\"os\": \"Android\",\"app_version\": \"2.0\",\"mobile\": \"" + mobileNumber + "\",\"payment_mode\": \"Credit Card\",\"payment_bank\": \"AXIS\",\"cou_cust_conv_fee\": \"0\",\"AuthCode\": \"1223\",\"CardNum\": \"4337-1234-5678-1234\",\"customer_mobile\":\"" + mobileNumber + "\",\"si_txn\":\"Yes\",\"app\":\"2.0\",\"bbpsAgentId\": \"" + Paytm_BBPSAgentId + "\"}}";
                    var apiResponse = APIConsume(endPoint, request, authResult.Token);
                    ClsMethods.ResponseLogs(1, number, clsVariables.ServiceType.BBPS, apiResponse.ApiUrl, apiResponse.ApiRequest, apiResponse.ApiResponse, orderId, "", "");
                    var apiResult = new JavaScriptSerializer().Deserialize<PaytmBillPaymentResponseEntity>(apiResponse.ApiResponse);
                    #region API Response
                    if (apiResult.status == "SUCCESS")
                    {
                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = apiResult.message;
                        _response.RefNumber = apiResult.bbpsRefId;
                    }
                    else if (apiResult.status == "FAILURE")
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = apiResult.message;
                        _response.RefNumber = apiResult.bbpsRefId;
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Pending;
                        _response.Message = apiResult.message;
                        _response.RefNumber = apiResult.bbpsRefId;
                    }
                    #endregion
                    #region Status Response
                    Thread.Sleep(1000);
                    var statusResult = StatusCheck(number, orderId);
                    if (statusResult.StatusCode == clsVariables.APIStatus.Success)
                    {
                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = statusResult.Message;
                        _response.RefNumber = statusResult.RefNumber;
                    }
                    else if (statusResult.StatusCode == clsVariables.APIStatus.Failed)
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = statusResult.Message;
                        _response.RefNumber = statusResult.RefNumber;
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Pending;
                        _response.Message = statusResult.Message;
                        _response.RefNumber = statusResult.RefNumber;
                    }
                    #endregion
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = authResult.Message;
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        public APIBBPSFetchReponseEntity StatusCheck(string number, long orderId)
        {
            APIBBPSFetchReponseEntity _response = new APIBBPSFetchReponseEntity();
            try
            {
                var authResult = GenerateToken();
                if (authResult.StatusCode == clsVariables.APIStatus.Success)
                {
                    string endPoint = "v2/agents/" + Paytm_AgentId + "/bill/status";
                    string request = "{\"ref_id\": \"" + orderId + "\"}";

                    var apiResponse = APIConsume(endPoint, request, authResult.Token);
                    ClsMethods.ResponseLogs(1, number, clsVariables.ServiceType.BBPS, apiResponse.ApiUrl, apiResponse.ApiRequest, apiResponse.ApiResponse, orderId, "", "");
                    var apiResult = new JavaScriptSerializer().Deserialize<PaytmBillPaymentResponseEntity>(apiResponse.ApiResponse);
                    if (apiResult.status == "SUCCESS")
                    {
                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = apiResult.message;
                        _response.RefNumber = apiResult.bbpsRefId;
                    }
                    else if (apiResult.status == "FAILURE")
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = apiResult.message;
                        _response.RefNumber = apiResult.bbpsRefId;
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Pending;
                        _response.Message = apiResult.message;
                        _response.RefNumber = apiResult.bbpsRefId;
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = authResult.Message;
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        public PTMBalanceResponseEntity Balance()
        {
            PTMBalanceResponseEntity _response = new PTMBalanceResponseEntity();
            try
            {
                var authResult = GenerateToken();
                if (authResult.StatusCode == clsVariables.APIStatus.Success)
                {
                    string endPoint = "v2/agents/" + Paytm_AgentId + "/fetchBalance";
                    var apiResponse = GetAPIConsume(endPoint, null, authResult.Token);
                    var apiResult = new JavaScriptSerializer().Deserialize<PTMAPIBalanceResponseEntity>(apiResponse.ApiResponse);
                    if (apiResult.code == 200)
                    {
                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = Message.Su;
                        _response.Balance = apiResult.balance;
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = Message.Unknownresponse;
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = authResult.Message;
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        #region API Consume
        private PayBillAPIResponseEntity APIConsume(string endPoint, string apiRequest, string token, Method method = Method.POST)
        {
            PayBillAPIResponseEntity _response = new PayBillAPIResponseEntity()
            {
                ApiRequest = apiRequest,
                ApiUrl = Url + endPoint
            };
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(_response.ApiUrl);
                var request = new RestRequest(method);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("jwt_token", token);
                request.AddParameter("application/json", apiRequest, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                string responseData = response.Content.ToString();
                _response.ApiResponse = responseData;
            }
            catch (WebException ex)
            {
                Stream s = ex.Response.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                _response.ApiResponse = sr.ReadToEnd();
            }
            return _response;
        }
        private PayBillAPIResponseEntity GetAPIConsume(string endPoint, object apiRequest, string token, Method method = Method.GET)
        {
            PayBillAPIResponseEntity _response = new PayBillAPIResponseEntity()
            {
                ApiRequest = apiRequest == null ? "" : new JavaScriptSerializer().Serialize(apiRequest),
                ApiUrl = Url + endPoint
            };
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(_response.ApiUrl);
                var request = new RestRequest(method);

                request.AddHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(token))
                    request.AddHeader("jwt_token", token);
                if (apiRequest != null)
                    request.AddJsonBody(apiRequest);

                IRestResponse response = client.Execute(request);
                string responseData = response.Content.ToString();
                _response.ApiResponse = responseData;
            }
            catch (WebException ex)
            {
                Stream s = ex.Response.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                _response.ApiResponse = sr.ReadToEnd();
            }
            return _response;
        }
        #endregion
    }
}
