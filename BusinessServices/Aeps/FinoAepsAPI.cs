using BusinessEntities;
using BusinessServices.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace BusinessServices.Aeps
{
    public class FinoAepsAPI
    {
        #region Private Variable
        private readonly string Fino_ClientID = ConfigurationManager.AppSettings["V1Fino_Client_ID_AEPS"];
        private readonly string Fino_AuthKey = ConfigurationManager.AppSettings["V1Fino_Auth_Key_AEPS"];
        private readonly string Fino_Header_Key = ConfigurationManager.AppSettings["V1Fino_Header_Key_AEPS"];
        private string Fino_Request_Key = ConfigurationManager.AppSettings["V1Fino_Request_Key_AEPS"];
        private string Fino_Url_Status = ConfigurationManager.AppSettings["V1Fino_URL_AEPS_Status"];
        private string Fino_Url = ConfigurationManager.AppSettings["V1Fino_URL_AEPS"];

        private readonly string UATFino_ClientID = ConfigurationManager.AppSettings["UATFino_Client_ID_AEPS"];
        private readonly string UATFino_AuthKey = ConfigurationManager.AppSettings["UATFino_Auth_Key_AEPS"];
        private readonly string UATFino_Header_Key = ConfigurationManager.AppSettings["UATFino_Header_Key_AEPS"];
        private readonly string UATFino_Request_Key = ConfigurationManager.AppSettings["UATFino_Request_Key_AEPS"];
        private readonly string UATFino_Url_Status = ConfigurationManager.AppSettings["UATFino_URL_AEPS_Status"];
        private readonly string UATFino_Url = ConfigurationManager.AppSettings["UATFino_URL_AEPS"];
        #endregion

        public APIReponseEntity EncryptionRequestKey(int sessionID)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                FNOAPIResponseEntity apiResponse = new FNOAPIResponseEntity();
                if (sessionID == 2)
                {
                    Fino_Url = UATFino_Url;
                    Fino_Request_Key = UATFino_Request_Key;
                    apiResponse = UATAPIConsume(Fino_Url + "ProcessRequest/GetEncKey", "", Fino_Request_Key);
                }
                else
                {
                    apiResponse = APIConsume(Fino_Url + "ProcessRequest/GetEncKey", "", Fino_Request_Key);
                }
                ClsMethods.AEPSRequestLogs(1, "EncKey", clsVariables.ServiceType.AEPS, apiResponse.ApiUrl, apiResponse.ApiRequest, apiResponse.ApiResponse, 0, string.Empty, string.Empty);
                var apiResult = new JavaScriptSerializer().Deserialize<FNOEncryptionKeyResponseEntity>(apiResponse.ApiResponse);
                if (apiResult.ResponseCode == 0)
                {
                    var responseData = FinoEncryption.OpenSSLDecrypt(apiResult.ResponseData, Fino_Request_Key);
                    var result = new JavaScriptSerializer().Deserialize<FNOEncryptionKeyResponseDataEntity>(responseData);

                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = apiResult.MessageString;
                    _response.Data = result.EncrytionKey;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = apiResult.MessageString;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return _response;
        }

        #region Private Auth Key
        public FNOBalanceCashWithdrawalEntity CashWithdrawal(int userID, string merchantID, string mobileNo, string aadharNo, decimal amount, string bankName, string pidData, string nBIN, string latitude, string longitude, string encKey, long txnID)
        {
            FNOBalanceCashWithdrawalEntity _response = new FNOBalanceCashWithdrawalEntity();
            try
            {
                string checkSum = FinoEncryption.GetSha256FromString(txnID + "+" + amount + "+" + aadharNo);
                FNOCashWithdrawalRequestEntity _request = new FNOCashWithdrawalRequestEntity()
                {
                    MerchantID = merchantID,
                    Version = "1001",
                    ServiceID = "151",
                    ClientRefID = txnID.ToString(),
                    MobileNo = mobileNo,
                    AadharNo = aadharNo,
                    TotalAmount = Convert.ToInt32(amount),
                    BankName = bankName,
                    PidData = FinoEncryption.Base64Encode(pidData),
                    RC = "Y",
                    NBIN = nBIN,
                    TerminalId = "",
                    IPAddress = System.Web.HttpContext.Current.Request.UserHostAddress,
                    Latitude = latitude,
                    Longitude = longitude,
                    IMEI_MAC = "",
                    DeviceNo = "",
                    CheckSum = checkSum
                };
                _response.ApiResponse = APIConsumeWithdrawal(Fino_Url + "ProcessRequest/AEPSCashWithdraw", _request, encKey);
                if (_response.ApiResponse.StatusCode == clsVariables.APIStatus.Success)
                {
                    #region Decryption Method
                    var apiResult = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseEntity>(_response.ApiResponse.ApiResponse);
                    if (apiResult.ResponseCode == "0")
                    {
                        _response.StatusCode = clsVariables.APIStatus.Pending;
                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                var responseData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataEntity>(apiResult.ResponseData);
                                if (!string.IsNullOrEmpty(responseData.ClientRes))
                                {
                                    string clientRes = FinoEncryption.OpenSSLDecrypt(responseData.ClientRes, encKey);
                                    if (!string.IsNullOrEmpty(clientRes))
                                    {
                                        _response.ApiResponse.ApiResponse += " / " + clientRes;
                                        var clientResData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataDetailsEntity>(clientRes);
                                        if (clientResData.Status == "Success")
                                        {
                                            _response.StatusCode = clsVariables.APIStatus.Success;
                                        }
                                        _response.Data = clientResData;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    else if (new[] { "1", "-5", "1046", "1040", "1010", "1003", "1023", "1032", "1024", "1002", "1019", "1025", "1022", "51", "30" }.Contains(apiResult.ResponseCode))
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                var responseData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataEntity>(apiResult.ResponseData);
                                if (!string.IsNullOrEmpty(responseData.ClientRes))
                                {
                                    var clientRes = FinoEncryption.OpenSSLDecrypt(responseData.ClientRes, encKey);
                                    if (!string.IsNullOrEmpty(clientRes))
                                    {
                                        _response.ApiResponse.ApiResponse += " / " + clientRes;
                                        var clientResData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataDetailsEntity>(clientRes);
                                        _response.Data = clientResData;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Pending;
                        _response.Message = apiResult.MessageString;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                var responseData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataEntity>(apiResult.ResponseData);
                                if (!string.IsNullOrEmpty(responseData.ClientRes))
                                {
                                    var clientRes = FinoEncryption.OpenSSLDecrypt(responseData.ClientRes, encKey);
                                    if (!string.IsNullOrEmpty(clientRes))
                                    {
                                        _response.ApiResponse.ApiResponse += " / " + clientRes;
                                        var clientResData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataDetailsEntity>(clientRes);
                                        if (clientResData != null && clientResData.Status == "Failed")
                                        {
                                            _response.StatusCode = clsVariables.APIStatus.Failed;
                                            _response.Message = apiResult.MessageString;
                                        }
                                        _response.Data = clientResData;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    #endregion
                }
                else
                {
                    FNOCashWithdrawalResponseDataDetailsEntity _subResponse = new FNOCashWithdrawalResponseDataDetailsEntity()
                    {
                        Amount = Convert.ToDouble(amount),
                        AdhaarNo = "XXXXXXXX" + aadharNo.Substring(8, 4),
                        TxnDate = "",
                        TxnTime = "",
                        BankName = bankName,
                        RRN = "",
                        Status = "PENDING",
                        CustomerMobile = mobileNo,
                        AvailableBalance = "",
                        LedgerBalance = ""
                    };
                    _response.StatusCode = clsVariables.APIStatus.Exception;
                    _response.Message = _response.ApiResponse.ApiResponse;
                    _response.Data = _subResponse;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            ClsMethods.AEPSRequestLogs(userID, mobileNo, "Cash Withdrawal", _response.ApiResponse.ApiUrl, _response.ApiResponse.ApiRequest, _response.ApiResponse.ApiResponse, txnID, string.Empty, string.Empty);
            return _response;
        }

        public FNOBalanceCashWithdrawalEntity BalanceInquiry(int userID, string merchantID, string mobileNo, string aadharNo, string bankName, string pidData, string nBIN, string latitude, string longitude, string clientRefID, string encKey, long txnID)
        {
            FNOBalanceCashWithdrawalEntity _response = new FNOBalanceCashWithdrawalEntity();
            try
            {
                string checkSum = FinoEncryption.GetSha256FromString(txnID + "+0+" + aadharNo);
                FNOCashWithdrawalRequestEntity _request = new FNOCashWithdrawalRequestEntity()
                {
                    MerchantID = merchantID,
                    Version = "1001",
                    ServiceID = "152",
                    ClientRefID = txnID.ToString(),
                    MobileNo = mobileNo,
                    AadharNo = aadharNo,
                    TotalAmount = 0,
                    BankName = bankName,
                    PidData = FinoEncryption.Base64Encode(pidData),
                    RC = "Y",
                    NBIN = nBIN,
                    TerminalId = "",
                    IPAddress = System.Web.HttpContext.Current.Request.UserHostAddress,
                    Latitude = latitude,
                    Longitude = longitude,
                    IMEI_MAC = "",
                    DeviceNo = "",
                    CheckSum = checkSum
                };

                _response.ApiResponse = APIConsumeInquiry(Fino_Url + "ProcessRequest/AEPSBalanceInquiry", _request, encKey);
                if (_response.ApiResponse.StatusCode == clsVariables.APIStatus.Success)
                {
                    #region Decryption Method
                    var apiResult = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseEntity>(_response.ApiResponse.ApiResponse);
                    if (apiResult.ResponseCode == "0")
                    {
                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                var responseData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataEntity>(apiResult.ResponseData);
                                if (!string.IsNullOrEmpty(responseData.ClientRes))
                                {
                                    string clientRes = FinoEncryption.OpenSSLDecrypt(responseData.ClientRes, encKey);
                                    if (!string.IsNullOrEmpty(clientRes))
                                    {
                                        _response.ApiResponse.ApiResponse += " / " + clientRes;
                                        var clientResData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataDetailsEntity>(clientRes);
                                        _response.Data = clientResData;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                var responseData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataEntity>(apiResult.ResponseData);
                                if (!string.IsNullOrEmpty(responseData.ClientRes))
                                {
                                    var clientRes = FinoEncryption.OpenSSLDecrypt(responseData.ClientRes, encKey);
                                    if (!string.IsNullOrEmpty(clientRes))
                                    {
                                        _response.ApiResponse.ApiResponse += " / " + clientRes;
                                        var clientResData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataDetailsEntity>(clientRes);
                                        _response.Data = clientResData;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    #endregion
                }
                else
                {
                    FNOCashWithdrawalResponseDataDetailsEntity _subResponse = new FNOCashWithdrawalResponseDataDetailsEntity()
                    {
                        Amount = 0,
                        AdhaarNo = "XXXXXXXX" + aadharNo.Substring(8, 4),
                        TxnDate = "",
                        TxnTime = "",
                        BankName = bankName,
                        RRN = "",
                        Status = "PENDING",
                        CustomerMobile = mobileNo,
                        AvailableBalance = "",
                        LedgerBalance = ""
                    };
                    _response.StatusCode = clsVariables.APIStatus.Exception;
                    _response.Message = _response.ApiResponse.ApiResponse;
                    _response.Data = _subResponse;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            ClsMethods.AEPSRequestLogs(userID, mobileNo, "Balance Inquiry", _response.ApiResponse.ApiUrl, _response.ApiResponse.ApiRequest, _response.ApiResponse.ApiResponse, txnID, clientRefID, string.Empty);
            return _response;
        }

        public FNOStatusInquiryResponseEntity TransactionInquiry(int userID, long txnID, string encKey)
        {
            FNOStatusInquiryResponseEntity _response = new FNOStatusInquiryResponseEntity();
            try
            {
                string checkSum = FinoEncryption.GetSha256FromString(txnID + "+159+" + "1001");
                FNOStatusCheckRequestEntity _request = new FNOStatusCheckRequestEntity()
                {
                    Version = "1001",
                    SERVICEID = "159",
                    ClientRefID = txnID.ToString(),
                    CheckSum = checkSum
                };
                string request_json = new JavaScriptSerializer().Serialize(_request);
                _response.ApiResponse = APIConsume(Fino_Url_Status, request_json, encKey);
                if (_response.ApiResponse.StatusCode == clsVariables.APIStatus.Success)
                {
                    #region Decryption Method
                    var apiResult = new JavaScriptSerializer().Deserialize<FNOTransactionStatusCheckResponseEntity>(_response.ApiResponse.ApiResponse);
                    if (apiResult.ResponseCode == "0")
                    {

                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                string clientRes = FinoEncryption.OpenSSLDecrypt(apiResult.ResponseData, encKey);
                                if (!string.IsNullOrEmpty(clientRes))
                                {
                                    _response.ApiResponse.ApiResponse += " / " + clientRes;
                                    var clientResData = new JavaScriptSerializer().Deserialize<FNOTransactionStatusCheckResponseDataEntity>(clientRes);
                                    _response.Data = clientResData;
                                    if (clientResData.TransactionStatus == "Success")
                                    {
                                        _response.Message = clientResData.TransactionStatus;
                                        _response.StatusCode = clsVariables.APIStatus.Success;
                                    }
                                    else if (clientResData.TransactionStatus == "Failed")
                                    {
                                        _response.Message = clientResData.TransactionStatus;
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                    }
                                    else
                                    {
                                        _response.Message = clientResData.TransactionStatus;
                                        _response.StatusCode = clsVariables.APIStatus.Pending;
                                    }
                                }
                                else
                                    _response.StatusCode = clsVariables.APIStatus.Pending;
                            }
                            else
                                _response.StatusCode = clsVariables.APIStatus.Pending;
                        }
                        catch { _response.StatusCode = clsVariables.APIStatus.Pending; }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Pending;
                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                string clientRes = FinoEncryption.OpenSSLDecrypt(apiResult.ResponseData, encKey);
                                if (!string.IsNullOrEmpty(clientRes))
                                {
                                    _response.ApiResponse.ApiResponse += " / " + clientRes;
                                    var clientResData = new JavaScriptSerializer().Deserialize<FNOTransactionStatusCheckResponseDataEntity>(clientRes);
                                    _response.Data = clientResData;
                                    _response.Message += " " + clientResData.TransactionStatus;
                                }
                            }
                        }
                        catch { }
                    }
                    #endregion
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Exception;
                    _response.Message = _response.ApiResponse.ApiResponse;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            ClsMethods.AEPSRequestLogs(userID, txnID.ToString(), "Transaction Status", _response.ApiResponse.ApiUrl, _response.ApiResponse.ApiRequest, _response.ApiResponse.ApiResponse, txnID, string.Empty, string.Empty);
            return _response;
        }

        public FNOBalanceCashWithdrawalEntity AadharPay(int userID, string merchantID, string mobileNo, string aadharNo, string bankName, string pidData, string nBIN, string latitude, string longitude, string clientRefID, string encKey, long txnID)
        {
            FNOBalanceCashWithdrawalEntity _response = new FNOBalanceCashWithdrawalEntity();
            try
            {
                string checkSum = FinoEncryption.GetSha256FromString(txnID + "+0+" + aadharNo);
                FNOCashWithdrawalRequestEntity _request = new FNOCashWithdrawalRequestEntity()
                {
                    MerchantID = merchantID,
                    Version = "1001",
                    ServiceID = "176",
                    ClientRefID = txnID.ToString(),
                    MobileNo = mobileNo,
                    AadharNo = aadharNo,
                    TotalAmount = 0,
                    BankName = bankName,
                    PidData = FinoEncryption.Base64Encode(pidData),
                    RC = "Y",
                    NBIN = nBIN,
                    TerminalId = "",
                    IPAddress = System.Web.HttpContext.Current.Request.UserHostAddress,
                    Latitude = latitude,
                    Longitude = longitude,
                    IMEI_MAC = "",
                    DeviceNo = "",
                    CheckSum = checkSum
                };

                _response.ApiResponse = APIConsumeInquiry(Fino_Url + "ProcessRequest/AdharPay", _request, encKey);
                if (_response.ApiResponse.StatusCode == clsVariables.APIStatus.Success)
                {
                    #region Decryption Method
                    var apiResult = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseEntity>(_response.ApiResponse.ApiResponse);
                    if (apiResult.ResponseCode == "0")
                    {
                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                var responseData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataEntity>(apiResult.ResponseData);
                                if (!string.IsNullOrEmpty(responseData.ClientRes))
                                {
                                    string clientRes = FinoEncryption.OpenSSLDecrypt(responseData.ClientRes, encKey);
                                    if (!string.IsNullOrEmpty(clientRes))
                                    {
                                        _response.ApiResponse.ApiResponse += " / " + clientRes;
                                        var clientResData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataDetailsEntity>(clientRes);
                                        _response.Data = clientResData;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                var responseData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataEntity>(apiResult.ResponseData);
                                if (!string.IsNullOrEmpty(responseData.ClientRes))
                                {
                                    var clientRes = FinoEncryption.OpenSSLDecrypt(responseData.ClientRes, encKey);
                                    if (!string.IsNullOrEmpty(clientRes))
                                    {
                                        _response.ApiResponse.ApiResponse += " / " + clientRes;
                                        var clientResData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataDetailsEntity>(clientRes);
                                        _response.Data = clientResData;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    #endregion
                }
                else
                {
                    FNOCashWithdrawalResponseDataDetailsEntity _subResponse = new FNOCashWithdrawalResponseDataDetailsEntity()
                    {
                        Amount = 0,
                        AdhaarNo = "XXXXXXXX" + aadharNo.Substring(8, 4),
                        TxnDate = "",
                        TxnTime = "",
                        BankName = bankName,
                        RRN = "",
                        Status = "PENDING",
                        CustomerMobile = mobileNo,
                        AvailableBalance = "",
                        LedgerBalance = ""
                    };
                    _response.StatusCode = clsVariables.APIStatus.Exception;
                    _response.Message = _response.ApiResponse.ApiResponse;
                    _response.Data = _subResponse;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            ClsMethods.AEPSRequestLogs(userID, mobileNo, "Balance Inquiry", _response.ApiResponse.ApiUrl, _response.ApiResponse.ApiRequest, _response.ApiResponse.ApiResponse, txnID, clientRefID, string.Empty);
            return _response;
        }

        public FNOMiniStatementEntity MiniStatement(int userID, string merchantID, string mobileNo, string aadharNo, string bankName, string pidData, string nBIN, string latitude, string longitude, string clientRefID, string encKey, long txnID)
        {
            FNOMiniStatementEntity _response = new FNOMiniStatementEntity();
            try
            {
                string checkSum = FinoEncryption.GetSha256FromString(txnID + "+0+" + aadharNo);
                FNOCashWithdrawalRequestEntity _request = new FNOCashWithdrawalRequestEntity()
                {
                    MerchantID = merchantID,
                    Version = "1001",
                    ServiceID = "177",
                    ClientRefID = txnID.ToString(),
                    MobileNo = mobileNo,
                    AadharNo = aadharNo,
                    TotalAmount = 0,
                    BankName = bankName,
                    PidData = FinoEncryption.Base64Encode(pidData),
                    RC = "Y",
                    NBIN = nBIN,
                    TerminalId = "",
                    IPAddress = System.Web.HttpContext.Current.Request.UserHostAddress,
                    Latitude = latitude,
                    Longitude = longitude,
                    IMEI_MAC = "",
                    DeviceNo = "",
                    CheckSum = checkSum
                };

                _response.ApiResponse = APIConsumeStatement(UATFino_Url + "ProcessRequest/AEPSMiniStatement", _request, encKey);
                if (_response.ApiResponse.StatusCode == clsVariables.APIStatus.Success)
                {
                    #region Decryption Method
                    var apiResult = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseEntity>(_response.ApiResponse.ApiResponse);
                    if (apiResult.ResponseCode == "0")
                    {
                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                var responseData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataEntity>(apiResult.ResponseData);
                                if (!string.IsNullOrEmpty(responseData.ClientRes))
                                {
                                    string clientRes = FinoEncryption.OpenSSLDecrypt(responseData.ClientRes, encKey);
                                    if (!string.IsNullOrEmpty(clientRes))
                                    {
                                        _response.ApiResponse.ApiResponse += " / " + clientRes;
                                        var clientResData = new JavaScriptSerializer().Deserialize<FNOCashMiniStatementResponseEntity>(clientRes);
                                        _response.Data = clientResData;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = apiResult.MessageString;
                        _response.APIRef = apiResult.RequestID;
                        try
                        {
                            if (!string.IsNullOrEmpty(apiResult.ResponseData))
                            {
                                var responseData = new JavaScriptSerializer().Deserialize<FNOCashWithdrawalResponseDataEntity>(apiResult.ResponseData);
                                if (!string.IsNullOrEmpty(responseData.ClientRes))
                                {
                                    var clientRes = FinoEncryption.OpenSSLDecrypt(responseData.ClientRes, encKey);
                                    if (!string.IsNullOrEmpty(clientRes))
                                    {
                                        _response.ApiResponse.ApiResponse += " / " + clientRes;
                                        var clientResData = new JavaScriptSerializer().Deserialize<FNOCashMiniStatementResponseEntity>(clientRes);
                                        _response.Data = clientResData;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    #endregion
                }
                else
                {
                    FNOCashMiniStatementResponseEntity _subResponse = new FNOCashMiniStatementResponseEntity()
                    {
                        Balance = 0,
                        AdhaarNo = "XXXXXXXX" + aadharNo.Substring(8, 4),
                        localDate = "",
                        localTime = "",
                        RRN = "",
                        UIDAuthCode = "",
                        CardAccceptorCode = mobileNo,
                        NameLocation = "",
                        txnCurCode = ""
                    };
                    _response.StatusCode = clsVariables.APIStatus.Exception;
                    _response.Message = _response.ApiResponse.ApiResponse;
                    _response.Data = _subResponse;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            ClsMethods.AEPSRequestLogs(userID, mobileNo, "Balance Inquiry", _response.ApiResponse.ApiUrl, _response.ApiResponse.ApiRequest, _response.ApiResponse.ApiResponse, txnID, clientRefID, string.Empty);
            return _response;
        }
        #endregion

        #region Private Method
        private FNOAPIResponseEntity APIConsume(string endPoint, string requestBody, string encKey)
        {
            FNOAPIResponseEntity _response = new FNOAPIResponseEntity();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                _response.ApiUrl = endPoint;
                _response.ApiRequest = requestBody;
                _response.HeaderEncryption = APIHeader();
                if (!string.IsNullOrEmpty(requestBody))
                    _response.BodyEncryption = "\"" + FinoEncryption.OpenSSLEncrypt(requestBody, encKey) + "\"";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
                request.Headers.Add("Authentication", _response.HeaderEncryption);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter2.Write(_response.BodyEncryption);
                }
                //  This actually does the request and gets the response back;
                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    _response.ApiResponse = responseReader.ReadToEnd();
                }
                _response.StatusCode = clsVariables.APIStatus.Success;
            }
            catch (Exception Ex)
            {
                _response.ApiResponse += Ex.Message;
                _response.StatusCode = clsVariables.APIStatus.Exception;
            }
            return _response;
        }

        private FNOAPIResponseEntity UATAPIConsume(string endPoint, string requestBody, string encKey)
        {
            FNOAPIResponseEntity _response = new FNOAPIResponseEntity();
            try
            {
                _response.ApiUrl = endPoint;
                _response.ApiRequest = requestBody;
                _response.HeaderEncryption = UATAPIHeader();
                if (!string.IsNullOrEmpty(requestBody))
                    _response.BodyEncryption = "\"" + FinoEncryption.OpenSSLEncrypt(requestBody, encKey) + "\"";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
                request.Headers.Add("Authentication", _response.HeaderEncryption);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter2.Write(_response.BodyEncryption);
                }
                //  This actually does the request and gets the response back;
                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    _response.ApiResponse = responseReader.ReadToEnd();
                }
                _response.StatusCode = clsVariables.APIStatus.Success;
            }
            catch (Exception Ex)
            {
                _response.ApiResponse += Ex.Message;
                _response.StatusCode = clsVariables.APIStatus.Exception;
            }
            return _response;
        }

        private FNOAPIResponseEntity APIConsumeWithdrawal(string endPoint, FNOCashWithdrawalRequestEntity requestobj, string encKey)
        {
            FNOAPIResponseEntity _response = new FNOAPIResponseEntity();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                string requestBody = new JavaScriptSerializer().Serialize(requestobj);
                requestobj.PidData = "XXXX";//remove pid data
                requestobj.AadharNo = "XXXXXXXX" + requestobj.AadharNo.Substring(8, 4);

                _response.ApiUrl = endPoint;
                _response.ApiRequest = new JavaScriptSerializer().Serialize(requestobj);
                _response.HeaderEncryption = APIHeader();
                if (!string.IsNullOrEmpty(requestBody))
                    _response.BodyEncryption = "\"" + FinoEncryption.OpenSSLEncrypt(requestBody, encKey) + "\"";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
                request.Headers.Add("Authentication", _response.HeaderEncryption);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter2.Write(_response.BodyEncryption);
                }
                //  This actually does the request and gets the response back;
                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    _response.ApiResponse = responseReader.ReadToEnd();
                }
                _response.StatusCode = clsVariables.APIStatus.Success;
            }
            catch (Exception Ex)
            {
                _response.ApiResponse += Ex.Message;
                _response.StatusCode = clsVariables.APIStatus.Exception;
            }
            return _response;
        }

        private FNOAPIResponseEntity APIConsumeInquiry(string endPoint, FNOCashWithdrawalRequestEntity requestobj, string encKey)
        {
            FNOAPIResponseEntity _response = new FNOAPIResponseEntity();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                string requestBody = new JavaScriptSerializer().Serialize(requestobj);
                requestobj.PidData = "XXXX";
                requestobj.AadharNo = "XXXXXXXX" + requestobj.AadharNo.Substring(8, 4);
                _response.ApiUrl = endPoint;
                _response.ApiRequest = new JavaScriptSerializer().Serialize(requestobj);
                _response.HeaderEncryption = APIHeader();
                if (!string.IsNullOrEmpty(requestBody))
                    _response.BodyEncryption = "\"" + FinoEncryption.OpenSSLEncrypt(requestBody, encKey) + "\"";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
                request.Headers.Add("Authentication", _response.HeaderEncryption);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter2.Write(_response.BodyEncryption);
                }
                //  This actually does the request and gets the response back;
                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    _response.ApiResponse = responseReader.ReadToEnd();
                }
                _response.StatusCode = clsVariables.APIStatus.Success;
            }
            catch (Exception Ex)
            {
                _response.ApiResponse += Ex.Message;
                _response.StatusCode = clsVariables.APIStatus.Exception;
            }
            return _response;
        }

        private FNOAPIResponseEntity APIConsumeStatement(string endPoint, FNOCashWithdrawalRequestEntity requestobj, string encKey)
        {
            FNOAPIResponseEntity _response = new FNOAPIResponseEntity();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                string requestBody = new JavaScriptSerializer().Serialize(requestobj);
                requestobj.PidData = "XXXX";
                requestobj.AadharNo = "XXXXXXXX" + requestobj.AadharNo.Substring(8, 4);
                _response.ApiUrl = endPoint;
                _response.ApiRequest = new JavaScriptSerializer().Serialize(requestobj);
                _response.HeaderEncryption = UATAPIHeader();
                if (!string.IsNullOrEmpty(requestBody))
                    _response.BodyEncryption = "\"" + FinoEncryption.OpenSSLEncrypt(requestBody, encKey) + "\"";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
                request.Headers.Add("Authentication", _response.HeaderEncryption);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter2.Write(_response.BodyEncryption);
                }
                //  This actually does the request and gets the response back;
                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    _response.ApiResponse = responseReader.ReadToEnd();
                }
                _response.StatusCode = clsVariables.APIStatus.Success;
            }
            catch (Exception Ex)
            {
                _response.ApiResponse += Ex.Message;
                _response.StatusCode = clsVariables.APIStatus.Exception;
            }
            return _response;
        }

        private string APIHeader()
        {
            Dictionary<string, string> innerHeader = new Dictionary<string, string>();
            innerHeader.Add("ClientId", Fino_ClientID);
            innerHeader.Add("AuthKey", Fino_AuthKey);
            String Header_json = new JavaScriptSerializer().Serialize(innerHeader);
            Header_json = FinoEncryption.OpenSSLEncrypt(Header_json, Fino_Header_Key);
            return Header_json;
        }

        private string UATAPIHeader()
        {
            Dictionary<string, string> innerHeader = new Dictionary<string, string>();
            innerHeader.Add("ClientId", UATFino_ClientID);
            innerHeader.Add("AuthKey", UATFino_AuthKey);
            String Header_json = new JavaScriptSerializer().Serialize(innerHeader);
            Header_json = FinoEncryption.OpenSSLEncrypt(Header_json, UATFino_Header_Key);
            return Header_json;
        }

        #endregion
    }
}
