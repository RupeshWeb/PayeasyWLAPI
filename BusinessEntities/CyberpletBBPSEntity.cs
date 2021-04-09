using System;

namespace BusinessEntities
{
    public class ApiRequestResponseEntity
    {
        public string ApiUrl { get; set; }
        public string ApiRequest { get; set; }
        public string ApiResponse { get; set; }
    }
    public class MartBBPSReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public MartBBPSBillFetchReponseEntity Data { get; set; }
    }
    public class MartBBPSBillFetchReponseEntity
    {
        public string BillerName { get; set; }
        public string BillDueDate { get; set; }
        public string BillDate { get; set; }
        public string Amount { get; set; }
        public string BillerNumber { get; set; }
        public string Partial { get; set; }
    }
    public class MArtBBPSBillReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public APIRechargeDataEntity Data { get; set; }
    }
    public class APIRechargeDataEntity
    {
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
        public string OperatorRef { get; set; }
        public long TxnRef { get; set; }
        public DateTime TxnDate { get; set; }
    }
}
