using AutoMapper;
using BusinessEntities;
using BusinessServices.BBPS;
using BusinessServices.Resource;
using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BusinessServices
{
    public class BBPSServices : IBBPSServices
    {
        #region Private member variables.
        private readonly UnitOfWork _unitOfWork;
        private ClsNotify _notifyServices;
        #endregion

        #region Public constructor.
        /// <summary>
        /// Public constructor.
        /// </summary>
        public BBPSServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

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

        public APIReponseEntity BillerCategory()
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _unitOfWork.SqlQuery<BBPSCategory_Result>("BBPSCategory").ToList();
                if (result.Any())
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = result;
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
                _response.Message = Message.Errorsm + ex.Message;
            }
            return _response;
        }

        public APIBBPSSubCatReponseEntity BillerCategory(short categoryID)
        {
            APIBBPSSubCatReponseEntity _response = new APIBBPSSubCatReponseEntity();
            try
            {
                var result = _unitOfWork.SqlQuery<BBPSSUBCategory_Result>("BBPSSUBCategory @CategoryID,@Flag", new SqlParameter("CategoryID", System.Data.SqlDbType.SmallInt) { Value = categoryID }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 1 }).ToList();
                if (result.Any())
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = result;
                    _response.States = result.Where(x => x.StateName != null).GroupBy(x => x.StateName).OrderBy(x=>x.Key).Select(x => x.Key).ToList();
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
                _response.Message = Message.Errorsm + ex.Message;
            }
            return _response;
        }

        public APIReponseEntity BillerParameters(int billerID)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _unitOfWork.SqlQuery<BBPSSUBCategoryParameter_Result>("BBPSSUBCategoryParameter @BillerD,@Flag", new SqlParameter("BillerD", System.Data.SqlDbType.Int) { Value = billerID }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 1 }).ToList();
                if (result.Any())
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = result;
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
                _response.Message = Message.Errorsm + ex.Message;
            }
            return _response;
        }

        public APIReponseEntity BillerParameterGrouping(int billerID, int paramId)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _unitOfWork.SqlQuery<BBPSSUBCategoryParameterGrouping_Result>("BBPSSUBCategoryParameterGrouping @BillerID,@ParamID,@Flag", new SqlParameter("BillerID", System.Data.SqlDbType.Int) { Value = billerID }, new SqlParameter("ParamID", System.Data.SqlDbType.Int) { Value = paramId }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 1 }).ToList();
                if (result.Any())
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = result;
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
                _response.Message = Message.Errorsm + ex.Message;
            }
            return _response;
        }

        ////public APIReponseEntity BillFetch(int userID, string agentID, int billerID, string customerNo, string consumerNo, string param1, string param2, string param3, string param4, string param5)
        ////{
        ////    return DoTransactions.BalanceInquiryTransaction(_unitOfWork, userID, agentID, billerID, customerNo, consumerNo, param1, param2, param3, param4, param5);
        ////}

        public APIBBPSFetchReponseEntity BillFetch(int userID, string agentID, int billerID, string customerNo, string consumerNo, string param1, string param2, string param3, string param4, string param5)
        {
            return DoTransactions.BillerFetch(_unitOfWork, userID, agentID, billerID, customerNo, consumerNo, param1, param2, param3, param4, param5);
        }

        public APIBBPSReponseEntity BillPayment(int userID, string agentID, string merchantID, int billerID, decimal Amount, string billnumber, string custmobileno, string billeraccount, string billercycle, string payment, string duedate, string billdate, string consumername, string billnumbers, string apiRefNo)
        {
            return DoTransactions.BillPayment(_unitOfWork, userID, agentID, merchantID, billerID, Amount, billnumber, custmobileno, billeraccount, billercycle, payment, duedate, billdate, consumername, billnumbers, apiRefNo);
        }

        public APIReponseEntity TransactionStatus(int userID, string agentID, string transactionId)
        {
            return DoTransactions.DOTransactionsStatus(_unitOfWork, userID, agentID, transactionId);
        }

        public List<BBPSTransactionReportEntity> TransactionReport(int userID, string agentID, string fromDate, string toDate, string mobileNo, string transactionId)
        {
            List<BBPSTransactionReportEntity> _response = new List<BBPSTransactionReportEntity>();
            try
            {
                DateTime fDate = Convert.ToDateTime(fromDate), tDate = Convert.ToDateTime(toDate).AddDays(1);
                if (!string.IsNullOrEmpty(mobileNo) && string.IsNullOrEmpty(transactionId))
                {
                    var result = _unitOfWork.SqlQuery<BBPSSWLTransactions_Result>("BBPSSWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
                        , new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }
                        , new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }
                        , new SqlParameter("FromDate", System.Data.SqlDbType.DateTime) { Value = fDate }
                        , new SqlParameter("ToDate", System.Data.SqlDbType.DateTime) { Value = tDate }
                        , new SqlParameter("Number", System.Data.SqlDbType.VarChar) { Value = mobileNo }
                        , new SqlParameter("TxnID", System.Data.SqlDbType.BigInt) { Value = 0 }
                        , new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 2 }).ToList();

                    if (result.Any())
                    {
                        Mapper.CreateMap<BBPSSWLTransactions_Result, BBPSTransactionReportEntity>();
                        _response = Mapper.Map<List<BBPSSWLTransactions_Result>, List<BBPSTransactionReportEntity>>(result);
                    }
                }
                else if (string.IsNullOrEmpty(mobileNo) && !string.IsNullOrEmpty(transactionId))
                {
                    long txnID = Convert.ToInt64(transactionId);
                    var result = _unitOfWork.SqlQuery<BBPSSWLTransactions_Result>("BBPSSWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
                        , new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }
                        , new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }
                        , new SqlParameter("FromDate", System.Data.SqlDbType.DateTime) { Value = fDate }
                        , new SqlParameter("ToDate", System.Data.SqlDbType.DateTime) { Value = tDate }
                        , new SqlParameter("Number", System.Data.SqlDbType.VarChar) { Value = mobileNo }
                        , new SqlParameter("TxnID", System.Data.SqlDbType.BigInt) { Value = txnID }
                        , new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 3 }).ToList();

                    if (result.Any())
                    {
                        Mapper.CreateMap<BBPSSWLTransactions_Result, BBPSTransactionReportEntity>();
                        _response = Mapper.Map<List<BBPSSWLTransactions_Result>, List<BBPSTransactionReportEntity>>(result);
                    }
                }
                else
                {
                    var result = _unitOfWork.SqlQuery<BBPSSWLTransactions_Result>("BBPSSWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
                        , new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }
                        , new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }
                        , new SqlParameter("FromDate", System.Data.SqlDbType.DateTime) { Value = fDate }
                        , new SqlParameter("ToDate", System.Data.SqlDbType.DateTime) { Value = tDate }
                        , new SqlParameter("Number", System.Data.SqlDbType.VarChar) { Value = mobileNo }
                        , new SqlParameter("TxnID", System.Data.SqlDbType.BigInt) { Value = 0 }
                        , new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 1 }).ToList();

                    if (result.Any())
                    {
                        Mapper.CreateMap<BBPSSWLTransactions_Result, BBPSTransactionReportEntity>();
                        _response = Mapper.Map<List<BBPSSWLTransactions_Result>, List<BBPSTransactionReportEntity>>(result);
                    }
                }
            }
            catch (Exception) { }
            return _response;
        }

        public BBPSReceiptReponseEntity Receipt(int userID, string agentID, string refNumber)
        {
            BBPSReceiptReponseEntity _response = new BBPSReceiptReponseEntity();
            try
            {
                long txnID = Convert.ToInt64(refNumber);
                var result = _unitOfWork.SqlQuery<BBPSSWLTransactions_Result>("BBPSSWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
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
                                   select new BBPSReceiptDetailsEntity
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
                                       Merchant = x.Merchant,
                                       TransMode = x.TransactionMode,
                                       AccountNo = x.AccountNo,
                                       Authenticator = x.Authenticator,
                                       BillDate = x.BillDate,
                                       BillDueDate = x.BillDueDate,
                                       BillNumber = x.BillNumber,
                                       ConsumerName = x.ConsumerName,
                                       CustomerNo = x.CustomerNo,
                                       EditDate = x.EditDate,
                                       PaymentType = x.PaymentType,
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

        public APIBBPSComplaintReponseEntity RaiseComplaint(int userID, string agentID, TransactionComplainRequestEntity model)
        {
            APIBBPSComplaintReponseEntity _response = new APIBBPSComplaintReponseEntity();
            try
            {
                long transId = Convert.ToInt64(model.TransactionRef);
                var complaint = _unitOfWork.ComplaintRegisterRepository.Get(x => x.TransactionId == transId && x.Status == clsVariables.TicketStatus.Pending);
                if (complaint == null)
                {
                    var transaction = ClsMethods.CustomerPendingTransaction(_unitOfWork, model.TransactionRef);
                    if (transaction != null)
                    {
                        _response.ConsumnerName = transaction.EXTRA10;
                        _response.TransactionId = transaction.ID.ToString();
                        _response.TxnDate = DateTime.Now.ToShortDateString();
                        if (transaction.Status == clsVariables.RechargeStatus.Success)
                        {
                            if (transaction.EXTRA9 == agentID && transaction.UserID == userID)
                            {
                                _notifyServices = new ClsNotify(_unitOfWork);
                                ComplaintRegister _add = new ComplaintRegister()
                                {
                                    UserId = userID,
                                    AgentId = agentID,
                                    MobileNo = model.MobileNumber,
                                    FromDate = model.FromDate,
                                    ToDate = model.ToDate,
                                    ComplaintType = model.ComplaintType,
                                    Disposition = model.Disposition,
                                    Description = model.Descriptions,
                                    TransactionId = transId,
                                    AddDate = DateTime.Now,
                                    EditDate = DateTime.Now,
                                    Status = clsVariables.TicketStatus.Pending
                                };
                                _unitOfWork.ComplaintRegisterRepository.Insert(_add);
                                _unitOfWork.Save();

                                _response.StatusCode = clsVariables.APIStatus.Success;
                                _response.Message = "Raised complaint successfully.";
                                _response.ComplaintId = _add.ID.ToString();
                                _notifyServices.ComplaintRegistration(userID, model.MobileNumber, model.TransactionRef, _add.ID.ToString());
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
                            _response.Message = "Your transaction status not in success mode.";
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Invalid transaction id.";
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Complain already in pending mode.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm + ex.Message;
            }
            return _response;
        }

        public List<ComplaintsRegisterEntity> ComplaintReport(int userID, string agentID, string fromDate, string toDate, string transactionId, string mobileNo)
        {
            List<ComplaintsRegisterEntity> _response = new List<ComplaintsRegisterEntity>();
            try
            {
                DateTime fDate = Convert.ToDateTime(fromDate), tDate = Convert.ToDateTime(toDate).AddDays(1);
                if (!string.IsNullOrEmpty(transactionId) && string.IsNullOrEmpty(mobileNo))
                {
                    long transId = Convert.ToInt64(transactionId);
                    var result = _unitOfWork.ComplaintRegisterRepository.GetMany(x => x.UserId == userID && x.AgentId == agentID && x.AddDate >= fDate && x.AddDate <= tDate && x.TransactionId == transId).ToList();
                    if (result.Any())
                    {
                        Mapper.CreateMap<ComplaintRegister, ComplaintsRegisterEntity>();
                        _response = Mapper.Map<List<ComplaintRegister>, List<ComplaintsRegisterEntity>>(result);
                    }
                }
                else if (string.IsNullOrEmpty(transactionId) && !string.IsNullOrEmpty(mobileNo))
                {
                    var result = _unitOfWork.ComplaintRegisterRepository.GetMany(x => x.UserId == userID && x.AgentId == agentID && x.AddDate >= fDate && x.AddDate <= tDate && x.MobileNo == mobileNo).ToList();
                    if (result.Any())
                    {
                        Mapper.CreateMap<ComplaintRegister, ComplaintsRegisterEntity>();
                        _response = Mapper.Map<List<ComplaintRegister>, List<ComplaintsRegisterEntity>>(result);
                    }
                }
                else
                {
                    var result = _unitOfWork.ComplaintRegisterRepository.GetMany(x => x.UserId == userID && x.AgentId == agentID && x.AddDate >= fDate && x.AddDate <= tDate).ToList();

                    if (result.Any())
                    {
                        Mapper.CreateMap<ComplaintRegister, ComplaintsRegisterEntity>();
                        _response = Mapper.Map<List<ComplaintRegister>, List<ComplaintsRegisterEntity>>(result);
                    }
                }
            }
            catch (Exception) { }
            return _response;
        }
    }
}
