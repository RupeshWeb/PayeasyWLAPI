using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
   public class PanRedirectResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string URL { get; set; }
    }

    public class UTIPancardResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string TxnID { get; set; }
        public string UTITxnID { get; set; }
        public string ApplicationNo { get; set; }
        public string Amount { get; set; }
    }

    public class PANTransactionReportEntity
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
        public string PancardType { get; set; }
        public string Payment { get; set; }
        public string Agentno { get; set; }
        public string APIRef { get; set; }
        public string TransactionID { get; set; }
        public string Reason { get; set; }
    }

    public class PANReceiptDetailsEntity
    {
        public long TranRef { get; set; }
        public string TxnDate { get; set; }
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public string ProviderName { get; set; }
        public string OperatorRef { get; set; }
        public string Status { get; set; }
        public string TranDate { get; set; }
        public string EditDate { get; set; }
        public string TransactionMode { get; set; }
        public string PancardType { get; set; }
        public string Payment { get; set; }
        public string AgentNo { get; set; }
        public string APIRef { get; set; }
        public string TransactionID { get; set; }
        public string Reason { get; set; }
        public string AmountInWords { get; set; }
    }
    public class PANReceiptReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public PANReceiptDetailsEntity Data { get; set; }
    }

    public class UtiPaymentDeductionEntity
    {
        public string transID { get; set; }
        public string UTITSLTransID { get; set; }
        public string transStatus { get; set; }
        public string applicationNo { get; set; }
        public string transAmt { get; set; }
    }
}
