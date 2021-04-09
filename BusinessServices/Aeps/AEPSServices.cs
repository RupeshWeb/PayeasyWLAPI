using AutoMapper;
using BusinessEntities;
using BusinessServices.Aeps;
using BusinessServices.Resource;
using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace BusinessServices
{
    public class AEPSServices : IAEPSServices
    {
        #region Private member variables.
        private readonly UnitOfWork _unitOfWork;
        #endregion

        #region Public constructor.
        /// <summary>
        /// Public constructor.
        /// </summary>
        public AEPSServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        public AEPSAPIReponseEntity CashWithdrawal(int userID, string agentID, string merchantID, string clientRefID, string mobileNo, string aadharNo, decimal amount, string bankName, string pidData, string latitude, string longitude)
        {
            return DoTransactions.CashWithdrawalTransaction(_unitOfWork, userID, merchantID, clientRefID, mobileNo, aadharNo, amount, bankName, pidData, latitude, longitude, agentID);
        }

        public AEPSAPIReponseEntity BalanceInquiry(int userID, string agentID, string merchantID, string clientRefID, string mobileNo, string aadharNo, string bankName, string pidData, string latitude, string longitude)
        {
            return DoTransactions.BalanceInquiryTransaction(_unitOfWork, userID, merchantID, clientRefID, mobileNo, aadharNo, bankName, pidData, latitude, longitude, agentID);
        }

        public AEPSAPIReponseEntity TransactionInquiry(int userID, string clientRefID)
        {
            AEPSAPIReponseEntity _response = new AEPSAPIReponseEntity();
            try
            {
                var transaction = ClsMethods.CustomerTransactionRefs(_unitOfWork, userID, clientRefID, 2);
                if (transaction != null)
                {
                    if (new[] { 10 }.Contains(transaction.ServiceID))
                    {
                        if (transaction.Status == clsVariables.RechargeStatus.Pending)
                        {
                            var statements = ClsMethods.CustomerStatementsRefs(_unitOfWork, userID, clientRefID);
                            int creditCount = statements.Where(x => x.CrDrType == clsVariables.PaymentType.Credit).Count();
                            int debitCount = statements.Where(x => x.CrDrType == clsVariables.PaymentType.Debit).Count();
                            if (creditCount == debitCount)
                            {
                                _response = DoTransactions.TransactionInquiry(_unitOfWork, transaction.ID.ToString());
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = Message.TxnAmountAlredycredit;
                            }
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
                        _response.Message = Message.Datanotfound;
                    }
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

        public AEPSMiniAPIReponseEntity Ministatement(int userID, string agentID, string merchantID, string clientRefID, string mobileNo, string aadharNo, string bankName, string pidData, string latitude, string longitude)
        {
            return DoTransactions.MiniStatements(_unitOfWork, userID, merchantID, clientRefID, mobileNo, aadharNo, bankName, pidData, latitude, longitude, agentID);
        }

        public APIReponseEntity BankName()
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _unitOfWork.MasterIfscRepository.GetMany(x => x.IIN != null && x.IIN != "").OrderBy(x => x.BankName).ToList();
                if (result.Any())
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    Mapper.CreateMap<MasterIFSCCode, MasterBankNameListEntity>();
                    _response.Data = Mapper.Map<List<MasterIFSCCode>, List<MasterBankNameListEntity>>(result);
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

        public List<TransactionReportEntity> TransactionReport(int userID, string agentID, string fromDate, string toDate, string mobileNo)
        {
            List<TransactionReportEntity> _response = new List<TransactionReportEntity>();
            try
            {
                DateTime fDate = Convert.ToDateTime(fromDate), tDate = Convert.ToDateTime(toDate).AddDays(1);
                if (!string.IsNullOrEmpty(mobileNo))
                {
                    var result = _unitOfWork.SqlQuery<AEPSWLTransactions_Result>("AEPSWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
                        , new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }
                        , new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }
                        , new SqlParameter("FromDate", System.Data.SqlDbType.DateTime) { Value = fDate }
                        , new SqlParameter("ToDate", System.Data.SqlDbType.DateTime) { Value = tDate }
                        , new SqlParameter("Number", System.Data.SqlDbType.VarChar) { Value = mobileNo }
                        , new SqlParameter("TxnID", System.Data.SqlDbType.BigInt) { Value = 0 }
                        , new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 2 }).ToList();

                    if (result.Any())
                    {
                        Mapper.CreateMap<AEPSWLTransactions_Result, TransactionReportEntity>();
                        _response = Mapper.Map<List<AEPSWLTransactions_Result>, List<TransactionReportEntity>>(result);
                    }
                }
                else
                {
                    var result = _unitOfWork.SqlQuery<AEPSWLTransactions_Result>("AEPSWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
                        , new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }
                        , new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }
                        , new SqlParameter("FromDate", System.Data.SqlDbType.DateTime) { Value = fDate }
                        , new SqlParameter("ToDate", System.Data.SqlDbType.DateTime) { Value = tDate }
                        , new SqlParameter("Number", System.Data.SqlDbType.VarChar) { Value = mobileNo }
                        , new SqlParameter("TxnID", System.Data.SqlDbType.BigInt) { Value = 0 }
                        , new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = 1 }).ToList();

                    if (result.Any())
                    {
                        Mapper.CreateMap<AEPSWLTransactions_Result, TransactionReportEntity>();
                        _response = Mapper.Map<List<AEPSWLTransactions_Result>, List<TransactionReportEntity>>(result);
                    }
                }
            }
            catch (Exception) { }
            return _response;
        }

        public APIReponseEntity LastTransaction(int userID, string agentID)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var result = _unitOfWork.SqlQuery<AEPSLastTransaction_Result>("AEPSLastTransaction @UserID,@AgentID", new SqlParameter("UserID", System.Data.SqlDbType.Int) { Value = userID }, new SqlParameter("AgentID", System.Data.SqlDbType.VarChar) { Value = agentID }).ToList();
                if (result.Any())
                {
                    Mapper.CreateMap<AEPSLastTransaction_Result, LastTransactionReportEntity>();
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = Mapper.Map<List<AEPSLastTransaction_Result>, List<LastTransactionReportEntity>>(result);
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

        public ReceiptReponseEntity Receipt(int userID, string agentID, string refNumber)
        {
            ReceiptReponseEntity _response = new ReceiptReponseEntity();
            try
            {
                long txnID = Convert.ToInt64(refNumber);
                var result = _unitOfWork.SqlQuery<AEPSWLTransactions_Result>("AEPSWLTransactions @UserID,@AgentID,@FromDate,@ToDate,@Number,@TxnID,@Flag"
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
                                   select new ReceiptDetailsEntity
                                   {
                                       Number = x.Number,
                                       TranRef = x.TransRef,
                                       TxnDate = x.TransactionDate,
                                       ProviderName = x.OperatorName,
                                       RRN = x.RRN,
                                       AadharNo = x.AadharNo,
                                       BankName = x.BankName,
                                       Status = x.Status,
                                       Amount = x.Amount,
                                       TranDate = x.TransactionDate,
                                       AgentNo = x.AgentNo,
                                       AVBalance = x.AVBalance,
                                       Merchant = x.Merchant,
                                       Reason = x.Reason,
                                       TransMode = x.TransMode,
                                       NIBN = x.NIBN,
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

        public APIReponseEntity CustomerBalanceRequest(int userID, string agentID)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                if (!string.IsNullOrEmpty(agentID))
                {
                    _response = DoTransactions.BalanceRequest(_unitOfWork, userID, agentID);
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

        public APIReponseEntity CustomerCommRequest(int userID, string agentID, string merchantID, string mobileNo, string aadharNo, decimal amount, string bankName)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                if (userID > 0 && !string.IsNullOrEmpty(agentID) && !string.IsNullOrEmpty(merchantID) && amount > 0 && amount <= 20000)
                {
                    Regex regx = new Regex("^[0-9]{10}");
                    if (regx.Matches(mobileNo).Count > 0)
                    {
                        Regex regxaadhar = new Regex("^[0-9]{12}");
                        if (regxaadhar.Matches(aadharNo).Count > 0)
                        {
                            Regex regxbankcode = new Regex("^[0-9]{1,9}");
                            if (regxbankcode.Matches(bankName).Count > 0)
                            {
                                _response = DoTransactions.CommRequest(_unitOfWork, userID, amount);
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = "Invalid bankCode.";
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Aadhar no should be 12 digits.";
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Mobile no should be 10 digits.";
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "All field's are required.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public APIReponseEntity CustomerCommV2Request(int userID, string agentID, string merchantID, string mobileNo, string aadharNo, decimal amount, string bankName, string mode)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                if (userID > 0 && !string.IsNullOrEmpty(agentID) && !string.IsNullOrEmpty(merchantID))
                {
                    if (mode == "51")
                    {
                        if (amount < 1 || amount > 10000)
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "The amount should be between 1 to 10000.";
                            return _response;
                        }
                    }
                    else if (!new[] { "51", "52", "53" }.Contains(mode))
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Invalid request format.";
                        return _response;
                    }
                    Regex regx = new Regex("^[0-9]{10}");
                    if (regx.Matches(mobileNo).Count > 0)
                    {
                        Regex regxaadhar = new Regex("^[0-9]{12}");
                        if (regxaadhar.Matches(aadharNo).Count > 0)
                        {
                            Regex regxbankcode = new Regex("^[0-9]{1,9}");
                            if (regxbankcode.Matches(bankName).Count > 0)
                            {
                                _response = DoTransactions.CommRequest(_unitOfWork, userID, amount);
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = "Invalid bankCode.";
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = "Aadhar no should be 12 digits.";
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Mobile no should be 10 digits.";
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "All field's are required.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = ex.Message;
            }
            return _response;
        }

    }
}
