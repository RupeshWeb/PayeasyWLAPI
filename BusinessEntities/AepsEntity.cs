using System.Collections.Generic;

namespace BusinessEntities
{
    public class APIReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
    }

    public class AEPSAPIReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string RefNumber { get; set; }
        public dynamic Data { get; set; }
    }

    public class FNOAPIResponseEntity
    {
        public int StatusCode { get; set; }
        public string ApiRequest { get; set; }
        public string ApiResponse { get; set; }
        public string ApiUrl { get; set; }
        public string HeaderEncryption { get; set; }
        public string BodyEncryption { get; set; }
    }

    public class FNOEncryptionKeyResponseEntity
    {
        public int ResponseCode { get; set; }
        public string MessageString { get; set; }
        public object ClientInitiatorId { get; set; }
        public string RequestID { get; set; }
        public object ClientRefID { get; set; }
        public string ResponseData { get; set; }
    }

    public class FNOEncryptionKeyResponseDataEntity
    {
        public string EncrytionKey { get; set; }
    }

    public class FNOCashWithdrawalRequestEntity
    {
        public string MerchantID { get; set; }
        public string Version { get; set; }
        public string ServiceID { get; set; }
        public string ClientRefID { get; set; }
        public string MobileNo { get; set; }
        public string AadharNo { get; set; }
        public int TotalAmount { get; set; }
        public string BankName { get; set; }
        public string PidData { get; set; }
        public string RC { get; set; }
        public string NBIN { get; set; }
        public string TerminalId { get; set; }
        public string IPAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string IMEI_MAC { get; set; }
        public string DeviceNo { get; set; }
        public string CheckSum { get; set; }
    }

    public class FNOStatusCheckRequestEntity
    {
        public string SERVICEID { get; set; }
        public string Version { get; set; }
        public string ClientRefID { get; set; }
        public string CheckSum { get; set; }
    }

    public class FNOCashWithdrawalResponseDataDetailsEntity
    {
        public double Amount { get; set; }
        public string AdhaarNo { get; set; }
        public string TxnTime { get; set; }
        public string TxnDate { get; set; }
        public string BankName { get; set; }
        public string RRN { get; set; }
        public string Status { get; set; }
        public string CustomerMobile { get; set; }
        public string AvailableBalance { get; set; }
        public string LedgerBalance { get; set; }
    }

    public class FNOCashWithdrawalResponseDataEntity
    {
        public string UIRes { get; set; }
        public string ClientRes { get; set; }
    }

    public class FNOCashWithdrawalResponseEntity
    {
        public string ResponseCode { get; set; }
        public string MessageString { get; set; }
        public string ClientInitiatorId { get; set; }
        public string RequestID { get; set; }
        public string ClientRefID { get; set; }
        public string ResponseData { get; set; }
        //public FNOCashWithdrawalResponseDataEntity ResponseData { get; set; }
    }

    public class FNOBalanceCashWithdrawalEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string APIRef { get; set; }
        public FNOCashWithdrawalResponseDataDetailsEntity Data { get; set; }
        public FNOAPIResponseEntity ApiResponse { get; set; }
    }

    public class FNOTransactionStatusCheckResponseEntity
    {
        public string ResponseCode { get; set; }
        public string MessageString { get; set; }
        public string RequestID { get; set; }
        public string ClientRefID { get; set; }
        public string ResponseData { get; set; }
    }

    public class FNOTransactionStatusCheckResponseDataEntity
    {
        public string AdhaarNo { get; set; }
        public string Amount { get; set; }
        public string TransactionDateTime { get; set; }
        public string RRN { get; set; }
        public string TransactionStatus { get; set; }
    }

    public class FNOStatusInquiryResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string APIRef { get; set; }
        public FNOTransactionStatusCheckResponseDataEntity Data { get; set; }
        public FNOAPIResponseEntity ApiResponse { get; set; }
    }

    public class FNAPIClientResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ClientTxnID { get; set; }
        public long RefId { get; set; }
        public decimal Amount { get; set; }
        public string AdhaarNo { get; set; }
        public string TxnDate { get; set; }
        public string BankName { get; set; }
        public string RRN { get; set; }
        public string Status { get; set; }
        public string CustomerMobile { get; set; }
        public string AvailableBalance { get; set; }
        public string LedgerBalance { get; set; }
        public string Mode { get; set; }
    }

    public class MasterBankNameListEntity
    {
        public long Id { get; set; }
        public string BankName { get; set; }
        public string IIN { get; set; }
        public string ImageUrl { get; set; }
    }

    public class UserValidateResponseEntity
    {
        public int UserID { get; set; }
        public string MerchantID { get; set; }
        public string AgentID { get; set; }
    }

    public class TransactionReportEntity
    {
        public long TransRef { get; set; }
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public string OperatorName { get; set; }
        public string RRN { get; set; }
        public string Status { get; set; }
        public string TransactionDate { get; set; }
        public string EditDate { get; set; }
        public string TransactionMode { get; set; }
        public string BankName { get; set; }
        public string AadharNo { get; set; }
        public string Reason { get; set; }
        public string Merchant { get; set; }
        public string AgentNo { get; set; }
        public string TransMode { get; set; }
        public string AVBalance { get; set; }
        public string NIBN { get; set; }
    }

    public class FNWLAPIClientResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public FNWLAPITransactionResponseEntity Transactions { get; set; }
        public string AgentID { get; set; }
        public long RefId { get; set; }
    }

    public class FNWLAPITransactionResponseEntity
    {
        public decimal Amount { get; set; }
        public string AdhaarNo { get; set; }
        public string TxnDate { get; set; }
        public string BankName { get; set; }
        public string RRN { get; set; }
        public string Status { get; set; }
        public string CustomerMobile { get; set; }
        public string AvailableBalance { get; set; }
        public string LedgerBalance { get; set; }
        public string Mode { get; set; }
    }

    public class LastTransactionReportEntity
    {
        public long TransRef { get; set; }
        public string RRN { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string BankName { get; set; }
        public string AadharNo { get; set; }
        public string Number { get; set; }
        public string TransactionDate { get; set; }
        public string Reason { get; set; }
        public string RefNumber { get; set; }
    }

    public class ReceiptDetailsEntity
    {
        public string Number { get; set; }
        public long TranRef { get; set; }
        public string TxnDate { get; set; }
        public string TranDate { get; set; }
        public string ProviderName { get; set; }
        public string RRN { get; set; }
        public string AadharNo { get; set; }
        public string BankName { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string AmountInWords { get; set; }
        public string Reason { get; set; }
        public string Merchant { get; set; }
        public string AgentNo { get; set; }
        public string TransMode { get; set; }
        public string AVBalance { get; set; }
        public string NIBN { get; set; }
    }

    public class ReceiptReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public ReceiptDetailsEntity Data { get; set; }
    }

    public class APIBalanceInquiryEntity
    {
        public string RequestID { get; set; }
        public string SessionID { get; set; }
        public string RetailerID { get; set; }
    }

    public class APIBalanceInquiryResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public APIBalanceInquiryTransactionResponseEntity Transactions { get; set; }
    }

    public class APIBalanceInquiryTransactionResponseEntity
    {
        public float Balance { get; set; }
        public float MainBalance { get; set; }
    }

    public class TransactionCommDetailsEntity
    {
        public decimal Amount { get; set; }
        public decimal Margin { get; set; }
        public decimal Surcharge { get; set; }
        public decimal GST { get; set; }
        public decimal TDS { get; set; }
    }

    public class FNOMiniStatementEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string APIRef { get; set; }
        public FNOCashMiniStatementResponseEntity Data { get; set; }
        public FNOAPIResponseEntity ApiResponse { get; set; }
    }

    public class FNOCashMiniStatementResponseEntity
    {
        public string AdhaarNo { get; set; }
        public IList<FNOCashMiniStatementTransactionResponseEntity> TransactionList { get; set; }
        public string NpciTranData { get; set; }
        public string IsFormated { get; set; }
        public string SystemTraceAudit { get; set; }
        public string localTime { get; set; }
        public string localDate { get; set; }
        public string RRN { get; set; }
        public string AuthIndentyResp { get; set; }
        public string txnCurCode { get; set; }
        public string UIDAuthCode { get; set; }
        public string TerminalIdenty { get; set; }
        public string CardAccceptorCode { get; set; }
        public string NameLocation { get; set; }
        public double Balance { get; set; }
    }

    public class FNOCashMiniStatementTransactionResponseEntity
    {
        public string Date { get; set; }
        public object ModeOfTxn { get; set; }
        public string Type { get; set; }
        public object RefNo { get; set; }
        public string DebitCredit { get; set; }
        public double Amount { get; set; }
    }

    public class StatementReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public FNOCashMiniStatementResponseEntity Data { get; set; }
    }

    public class AEPSMiniAPIReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string RefNumber { get; set; }
        public string BankName { get; set; }
        public string AgentID { get; set; }
        public string Merchant { get; set; }
        public string MobileNo { get; set; }
        public FNOCashMiniStatementResponseEntity Data { get; set; }
    }

    
}
