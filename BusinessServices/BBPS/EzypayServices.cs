using BusinessEntities;
using System.Configuration;

namespace BusinessServices.BillPayment
{
    public class EzypayServices
    {
        public string Url = ConfigurationManager.AppSettings["EzyPay_URL"];

        private string Geo_Code = "23.6762,86.9914";
        private string Pincode = "411041";
        private string AadharNumber = "411041";
        private string PanNumber = "411041";
        private string AgentID = "411041";
        private string UniqueNumber = "411041";

        public BBPSAPIReponseEntity FetchBill(int userId,string token,string operatorID,string agentName,string number,string accountNumber,string authenticator)
        {
            BBPSAPIReponseEntity _response = new BBPSAPIReponseEntity();
            string requestID = ClsMethods.GenereateUniqueNumber(9);
            string apiRequest = "msg=B06003~" + token + "~M" + requestID + "~" + operatorID + "~" + AgentID + "~" + UniqueNumber + "~" + agentName + "~" + PanNumber + "~" + AadharNumber + "~" + Pincode + "~" + Geo_Code + "~" + number + "~" + accountNumber + "~" + authenticator + "~1234567892~NA~NA~NA~NA";
            _response.RequestUrl = Url;
            _response.ApiRequest = apiRequest;
            _response.ApiResponse = ClsMethods.POSTAPIConsume(Url, apiRequest);
            ClsMethods.RequestLogs(userId, number, clsVariables.ServiceType.BBPS, Url, apiRequest, _response.ApiResponse, 0, "", "");
            return _response;
        }

        public BBPSAPIReponseEntity SendPaymentRequest(string token,string requestID, string operatorID, string agentName, string number, string accountNumber, string authenticator,string customerNumber,decimal amount,string dueDate)
        {
            BBPSAPIReponseEntity _response = new BBPSAPIReponseEntity();
            string apiRequest = "msg=B06005~" + token + "~" + requestID + "~" + operatorID + "~" + AgentID + "~" + UniqueNumber + "~" + agentName + "~" + PanNumber + "~" + AadharNumber + "~" + Pincode + "~" + Geo_Code + "~" + number + "~" + accountNumber + "~" + authenticator + "~" + customerNumber + "~" + amount + "~" + dueDate + "~NA~NA~NA~NA";
            _response.RequestUrl = Url;
            _response.ApiRequest = apiRequest;
            _response.ApiResponse = ClsMethods.POSTAPIConsume(Url, apiRequest);
            return _response;
        }
    }
}
