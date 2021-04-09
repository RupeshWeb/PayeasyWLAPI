namespace BusinessEntities
{
    public class PayBillAPIResponseEntity
    {
        public string ApiRequest { get; set; }
        public string ApiResponse { get; set; }
        public string ApiUrl { get; set; }
    }

    public class PTMBillLoginRequestEntity
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class PTMLoginResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
    public class PTMBillLoginResponseEntity
    {
        public string token { get; set; }
    }

    public class PTMAPIBBPSBillReponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string RefNumber { get; set; }
    }

    public class PTMAPIBalanceResponseEntity
    {
        public int code { get; set; }
        public string message { get; set; }
        public string balance { get; set; }
    }

    public class PTMBalanceResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Balance { get; set; }
    }
    public class PaytmBillPaymentResponseEntity
    {
        public string status { get; set; }
        public string message { get; set; }
        public string bbpsRefId { get; set; }
    }
}
