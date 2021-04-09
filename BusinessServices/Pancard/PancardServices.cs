using AutoMapper;
using BusinessEntities;
using BusinessServices.Pancard;
using BusinessServices.Resource;
using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace BusinessServices
{
    public class PancardServices : IPancardServices
    {
        #region Private member variables.
        private readonly UnitOfWork _unitOfWork;
        private UTIAPIServices _panServices;
        private readonly string PanRedirectURL = ConfigurationManager.AppSettings["Pan_Redirect_URL"];
        private readonly string PanPaymentRedirectURL = ConfigurationManager.AppSettings["Pan_Payment_Redirect_URL"];
        #endregion

        #region Public constructor.
        /// <summary>
        /// Public constructor.
        /// </summary>
        public PancardServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        public PanRedirectResponseEntity PancardRedirect(int userID, string agentID,string mobileNo)
        {
            PanRedirectResponseEntity _response = new PanRedirectResponseEntity();
            try
            {
                var currentUser = _unitOfWork.UserRepository.Get(x => x.ID == userID);
                if (currentUser != null && currentUser.Usertype == clsVariables.UserType.API)
                {
                    var balanceResult = ClsPayment.BalanceRequest(_unitOfWork, userID, agentID);
                    if (balanceResult.StatusCode == clsVariables.APIStatus.Success)
                    {
                        if (balanceResult.MainBalance > 0)
                        {
                            int operatorId = 151;
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
                            PANTransactionRequest _add = new PANTransactionRequest()
                            {
                                UserID = currentUser.ID,
                                AgentID = agentID,
                                MobileNo = mobileNo,
                                AddDate = DateTime.Now,
                                Status = clsVariables.RechargeStatus.Pending,
                                RequestLog = null
                            };
                            _unitOfWork.PANTransactionRequestRepository.Insert(_add);
                            _unitOfWork.Save();

                            _panServices = new UTIAPIServices();
                            string response = _panServices.UtiGenerateChecksum(mobileNo, _add.ID);
                            _add.RequestLog = PanRedirectURL + " / " + response;
                            _unitOfWork.PANTransactionRequestRepository.Update(_add);
                            _unitOfWork.Save();
                            _response.StatusCode = clsVariables.APIStatus.Success;
                            _response.Message = Message.Su;
                            _response.URL = PanRedirectURL + response;
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Merchant main balance is very low.";
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = balanceResult.Message;
                    }
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
                _response.Message = Message.Errorsm + ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// UTI Payment Gateway
        /// </summary>
        /// <param name="applicationNo"></param>
        /// <param name="userID"></param>
        /// <param name="UTITSLTransID"></param>
        /// <param name="transAmt"></param>
        /// <param name="transactionNo"></param>
        /// <param name="appName"></param>
        /// <param name="PANCardType"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        public UTIPancardResponseEntity PaymentGateway(string applicationNo, string userID, string UTITSLTransID, string transAmt, string transactionNo, string appName, string PANCardType, string payment)
        {
            UTIPancardResponseEntity _response = new UTIPancardResponseEntity();
            int SchemeID = 0;
            decimal balance = 0;
            bool isPayment = false;
            int operatorId = 151;
            try
            {
                applicationNo = DecryptionData(applicationNo);
                userID = DecryptionData(userID);
                UTITSLTransID = DecryptionData(UTITSLTransID);
                transAmt = DecryptionData(transAmt);
                transactionNo = DecryptionData(transactionNo);
                appName = DecryptionData(appName);
                PANCardType = DecryptionData(PANCardType);
                payment = DecodeData(payment);
                _response.UTITxnID = UTITSLTransID;
                _response.TxnID = "0";
                _response.UTITxnID = UTITSLTransID;
                _response.ApplicationNo = applicationNo;
                _response.Amount = transAmt;

                decimal amount = Convert.ToDecimal(transAmt);
                long requestRefId = Convert.ToInt64(transactionNo.Replace("MASFPL", ""));
                var panTransaction = _unitOfWork.PANTransactionRequestRepository.Get(x => x.ID == requestRefId);
                if (panTransaction != null)
                {
                    var currentUser = _unitOfWork.UserRepository.Get(obj => obj.ID == panTransaction.UserID && obj.Status == true);
                    if (currentUser != null && currentUser.Status == true)
                    {
                        if (currentUser.Usertype == clsVariables.UserType.API)
                        {
                            SchemeID = currentUser.SchemeID;

                            balance = ClsPayment.GetMainBalance(_unitOfWork, currentUser.ID);
                            if (balance >= amount && amount > 0)
                            {
                                var getAPI = ClsPayment.GetAPI(_unitOfWork, operatorId, SchemeID, 0, 1);
                                if (getAPI != null)
                                {
                                    #region Commission Setting
                                    #region Operator and API Auth Method
                                    if (getAPI.ApiStatus == false)
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = Message.ServiceDown;
                                        _response.TxnID = transactionNo;
                                        _response.UTITxnID = UTITSLTransID;
                                        _response.ApplicationNo = applicationNo;
                                        _response.Amount = transAmt;
                                        return _response;
                                    }
                                    else if (getAPI.OperatorStatus == "0")
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = "Operator server down.";
                                        _response.TxnID = transactionNo;
                                        _response.UTITxnID = UTITSLTransID;
                                        _response.ApplicationNo = applicationNo;
                                        _response.Amount = transAmt;
                                        return _response;
                                    }
                                    else if (!getAPI.ApiStatus)
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = Message.ServiceDown;
                                        _response.TxnID = transactionNo;
                                        _response.UTITxnID = UTITSLTransID;
                                        _response.ApplicationNo = applicationNo;
                                        _response.Amount = transAmt;
                                        return _response;
                                    }
                                    #endregion
                                    #region Service Auth Method
                                    if (!ClsPayment.ServiceAuth(_unitOfWork, currentUser.ID, getAPI.ServiceID))
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = Message.ServiceNotAuth;
                                        return _response;
                                    }
                                    decimal serviceCharge = ((getAPI.ServiceChargePer * amount) / 100) + getAPI.ServiceChargeVal;
                                    decimal commAmount = ((getAPI.CommPer * amount) / 100) + getAPI.CommVal;
                                    if (balance < (amount + serviceCharge - commAmount))
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = "Low Balance";
                                        _response.TxnID = transactionNo;
                                        _response.UTITxnID = UTITSLTransID;
                                        _response.ApplicationNo = applicationNo;
                                        _response.Amount = transAmt;
                                        return _response;
                                    }
                                    #endregion
                                    #region API Comm method
                                    decimal chargeAmt = 0, commAmt = 0;
                                    var comms = _unitOfWork.ApiCommissionRepository.Get(x => x.APIID == getAPI.ApiID && x.OperatorID == getAPI.OperatorID);
                                    if (comms != null)
                                    {
                                        chargeAmt = ((comms.ChargePer * amount) / 100) + comms.ChargeVal;
                                        commAmt = ((comms.CommPer * amount) / 100) + comms.CommVal;
                                    }
                                    #endregion
                                    #endregion
                                    var paymentResult = ClsPayment.PaymentRequest(_unitOfWork, currentUser.ID, panTransaction.AgentID, applicationNo, getAPI.OperatorApiCode, amount, clsVariables.PaymentType.PAN);
                                    if (paymentResult.StatusCode == clsVariables.APIStatus.Success)
                                    {
                                        var isTxnNotExist = ClsMethods.TransactionValidate(_unitOfWork, currentUser.ID, applicationNo, amount, paymentResult.ClientTransactionID);
                                        if (isTxnNotExist != 2)
                                        {
                                            if (isTxnNotExist == 0)
                                            {
                                                #region Transaction Method
                                                decimal gstAmount = 0, tdsAmount = 0;
                                                if (getAPI.OperatorType == clsVariables.OperatorType.P2A)
                                                {
                                                    gstAmount = (commAmount * 18) / 118;
                                                    tdsAmount = ((commAmount - gstAmount) * 5) / 100;
                                                }
                                                RechargeTransaction objRechargeTransaction = new RechargeTransaction()
                                                {
                                                    OperatorID = getAPI.OperatorID,
                                                    UserID = currentUser.ID,
                                                    UserDetails = currentUser.OwnerName,
                                                    Number = applicationNo,
                                                    OpeningBalance = balance,
                                                    Amount = amount,
                                                    CommPer = getAPI.CommPer,
                                                    CommVal = getAPI.CommVal,
                                                    CommAmt = commAmount,
                                                    OtherCharge = 0,
                                                    ServiceCharge = getAPI.ServiceChargePer,
                                                    ServiceChargeVal = getAPI.ServiceChargeVal,
                                                    ServiceChargeAmt = serviceCharge,
                                                    Cost = amount + serviceCharge - commAmount + gstAmount + tdsAmount,
                                                    ClosingBalance = balance - (amount + serviceCharge - commAmount + gstAmount + tdsAmount),
                                                    ChargeAmount = chargeAmt,
                                                    CommAmount = commAmt,
                                                    TransactionMode = clsVariables.RechargeMode.API,
                                                    TransactionDate = DateTime.Now,
                                                    EditDate = DateTime.Now,
                                                    Status = clsVariables.RechargeStatus.Pending,
                                                    ServiceID = getAPI.ServiceID,
                                                    ServiceName = "Pancard",
                                                    APIID = getAPI.ApiID,
                                                    TransactionID = paymentResult.ClientTransactionID,
                                                    Reason = "",
                                                    ApiLogs = "",
                                                    IsProcessed = false,
                                                    RevertTran = false,
                                                    EXTRA1 = "Cash",
                                                    EXTRA2 = PANCardType,
                                                    EXTRA3 = payment,
                                                    EXTRA4 = transactionNo,
                                                    EXTRA5 = getAPI.OperatorType,
                                                    EXTRA6 = panTransaction.MobileNo,
                                                    EXTRA7 = null,
                                                    EXTRA8 = null,
                                                    EXTRA9 = panTransaction.AgentID,
                                                    EXTRA10 = null,
                                                    IpAddress = HttpContext.Current.Request.UserHostAddress,
                                                    APIRef = appName,
                                                    ActionName = null,
                                                    CircleID = 0,
                                                    LapuNumber = null,
                                                    OperatorRef = UTITSLTransID,
                                                    RetryAPiName = null,
                                                    UpdatedBy = null
                                                };
                                                _unitOfWork.RechargeTransactionRepository.Insert(objRechargeTransaction);
                                                _unitOfWork.Save();
                                                #endregion
                                                String remarks = "Number: " + objRechargeTransaction.Number + " |" + getAPI.OperatorName + "| Amount: " + amount.ToString("0,0.00") + " | Comm Per.: " + getAPI.CommPer.ToString("0,0.00") + " | Comm Val.: " + getAPI.CommVal.ToString("0,0.00") + " | Charg Per.: " + getAPI.ServiceChargePer.ToString("0,0.00") + " | Charg Val.: " + getAPI.ServiceChargeVal.ToString("0,0.00"); if (getAPI.OperatorType == clsVariables.OperatorType.P2A) { remarks += " | GST: " + gstAmount.ToString("0,0.00") + " | TDS: " + tdsAmount.ToString("0,0.00"); }
                                                remarks += " Ref Id = " + objRechargeTransaction.ID;
                                                isPayment = ClsPayment.DoPaymentMain(_unitOfWork, 1, currentUser.ID, objRechargeTransaction.Cost, remarks, clsVariables.PaymentType.PAN, objRechargeTransaction.ID);
                                                if (isPayment)
                                                {
                                                    #region S
                                                    objRechargeTransaction.Status = clsVariables.RechargeStatus.Pending;
                                                    objRechargeTransaction.Reason = "Transaction accepted";
                                                    objRechargeTransaction.EditDate = DateTime.Now;
                                                    _unitOfWork.Save();

                                                    panTransaction.Status = clsVariables.RechargeStatus.Success;
                                                    _unitOfWork.PANTransactionRequestRepository.Update(panTransaction);
                                                    _unitOfWork.Save();

                                                    _response.StatusCode = clsVariables.APIStatus.Success;
                                                    _response.Message = "Your transaction has been success. Transaction Id: " + objRechargeTransaction.ID;
                                                    _response.TxnID = objRechargeTransaction.ID.ToString();
                                                    _response.UTITxnID = objRechargeTransaction.OperatorRef;
                                                    _response.ApplicationNo = objRechargeTransaction.Number;
                                                    _response.Amount = objRechargeTransaction.Amount.ToString();
                                                    return _response;
                                                    #endregion

                                                }
                                                else
                                                {
                                                    #region F
                                                    isPayment = ClsPayment.DoPaymentMain(_unitOfWork, currentUser.ID, 1, objRechargeTransaction.Cost, remarks, clsVariables.PaymentType.TransactionR, objRechargeTransaction.ID);
                                                    if (isPayment)
                                                    {
                                                        objRechargeTransaction.Status = clsVariables.RechargeStatus.Failure;
                                                        objRechargeTransaction.EditDate = DateTime.Now;
                                                        objRechargeTransaction.Reason = clsVariables.RechargeStatus.Failure;
                                                        objRechargeTransaction.ClosingBalance = objRechargeTransaction.OpeningBalance;
                                                        objRechargeTransaction.RevertTran = true;
                                                        _unitOfWork.Save();

                                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                                        _response.Message = "Your transaction has been failure. Transaction Id: " + objRechargeTransaction.ID;
                                                        _response.TxnID = objRechargeTransaction.ID.ToString();
                                                        _response.UTITxnID = objRechargeTransaction.OperatorRef;
                                                        _response.ApplicationNo = objRechargeTransaction.Number;
                                                        _response.Amount = objRechargeTransaction.Amount.ToString();
                                                    }
                                                    else
                                                    {
                                                        _response.StatusCode = clsVariables.APIStatus.Pending;
                                                        _response.Message = "Your transaction has been pending. Transaction Id: " + objRechargeTransaction.ID;
                                                        _response.TxnID = objRechargeTransaction.ID.ToString();
                                                        _response.UTITxnID = objRechargeTransaction.OperatorRef;
                                                        _response.ApplicationNo = objRechargeTransaction.Number;
                                                        _response.Amount = objRechargeTransaction.Amount.ToString();
                                                    }
                                                    panTransaction.Status = clsVariables.RechargeStatus.Success;
                                                    _unitOfWork.PANTransactionRequestRepository.Update(panTransaction);
                                                    _unitOfWork.Save();
                                                    return _response;
                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                                _response.Message = "You are trying transaction within block time of successful transaction. Please re-try after sometime.";
                                                _response.TxnID = transactionNo;
                                                _response.UTITxnID = UTITSLTransID;
                                                _response.ApplicationNo = applicationNo;
                                                _response.Amount = transAmt;
                                                return _response;
                                            }
                                        }
                                        else
                                        {
                                            _response.StatusCode = clsVariables.APIStatus.Failed;
                                            _response.Message = "Check PSA registration status.";
                                            _response.TxnID = transactionNo;
                                            _response.UTITxnID = UTITSLTransID;
                                            _response.ApplicationNo = applicationNo;
                                            _response.Amount = transAmt;
                                            return _response;
                                        }
                                    }
                                    else
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = paymentResult.Message;
                                        return _response;
                                    }
                                }
                                else
                                {
                                    _response.StatusCode = clsVariables.APIStatus.Failed;
                                    _response.Message = "API configuration not set properly. Contact your service provider.";
                                    _response.TxnID = transactionNo;
                                    _response.UTITxnID = UTITSLTransID;
                                    _response.ApplicationNo = applicationNo;
                                    _response.Amount = transAmt;
                                    return _response;
                                }
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = "You don't have sufficient wallet balance, please topup your account.";
                                _response.TxnID = transactionNo;
                                _response.UTITxnID = UTITSLTransID;
                                _response.ApplicationNo = applicationNo;
                                _response.Amount = transAmt;
                                return _response;
                            }

                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "You are not authorized to use services.";
                            _response.TxnID = transactionNo;
                            _response.UTITxnID = UTITSLTransID;
                            _response.ApplicationNo = applicationNo;
                            _response.Amount = transAmt;
                            return _response;
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Your Account is deactivated.";
                        _response.TxnID = transactionNo;
                        _response.UTITxnID = UTITSLTransID;
                        _response.ApplicationNo = applicationNo;
                        _response.Amount = transAmt;
                        return _response;
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Your transaction refnumber is invalid.";
                    _response.TxnID = transactionNo;
                    _response.UTITxnID = UTITSLTransID;
                    _response.ApplicationNo = applicationNo;
                    _response.Amount = transAmt;
                    return _response;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Pending;
                _response.Message = ex.Message;
                _response.TxnID = transactionNo;
                _response.UTITxnID = UTITSLTransID;
                _response.ApplicationNo = applicationNo;
                _response.Amount = transAmt;
                return _response;
            }
            finally { _unitOfWork.Dispose(); }
        }

        /// <summary>
        /// Decryption data
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string DecryptionData(string item)
        {
            try
            {
                item = HttpUtility.UrlDecode(item);
                return UTIEncryptions.DecryptData(item);
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Decoded data
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string DecodeData(string item)
        {
            try
            {
                return HttpUtility.UrlDecode(item);
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Encryption Data
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string EncryptionData(string item)
        {
            try
            {
                item = UTIEncryptions.EncryptData(item);
                //return HttpUtility.UrlEncode(item);
                return item;
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Payment Verification
        /// </summary>
        /// <param name="encData"></param>
        /// <returns></returns>
        public UTIPancardResponseEntity PaymentGatewayVerification(string encData)
        {
            UTIPancardResponseEntity _response = new UTIPancardResponseEntity();
            string transID = string.Empty, UTITSLTransID = string.Empty, transAmt = string.Empty;
            try
            {
                encData = HttpUtility.UrlDecode(encData);
                encData = UTIEncryptions.DecryptData(encData);
                string[] line = encData.Split('&');
                transID = line[0].Split('|')[1];
                UTITSLTransID = line[1].Split('|')[1];
                transAmt = line[2].Split('|')[1];
                long txnID = Convert.ToInt64(transID);
                decimal amount = Convert.ToDecimal(transAmt);

                var rechargeTransaction = ClsMethods.CustomerPendingTransaction(_unitOfWork, transID);
                if (rechargeTransaction != null && rechargeTransaction.OperatorRef == UTITSLTransID)
                {
                    ClsMethods.CustomerPendingTransaction(_unitOfWork, rechargeTransaction.ID, rechargeTransaction.OperatorRef, 1);

                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Datanotfound;
                    _response.TxnID = transID;
                    _response.UTITxnID = UTITSLTransID;
                    _response.Amount = transAmt;
                    rechargeTransaction = ClsMethods.CustomerPendingTransaction(_unitOfWork, transID);
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
                    AgentCallBack(rechargeTransaction.UserID, _add, rechargeTransaction.ID);
                    #endregion
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.Datanotfound;
                    _response.TxnID = transID;
                    _response.UTITxnID = UTITSLTransID;
                    _response.Amount = transAmt;
                }

            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Pending;
                _response.Message = ex.Message;
                _response.TxnID = transID;
                _response.UTITxnID = UTITSLTransID;
                _response.Amount = transAmt;
            }
            finally { _unitOfWork.Dispose(); }
            return _response;
        }

        public UserValidateResponseEntity ValidateUser(string refName)
        {
            UserValidateResponseEntity _response = new UserValidateResponseEntity();
            try
            {
                string[] splitData = refName.Split('|');
                string token = splitData[0];
                _response.AgentID = splitData[1];
                _response.MerchantID = splitData[2];
                var user = _unitOfWork.UserTokenRepository.Get(x => x.SessionValue == token);
                if (user != null)
                    _response.UserID = user.UserID;
                else
                    _response.UserID = 0;
            }
            catch (Exception) { _response.UserID = 0; }
            return _response;
        }

        public APIReponseEntity LastTransaction(int userID, string agentID)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                //var result = _unitOfWork.SqlQuery<AEPSLastTransaction_Result>("AEPSLastTransaction @UserID,@AgentID", new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }, new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }).ToList();
                var result = _unitOfWork.SqlQuery<PANWLTransactions_Result>("PANWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
                        , new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }
                        , new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }
                        , new SqlParameter("FromDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now }
                        , new SqlParameter("ToDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now }
                        , new SqlParameter("Number", System.Data.SqlDbType.VarChar) { Value = "" }
                        , new SqlParameter("TxnID", System.Data.SqlDbType.BigInt) { Value = 0 }
                        , new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 4 }).ToList();
                if (result.Any())
                {
                    Mapper.CreateMap<PANWLTransactions_Result, PANTransactionReportEntity>();
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = Mapper.Map<List<PANWLTransactions_Result>, List<PANTransactionReportEntity>>(result);
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.Datanotfound;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public List<PANTransactionReportEntity> TransactionReport(int userID, string agentID, string fromDate, string toDate, string mobileNo)
        {
            List<PANTransactionReportEntity> _response = new List<PANTransactionReportEntity>();
            try
            {
                DateTime fDate = Convert.ToDateTime(fromDate), tDate = Convert.ToDateTime(toDate).AddDays(1);
                if (!string.IsNullOrEmpty(mobileNo))
                {
                    var result = _unitOfWork.SqlQuery<PANWLTransactions_Result>("PANWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
                        , new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }
                        , new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }
                        , new SqlParameter("FromDate", System.Data.SqlDbType.DateTime) { Value = fDate }
                        , new SqlParameter("ToDate", System.Data.SqlDbType.DateTime) { Value = tDate }
                        , new SqlParameter("Number", System.Data.SqlDbType.VarChar) { Value = mobileNo }
                        , new SqlParameter("TxnID", System.Data.SqlDbType.BigInt) { Value = 0 }
                        , new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 2 }).ToList();

                    if (result.Any())
                    {
                        Mapper.CreateMap<PANWLTransactions_Result, PANTransactionReportEntity>();
                        _response = Mapper.Map<List<PANWLTransactions_Result>, List<PANTransactionReportEntity>>(result);
                    }
                }
                else
                {
                    var result = _unitOfWork.SqlQuery<PANWLTransactions_Result>("PANWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
                        , new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }
                        , new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }
                        , new SqlParameter("FromDate", System.Data.SqlDbType.DateTime) { Value = fDate }
                        , new SqlParameter("ToDate", System.Data.SqlDbType.DateTime) { Value = tDate }
                        , new SqlParameter("Number", System.Data.SqlDbType.VarChar) { Value = mobileNo }
                        , new SqlParameter("TxnID", System.Data.SqlDbType.BigInt) { Value = 0 }
                        , new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 1 }).ToList();

                    if (result.Any())
                    {
                        Mapper.CreateMap<PANWLTransactions_Result, PANTransactionReportEntity>();
                        _response = Mapper.Map<List<PANWLTransactions_Result>, List<PANTransactionReportEntity>>(result);
                    }
                }
            }
            catch (Exception) { }
            return _response;
        }

        public PANReceiptReponseEntity Receipt(int userID, string agentID, string refNumber)
        {
            PANReceiptReponseEntity _response = new PANReceiptReponseEntity();
            try
            {
                long txnID = Convert.ToInt64(refNumber);
                var result = _unitOfWork.SqlQuery<PANWLTransactions_Result>("PANWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
                        , new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }
                        , new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }
                        , new SqlParameter("FromDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now }
                        , new SqlParameter("ToDate", System.Data.SqlDbType.DateTime) { Value = DateTime.Now }
                        , new SqlParameter("Number", System.Data.SqlDbType.VarChar) { Value = "" }
                        , new SqlParameter("TxnID", System.Data.SqlDbType.BigInt) { Value = txnID }
                        , new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 3 }).ToList();

                if (result.Any())
                {
                    var results = (from x in result
                                   select new PANReceiptDetailsEntity
                                   {
                                       Number = x.Number,
                                       TranRef = x.TransRef,
                                       TxnDate = x.TransactionDate,
                                       ProviderName = x.OperatorName,
                                       OperatorRef = x.OperatorRef,
                                       Status = x.Status,
                                       Amount = x.Amount,
                                       TranDate = x.TransactionDate,
                                       AgentNo = x.Agentno,
                                       APIRef = x.APIRef,
                                       PancardType = x.PancardType,
                                       Payment = x.Payment,
                                       Reason = x.Reason,
                                       TransactionID = x.TransactionID,
                                       TransactionMode = x.TransactionMode,
                                       EditDate = x.EditDate,
                                       AmountInWords = ClsMethods.NumberToWords(Convert.ToDouble(x.Amount))
                                   }).FirstOrDefault();

                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = results;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.Datanotfound;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public APIReponseEntity ConvertToWords(double amount)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                if (amount > 0)
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = ClsMethods.NumberToWords(amount);
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Value can not be zero.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return _response;
        }

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
        private bool AgentCallBack(int userId, BBPSAPITransactionResponseEntity item, long transID)
        {
            string requestUrl = string.Empty, requestBody = string.Empty, callbackResponse = string.Empty; bool doStatus = false;
            try
            {
                var user = _unitOfWork.UserRepository.Get(x => x.ID == userId);
                if (user != null && !string.IsNullOrEmpty(user.PanVerificationURL) && user.PanVerificationURL.Contains("http"))
                {
                    requestUrl = user.PanVerificationURL;
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
    }
}
