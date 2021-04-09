using BusinessEntities;
using System.Collections.Generic;

namespace BusinessServices
{
    public interface IAEPSServices
    {
        AEPSAPIReponseEntity CashWithdrawal(int userID, string agentID, string merchantID, string clientRefID, string mobileNo, string aadharNo, decimal amount, string bankName, string pidData, string latitude, string longitude);

        AEPSAPIReponseEntity BalanceInquiry(int userID, string agentID, string merchantID, string clientRefID, string mobileNo, string aadharNo, string bankName, string pidData, string latitude, string longitude);

        AEPSAPIReponseEntity TransactionInquiry(int userID, string clientRefID);

        AEPSMiniAPIReponseEntity Ministatement(int userID, string agentID, string merchantID, string clientRefID, string mobileNo, string aadharNo, string bankName, string pidData, string latitude, string longitude);

        APIReponseEntity BankName();

        UserValidateResponseEntity ValidateUser(string refName);

        List<TransactionReportEntity> TransactionReport(int userID, string agentID, string fromDate, string toDate, string mobileNo);

        APIReponseEntity LastTransaction(int userID, string agentID);

        ReceiptReponseEntity Receipt(int userID, string agentID, string refNumber);

        APIReponseEntity ConvertToWords(double amount);

        APIReponseEntity CustomerBalanceRequest(int userID, string agentID);

        APIReponseEntity CustomerCommRequest(int userID, string agentID, string merchantID, string mobileNo, string aadharNo, decimal amount, string bankName);

        APIReponseEntity CustomerCommV2Request(int userID, string agentID, string merchantID, string mobileNo, string aadharNo, decimal amount, string bankName, string mode);
    }
}
