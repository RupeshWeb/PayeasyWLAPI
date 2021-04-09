#region Using Namespaces...

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.Entity.Validation;
using DataModel.GenericRepository;
using System.Data.Entity.Infrastructure;

#endregion

namespace DataModel.UnitOfWork
{
    /// <summary>
    /// Unit of Work class responsible for DB transactions
    /// </summary>
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        #region Private member variables...
        private readonly Entities _context = null;
        private GenericRepository<User> _userRepository;
        private GenericRepository<UserSession> _tokenRepository;
        private GenericRepository<Operator> _operatorRepository;
        private GenericRepository<Balance> _balanceRepository;
        private GenericRepository<DmtBalance> _dmtBalanceRepository;
        private GenericRepository<TransactionQueue> _transactionRepository;
        private GenericRepository<Api> _apiRepository;
        private GenericRepository<Commission> _commissionRepository;
        private GenericRepository<LoginHistory> _loginHistoryRepository;
        private GenericRepository<RechargeTransaction> _rechargeTransactionsRepository;
        private GenericRepository<Service> _serviceRepository;
        private GenericRepository<ValidateParameter> _validateParameterRepository;
        private GenericRepository<Remitter> _remitterRepository;
        private GenericRepository<Recipient> _recipientRepository;
        private GenericRepository<MasterIFSCCode> _masterifscRepository;
        private GenericRepository<ChargeMoneyTran> _chargeMoneyTranRepository;
        private GenericRepository<Ticket> _ticketRepository;
        private GenericRepository<PaymentRequest> _paymentRequestRepository;
        private GenericRepository<SMSApi> _smsapiRepository;
        private GenericRepository<SmsLog> _smslogRepository;
        private GenericRepository<RequestLog> _requestlogRepository;
        private GenericRepository<ActivityLog> _activitylogRepository;
        private GenericRepository<ResponsesLog> _responselogRepository;
        private GenericRepository<Bank> _bankRepository;
        private GenericRepository<AllBankName> _allBankRepository;
        private GenericRepository<Remittervalidate> _remittervalidateRepository;
        private GenericRepository<UserBank> _userBankRepository;
        private GenericRepository<BankWithdrawal> _bankWithdrawalRepository;
        private GenericRepository<Brand> _brandRepository;
        private GenericRepository<ServiceAuthorization> _serviceAuthorizationRepository;
        private GenericRepository<UserToken> _userTokenRepository;
        private GenericRepository<AmountWiseSwitching> _amountWiseSwitchingRepository;
        private GenericRepository<RandomSwitch> _randomSwitchRepository;
        private GenericRepository<Setting> _settingRepository;
        private GenericRepository<FailSwitching> _failSwitchingRepository;
        private GenericRepository<OperatorSwitching> _operatorSwitchingRepository;
        private GenericRepository<APICommission> _apiCommissionRepository;
        private GenericRepository<MasterValidate> _masterValidateRepository;
        private GenericRepository<ClientRequestLog> _clientRequestlogRepository;
        private GenericRepository<ErrorTransaction> _errorTransactionRepository;
        private GenericRepository<MPOSTransaction> _mPOSTransactionRepository;
        private GenericRepository<GCTransaction> _gcTransactionRepository;
        private GenericRepository<AEPSATMRequest> _aepsatmRequestRepository;
        private GenericRepository<MerchantActivation> _merchantActivationRepository;
        private GenericRepository<AEPSFNOSession> _aepsFNOSessionRepository;
        private GenericRepository<AEPSRequestLog> _aepsRequestLogRepository;
        private GenericRepository<SDKAuthRequestLog> _sdkAuthRequestLogRepository;
        private GenericRepository<PANTransactionRequest> _panTransactionRequestRepository;
        private GenericRepository<ComplaintRegister> _complaintRegisterRepository;
        #endregion

        public UnitOfWork()
        {
            _context = new Entities();
        }

        #region Public Repository Creation properties...

        /// <summary>
        /// Get/Set Property for Service repository.
        /// </summary>
        public GenericRepository<Service> ServiceRepository
        {
            get
            {
                if (this._serviceRepository == null)
                    this._serviceRepository = new GenericRepository<Service>(_context);
                return _serviceRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for Commission repository.
        /// </summary>
        public GenericRepository<Commission> CommissionRepository
        {
            get
            {
                if (this._commissionRepository == null)
                    this._commissionRepository = new GenericRepository<Commission>(_context);
                return _commissionRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for LoginHistory repository.
        /// </summary>
        public GenericRepository<LoginHistory> LoginHistoryRepository
        {
            get
            {
                if (this._loginHistoryRepository == null)
                    this._loginHistoryRepository = new GenericRepository<LoginHistory>(_context);
                return _loginHistoryRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for RechargeTransaction repository.
        /// </summary>
        public GenericRepository<RechargeTransaction> RechargeTransactionRepository
        {
            get
            {
                if (this._rechargeTransactionsRepository == null)
                    this._rechargeTransactionsRepository = new GenericRepository<RechargeTransaction>(_context);
                return _rechargeTransactionsRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for Api repository.
        /// </summary>
        public GenericRepository<Api> ApiRepository
        {
            get
            {
                if (this._apiRepository == null)
                    this._apiRepository = new GenericRepository<Api>(_context);
                return _apiRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for TransactionQueue repository.
        /// </summary>
        public GenericRepository<TransactionQueue> TransactionRepository
        {
            get
            {
                if (this._transactionRepository == null)
                    this._transactionRepository = new GenericRepository<TransactionQueue>(_context);
                return _transactionRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for operator repository.
        /// </summary>
        public GenericRepository<Operator> OperatorRepository
        {
            get
            {
                if (this._operatorRepository == null)
                    this._operatorRepository = new GenericRepository<Operator>(_context);
                return _operatorRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for Balance repository.
        /// </summary>
        public GenericRepository<Balance> BalanceRepository
        {
            get
            {
                if (this._balanceRepository == null)
                    this._balanceRepository = new GenericRepository<Balance>(_context);
                return _balanceRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for DmtBalance repository.
        /// </summary>
        public GenericRepository<DmtBalance> DmtBalanceRepository
        {
            get
            {
                if (this._dmtBalanceRepository == null)
                    this._dmtBalanceRepository = new GenericRepository<DmtBalance>(_context);
                return _dmtBalanceRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for user repository.
        /// </summary>
        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this._userRepository == null)
                    this._userRepository = new GenericRepository<User>(_context);
                return _userRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<UserSession> TokenRepository
        {
            get
            {
                if (this._tokenRepository == null)
                    this._tokenRepository = new GenericRepository<UserSession>(_context);
                return _tokenRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<Remitter> RemitterRepository
        {
            get
            {
                if (this._remitterRepository == null)
                    this._remitterRepository = new GenericRepository<Remitter>(_context);
                return _remitterRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<Recipient> RecipientRepository
        {
            get
            {
                if (this._recipientRepository == null)
                    this._recipientRepository = new GenericRepository<Recipient>(_context);
                return _recipientRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<MasterIFSCCode> MasterIfscRepository
        {
            get
            {
                if (this._masterifscRepository == null)
                    this._masterifscRepository = new GenericRepository<MasterIFSCCode>(_context);
                return _masterifscRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<ChargeMoneyTran> ChargeMoneyTranRepository
        {
            get
            {
                if (this._chargeMoneyTranRepository == null)
                    this._chargeMoneyTranRepository = new GenericRepository<ChargeMoneyTran>(_context);
                return _chargeMoneyTranRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<Ticket> TicketRepository
        {
            get
            {
                if (this._ticketRepository == null)
                    this._ticketRepository = new GenericRepository<Ticket>(_context);
                return _ticketRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<SMSApi> SmaApiRepository
        {
            get
            {
                if (this._smsapiRepository == null)
                    this._smsapiRepository = new GenericRepository<SMSApi>(_context);
                return _smsapiRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<SmsLog> SmsLogRepository
        {
            get
            {
                if (this._smslogRepository == null)
                    this._smslogRepository = new GenericRepository<SmsLog>(_context);
                return _smslogRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<RequestLog> RequestLogRepository
        {
            get
            {
                if (this._requestlogRepository == null)
                    this._requestlogRepository = new GenericRepository<RequestLog>(_context);
                return _requestlogRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<ActivityLog> ActivityRepository
        {
            get
            {
                if (this._activitylogRepository == null)
                    this._activitylogRepository = new GenericRepository<ActivityLog>(_context);
                return _activitylogRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<ResponsesLog> ResponseLogRepository
        {
            get
            {
                if (this._responselogRepository == null)
                    this._responselogRepository = new GenericRepository<ResponsesLog>(_context);
                return _responselogRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<ValidateParameter> ValidateParameterRepository
        {
            get
            {
                if (this._validateParameterRepository == null)
                    this._validateParameterRepository = new GenericRepository<ValidateParameter>(_context);
                return _validateParameterRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<PaymentRequest> PaymentRequestRepository
        {
            get
            {
                if (this._paymentRequestRepository == null)
                    this._paymentRequestRepository = new GenericRepository<PaymentRequest>(_context);
                return _paymentRequestRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<Bank> BankRepository
        {
            get
            {
                if (this._bankRepository == null)
                    this._bankRepository = new GenericRepository<Bank>(_context);
                return _bankRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<AllBankName> AllBankRepository
        {
            get
            {
                if (this._allBankRepository == null)
                    this._allBankRepository = new GenericRepository<AllBankName>(_context);
                return _allBankRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<Remittervalidate> RemiterValidateRepository
        {
            get
            {
                if (this._remittervalidateRepository == null)
                    this._remittervalidateRepository = new GenericRepository<Remittervalidate>(_context);
                return _remittervalidateRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<UserBank> UserBankRepository
        {
            get
            {
                if (this._userBankRepository == null)
                    this._userBankRepository = new GenericRepository<UserBank>(_context);
                return _userBankRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<BankWithdrawal> BankWithdrawalRepository
        {
            get
            {
                if (this._bankWithdrawalRepository == null)
                    this._bankWithdrawalRepository = new GenericRepository<BankWithdrawal>(_context);
                return _bankWithdrawalRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<Brand> BrandRepository
        {
            get
            {
                if (this._brandRepository == null)
                    this._brandRepository = new GenericRepository<Brand>(_context);
                return _brandRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<ServiceAuthorization> ServiceAuthorizationRepository
        {
            get
            {
                if (this._serviceAuthorizationRepository == null)
                    this._serviceAuthorizationRepository = new GenericRepository<ServiceAuthorization>(_context);
                return _serviceAuthorizationRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<UserToken> UserTokenRepository
        {
            get
            {
                if (this._userTokenRepository == null)
                    this._userTokenRepository = new GenericRepository<UserToken>(_context);
                return _userTokenRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<AmountWiseSwitching> AmountWiseSwitchingRepository
        {
            get
            {
                if (this._amountWiseSwitchingRepository == null)
                    this._amountWiseSwitchingRepository = new GenericRepository<AmountWiseSwitching>(_context);
                return _amountWiseSwitchingRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<RandomSwitch> RandomSwitchRepository
        {
            get
            {
                if (this._randomSwitchRepository == null)
                    this._randomSwitchRepository = new GenericRepository<RandomSwitch>(_context);
                return _randomSwitchRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<Setting> SettingRepository
        {
            get
            {
                if (this._settingRepository == null)
                    this._settingRepository = new GenericRepository<Setting>(_context);
                return _settingRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<FailSwitching> FailSwitchingRepository
        {
            get
            {
                if (this._failSwitchingRepository == null)
                    this._failSwitchingRepository = new GenericRepository<FailSwitching>(_context);
                return _failSwitchingRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<OperatorSwitching> OperatorSwitchingRepository
        {
            get
            {
                if (this._operatorSwitchingRepository == null)
                    this._operatorSwitchingRepository = new GenericRepository<OperatorSwitching>(_context);
                return _operatorSwitchingRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<APICommission> ApiCommissionRepository
        {
            get
            {
                if (this._apiCommissionRepository == null)
                    this._apiCommissionRepository = new GenericRepository<APICommission>(_context);
                return _apiCommissionRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for bank with repository.
        /// </summary>
        public GenericRepository<MasterValidate> MasterValidateRepository
        {
            get
            {
                if (this._masterValidateRepository == null)
                    this._masterValidateRepository = new GenericRepository<MasterValidate>(_context);
                return _masterValidateRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<ClientRequestLog> ClientRequestLogRepository
        {
            get
            {
                if (this._clientRequestlogRepository == null)
                    this._clientRequestlogRepository = new GenericRepository<ClientRequestLog>(_context);
                return _clientRequestlogRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<ErrorTransaction> ErrorTransactionRepository
        {
            get
            {
                if (this._errorTransactionRepository == null)
                    this._errorTransactionRepository = new GenericRepository<ErrorTransaction>(_context);
                return _errorTransactionRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<MPOSTransaction> MPOSTransactionRepository
        {
            get
            {
                if (this._mPOSTransactionRepository == null)
                    this._mPOSTransactionRepository = new GenericRepository<MPOSTransaction>(_context);
                return _mPOSTransactionRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<GCTransaction> GCTransactionRepository
        {
            get
            {
                if (this._gcTransactionRepository == null)
                    this._gcTransactionRepository = new GenericRepository<GCTransaction>(_context);
                return _gcTransactionRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<AEPSATMRequest> AEPSATMRequestRepository
        {
            get
            {
                if (this._aepsatmRequestRepository == null)
                    this._aepsatmRequestRepository = new GenericRepository<AEPSATMRequest>(_context);
                return _aepsatmRequestRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<MerchantActivation> MerchantActivationRepository
        {
            get
            {
                if (this._merchantActivationRepository == null)
                    this._merchantActivationRepository = new GenericRepository<MerchantActivation>(_context);
                return _merchantActivationRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<AEPSFNOSession> AEPSFNOSessionRepository
        {
            get
            {
                if (this._aepsFNOSessionRepository == null)
                    this._aepsFNOSessionRepository = new GenericRepository<AEPSFNOSession>(_context);
                return _aepsFNOSessionRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<AEPSRequestLog> AEPSRequestLogRepository
        {
            get
            {
                if (this._aepsRequestLogRepository == null)
                    this._aepsRequestLogRepository = new GenericRepository<AEPSRequestLog>(_context);
                return _aepsRequestLogRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<SDKAuthRequestLog> SDKAuthRequestLogRepository
        {
            get
            {
                if (this._sdkAuthRequestLogRepository == null)
                    this._sdkAuthRequestLogRepository = new GenericRepository<SDKAuthRequestLog>(_context);
                return _sdkAuthRequestLogRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<PANTransactionRequest> PANTransactionRequestRepository
        {
            get
            {
                if (this._panTransactionRequestRepository == null)
                    this._panTransactionRequestRepository = new GenericRepository<PANTransactionRequest>(_context);
                return _panTransactionRequestRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<ComplaintRegister> ComplaintRegisterRepository
        {
            get
            {
                if (this._complaintRegisterRepository == null)
                    this._complaintRegisterRepository = new GenericRepository<ComplaintRegister>(_context);
                return _complaintRegisterRepository;
            }
        }

        public virtual DbRawSqlQuery<T> SqlQuery<T>(string query, params object[] parameters)
        {
            return _context.Database.SqlQuery<T>(query, parameters);
        }

        #endregion

        #region Public member methods...
        /// <summary>
        /// Save method.
        /// </summary>
        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {

                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:", DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
                System.IO.File.AppendAllLines(@"C:\errors.txt", outputLines);

                throw e;
            }

        }

        #endregion

        #region Implementing IDiosposable...

        #region private dispose variable declaration...
        private bool disposed = false;
        #endregion

        /// <summary>
        /// Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("UnitOfWork is being disposed");
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}