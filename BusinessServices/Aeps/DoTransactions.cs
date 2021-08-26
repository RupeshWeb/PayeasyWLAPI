using BusinessEntities;
using BusinessServices.Resource;
using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace BusinessServices.Aeps
{
    public class DoTransactions
    {
        #region Private variable
        private static FinoAepsAPI _apiServices;
        #endregion

        public static APIReponseEntity EncryptionKeyRequest(UnitOfWork _unitOfWork, int sessionID = 1)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                _apiServices = new FinoAepsAPI();
                var fnoSession = _unitOfWork.AEPSFNOSessionRepository.Get(x => x.ID == sessionID);
                if (fnoSession != null)
                {
                    if (fnoSession.EditDate.Date != DateTime.Now.Date)
                    {
                        var result = _apiServices.EncryptionRequestKey(fnoSession.ID);
                        if (result.StatusCode == clsVariables.APIStatus.Success)
                        {
                            fnoSession.Token = result.Data;
                            fnoSession.EditDate = DateTime.Now;
                            _unitOfWork.Save();

                            _response.StatusCode = clsVariables.APIStatus.Success;
                            _response.Message = result.Message;
                            _response.Data = result.Data;
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = result.Message;
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = Message.Su;
                        _response.Data = fnoSession.Token;
                    }
                }
                else
                {
                    var result = _apiServices.EncryptionRequestKey(0);
                    if (result.StatusCode == clsVariables.APIStatus.Success)
                    {
                        AEPSFNOSession _add = new AEPSFNOSession()
                        {
                            Token = result.Data,
                            AddDate = DateTime.Now,
                            EditDate = DateTime.Now,
                            IsActive = true,
                            UserID = 1
                        };
                        _unitOfWork.AEPSFNOSessionRepository.Insert(_add);
                        _unitOfWork.Save();

                        _response.StatusCode = clsVariables.APIStatus.Success;
                        _response.Message = result.Message;
                        _response.Data = result.Data;
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = result.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm + ex.Message;
            }
            return _response;
        }

        private static int TransferAmount(decimal amount)
        {
            if (amount > 0 && amount <= 500)
            {
                return 150;
            }
            else if (amount > 500 && amount <= 3000)
            {
                return 7;
            }
            else if (amount > 3000 && amount <= 10000)
            {
                return 75;
            }
            else
                return 75;
        }

        public static AEPSAPIReponseEntity CashWithdrawalTransaction(UnitOfWork _unitOfWork, int userID, string merchantID, string clientRefID, string mobileNo, string aadharNo, decimal amount, string bankCode, string pidData, string latitude, string longitude, string agentID)
        {
            AEPSAPIReponseEntity _response = new AEPSAPIReponseEntity();
            try
            {
                if (amount > 0 && amount <= 200000)
                {
                    var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == userID);
                    if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
                    {
                        long bankID = Convert.ToInt64(bankCode);
                        var bank = _unitOfWork.MasterIfscRepository.Get(x => x.Id == bankID);
                        if (bank != null)
                        {
                            _apiServices = new FinoAepsAPI();
                            var isTxnNotExist = ClsMethods.TransactionValidate(_unitOfWork, userID, mobileNo, amount, clientRefID);
                            if (isTxnNotExist != 2)
                            {
                                if (isTxnNotExist != 3)
                                {
                                    if (isTxnNotExist == 0)
                                    {
                                        #region Transaction Method
                                        #region Comm Method
                                        var operatorId = TransferAmount(amount);
                                        var getApi = ClsPayment.GetAPI(_unitOfWork, operatorId, currentUser.SchemeID, 0, 1);
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
                                        decimal currentBalance = ClsPayment.GetBalance(_unitOfWork, currentUser.ID);

                                        decimal chargeAmt = 0, commAmt = 0;
                                        var comms = _unitOfWork.ApiCommissionRepository.Get(x => x.APIID == getApi.ApiID && x.OperatorID == getApi.OperatorID);
                                        if (comms != null)
                                        {
                                            chargeAmt = ((comms.ChargePer * amount) / 100) + comms.ChargeVal;
                                            commAmt = ((comms.CommPer * amount) / 100) + comms.CommVal;
                                        }
                                        decimal serviceCharge = ((getApi.ServiceChargePer * amount) / 100) + getApi.ServiceChargeVal;
                                        decimal commAmount = ((getApi.CommPer * amount) / 100) + getApi.CommVal;
                                        #endregion
                                        var encResult = EncryptionKeyRequest(_unitOfWork);
                                        if (encResult.StatusCode == clsVariables.APIStatus.Success)
                                        {
                                            #region Transaction
                                            RechargeTransaction objRechargeTransaction = new RechargeTransaction();
                                            objRechargeTransaction.OperatorID = operatorId;
                                            objRechargeTransaction.UserID = currentUser.ID;
                                            objRechargeTransaction.UserDetails = currentUser.OwnerName;
                                            objRechargeTransaction.Number = mobileNo;
                                            objRechargeTransaction.OpeningBalance = currentBalance;
                                            objRechargeTransaction.Amount = amount;
                                            objRechargeTransaction.CommPer = getApi.CommPer;
                                            objRechargeTransaction.CommVal = getApi.CommVal;
                                            objRechargeTransaction.CommAmt = commAmount;
                                            objRechargeTransaction.OtherCharge = 0;
                                            objRechargeTransaction.ServiceCharge = getApi.ServiceChargePer;
                                            objRechargeTransaction.ServiceChargeVal = getApi.ServiceChargeVal;
                                            objRechargeTransaction.ServiceChargeAmt = serviceCharge;
                                            decimal gstAmount = 0, tdsAmount = 0;
                                            if (getApi.OperatorType == clsVariables.OperatorType.P2A)
                                            {
                                                gstAmount = (commAmount * 18) / 118;
                                                tdsAmount = ((commAmount - gstAmount) * 5) / 100;
                                            }
                                            objRechargeTransaction.Cost = amount - serviceCharge + commAmount - gstAmount - tdsAmount;
                                            objRechargeTransaction.ClosingBalance = currentBalance + objRechargeTransaction.Cost;
                                            objRechargeTransaction.ChargeAmount = chargeAmt;
                                            objRechargeTransaction.CommAmount = commAmt;
                                            objRechargeTransaction.TransactionMode = clsVariables.RechargeMode.API;
                                            objRechargeTransaction.TransactionDate = DateTime.Now;
                                            objRechargeTransaction.EditDate = DateTime.Now;
                                            objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                            objRechargeTransaction.ServiceID = getApi.ServiceID;
                                            objRechargeTransaction.ServiceName = "AEPS";
                                            objRechargeTransaction.APIID = getApi.ApiID;
                                            objRechargeTransaction.TransactionID = clientRefID;
                                            objRechargeTransaction.Reason = "NA";
                                            objRechargeTransaction.ApiLogs = "NA";
                                            objRechargeTransaction.OperatorRef = "NA";
                                            objRechargeTransaction.IsProcessed = false;
                                            objRechargeTransaction.RevertTran = false;
                                            objRechargeTransaction.EXTRA1 = bank.IIN;
                                            objRechargeTransaction.EXTRA2 = bank.BankName;
                                            objRechargeTransaction.EXTRA3 = merchantID;
                                            objRechargeTransaction.EXTRA4 = "XXXXXXXX" + aadharNo.Substring(8, 4);
                                            objRechargeTransaction.EXTRA5 = getApi.OperatorType;
                                            objRechargeTransaction.EXTRA7 = "Live";
                                            objRechargeTransaction.EXTRA8 = agentID;
                                            objRechargeTransaction.APIRef = "NA";
                                            objRechargeTransaction.EXTRA9 = "Cash Withdrawal";
                                            objRechargeTransaction.EXTRA10 = "NA";
                                            objRechargeTransaction.IpAddress = HttpContext.Current.Request.UserHostAddress;
                                            _unitOfWork.RechargeTransactionRepository.Insert(objRechargeTransaction);
                                            _unitOfWork.Save();
                                            _response.RefNumber = objRechargeTransaction.ID.ToString(); //ClsMethods.Encrypt(objRechargeTransaction.ID.ToString());
                                            #endregion
                                            bool isPaymentDone = true;
                                            if (isPaymentDone)
                                            {
                                                FNOBalanceCashWithdrawalEntity apiResponse = _apiServices.CashWithdrawal(currentUser.ID, merchantID, mobileNo, aadharNo, amount, bank.BankName, pidData, bank.IIN, latitude, longitude, encResult.Data, objRechargeTransaction.ID);
                                                try
                                                {
                                                    if (apiResponse.StatusCode == clsVariables.APIStatus.Failed)
                                                    {
                                                        #region Failed
                                                        objRechargeTransaction.Status = clsVariables.RechargeStatus.Failure;
                                                        objRechargeTransaction.RevertTran = true;
                                                        objRechargeTransaction.APIRef = apiResponse.APIRef;
                                                        objRechargeTransaction.ClosingBalance = objRechargeTransaction.OpeningBalance;
                                                        objRechargeTransaction.APIRef = apiResponse.APIRef;
                                                        objRechargeTransaction.OperatorRef = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.RRN) ? "NA" : apiResponse.Data.RRN;
                                                        objRechargeTransaction.EXTRA6 = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.Status) ? "NA" : apiResponse.Data.Status;
                                                        objRechargeTransaction.EXTRA10 = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.AvailableBalance) ? "NA" : apiResponse.Data.AvailableBalance;

                                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                                        _response.Message = apiResponse.Message;
                                                        _response.Data = apiResponse.Data;
                                                        #endregion
                                                    }
                                                    else if (apiResponse.StatusCode == clsVariables.APIStatus.Success)
                                                    {
                                                        #region Success
                                                        String remarks = "Number: " + objRechargeTransaction.Number + " |" + getApi.OperatorName + "| Amount: " + objRechargeTransaction.Amount.ToString("0,0.00") + " | Comm Per.: " + getApi.CommPer.ToString("0,0.00") + " | Comm Val.: " + getApi.CommVal.ToString("0,0.00") + " | Charg Per.: " + getApi.ServiceChargePer.ToString("0,0.00") + " | Charg Val.: " + getApi.ServiceChargeVal.ToString("0,0.00") + " Ref Id = " + objRechargeTransaction.ID;
                                                        if (getApi.OperatorType == clsVariables.OperatorType.P2A)
                                                            remarks += " | GST: " + gstAmount.ToString("0,0.00") + " | TDS: " + tdsAmount.ToString("0,0.00");

                                                        isPaymentDone = ClsPayment.DoPaymentAEPS(_unitOfWork, objRechargeTransaction.UserID, 1, objRechargeTransaction.Cost, remarks, clsVariables.PaymentType.AEPSTransaction, objRechargeTransaction.ID);
                                                        if (isPaymentDone)
                                                        {
                                                            objRechargeTransaction.Status = clsVariables.RechargeStatus.Success;
                                                            objRechargeTransaction.APIRef = apiResponse.APIRef;
                                                            objRechargeTransaction.OperatorRef = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.RRN) ? "NA" : apiResponse.Data.RRN;
                                                            objRechargeTransaction.EXTRA6 = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.Status) ? "NA" : apiResponse.Data.Status;
                                                            objRechargeTransaction.EXTRA10 = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.AvailableBalance) ? "NA" : apiResponse.Data.AvailableBalance;

                                                            _response.StatusCode = clsVariables.APIStatus.Success;
                                                            _response.Message = apiResponse.Message;
                                                            _response.Data = apiResponse.Data;
                                                        }
                                                        else
                                                        {
                                                            objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                            objRechargeTransaction.APIRef = apiResponse.APIRef;
                                                            objRechargeTransaction.OperatorRef = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.RRN) ? "NA" : apiResponse.Data.RRN;
                                                            objRechargeTransaction.EXTRA6 = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.Status) ? "NA" : apiResponse.Data.Status;
                                                            objRechargeTransaction.EXTRA10 = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.AvailableBalance) ? "NA" : apiResponse.Data.AvailableBalance;

                                                            _response.StatusCode = clsVariables.APIStatus.Pending;
                                                            _response.Message = apiResponse.Message;
                                                            _response.Data = apiResponse.Data;
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        #region Pending
                                                        objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                        objRechargeTransaction.APIRef = apiResponse.APIRef;
                                                        objRechargeTransaction.OperatorRef = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.RRN) ? "NA" : apiResponse.Data.RRN;
                                                        objRechargeTransaction.EXTRA6 = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.Status) ? "NA" : apiResponse.Data.Status;
                                                        objRechargeTransaction.EXTRA10 = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.AvailableBalance) ? "NA" : apiResponse.Data.AvailableBalance;

                                                        _response.StatusCode = clsVariables.APIStatus.Pending;
                                                        _response.Message = apiResponse.Message;
                                                        _response.Data = apiResponse.Data;
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
                                                objRechargeTransaction.ApiLogs = apiResponse.ApiResponse.ApiUrl + " / " + apiResponse.ApiResponse.ApiRequest + " / " + apiResponse.ApiResponse.ApiResponse;
                                                _unitOfWork.Save();

                                                #region Call Back Method
                                                if (new[] { clsVariables.APIStatus.Pending, clsVariables.APIStatus.Success }.Contains(_response.StatusCode))
                                                {

                                                    FNWLAPITransactionResponseEntity _subAdd = new FNWLAPITransactionResponseEntity()
                                                    {
                                                        Amount = objRechargeTransaction.Amount,
                                                        AdhaarNo = objRechargeTransaction.EXTRA4,
                                                        TxnDate = DateTime.Now.ToString(),
                                                        BankName = objRechargeTransaction.EXTRA2,
                                                        RRN = objRechargeTransaction.OperatorRef,
                                                        Status = objRechargeTransaction.Status,
                                                        CustomerMobile = objRechargeTransaction.Number,
                                                        AvailableBalance = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.AvailableBalance) ? "NA" : apiResponse.Data.AvailableBalance,
                                                        LedgerBalance = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.LedgerBalance) ? "NA" : apiResponse.Data.LedgerBalance,
                                                        Mode = "Cash Withdrawal"
                                                    };
                                                    FNWLAPIClientResponseEntity _add = new FNWLAPIClientResponseEntity()
                                                    {
                                                        StatusCode = _response.StatusCode == clsVariables.APIStatus.Success ? clsVariables.APIStatus.Success : clsVariables.APIStatus.Pending,
                                                        Message = _response.Message,
                                                        Transactions = _subAdd,
                                                        AgentID = objRechargeTransaction.EXTRA8,
                                                        RefId = objRechargeTransaction.ID
                                                    };
                                                    AgentCallBack(_unitOfWork, currentUser.ID, _add);
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                                _response.Message = "Payment configuration not set.";
                                            }
                                        }
                                        else
                                        {
                                            _response.StatusCode = clsVariables.APIStatus.Failed;
                                            _response.Message = encResult.Message;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = "You are trying transaction within block time of successful transaction. Please re-try after sometime.";
                                    }
                                }
                                else
                                {
                                    _response.StatusCode = clsVariables.APIStatus.Failed;
                                    _response.Message = "Transaction already in pending mode. Please try after sometime.";
                                }
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
                            _response.Message = "Invalid bank name.";
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "You are not authorized.";
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Transaction amount should be between 101 to 10000.";
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        public static AEPSAPIReponseEntity BalanceInquiryTransaction(UnitOfWork _unitOfWork, int userID, string merchantID, string clientRefID, string mobileNo, string aadharNo, string bankCode, string pidData, string latitude, string longitude, string agentID)
        {
            AEPSAPIReponseEntity _response = new AEPSAPIReponseEntity();
            try
            {
                var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == userID);
                if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
                {
                    long bankID = Convert.ToInt64(bankCode);
                    var bank = _unitOfWork.MasterIfscRepository.Get(x => x.Id == bankID);
                    if (bank != null)
                    {
                        #region Comm Method
                        var operatorId = TransferAmount(100);
                        var getApi = ClsPayment.GetAPI(_unitOfWork, operatorId, currentUser.SchemeID, 0, 1);
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

                        long txnNumber = Convert.ToInt64(ClsMethods.GenereateUniqueNumber(9));
                        _apiServices = new FinoAepsAPI();

                        var encResult = EncryptionKeyRequest(_unitOfWork);
                        if (encResult.StatusCode == clsVariables.APIStatus.Success)
                        {
                            FNOBalanceCashWithdrawalEntity apiResponse = _apiServices.BalanceInquiry(currentUser.ID, merchantID, mobileNo, aadharNo, bank.BankName, pidData, bank.IIN, latitude, longitude, clientRefID, encResult.Data, txnNumber);
                            if (apiResponse.StatusCode == clsVariables.APIStatus.Failed)
                            {
                                #region Failed
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = apiResponse.Message;
                                _response.Data = apiResponse.Data;
                                #endregion
                            }
                            else if (apiResponse.StatusCode == clsVariables.APIStatus.Success)
                            {
                                #region Success
                                _response.StatusCode = clsVariables.APIStatus.Success;
                                _response.Message = apiResponse.Message;
                                _response.Data = apiResponse.Data;

                                #region Call Back Method
                                FNWLAPITransactionResponseEntity _subAdd = new FNWLAPITransactionResponseEntity()
                                {
                                    Amount = 0,
                                    AdhaarNo = "XXXXXXXX" + aadharNo.Substring(0, 8),
                                    TxnDate = DateTime.Now.ToString(),
                                    BankName = bank.BankName,
                                    RRN = apiResponse.Data.RRN,
                                    Status = clsVariables.RechargeStatus.Success,
                                    CustomerMobile = mobileNo,
                                    AvailableBalance = apiResponse.Data.AvailableBalance,
                                    LedgerBalance = apiResponse.Data.LedgerBalance,
                                    Mode = "Balance Inquiry"
                                };
                                FNWLAPIClientResponseEntity _add = new FNWLAPIClientResponseEntity()
                                {
                                    StatusCode = clsVariables.APIStatus.Success,
                                    Message = _response.Message,
                                    Transactions = _subAdd,
                                    AgentID = agentID,
                                    RefId = txnNumber
                                };
                                AgentCallBack(_unitOfWork, userID, _add);
                                #endregion
                                #endregion
                            }
                            else
                            {
                                #region Pending
                                _response.StatusCode = clsVariables.APIStatus.Pending;
                                _response.Message = apiResponse.Message;
                                _response.Data = apiResponse.Data;
                                #endregion
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = encResult.Message;
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Invalid bank name.";
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

        public static AEPSAPIReponseEntity TransactionInquiry(UnitOfWork _unitOfWork, string transactionRef)
        {
            AEPSAPIReponseEntity _response = new AEPSAPIReponseEntity();
            try
            {
                var rechargeTransaction = ClsMethods.CustomerPendingTransaction(_unitOfWork, transactionRef);
                if (rechargeTransaction != null)
                {
                    if (rechargeTransaction.TransactionDate.AddMinutes(5) < DateTime.Now)
                    {
                        if (10 == rechargeTransaction.ServiceID)
                        {
                            var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == rechargeTransaction.UserID);
                            if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
                            {
                                if (rechargeTransaction.Status == clsVariables.RechargeStatus.Pending)
                                {
                                    #region Transaction Method
                                    _apiServices = new FinoAepsAPI();
                                    var encResult = EncryptionKeyRequest(_unitOfWork);
                                    if (encResult.StatusCode == clsVariables.APIStatus.Success)
                                    {
                                        var apiResponse = _apiServices.TransactionInquiry(rechargeTransaction.UserID, rechargeTransaction.ID, encResult.Data);
                                        try
                                        {
                                            if (apiResponse.StatusCode == clsVariables.APIStatus.Failed)
                                            {
                                                #region Transaction Failed method
                                                string rrn = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.RRN) ? "NA" : apiResponse.Data.RRN;
                                                string apiLogs = apiResponse.ApiResponse == null ? "Empty" : apiResponse.ApiResponse.ApiUrl + "/" + apiResponse.ApiResponse.ApiRequest + "/" + apiResponse.ApiResponse.ApiResponse;
                                                string txnStatus = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.TransactionStatus) ? "NA" : apiResponse.Data.TransactionStatus;
                                                string aadharNo = string.IsNullOrEmpty(rechargeTransaction.EXTRA4) ? "XXXXXXXX" : rechargeTransaction.EXTRA4;
                                                string bankName = string.IsNullOrEmpty(rechargeTransaction.EXTRA2) ? "NA" : rechargeTransaction.EXTRA2;
                                                string customerNo = string.IsNullOrEmpty(rechargeTransaction.EXTRA3) ? "NA" : rechargeTransaction.EXTRA3;
                                                ClsMethods.AEPSCustomerPendingTransaction(_unitOfWork, rechargeTransaction.ID, rrn, aadharNo, bankName, customerNo, txnStatus, apiLogs, 3);
                                                rechargeTransaction = ClsMethods.CustomerPendingTransaction(_unitOfWork, rechargeTransaction.ID.ToString());

                                                _response.StatusCode = rechargeTransaction.Status == clsVariables.RechargeStatus.Success ? clsVariables.APIStatus.Success : rechargeTransaction.Status == clsVariables.RechargeStatus.Pending ? clsVariables.APIStatus.Pending : clsVariables.APIStatus.Failed;
                                                _response.Message = apiResponse.Message;
                                                _response.Data = apiResponse.Data;

                                                //FNWLAPITransactionResponseEntity _subAdd = new FNWLAPITransactionResponseEntity()
                                                //{
                                                //    Amount = rechargeTransaction.Amount,
                                                //    AdhaarNo = rechargeTransaction.EXTRA4,
                                                //    TxnDate = rechargeTransaction.EditDate.ToString(),
                                                //    BankName = rechargeTransaction.EXTRA2,
                                                //    RRN = rechargeTransaction.OperatorRef,
                                                //    Status = rechargeTransaction.Status,
                                                //    CustomerMobile = rechargeTransaction.EXTRA3,
                                                //    AvailableBalance = "",
                                                //    LedgerBalance = "",
                                                //    Mode = "Cash Withdrawal"
                                                //};

                                                //FNWLAPIClientResponseEntity _add = new FNWLAPIClientResponseEntity()
                                                //{
                                                //    StatusCode = _response.StatusCode,
                                                //    Message = _response.Message,
                                                //    Transactions = _subAdd,
                                                //    AgentID = rechargeTransaction.EXTRA8,
                                                //    RefId = rechargeTransaction.ID
                                                //};
                                                //AgentCallBack(_unitOfWork, rechargeTransaction.UserID, _add);
                                                #endregion
                                            }
                                            else if (apiResponse.StatusCode == clsVariables.APIStatus.Success)
                                            {
                                                #region Transaction Pending To Success
                                                var operatorName = _unitOfWork.OperatorRepository.Get(x => x.ID == rechargeTransaction.OperatorID);
                                                decimal gstAmount = 0, tdsAmount = 0;
                                                String remarks = "Number: " + rechargeTransaction.Number + " |" + operatorName.OperatorName + "| Amount: " + rechargeTransaction.Amount.ToString("0,0.00") + " | Comm Per.: " + rechargeTransaction.CommPer.ToString("0,0.00") + " | Comm Val.: " + rechargeTransaction.CommVal.ToString("0,0.00") + " | Charg Per.: " + rechargeTransaction.ServiceCharge.ToString("0,0.00") + " | Charg Val.: " + rechargeTransaction.ServiceChargeVal.ToString("0,0.00") + " Ref Id = " + rechargeTransaction.ID;
                                                if (operatorName.OperatorType == clsVariables.OperatorType.P2A)
                                                {
                                                    gstAmount = (rechargeTransaction.CommAmt * 18) / 118;
                                                    tdsAmount = ((rechargeTransaction.CommAmt - gstAmount) * 5) / 100;
                                                    remarks += " | GST: " + gstAmount.ToString("0,0.00") + " | TDS: " + tdsAmount.ToString("0,0.00");
                                                }

                                                bool isPaymentReverted = ClsPayment.DoPaymentAEPS(_unitOfWork, rechargeTransaction.UserID, 1, rechargeTransaction.Cost, remarks, clsVariables.PaymentType.AEPSTransaction, rechargeTransaction.ID);
                                                if (isPaymentReverted)
                                                {
                                                    string rrn = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.RRN) ? "NA" : apiResponse.Data.RRN;
                                                    string apiLogs = apiResponse.ApiResponse == null ? "Empty" : apiResponse.ApiResponse.ApiUrl + "/" + apiResponse.ApiResponse.ApiRequest + "/" + apiResponse.ApiResponse.ApiResponse;
                                                    string txnStatus = apiResponse.Data == null ? "NA" : string.IsNullOrEmpty(apiResponse.Data.TransactionStatus) ? "NA" : apiResponse.Data.TransactionStatus;
                                                    string aadharNo = string.IsNullOrEmpty(rechargeTransaction.EXTRA4) ? "XXXXXXXX" : rechargeTransaction.EXTRA4;
                                                    string bankName = string.IsNullOrEmpty(rechargeTransaction.EXTRA2) ? "NA" : rechargeTransaction.EXTRA2;
                                                    string customerNo = string.IsNullOrEmpty(rechargeTransaction.EXTRA3) ? "NA" : rechargeTransaction.EXTRA3;
                                                    ClsMethods.AEPSCustomerPendingTransaction(_unitOfWork, rechargeTransaction.ID, rrn, aadharNo, bankName, customerNo, txnStatus, apiLogs, 1);
                                                    rechargeTransaction = ClsMethods.CustomerPendingTransaction(_unitOfWork, rechargeTransaction.ID.ToString());

                                                    _response.StatusCode = rechargeTransaction.Status == clsVariables.RechargeStatus.Success ? clsVariables.APIStatus.Success : rechargeTransaction.Status == clsVariables.RechargeStatus.Pending ? clsVariables.APIStatus.Pending : clsVariables.APIStatus.Failed;
                                                    _response.Message = apiResponse.Message;
                                                    _response.Data = apiResponse.Data;

                                                    FNWLAPITransactionResponseEntity _subAdd = new FNWLAPITransactionResponseEntity()
                                                    {
                                                        Amount = rechargeTransaction.Amount,
                                                        AdhaarNo = rechargeTransaction.EXTRA4,
                                                        TxnDate = rechargeTransaction.EditDate.ToString(),
                                                        BankName = rechargeTransaction.EXTRA2,
                                                        RRN = rechargeTransaction.OperatorRef,
                                                        Status = rechargeTransaction.Status,
                                                        CustomerMobile = rechargeTransaction.EXTRA3,
                                                        AvailableBalance = "",
                                                        LedgerBalance = "",
                                                        Mode = "Cash Withdrawal"
                                                    };

                                                    FNWLAPIClientResponseEntity _add = new FNWLAPIClientResponseEntity()
                                                    {
                                                        StatusCode = _response.StatusCode,
                                                        Message = _response.Message,
                                                        Transactions = _subAdd,
                                                        AgentID = rechargeTransaction.EXTRA8,
                                                        RefId = rechargeTransaction.ID
                                                    };
                                                    AgentCallBack(_unitOfWork, rechargeTransaction.UserID, _add);
                                                }
                                                else
                                                {
                                                    _response.StatusCode = clsVariables.APIStatus.Pending;
                                                    _response.Message = "Transaction Pending.";
                                                    _response.Data = apiResponse.Data;
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                _response.StatusCode = clsVariables.APIStatus.Pending;
                                                _response.Message = apiResponse.Message;
                                                _response.Data = apiResponse.Data;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _response.StatusCode = clsVariables.APIStatus.Pending;
                                            _response.Message = Message.Unknownresponse + ex.Message;
                                        }
                                    }
                                    else
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = encResult.Message;
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
                            _response.Message = "AEPS and ATM transaction are allowed.";
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

        public static APIReponseEntity BalanceRequest(UnitOfWork _unitOfWork, int userID, string agentID)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var user = _unitOfWork.UserRepository.Get(x => x.ID == userID);
                if (user != null)
                {
                    string url = user.AepsInquiryUrl;
                    if (!string.IsNullOrEmpty(url))
                    {
                        AEPSRequestLog _add = new AEPSRequestLog()
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
                        _unitOfWork.AEPSRequestLogRepository.Insert(_add);
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
                                _response.Data = result.Transactions;
                            }
                            else if (result.StatusCode == clsVariables.APIStatus.Failed)
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = result.Message;
                                _response.Data = result.Transactions;
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Pending;
                                _response.Message = result.Message;
                                _response.Data = result.Transactions;
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

        public static APIReponseEntity CommRequest(UnitOfWork _unitOfWork, int userID, decimal amount)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == userID);
                if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
                {
                    #region Comm Method
                    amount = amount == 0 ? 1 : amount;
                    var operatorId = TransferAmount(amount);
                    var getApi = ClsPayment.GetAPI(_unitOfWork, operatorId, currentUser.SchemeID, 0, 1);
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

                    TransactionCommDetailsEntity _subResponse = new TransactionCommDetailsEntity();
                    _subResponse.Amount = amount;
                    _subResponse.Surcharge = ((getApi.ServiceChargePer * amount) / 100) + getApi.ServiceChargeVal;
                    _subResponse.Margin = ((getApi.CommPer * amount) / 100) + getApi.CommVal;
                    if (getApi.OperatorType == clsVariables.OperatorType.P2A)
                    {
                        _subResponse.GST = (_subResponse.Margin * 18) / 118;
                        _subResponse.TDS = ((_subResponse.Margin - _subResponse.GST) * 5) / 100;
                    }

                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = _subResponse;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "You are not authorized.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public static AEPSMiniAPIReponseEntity MiniStatements(UnitOfWork _unitOfWork, int userID, string merchantID, string clientRefID, string mobileNo, string aadharNo, string bankCode, string pidData, string latitude, string longitude, string agentID)
        {
            AEPSMiniAPIReponseEntity _response = new AEPSMiniAPIReponseEntity();
            try
            {
                var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == userID);
                if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
                {
                    long bankID = Convert.ToInt64(bankCode);
                    var bank = _unitOfWork.MasterIfscRepository.Get(x => x.Id == bankID);
                    if (bank != null)
                    {
                        _response.BankName = bank.BankName;
                        _response.AgentID = agentID;
                        _response.Merchant = merchantID;
                        _response.MobileNo = mobileNo;
                        #region Comm Method
                        var operatorId = TransferAmount(100);
                        var getApi = ClsPayment.GetAPI(_unitOfWork, operatorId, currentUser.SchemeID, 0, 1);
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

                        long txnNumber = Convert.ToInt64(ClsMethods.GenereateUniqueNumber(9));
                        _apiServices = new FinoAepsAPI();
                        _response.RefNumber = Helpers.FinoEncryption.Base64Encode(txnNumber.ToString());
                        var encResult = EncryptionKeyRequest(_unitOfWork, 2);
                        if (encResult.StatusCode == clsVariables.APIStatus.Success)
                        {
                            FNOMiniStatementEntity apiResponse = _apiServices.MiniStatement(currentUser.ID, merchantID, mobileNo, aadharNo, bank.BankName, pidData, bank.IIN, latitude, longitude, clientRefID, encResult.Data, txnNumber);
                            if (apiResponse.StatusCode == clsVariables.APIStatus.Failed)
                            {
                                #region Failed
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = apiResponse.Message;
                                _response.Data = apiResponse.Data;
                                #endregion
                            }
                            else if (apiResponse.StatusCode == clsVariables.APIStatus.Success)
                            {
                                #region Success
                                _response.StatusCode = clsVariables.APIStatus.Success;
                                _response.Message = apiResponse.Message;
                                _response.Data = apiResponse.Data;
                                if (_response.Data != null)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(_response.Data.localDate))
                                        {
                                            string month = _response.Data.localDate.Substring(0, 2);
                                            string date = _response.Data.localDate.Substring(2, 2);
                                            _response.Data.localDate = date + "/" + month;
                                        }
                                        if (!string.IsNullOrEmpty(_response.Data.localTime))
                                        {
                                            string hh = _response.Data.localTime.Substring(0, 2);
                                            string mm = _response.Data.localTime.Substring(2, 2);
                                            string ss = _response.Data.localTime.Substring(4, 2);
                                            _response.Data.localTime = hh + ":" + mm + ":" + ss;
                                        }
                                    }
                                    catch { }
                                }
                                #endregion
                            }
                            else
                            {
                                #region Pending
                                _response.StatusCode = clsVariables.APIStatus.Pending;
                                _response.Message = apiResponse.Message;
                                _response.Data = apiResponse.Data;
                                #endregion
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = encResult.Message;
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Invalid bank name.";
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
        private static bool AgentCallBack(UnitOfWork _unitOfWork, int userId, FNWLAPIClientResponseEntity item)
        {
            string requestUrl = string.Empty, requestBody = string.Empty, callbackResponse = string.Empty; bool doStatus = false;
            try
            {
                var user = _unitOfWork.UserRepository.Get(x => x.ID == userId);
                if (user != null && !string.IsNullOrEmpty(user.AepsTransactionUrl))
                {
                    requestUrl = user.AepsTransactionUrl;
                    requestBody = new JavaScriptSerializer().Serialize(item);
                    callbackResponse = ClsMethods.HttpRequestJson(requestUrl, item);
                }
                else
                    callbackResponse = "Callbackurl not found.";
            }
            catch (Exception EX) { callbackResponse += EX.Message; }
            ClsMethods.ResponseLogs(userId, item.Transactions.CustomerMobile, "ClientCallBack", requestUrl, requestBody, callbackResponse, item.RefId, "WL", "");
            return doStatus;
        }
        #endregion
    }
}
