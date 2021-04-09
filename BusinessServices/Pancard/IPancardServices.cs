using BusinessEntities;
using System.Collections.Generic;

namespace BusinessServices
{
    public interface IPancardServices
    {
        PanRedirectResponseEntity PancardRedirect(int userID, string agentID, string mobileNo);
        UTIPancardResponseEntity PaymentGateway(string applicationNo, string userID, string UTITSLTransID, string transAmt, string transactionNo, string appName, string PANCardType, string payment);
        UTIPancardResponseEntity PaymentGatewayVerification(string encData);
        UserValidateResponseEntity ValidateUser(string refName);
        APIReponseEntity LastTransaction(int userID, string agentID);
        List<PANTransactionReportEntity> TransactionReport(int userID, string agentID, string fromDate, string toDate, string mobileNo);
        PANReceiptReponseEntity Receipt(int userID, string agentID, string refNumber);
        string EncryptionData(string item);
        string DecryptionData(string item);
    }
}
