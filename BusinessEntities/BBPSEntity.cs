using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class APIBBPSSubCatReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
        public List<string> States { get; set; }
    }
    public class APIBBPSFetchReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string RefNumber { get; set; }
        public dynamic Data { get; set; }
    }

    public class APIBBPSReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string RefNumber { get; set; }
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
        public string OperatorRef { get; set; }
        public long TxnRef { get; set; }
        public string TxnDate { get; set; }
    }
    public class BBPSBillReponseDataEntity
    {
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
        public string OperatorRef { get; set; }
        public long TxnRef { get; set; }
        public DateTime TxnDate { get; set; }
    }

    public class BBPSTransactionReportEntity
    {
        public long TransRef { get; set; }
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public string OperatorName { get; set; }
        public string OperatorRef { get; set; }
        public string Status { get; set; }
        public string TransactionDate { get; set; }
        public string EditDate { get; set; }
        public string TransactionMode { get; set; }
        public string CustomerNo { get; set; }
        public string ConsumerName { get; set; }
        public string BillDate { get; set; }
        public string BillDueDate { get; set; }
        public string AccountNo { get; set; }
        public string Authenticator { get; set; }
        public string PaymentType { get; set; }
        public string BillNumber { get; set; }
        public string Agentno { get; set; }
        public string Merchant { get; set; }
        public string Reason { get; set; }
        public string TransactionID { get; set; }
    }

    public class BBPSReceiptDetailsEntity
    {
        public long TranRef { get; set; }
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public string ProviderName { get; set; }
        public string OperatorRef { get; set; }
        public string Status { get; set; }
        public string TxnDate { get; set; }
        public string TranDate { get; set; }
        public string EditDate { get; set; }
        public string TransMode { get; set; }
        public string CustomerNo { get; set; }
        public string ConsumerName { get; set; }
        public string BillDate { get; set; }
        public string BillDueDate { get; set; }
        public string AccountNo { get; set; }
        public string Authenticator { get; set; }
        public string PaymentType { get; set; }
        public string BillNumber { get; set; }
        public string AgentNo { get; set; }
        public string Merchant { get; set; }
        public string AmountInWords { get; set; }
    }
    public class BBPSReceiptReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public BBPSReceiptDetailsEntity Data { get; set; }
    }

    public class BBPSAPIReponseEntity
    {
        public string RequestUrl { get; set; }
        public string ApiRequest { get; set; }
        public string ApiResponse { get; set; }
    }

    public class BBPSFetchReponseDataEntity
    {
        public string BillerName { get; set; }
        public string BillDueDate { get; set; }
        public string BillDate { get; set; }
        public string Amount { get; set; }
        public string BillerNumber { get; set; }
        public string Partial { get; set; }
    }

    public class APIBalanceInquiryReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public decimal MainBalance { get; set; }
        public decimal AEPSBalance { get; set; }
    }

    public class BBPSAPITransactionResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public BBPSAPITransactionResponseDataEntity Transactions { get; set; }
        public string AgentID { get; set; }
        public string ClientRefId { get; set; }
    }
    public class BBPSAPITransactionResponseDataEntity
    {
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public string OperatorRef { get; set; }
        public string OrderID { get; set; }
        public string TxnDate { get; set; }
        public string Status { get; set; }
    }

    public class BBPSPaymentRequestEntity
    {
        public string BillNumber { get; set; }
        public string BillerCode { get; set; }
        public string BillAmount { get; set; }
        public string MobileNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Authenticator { get; set; }
        public string Payment { get; set; }
        public string DueDate { get; set; }
        public string BillDate { get; set; }
        public string ConsumerName { get; set; }
        public string BillerNumber { get; set; }
        public string RefNumber { get; set; }
    }

    public class APIPaymentInquiryEntity
    {
        public string RequestID { get; set; }
        public string SessionID { get; set; }
        public string RetailerID { get; set; }
        public string Number { get; set; }
        public string ServiceProviderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
    }

    public class APIPaymentInquiryResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public APIPaymentInquiryResponseDataEntity Transaction { get; set; }
    }
    public class APIPaymentInquiryResponseDataEntity
    {
        public string RequestID { get; set; }
        public string ClientTransactionID { get; set; }
        public string RetailerID { get; set; }
        public string Number { get; set; }
        public string ServiceProviderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
    }

    public class APIPaymentResponseDataEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string RequestID { get; set; }
        public string ClientTransactionID { get; set; }
        public string RetailerID { get; set; }
        public string Number { get; set; }
        public string ServiceProviderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
    }

    public class TransactionComplainRequestEntity
    {
        [Required(ErrorMessage = "Mobile No is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Contact no should be 10 digits.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "From date is required.")]
        public string FromDate { get; set; }

        [Required(ErrorMessage = "To date is required.")]
        public string ToDate { get; set; }

        [Required(ErrorMessage = "Complaint type is required.")]
        public string ComplaintType { get; set; }

        [Required(ErrorMessage = "Disposition is required.")]
        public string Disposition { get; set; }

        [Required(ErrorMessage = "Transaction Id is required.")]
        [RegularExpression("[0-9]{9}", ErrorMessage = "Transaction Id should be 9 digits.")]
        public string TransactionRef { get; set; }

        [Required(ErrorMessage = "Descriptions is required.")]
        public string Descriptions { get; set; }
    }

    public class ComplaintsRegisterEntity
    {
        public int ID { get; set; }
        public string AgentId { get; set; }
        public string MobileNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ComplaintType { get; set; }
        public string Disposition { get; set; }
        public string Description { get; set; }
        public long TransactionId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime EditDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }

    public class APIBBPSComplaintReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ConsumnerName { get; set; }
        public string TransactionId { get; set; }
        public string ComplaintId { get; set; }
        public string TxnDate { get; set; }
    }
}
