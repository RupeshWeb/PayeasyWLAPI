using System.Collections.Generic;

namespace BusinessEntities
{
    public class CCXmlEntity
    {
    }

    public class CCErrorInfoEntity
    {
        public CCErrorEntity error { get; set; }
    }

    public class CCErrorEntity
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }

    public class CCCoverageResponseEntity
    {
        public CCXmlEntity xml { get; set; }
        public CCCoverageBillerInfoResponseEntity billerInfoResponse { get; set; }
    }

    public class CCCoverageBillerInfoResponseEntity
    {
        public string responseCode { get; set; }
        public List<CCCoverageBillerEntity> biller { get; set; }
        public CCErrorInfoEntity errorInfo { get; set; }
    }

    public class CCCoverageBillerEntity
    {
        public string billerId { get; set; }
        public string billerName { get; set; }
        public string billerCategory { get; set; }
        public string billerAdhoc { get; set; }
        public string billerCoverage { get; set; }
        public string billerFetchRequiremet { get; set; }
        public string billerPaymentExactness { get; set; }
        public string billerSupportBillValidation { get; set; }
        public string supportPendingStatus { get; set; }
        public string supportDeemed { get; set; }
        public string billerTimeout { get; set; }
        public CCBillerInputParamsEntity billerInputParams { get; set; }
        public string billerAmountOptions { get; set; }
        public string billerPaymentModes { get; set; }
        public string billerDescription { get; set; }
        public string rechargeAmountInValidationRequest { get; set; }
        public CCBillerPaymentChannelsEntity billerPaymentChannels { get; set; }
    }

    public class CCBillerInputParamsEntity
    {
        public object paramInfo { get; set; }
    }

    public class CCBillerPaymentChannelsEntity
    {
        public List<CCPaymentChannelInfoEntity> paymentChannelInfo { get; set; }
    }

    public class CCPaymentChannelInfoEntity
    {
        public string paymentChannelName { get; set; }
        public string minAmount { get; set; }
        public string maxAmount { get; set; }
    }

    public class CCBillFetchResponseEntity
    {
        public CCXmlEntity xml { get; set; }
        public CCBillFetchsResponseEntity billFetchResponse { get; set; }
    }

    public class CCBillFetchsResponseEntity
    {
        public string responseCode { get; set; }
        public CCInputParamsEntity inputParams { get; set; }
        public CCBillerResponseEntity billerResponse { get; set; }
        public CCAdditionalInfoEntity additionalInfo { get; set; }
        public CCErrorInfoEntity errorInfo { get; set; }
    }

    public class CCInputParamsEntity
    {
        public List<CCInputEntity> input { get; set; }
    }

    public class CCInputEntity
    {
        public string paramName { get; set; }
        public string paramValue { get; set; }
    }

    public class CCOptionEntity
    {
        public string amountName { get; set; }
        public string amountValue { get; set; }
    }

    public class CCAmountOptionsEntity
    {
        public List<CCOptionEntity> option { get; set; }
    }

    public class CCBillerResponseEntity
    {
        public CCAmountOptionsEntity amountOptions { get; set; }
        public string billAmount { get; set; }
        public string billDate { get; set; }
        public string billNumber { get; set; }
        public string billPeriod { get; set; }
        public string customerName { get; set; }
        public string dueDate { get; set; }
    }

    public class CCInfoEntity
    {
        public string infoName { get; set; }
        public string infoValue { get; set; }
    }

    public class CCAdditionalInfoEntity
    {
        public List<CCInfoEntity> info { get; set; }
    }
}
