using BusinessEntities;
using BusinessServices.BillPayment;
using BusinessServices.Resource;
using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;

namespace BusinessServices.BBPS
{
    public class DoTransactions
    {
        #region Private Variable
        //private static AvenueAPI _apiService;
        private static EzypayServices _ezyPayServices;
        private static CyberpletAPIServices _cyberpletServices;
        private static PaymtBillPaymentAPI _paytmServices;
        private static ClsNotify _notifyServices;
        #endregion
        //private static APIReponseEntity BillerParameters(UnitOfWork _unitOfWork, int billerID)
        //{
        //    APIReponseEntity _response = new APIReponseEntity();
        //    try
        //    {
        //        var result = _unitOfWork.SqlQuery<BBPSSUBCategoryParameter_Result>("BBPSSUBCategoryParameter @BillerD,@Flag", new SqlParameter("BillerD", System.Data.SqlDbType.Int) { Value = billerID }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 1 }).ToList();
        //        if (result.Any())
        //        {
        //            _response.StatusCode = clsVariables.APIStatus.Success;
        //            _response.Message = Message.Su;
        //            _response.Data = result;
        //        }
        //        else
        //        {
        //            _response.StatusCode = clsVariables.APIStatus.Failed;
        //            _response.Message = Message.Datanotfound;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.StatusCode = clsVariables.APIStatus.Exception;
        //        _response.Message = Message.Errorsm + ex.Message;
        //    }
        //    return _response;
        //}

        //public static APIReponseEntity BalanceInquiryTransaction(UnitOfWork _unitOfWork, int userID, string agentID, int billerID, string mobileNo, string consumerNo, string param1, string param2, string param3, string param4, string param5)
        //{
        //    APIReponseEntity _response = new APIReponseEntity();
        //    try
        //    {
        //        var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == userID);
        //        if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
        //        {
        //            #region Comm Method
        //            var getApi = ClsPayment.GetAPI(_unitOfWork, billerID, currentUser.SchemeID, 0, 1);
        //            if (getApi == null)
        //            {
        //                _response.StatusCode = clsVariables.APIStatus.Failed;
        //                _response.Message = "Configuration not set.";
        //                return _response;
        //            }
        //            else if (getApi.ApiStatus == false)
        //            {
        //                _response.StatusCode = clsVariables.APIStatus.Failed;
        //                _response.Message = "Transaction service temporary is unavailable.";
        //                return _response;
        //            }
        //            else if (getApi.OperatorStatus == "0")
        //            {
        //                _response.StatusCode = clsVariables.APIStatus.Failed;
        //                _response.Message = "Operator server down.";
        //                return _response;
        //            }
        //            if (!ClsPayment.ServiceAuth(_unitOfWork, currentUser.ID, getApi.ServiceID))
        //            {
        //                _response.StatusCode = clsVariables.APIStatus.Failed;
        //                _response.Message = Message.ServiceNotAuth;
        //                return _response;
        //            }
        //            #endregion

        //            _apiService = new AvenueAPI();
        //            var apiResponse = _apiService.SendBillFetchRequest(currentUser.ID, agentID, getApi.BBPSCode, mobileNo, consumerNo, param1, param2, param3, param4, param5);
        //            if (apiResponse.billFetchResponse.responseCode == "000")
        //            {
        //                _response.StatusCode = clsVariables.APIStatus.Success;
        //                _response.Message = Message.Su;
        //                _response.Data = apiResponse.billFetchResponse;
        //            }
        //            else
        //            {
        //                _response.StatusCode = clsVariables.APIStatus.Failed;
        //                _response.Message = apiResponse.billFetchResponse.errorInfo.error.errorMessage;
        //                _response.Data = apiResponse.billFetchResponse;
        //            }
        //            //var paramResult = BillerParameters(_unitOfWork, billerID);
        //            //if (paramResult.StatusCode == clsVariables.APIStatus.Success)
        //            //{

        //            //}
        //            //else
        //            //{
        //            //    _response.StatusCode = clsVariables.APIStatus.Failed;
        //            //    _response.Message = Message.InvalidOurcode;
        //            //}
        //        }
        //        else
        //        {
        //            _response.StatusCode = clsVariables.APIStatus.Failed;
        //            _response.Message = "You are not authorized.";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        _response.StatusCode = clsVariables.APIStatus.Exception;
        //        _response.Message = Message.Errorsm;
        //    }
        //    return _response;
        //}


        public static APIBBPSFetchReponseEntity BillerFetch(UnitOfWork _unitOfWork, int userID, string agentID, int billerID, string mobileNo, string consumerNo, string param1, string param2, string param3, string param4, string param5)
        {
            APIBBPSFetchReponseEntity _response = new APIBBPSFetchReponseEntity();
            try
            {
                var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == userID);
                if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
                {
                    #region Comm Method
                    var getApi = ClsPayment.GetAPI(_unitOfWork, billerID, currentUser.SchemeID, 0, 1);
                    if (getApi == null)
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Configuration not set.";
                        return _response;
                    }
                    else if (getApi.ApiStatus == false)
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Transaction service temporary is unavailable.";
                        return _response;
                    }
                    else if (getApi.OperatorStatus == "0")
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Operator server down.";
                        return _response;
                    }
                    if (!ClsPayment.ServiceAuth(_unitOfWork, currentUser.ID, getApi.ServiceID))
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = Message.ServiceNotAuth;
                        return _response;
                    }
                    #endregion

                    long transID = Convert.ToInt64(ClsMethods.GenereateUniqueNumber(8));
                    _response.RefNumber = transID.ToString();
                    if (getApi.APIName == clsVariables.RechargeAPI.EzyPay)
                    {
                        #region EzyPay
                        _ezyPayServices = new EzypayServices();
                        var apiResponse = _ezyPayServices.FetchBill(currentUser.ID, getApi.Username, getApi.APIOperatorCode1, currentUser.OwnerName, consumerNo, param1, param2);
                        if (!string.IsNullOrEmpty(apiResponse.ApiResponse) && apiResponse.ApiResponse != "Error")
                        {
                            string[] responseLine = apiResponse.ApiResponse.Split('~');
                            if (responseLine[0].ToString() == "B06004" && responseLine[3].ToString() == "Y")
                            {
                                string billDate = string.Empty, billDueDate = string.Empty; try { billDate = responseLine[9]; billDueDate = responseLine[10]; billDate = DateTime.ParseExact(billDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString(); billDueDate = DateTime.ParseExact(billDueDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString(); } catch { }
                                BBPSFetchReponseDataEntity _subResponse = new BBPSFetchReponseDataEntity()
                                {
                                    BillerName = responseLine[13],
                                    BillDueDate = billDueDate,
                                    BillDate = billDate,
                                    Amount = responseLine[11],
                                    BillerNumber = responseLine[14],
                                    Partial = responseLine[12],
                                };
                                _response.StatusCode = clsVariables.APIStatus.Success;
                                _response.Message = responseLine[4];
                                _response.Data = _subResponse;
                            }
                            else if (responseLine[0].ToString() == "B06004" && responseLine[3].ToString() == "N")
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = responseLine[4];
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = responseLine[1];
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Can not fetch bill try later.";
                        }
                        #endregion
                    }
                    else if (getApi.APIName == clsVariables.RechargeAPI.Cyberplet)
                    {
                        #region Cyberplet
                        _cyberpletServices = new CyberpletAPIServices();
                        _response = _cyberpletServices.BillFetch(currentUser.ID, getApi.Password, getApi.APIOperatorCode9, transID, mobileNo, consumerNo, param1, param2);
                        #endregion
                    }
                    else if (getApi.APIName == clsVariables.RechargeAPI.PayTmBBPS)
                    {
                        #region PayTmBBPS
                        _paytmServices = new PaymtBillPaymentAPI();
                        _response = _paytmServices.BillFetch(_unitOfWork, getApi.OperatorID, consumerNo, getApi.APIOperatorCode7, mobileNo, param1, param2);
                        #endregion
                    }
                    else if (getApi.APIName == clsVariables.RechargeAPI.Offline)
                    {
                        #region EzyPay Offline
                        _ezyPayServices = new EzypayServices();
                        var apiResponse = _ezyPayServices.FetchBill(currentUser.ID, getApi.Username, getApi.APIOperatorCode1, currentUser.OwnerName, consumerNo, param1, param2);
                        if (!string.IsNullOrEmpty(apiResponse.ApiResponse) && apiResponse.ApiResponse != "Error")
                        {
                            string[] responseLine = apiResponse.ApiResponse.Split('~');
                            if (responseLine[0].ToString() == "B06004" && responseLine[3].ToString() == "Y")
                            {
                                string billDate = string.Empty, billDueDate = string.Empty; try { billDate = responseLine[9]; billDueDate = responseLine[10]; billDate = DateTime.ParseExact(billDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString(); billDueDate = DateTime.ParseExact(billDueDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString(); } catch { }
                                BBPSFetchReponseDataEntity _subResponse = new BBPSFetchReponseDataEntity()
                                {
                                    BillerName = responseLine[13],
                                    BillDueDate = billDueDate,
                                    BillDate = billDate,
                                    Amount = responseLine[11],
                                    BillerNumber = responseLine[14],
                                    Partial = responseLine[12],
                                };
                                _response.StatusCode = clsVariables.APIStatus.Success;
                                _response.Message = responseLine[4];
                                _response.Data = _subResponse;
                            }
                            else if (responseLine[0].ToString() == "B06004" && responseLine[3].ToString() == "N")
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = responseLine[4];
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = responseLine[1];
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Can not fetch bill try later.";
                        }
                        #endregion
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "API configuration not set.";
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "You are not authorized.";
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        public static APIBBPSReponseEntity BillPayment(UnitOfWork _unitOfWork, int userID, string agentID, string merchantID, int billerID, decimal amount, string billnumber, string custmobileno, string billeraccount, string billercycle, string payment, string duedate, string billdate, string consumername, string billnumbers, string apiRefNo)
        {
            APIBBPSReponseEntity _response = new APIBBPSReponseEntity()
            {
                Amount = amount,
                Number = billnumber
            };
            try
            {
                var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == userID);
                if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
                {
                    #region Transaction Method
                    decimal currentBalance = ClsPayment.GetMainBalance(_unitOfWork, currentUser.ID);
                    if (currentBalance >= amount && amount > 0)
                    {
                        #region Comm Method
                        var getApi = ClsPayment.GetAPI(_unitOfWork, billerID, currentUser.SchemeID, 0, 1);
                        if (getApi == null)
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Configuration not set.";
                            return _response;
                        }
                        else if (getApi.ApiStatus == false)
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Transaction service temporary is unavailable.";
                            return _response;
                        }
                        else if (getApi.OperatorStatus == "0")
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Operator server down.";
                            return _response;
                        }
                        if (!ClsPayment.ServiceAuth(_unitOfWork, currentUser.ID, getApi.ServiceID))
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = Message.ServiceNotAuth;
                            return _response;
                        }
                        if (amount < 0)
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Invalid Amount";
                            return _response;
                        }
                        if (currentBalance < amount)
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Low Balance";
                            return _response;
                        }
                        decimal userComm = ((getApi.CommPer * amount) / 100) + getApi.CommVal;
                        decimal charge = ((getApi.ServiceChargePer * amount) / 100) + getApi.ServiceChargeVal;
                        if (currentBalance < (amount - userComm + charge))
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Low balance.";
                            return _response;
                        }

                        #endregion

                        var paymentResult = ClsPayment.PaymentRequest(_unitOfWork, currentUser.ID, agentID, billnumber, getApi.OperatorApiCode, amount, clsVariables.PaymentType.BBPS);
                        if (paymentResult.StatusCode == clsVariables.APIStatus.Success)
                        {
                            var isTxnNotExist = ClsMethods.TransactionValidate(_unitOfWork, userID, billnumber, amount, paymentResult.ClientTransactionID);
                            if (isTxnNotExist != 2)
                            {
                                //if (isTxnNotExist != 3)
                                //{
                                if (isTxnNotExist == 0)
                                {
                                    decimal chargeAmt = 0, commAmt = 0;
                                    var comms = _unitOfWork.ApiCommissionRepository.Get(x => x.APIID == getApi.ApiID && x.OperatorID == getApi.OperatorID);
                                    if (comms != null)
                                    {
                                        chargeAmt = ((comms.ChargePer * amount) / 100) + comms.ChargeVal;
                                        commAmt = ((comms.CommPer * amount) / 100) + comms.CommVal;
                                    }
                                    decimal serviceCharge = ((getApi.ServiceChargePer * amount) / 100) + getApi.ServiceChargeVal;
                                    decimal commAmount = ((getApi.CommPer * amount) / 100) + getApi.CommVal;

                                    #region Transaction
                                    decimal gstAmount = 0, tdsAmount = 0;
                                    if (getApi.OperatorType == clsVariables.OperatorType.P2A)
                                    {
                                        gstAmount = (commAmount * 18) / 118;
                                        tdsAmount = ((commAmount - gstAmount) * 5) / 100;
                                    }
                                    RechargeTransaction objRechargeTransaction = new RechargeTransaction()
                                    {
                                        OperatorID = getApi.OperatorID,
                                        UserID = currentUser.ID,
                                        UserDetails = currentUser.OwnerName,
                                        Number = billnumber,
                                        OpeningBalance = currentBalance,
                                        Amount = amount,
                                        CommPer = getApi.CommPer,
                                        CommVal = getApi.CommVal,
                                        CommAmt = commAmount,
                                        OtherCharge = 0,
                                        ServiceCharge = getApi.ServiceChargePer,
                                        ServiceChargeVal = getApi.ServiceChargeVal,
                                        ServiceChargeAmt = serviceCharge,
                                        Cost = amount + serviceCharge - commAmount + gstAmount + tdsAmount,
                                        ClosingBalance = currentBalance - (amount + serviceCharge - commAmount + gstAmount + tdsAmount),
                                        ChargeAmount = chargeAmt,
                                        CommAmount = commAmt,
                                        TransactionMode = clsVariables.RechargeMode.API,
                                        TransactionDate = DateTime.Now,
                                        EditDate = DateTime.Now,
                                        Status = clsVariables.RechargeStatus.Pending,
                                        ServiceID = getApi.ServiceID,
                                        ServiceName = "BBPS",
                                        APIID = getApi.ApiID,
                                        TransactionID = paymentResult.ClientTransactionID,
                                        Reason = "",
                                        ApiLogs = "",
                                        IsProcessed = false,
                                        RevertTran = false,
                                        EXTRA1 = "Cash",
                                        EXTRA2 = "NA",
                                        EXTRA3 = merchantID,
                                        EXTRA4 = duedate,
                                        EXTRA5 = getApi.OperatorType,
                                        EXTRA6 = billeraccount,
                                        EXTRA7 = billercycle,
                                        EXTRA8 = custmobileno,
                                        EXTRA9 = agentID,
                                        EXTRA10 = consumername,
                                        IpAddress = System.Web.HttpContext.Current.Request.UserHostAddress,
                                        APIRef = "NA",
                                        ActionName = billdate,
                                        CircleID = 0,
                                        LapuNumber = null,
                                        OperatorRef = "NA",
                                        RetryAPiName = billnumbers,
                                        UpdatedBy = null
                                    };
                                    _unitOfWork.RechargeTransactionRepository.Insert(objRechargeTransaction);
                                    _unitOfWork.Save();

                                    _response.RefNumber = objRechargeTransaction.ID.ToString(); //ClsMethods.Encrypt(objRechargeTransaction.ID.ToString());
                                    #endregion
                                    #region Payment And API Method
                                    String remarks = "Number: " + objRechargeTransaction.Number + " |" + getApi.OperatorName + "| Amount: " + amount.ToString("0,0.00") + " | Comm Per.: " + getApi.CommPer.ToString("0,0.00") + " | Comm Val.: " + getApi.CommVal.ToString("0,0.00") + " | Charg Per.: " + getApi.ServiceChargePer.ToString("0,0.00") + " | Charg Val.: " + getApi.ServiceChargeVal.ToString("0,0.00"); if (getApi.OperatorType == clsVariables.OperatorType.P2A) { remarks += " | GST: " + gstAmount.ToString("0,0.00") + " | TDS: " + tdsAmount.ToString("0,0.00"); }
                                    remarks += " Ref Id = " + objRechargeTransaction.ID;
                                    bool isPaymentDone = ClsPayment.DoPaymentMain(_unitOfWork, 1, currentUser.ID, objRechargeTransaction.Cost, remarks, clsVariables.PaymentType.BBPS, objRechargeTransaction.ID);
                                    if (isPaymentDone)
                                    {
                                        _response.RefNumber = objRechargeTransaction.ID.ToString();
                                        if (getApi.APIName == clsVariables.RechargeAPI.EzyPay)
                                        {
                                            #region EzyPay>>APIOperatorCode1
                                            _ezyPayServices = new EzypayServices();
                                            try { if (!string.IsNullOrEmpty(duedate)) { string[] line = duedate.Split('/'); string m = line[0].Length == 2 ? line[0] : "0" + line[0]; string d = line[1].Length == 2 ? line[1] : "0" + line[1]; string y = line[2]; duedate = y + d + m; } } catch { }
                                            var apiResponse = _ezyPayServices.SendPaymentRequest(getApi.Username, objRechargeTransaction.ID.ToString(), getApi.APIOperatorCode1, currentUser.OwnerName, billnumber, billeraccount, billercycle, custmobileno, amount, duedate);
                                            if (!string.IsNullOrEmpty(apiResponse.ApiResponse) && apiResponse.ApiResponse != "Error")
                                            {
                                                try
                                                {
                                                    string[] responseLine = apiResponse.ApiResponse.Split('~');
                                                    if (responseLine[0].ToString() == "B06006" && responseLine[3].ToString() == "FAILED")
                                                    {
                                                        #region Failed
                                                        isPaymentDone = ClsPayment.DoPaymentMain(_unitOfWork, currentUser.ID, 1, objRechargeTransaction.Cost, remarks, clsVariables.PaymentType.BBPSR, objRechargeTransaction.ID);
                                                        objRechargeTransaction.Status = clsVariables.RechargeStatus.Failure;
                                                        objRechargeTransaction.RevertTran = true;
                                                        objRechargeTransaction.ClosingBalance = objRechargeTransaction.OpeningBalance;
                                                        objRechargeTransaction.Reason = responseLine[4];
                                                        if (responseLine[4].Contains("Insufficient Balance"))
                                                            objRechargeTransaction.Reason = "Sorry! Transaction could not be processed.";
                                                        else
                                                            objRechargeTransaction.Reason = responseLine[4];

                                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                                        _response.Message = objRechargeTransaction.Reason;
                                                        #endregion
                                                    }
                                                    else if (responseLine[0].ToString() == "B06006" && responseLine[3].ToString() == "SUCCESS")
                                                    {
                                                        #region Success
                                                        objRechargeTransaction.Status = clsVariables.RechargeStatus.Success;
                                                        objRechargeTransaction.APIRef = string.IsNullOrEmpty(responseLine[7]) ? "NA" : responseLine[7];
                                                        objRechargeTransaction.OperatorRef = string.IsNullOrEmpty(responseLine[5]) ? "NA" : responseLine[5];
                                                        objRechargeTransaction.EXTRA1 = responseLine[6];

                                                        _response.StatusCode = clsVariables.APIStatus.Success;
                                                        _response.Message = responseLine[4];
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        #region Pending
                                                        objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                        objRechargeTransaction.APIRef = string.IsNullOrEmpty(responseLine[7]) ? "NA" : responseLine[7];
                                                        objRechargeTransaction.OperatorRef = string.IsNullOrEmpty(responseLine[5]) ? "NA" : responseLine[5];

                                                        _response.StatusCode = clsVariables.APIStatus.Pending;
                                                        _response.Message = responseLine[4];
                                                        #endregion
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                    _response.StatusCode = clsVariables.APIStatus.Pending;
                                                    _response.Message += ex.Message;
                                                }
                                            }
                                            else
                                            {
                                                objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                _response.StatusCode = clsVariables.APIStatus.Pending;
                                                _response.Message = Message.Unknownresponse;
                                            }

                                            objRechargeTransaction.Reason = _response.Message;
                                            objRechargeTransaction.EditDate = DateTime.Now;
                                            _unitOfWork.Save();

                                            _response.Balance = objRechargeTransaction.Amount;
                                            _response.OperatorRef = objRechargeTransaction.OperatorRef;
                                            _response.Status = objRechargeTransaction.Status;
                                            _response.TxnDate = objRechargeTransaction.TransactionDate.ToShortDateString();
                                            _response.TxnRef = objRechargeTransaction.ID;
                                            #endregion
                                        }
                                        else if (getApi.APIName == clsVariables.RechargeAPI.Cyberplet)
                                        {
                                            #region Cyberplet>>APIOperatorCode9
                                            _cyberpletServices = new CyberpletAPIServices();
                                            try { if (!string.IsNullOrEmpty(duedate)) { string[] line = duedate.Split('/'); string m = line[0].Length == 2 ? line[0] : "0" + line[0]; string d = line[1].Length == 2 ? line[1] : "0" + line[1]; string y = line[2]; duedate = y + d + m; } } catch { }
                                            var apiResponse = _cyberpletServices.BillPayment(currentUser.ID, getApi.Password, getApi.APIOperatorCode9, objRechargeTransaction.ID, custmobileno, billnumber, billeraccount, billercycle, amount, duedate);
                                            try
                                            {
                                                if (apiResponse.StatusCode == clsVariables.APIStatus.Failed)
                                                {
                                                    #region Failed
                                                    isPaymentDone = ClsPayment.DoPaymentMain(_unitOfWork, currentUser.ID, 1, objRechargeTransaction.Cost, remarks, clsVariables.PaymentType.BBPSR, objRechargeTransaction.ID);
                                                    objRechargeTransaction.Status = clsVariables.RechargeStatus.Failure;
                                                    objRechargeTransaction.RevertTran = true;
                                                    objRechargeTransaction.ClosingBalance = objRechargeTransaction.OpeningBalance;
                                                    objRechargeTransaction.Reason = apiResponse.Message.Replace("Low Balance", "Sorry! Transaction could not be processed.");

                                                    _response.StatusCode = clsVariables.APIStatus.Failed;
                                                    _response.Message = objRechargeTransaction.Reason;
                                                    #endregion
                                                }
                                                else if (apiResponse.StatusCode == clsVariables.APIStatus.Failed)
                                                {
                                                    #region Success
                                                    objRechargeTransaction.Status = clsVariables.RechargeStatus.Success;
                                                    objRechargeTransaction.APIRef = apiResponse.Data == null ? "NA" : apiResponse.Data.TxnRef == 0 ? "NA" : apiResponse.Data.TxnRef.ToString();
                                                    objRechargeTransaction.OperatorRef = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.OperatorRef) ? "NA" : apiResponse.Data.OperatorRef;
                                                    objRechargeTransaction.EXTRA1 = apiResponse.Data.Status;

                                                    _response.StatusCode = clsVariables.APIStatus.Success;
                                                    _response.Message = apiResponse.Message;
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region Pending
                                                    objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                    objRechargeTransaction.APIRef = apiResponse.Data == null ? "NA" : apiResponse.Data.TxnRef == 0 ? "NA" : apiResponse.Data.TxnRef.ToString();
                                                    objRechargeTransaction.OperatorRef = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.OperatorRef) ? "NA" : apiResponse.Data.OperatorRef;

                                                    _response.StatusCode = clsVariables.APIStatus.Pending;
                                                    _response.Message = apiResponse.Message;
                                                    #endregion
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                _response.StatusCode = clsVariables.APIStatus.Pending;
                                                _response.Message += ex.Message;
                                            }

                                            objRechargeTransaction.Reason = _response.Message;
                                            objRechargeTransaction.EditDate = DateTime.Now;
                                            _unitOfWork.Save();

                                            _response.Balance = objRechargeTransaction.Amount;
                                            _response.OperatorRef = objRechargeTransaction.OperatorRef;
                                            _response.Status = objRechargeTransaction.Status;
                                            _response.TxnDate = objRechargeTransaction.TransactionDate.ToShortDateString();
                                            _response.TxnRef = objRechargeTransaction.ID;
                                            #endregion
                                        }
                                        else if (getApi.APIName == clsVariables.RechargeAPI.PayTmBBPS)
                                        {
                                            #region PayTmBBPS>>APIOperatorCode7
                                            _paytmServices = new PaymtBillPaymentAPI();
                                            _notifyServices = new ClsNotify(_unitOfWork);
                                            var apiResponse = _paytmServices.BillPayment(_unitOfWork, getApi.OperatorID, billnumber, getApi.APIOperatorCode7, amount, custmobileno, billeraccount, billercycle, objRechargeTransaction.ID);
                                            try
                                            {
                                                if (apiResponse.StatusCode == clsVariables.APIStatus.Failed)
                                                {
                                                    #region Failed
                                                    isPaymentDone = ClsPayment.DoPaymentMain(_unitOfWork, currentUser.ID, 1, objRechargeTransaction.Cost, remarks, clsVariables.PaymentType.BBPSR, objRechargeTransaction.ID);
                                                    objRechargeTransaction.Status = clsVariables.RechargeStatus.Failure;
                                                    objRechargeTransaction.RevertTran = true;
                                                    objRechargeTransaction.ClosingBalance = objRechargeTransaction.OpeningBalance;
                                                    objRechargeTransaction.Reason = apiResponse.Message.Replace("Low Balance", "Sorry! Transaction could not be processed.");
                                                    objRechargeTransaction.OperatorRef = string.IsNullOrEmpty(apiResponse.RefNumber) ? "NA" : apiResponse.RefNumber;

                                                    _response.StatusCode = clsVariables.APIStatus.Failed;
                                                    _response.Message = objRechargeTransaction.Reason;
                                                    #endregion
                                                }
                                                else if (apiResponse.StatusCode == clsVariables.APIStatus.Success)
                                                {
                                                    #region Success
                                                    objRechargeTransaction.Status = clsVariables.RechargeStatus.Success;
                                                    objRechargeTransaction.APIRef = string.IsNullOrEmpty(apiResponse.RefNumber) ? "NA" : apiResponse.RefNumber;
                                                    objRechargeTransaction.OperatorRef = string.IsNullOrEmpty(apiResponse.RefNumber) ? "NA" : apiResponse.RefNumber;
                                                    //objRechargeTransaction.EXTRA1 = apiResponse.Data.Status;

                                                    _response.StatusCode = clsVariables.APIStatus.Success;
                                                    _response.Message = apiResponse.Message;
                                                    _notifyServices.BillPayemnt(currentUser.ID, custmobileno, amount.ToString(), billnumber, objRechargeTransaction.ID.ToString());
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region Pending
                                                    objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                    //objRechargeTransaction.APIRef = apiResponse.Data == null ? "NA" : apiResponse.Data.TxnRef == 0 ? "NA" : apiResponse.Data.TxnRef.ToString();
                                                    objRechargeTransaction.OperatorRef = string.IsNullOrEmpty(apiResponse.RefNumber) ? "NA" : apiResponse.RefNumber;

                                                    _response.StatusCode = clsVariables.APIStatus.Pending;
                                                    _response.Message = apiResponse.Message;
                                                    #endregion
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                _response.StatusCode = clsVariables.APIStatus.Pending;
                                                _response.Message += ex.Message;
                                            }

                                            objRechargeTransaction.Reason = _response.Message;
                                            objRechargeTransaction.EditDate = DateTime.Now;
                                            _unitOfWork.Save();

                                            _response.Balance = objRechargeTransaction.Amount;
                                            _response.OperatorRef = objRechargeTransaction.OperatorRef;
                                            _response.Status = objRechargeTransaction.Status;
                                            _response.TxnDate = objRechargeTransaction.TransactionDate.ToShortDateString();
                                            _response.TxnRef = objRechargeTransaction.ID;
                                            #endregion
                                        }
                                        else if (getApi.APIName == clsVariables.RechargeAPI.Offline)
                                        {
                                            #region Offline
                                            objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                            objRechargeTransaction.APIRef = "NA";
                                            objRechargeTransaction.OperatorRef = "NA";
                                            objRechargeTransaction.Reason = "Transaction accepted.";
                                            objRechargeTransaction.EditDate = DateTime.Now;
                                            _unitOfWork.Save();

                                            _response.StatusCode = clsVariables.APIStatus.Pending;
                                            _response.Message = "Transaction accepted.";
                                            #endregion
                                        }
                                        else
                                        {
                                            #region API Not Set
                                            isPaymentDone = ClsPayment.DoPaymentMain(_unitOfWork, currentUser.ID, 1, objRechargeTransaction.Cost, remarks, clsVariables.PaymentType.BBPSR, objRechargeTransaction.ID);
                                            objRechargeTransaction.Status = clsVariables.RechargeStatus.Failure;
                                            objRechargeTransaction.RevertTran = true;
                                            objRechargeTransaction.ClosingBalance = objRechargeTransaction.OpeningBalance;
                                            objRechargeTransaction.APIRef = "NA";
                                            objRechargeTransaction.OperatorRef = "NA";
                                            _unitOfWork.Save();

                                            _response.StatusCode = clsVariables.APIStatus.Failed;
                                            _response.Message = "Configuration not set.";
                                            #endregion
                                        }
                                        #region Call Back Method
                                        BBPSAPITransactionResponseDataEntity _addSub = new BBPSAPITransactionResponseDataEntity()
                                        {
                                            Amount = objRechargeTransaction.Amount,
                                            Number = objRechargeTransaction.Number,
                                            Status = objRechargeTransaction.Status,
                                            OperatorRef = objRechargeTransaction.OperatorRef,
                                            OrderID = objRechargeTransaction.ID.ToString(),
                                            TxnDate = objRechargeTransaction.TransactionDate.ToString()
                                        };
                                        BBPSAPITransactionResponseEntity _add = new BBPSAPITransactionResponseEntity()
                                        {
                                            StatusCode = objRechargeTransaction.Status == clsVariables.RechargeStatus.Success ? clsVariables.APIStatus.Success : objRechargeTransaction.Status == clsVariables.RechargeStatus.Pending ? clsVariables.APIStatus.Pending : clsVariables.APIStatus.Failed,
                                            Message = objRechargeTransaction.Reason,
                                            AgentID = objRechargeTransaction.EXTRA9,
                                            ClientRefId = objRechargeTransaction.TransactionID,
                                            Transactions = _addSub
                                        };
                                        AgentCallBack(_unitOfWork, currentUser.ID, _add, objRechargeTransaction.ID);
                                        #endregion
                                    }
                                    else
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = "Payment configuration not set.";
                                    }
                                    #endregion
                                }
                                else
                                {
                                    _response.StatusCode = clsVariables.APIStatus.Failed;
                                    _response.Message = "You are trying transaction within block time of successful transaction. Please re-try after sometime.";
                                }
                                //}
                                //else
                                //{
                                //    _response.StatusCode = clsVariables.APIStatus.Failed;
                                //    _response.Message = "Transaction already in pending mode. Please try after sometime.";
                                //}
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = "Transaction ref already exist.";
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = paymentResult.Message;
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Low balance.";
                    }
                    #endregion
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "You are not authorized.";
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        /// <summary>
        /// Transaction Status Check
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="transactionRef"></param>
        /// <returns></returns>
        public static APIReponseEntity DOTransactionsStatus(UnitOfWork _unitOfWork, int userID, string agentID, string transactionRef)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var rechargeTransaction = ClsMethods.CustomerTransactionRefs(_unitOfWork, userID, transactionRef, 3);
                if (rechargeTransaction != null)
                {
                    if (!new[] { 10, 15, 17, 18 }.Contains(rechargeTransaction.ServiceID))
                    {
                        if (rechargeTransaction.TransactionDate.AddMinutes(5) < DateTime.Now)
                        {
                            var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == rechargeTransaction.UserID);
                            if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
                            {
                                if (rechargeTransaction.Status == clsVariables.RechargeStatus.Pending)
                                {
                                    #region Transaction Method
                                    var api = _unitOfWork.ApiRepository.Get(x => x.ID == rechargeTransaction.APIID);
                                    if (api.APIName == clsVariables.RechargeAPI.PayTmBBPS)
                                    {
                                        #region PayTmBBPS
                                        _paytmServices = new PaymtBillPaymentAPI();
                                        var apiResponse = _paytmServices.StatusCheck(rechargeTransaction.Number, rechargeTransaction.ID);
                                        try
                                        {
                                            if (apiResponse.StatusCode == clsVariables.APIStatus.Failed)
                                            {
                                                //bool isPaymentReverted = ClsPayment.DoPayment(_unitOfWork, rechargeTransaction.UserID, 1, rechargeTransaction.Cost, remarks, clsVariables.PaymentType.RechargeR, rechargeTransaction.ID);
                                                //rechargeTransaction.ClosingBalance = rechargeTransaction.OpeningBalance;
                                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                                _response.Message = apiResponse.Message;
                                            }
                                            else if (apiResponse.StatusCode == clsVariables.APIStatus.Success)
                                            {
                                                rechargeTransaction.Status = clsVariables.RechargeStatus.Success;
                                                rechargeTransaction.OperatorRef = string.IsNullOrEmpty(apiResponse.RefNumber) ? "NA" : apiResponse.RefNumber;


                                                ClsMethods.CustomerPendingTransaction(_unitOfWork, rechargeTransaction.ID, rechargeTransaction.OperatorRef, 1);
                                                rechargeTransaction = ClsMethods.CustomerPendingTransaction(_unitOfWork, rechargeTransaction.ID.ToString(), 2);

                                                _response.StatusCode = clsVariables.APIStatus.Success;
                                                _response.Message = apiResponse.Message;
                                            }
                                            else
                                            {
                                                _response.StatusCode = clsVariables.APIStatus.Pending;
                                                _response.Message = apiResponse.Message;
                                            }

                                            #region Call Back Method
                                            BBPSAPITransactionResponseDataEntity _addSub = new BBPSAPITransactionResponseDataEntity()
                                            {
                                                Amount = rechargeTransaction.Amount,
                                                Number = rechargeTransaction.Number,
                                                Status = rechargeTransaction.Status,
                                                OperatorRef = rechargeTransaction.OperatorRef,
                                                OrderID = rechargeTransaction.ID.ToString(),
                                                TxnDate = rechargeTransaction.TransactionDate.ToString()
                                            };
                                            BBPSAPITransactionResponseEntity _add = new BBPSAPITransactionResponseEntity()
                                            {
                                                StatusCode = rechargeTransaction.Status == clsVariables.RechargeStatus.Success ? clsVariables.APIStatus.Success : rechargeTransaction.Status == clsVariables.RechargeStatus.Pending ? clsVariables.APIStatus.Pending : clsVariables.APIStatus.Failed,
                                                Message = rechargeTransaction.Reason,
                                                AgentID = rechargeTransaction.EXTRA9,
                                                ClientRefId = rechargeTransaction.TransactionID,
                                                Transactions = _addSub
                                            };
                                            AgentCallBack(_unitOfWork, currentUser.ID, _add, rechargeTransaction.ID);
                                            #endregion
                                        }
                                        catch (Exception Ex)
                                        {
                                            _response.StatusCode = clsVariables.APIStatus.Pending;
                                            _response.Message = Message.Unknownresponse + Ex.Message;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = Message.ServiceNotAuth;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    _response.StatusCode = clsVariables.APIStatus.Failed;
                                    _response.Message = Message.TxnNtPending;
                                }
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = Message.ServiceNotAuth;
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = Message.TxnStatusCheckBlock;
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Mobile,DTH,LandLine & Postpaid transaction are allowed.";
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.Datanotfound;
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        #region Call Back Request
        /// <summary>
        /// cliend call back request
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="operatorRef"></param>
        /// <param name="transactionID"></param>
        /// <param name="clientTxnID"></param>
        /// <returns></returns>
        private static bool AgentCallBack(UnitOfWork _unitOfWork, int userId, BBPSAPITransactionResponseEntity item, long transID)
        {
            string requestUrl = string.Empty, requestBody = string.Empty, callbackResponse = string.Empty; bool doStatus = false;
            try
            {
                var user = _unitOfWork.UserRepository.Get(x => x.ID == userId);
                if (user != null && !string.IsNullOrEmpty(user.BBPSTransactionURL) && user.BBPSTransactionURL.Contains("http"))
                {
                    requestUrl = user.BBPSTransactionURL;
                    requestBody = new JavaScriptSerializer().Serialize(item);
                    callbackResponse = ClsMethods.HttpRequestJson(requestUrl, item);
                }
                else
                    callbackResponse = "Callbackurl not found.";
            }
            catch (Exception EX) { callbackResponse += EX.Message; }
            ClsMethods.RequestLogs(userId, item.Transactions.Number, "ClientCallBack", requestUrl, requestBody, callbackResponse, transID, "WL", "");
            return doStatus;
        }
        #endregion
    }
}
