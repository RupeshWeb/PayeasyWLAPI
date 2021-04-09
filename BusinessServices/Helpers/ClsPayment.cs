using AutoMapper;
using BusinessEntities;
using BusinessServices;
using BusinessServices.Resource;
using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public static class ClsPayment
{
    private static readonly Object thisLock = new Object();
    private static readonly Object thisLockMain = new Object();
    public static decimal GetBalance(UnitOfWork _unitOfWork, int userId)
    {
        List<getDmtBalanceByID_Result> result = _unitOfWork.SqlQuery<getDmtBalanceByID_Result>("getDmtBalanceByID @UserID", new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userId }).ToList();
        if (result.Any())
        {
            return result[0].NewBal;
        }
        return 0;
    }
    public static decimal GetMainBalance(UnitOfWork _unitOfWork, int userId)
    {
        List<getBalanceByID_Result> result = _unitOfWork.SqlQuery<getBalanceByID_Result>("getBalanceByID @UserID", new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userId }).ToList();
        if (result.Any())
        {
            return result[0].NewBal;
        }
        return 0;
    }

    public static APIDetailEntity GetAPI(UnitOfWork _unitOfWork, int operatorId, int schemeId, short apiID, short mode)
    {
        APIDetailEntity apidetail = new APIDetailEntity();
        List<APIDetail_Result> result = _unitOfWork.SqlQuery<APIDetail_Result>("APIDetail @OperatorId,@SchemeId,@ApiID,@OutPut", new SqlParameter("OperatorId", System.Data.SqlDbType.Int) { Value = operatorId }, new SqlParameter("SchemeId", System.Data.SqlDbType.Int) { Value = schemeId }, new SqlParameter("ApiID", System.Data.SqlDbType.SmallInt) { Value = apiID }, new SqlParameter("OutPut", System.Data.SqlDbType.SmallInt) { Value = mode }).ToList();
        if (result.Any())
        {
            Mapper.CreateMap<APIDetail_Result, APIDetailEntity>();
            apidetail = Mapper.Map<List<APIDetail_Result>, List<APIDetailEntity>>(result).FirstOrDefault();

            return apidetail;
        }
        return apidetail;
    }

    public static decimal ServiceCharge(UnitOfWork _unitOfWork, int operatorId, int schemeId, decimal amount)
    {
        var commission = _unitOfWork.CommissionRepository.Get(x => x.SchemeID == x.SchemeID && x.OperatorID == operatorId);
        if (commission != null)
        {
            decimal charge = (commission.ServiceChargePer * amount) / 100;
            return charge + commission.ServiceChargeVal;
        }
        else
            return 0;
    }

    public static bool DoPaymentAEPS(UnitOfWork _unitOfWork, int CreditUserId, int DebitUserId, Decimal Amount, String Remarks, String PaymentType, long payRefId)
    {
        lock (thisLock)
        {
            try
            {
                if (Amount <= 0)
                {
                    return false;
                }
                bool isPayment = false;
                List<int> result = _unitOfWork.SqlQuery<int>("DodmtPayment @CreditUserId,@DebitUserId,@Amount,@PaymentType,@Remarks,@IP,@RefrenceID,@Output"
                    , new SqlParameter("CreditUserId", System.Data.SqlDbType.Int) { Value = CreditUserId }
                    , new SqlParameter("DebitUserId", System.Data.SqlDbType.Int) { Value = DebitUserId }
                    , new SqlParameter("Amount", System.Data.SqlDbType.Decimal) { Value = Amount }
                    , new SqlParameter("PaymentType", System.Data.SqlDbType.VarChar) { Value = PaymentType }
                    , new SqlParameter("Remarks", System.Data.SqlDbType.VarChar) { Value = Remarks }
                    , new SqlParameter("IP", System.Data.SqlDbType.VarChar) { Value = System.Web.HttpContext.Current.Request.UserHostAddress }
                    , new SqlParameter("RefrenceID", System.Data.SqlDbType.BigInt) { Value = payRefId }
                    , new SqlParameter("Output", System.Data.SqlDbType.Bit) { Value = true }
                    ).ToList();
                if (result.Count > 0 && result[0] == 1)
                    isPayment = true;
                else
                    isPayment = false;
                return isPayment;
            }
            catch (Exception) { return false; }
        }
    }
    public static bool DoPaymentMain(UnitOfWork _unitOfWork, int CreditUserId, int DebitUserId, Decimal Amount, String Remarks, String PaymentType, long payRefId)
    {
        lock (thisLockMain)
        {
            try
            {
                if (Amount <= 0)
                {
                    return false;
                }
                bool isPayment = false;
                List<int> result = _unitOfWork.SqlQuery<int>("DowlPayment @CreditUserId,@DebitUserId,@Amount,@PaymentType,@Remarks,@IP,@RefrenceID,@Output"
                    , new SqlParameter("CreditUserId", System.Data.SqlDbType.Int) { Value = CreditUserId }
                    , new SqlParameter("DebitUserId", System.Data.SqlDbType.Int) { Value = DebitUserId }
                    , new SqlParameter("Amount", System.Data.SqlDbType.Decimal) { Value = Amount }
                    , new SqlParameter("PaymentType", System.Data.SqlDbType.VarChar) { Value = PaymentType }
                    , new SqlParameter("Remarks", System.Data.SqlDbType.VarChar) { Value = Remarks }
                    , new SqlParameter("IP", System.Data.SqlDbType.VarChar) { Value = System.Web.HttpContext.Current.Request.UserHostAddress }
                    , new SqlParameter("RefrenceID", System.Data.SqlDbType.BigInt) { Value = payRefId }
                    , new SqlParameter("Output", System.Data.SqlDbType.Bit) { Value = true }
                    ).ToList();
                if (result.Count > 0 && result[0] == 1)
                    isPayment = true;
                else
                    isPayment = false;
                return isPayment;
            }
            catch (Exception) { return false; }
        }
    }

    public static bool ServiceAuth(UnitOfWork _unitOfWork, int userID, Int16 serviceID)
    {
        try
        {
            var serviceAuthorization = _unitOfWork.ServiceAuthorizationRepository.Get(x => x.UserID == userID && x.ServiceID == serviceID);
            if (serviceAuthorization != null)
                return serviceAuthorization.IsActive;
            else
                return false;
        }
        catch (Exception) { return false; }
    }

    public static APIBalanceInquiryReponseEntity BalanceRequest(UnitOfWork _unitOfWork, int userID, string agentID)
    {
        APIBalanceInquiryReponseEntity _response = new APIBalanceInquiryReponseEntity();
        try
        {
            var user = _unitOfWork.UserRepository.Get(x => x.ID == userID);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.AepsInquiryUrl) && user.AepsInquiryUrl.Contains("http"))
                {
                    string url = user.AepsInquiryUrl;
                    RequestLog _add = new RequestLog()
                    {
                        UserId = user.ID,
                        Number = user.AltNo,
                        RequestType = "Balance request",
                        RequestUrl = url,
                        Request = "NA",
                        Response = "NA",
                        AddDate = DateTime.Now,
                        IpAddress = HttpContext.Current.Request.UserHostAddress,
                        Extra1 = agentID,
                        Extra2 = "NA",
                        RefId = 0
                    };
                    _unitOfWork.RequestLogRepository.Insert(_add);
                    _unitOfWork.Save();
                    APIBalanceInquiryEntity _request = new APIBalanceInquiryEntity()
                    {
                        RequestID = _add.Id.ToString(),
                        RetailerID = agentID,
                        SessionID = ClsMethods.GenereateUniqueNumber(7)
                    };
                    var jsonStr = new JavaScriptSerializer().Serialize(_request);
                    var apiResponse = ClsMethods.HttpRequestJson(url, _request);
                    _add.Request = jsonStr;
                    _add.Response = apiResponse;
                    _unitOfWork.Save();
                    if (!string.IsNullOrEmpty(apiResponse) && !apiResponse.Contains("Error$"))
                    {
                        var result = new JavaScriptSerializer().Deserialize<APIBalanceInquiryResponseEntity>(apiResponse);
                        if (result.StatusCode == clsVariables.APIStatus.Success)
                        {
                            _response.StatusCode = clsVariables.APIStatus.Success;
                            _response.Message = result.Message;
                            _response.AEPSBalance = Convert.ToDecimal(result.Transactions.Balance);
                            _response.MainBalance = Convert.ToDecimal(result.Transactions.MainBalance);
                        }
                        else if (result.StatusCode == clsVariables.APIStatus.Failed)
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = result.Message;
                            _response.AEPSBalance = result.Transactions == null ? 0 : Convert.ToDecimal(result.Transactions.Balance);
                            _response.MainBalance = result.Transactions == null ? 0 : Convert.ToDecimal(result.Transactions.MainBalance);
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Pending;
                            _response.Message = result.Message;
                            _response.AEPSBalance = result.Transactions == null ? 0 : Convert.ToDecimal(result.Transactions.Balance);
                            _response.MainBalance = result.Transactions == null ? 0 : Convert.ToDecimal(result.Transactions.MainBalance);
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = Message.Errorsm;
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Balance inquiry URL not configured.";
                }
            }
            else
            {
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = Message.ServiceNotAuth;
            }
        }
        catch (Exception ex)
        {
            _response.StatusCode = clsVariables.APIStatus.Exception;
            _response.Message = ex.Message;
        }
        return _response;
    }

    public static APIPaymentResponseDataEntity PaymentRequest(UnitOfWork _unitOfWork, int userID, string agentID, string number, string serviceID, decimal amount, string paymentType, string extra1 = null, string extra2 = null, string extra3 = null)
    {
        APIPaymentResponseDataEntity _response = new APIPaymentResponseDataEntity();
        try
        {
            var user = _unitOfWork.UserRepository.Get(x => x.ID == userID);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.PaymentURL) && user.PaymentURL.Contains("http"))
                {
                    string url = user.PaymentURL;
                    RequestLog _add = new RequestLog()
                    {
                        UserId = user.ID,
                        Number = user.AltNo,
                        RequestType = "Balance Pay request",
                        RequestUrl = url,
                        Request = "NA",
                        Response = "NA",
                        AddDate = DateTime.Now,
                        IpAddress = HttpContext.Current.Request.UserHostAddress,
                        Extra1 = agentID,
                        Extra2 = "NA",
                        RefId = 0
                    };
                    _unitOfWork.RequestLogRepository.Insert(_add);
                    _unitOfWork.Save();
                    APIPaymentInquiryEntity _request = new APIPaymentInquiryEntity()
                    {
                        RequestID = _add.Id.ToString(),
                        RetailerID = agentID,
                        SessionID = ClsMethods.GenereateUniqueNumber(7),
                        Number = number,
                        ServiceProviderId = serviceID,
                        Amount = amount,
                        PaymentType = paymentType,
                        Extra1 = extra1,
                        Extra2 = extra2,
                        Extra3 = extra3
                    };
                    var jsonStr = new JavaScriptSerializer().Serialize(_request);
                    var apiResponse = ClsMethods.HttpRequestJson(url, _request);
                    _add.Request = jsonStr;
                    _add.Response = apiResponse;
                    _unitOfWork.Save();
                    if (!string.IsNullOrEmpty(apiResponse) && !apiResponse.Contains("Error$"))
                    {
                        var result = new JavaScriptSerializer().Deserialize<APIPaymentInquiryResponseEntity>(apiResponse);
                        if (result.StatusCode == clsVariables.APIStatus.Success)
                        {
                            _response.StatusCode = clsVariables.APIStatus.Success;
                            _response.Message = result.Message;
                            _response.RequestID = result.Transaction.RequestID;
                            _response.ClientTransactionID = result.Transaction.ClientTransactionID;
                            _response.RetailerID = result.Transaction.RetailerID;
                            _response.Number = result.Transaction.Number;
                            _response.ServiceProviderId = result.Transaction.ServiceProviderId;
                            _response.Amount = result.Transaction.Amount;
                            _response.PaymentType = result.Transaction.PaymentType;
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = result.Message;
                            if (result.Transaction != null)
                            {
                                _response.RequestID = result.Transaction.RequestID;
                                _response.ClientTransactionID = result.Transaction.ClientTransactionID;
                                _response.RetailerID = result.Transaction.RetailerID;
                                _response.Number = result.Transaction.Number;
                                _response.ServiceProviderId = result.Transaction.ServiceProviderId;
                                _response.Amount = result.Transaction.Amount;
                                _response.PaymentType = result.Transaction.PaymentType;
                            }
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = Message.Errorsm;
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Payment request URL not configured.";
                }
            }
            else
            {
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = Message.ServiceNotAuth;
            }
        }
        catch (Exception ex)
        {
            _response.StatusCode = clsVariables.APIStatus.Exception;
            _response.Message = ex.Message;
        }
        return _response;
    }

}
