using BusinessEntities;
using System.Collections.Generic;

namespace BusinessServices
{
    public interface IBBPSServices
    {
        UserValidateResponseEntity ValidateUser(string refName);
        APIReponseEntity BillerCategory();
        APIBBPSSubCatReponseEntity BillerCategory(short categoryID);
        APIReponseEntity BillerParameters(int billerID);
        APIReponseEntity BillerParameterGrouping(int billerID, int paramId);
        APIBBPSFetchReponseEntity BillFetch(int userID, string agentID, int billerID, string customerNo, string consumerNo, string param1, string param2, string param3, string param4, string param5);
        APIBBPSReponseEntity BillPayment(int userID, string agentID, string merchantID, int billerID, decimal Amount, string billnumber, string custmobileno, string billeraccount, string billercycle, string payment, string duedate, string billdate, string consumername, string billnumbers, string apiRefNo);
        APIReponseEntity TransactionStatus(int userID, string agentID, string transactionId);
        List<BBPSTransactionReportEntity> TransactionReport(int userID, string agentID, string fromDate, string toDate, string mobileNo, string transactionId);
        BBPSReceiptReponseEntity Receipt(int userID, string agentID, string refNumber);
        //APIReponseEntity BillFetch(int userID, string agentID, int billerID, string customerNo, string consumerNo, string param1, string param2, string param3, string param4, string param5);
        APIBBPSComplaintReponseEntity RaiseComplaint(int userID, string agentID, TransactionComplainRequestEntity model);
        List<ComplaintsRegisterEntity> ComplaintReport(int userID, string agentID, string fromDate, string toDate, string transactionId, string mobileNo);
    }
}
