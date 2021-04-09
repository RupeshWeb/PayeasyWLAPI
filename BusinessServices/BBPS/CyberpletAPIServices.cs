using BusinessEntities;
using BusinessServices.Resource;
using System;
using System.Configuration;
using System.Web.Script.Serialization;

namespace BusinessServices.BBPS
{
    public class CyberpletAPIServices
    {
        #region Private Variable
        private readonly string URL= ConfigurationManager.AppSettings["CyberAuthdomain"];
        #endregion

        /// <summary>
        /// Bill Fetch
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="token"></param>
        /// <param name="operatorCode"></param>
        /// <param name="orderID"></param>
        /// <param name="customerNo"></param>
        /// <param name="consumerNo"></param>
        /// <param name="accountNo"></param>
        /// <param name="authenticator"></param>
        /// <returns></returns>
        public APIBBPSFetchReponseEntity BillFetch(int userID, string token, string operatorCode, long orderID, string customerNo, string consumerNo, string accountNo, string authenticator)
        {
            APIBBPSFetchReponseEntity _response = new APIBBPSFetchReponseEntity();
            try
            {
                var apiResponse = SentValidation(userID, token, operatorCode, orderID, customerNo, consumerNo, accountNo, authenticator);
                var result = new JavaScriptSerializer().Deserialize<MartBBPSReponseEntity>(apiResponse.ApiResponse);
                if (result.StatusCode == clsVariables.APIStatus.Success)
                {
                    BBPSFetchReponseDataEntity _subResponse = new BBPSFetchReponseDataEntity()
                    {
                        Amount = result.Data.Amount,
                        BillDueDate = result.Data.BillDueDate,
                        BillerName = result.Data.BillerName,
                        BillerNumber = result.Data.BillerNumber,
                        BillDate = result.Data.BillDate,
                        Partial = result.Data.Partial
                    };

                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = result.Message;
                    _response.Data = _subResponse;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = result.Message;
                }
                return _response;
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = Message.Errorsm + ex.Message;
                return _response;
            }
        }

        /// <summary>
        /// Sub Bill Pay
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="token"></param>
        /// <param name="operatorCode"></param>
        /// <param name="orderID"></param>
        /// <param name="customerNo"></param>
        /// <param name="consumerNo"></param>
        /// <param name="accountNo"></param>
        /// <param name="authenticator"></param>
        /// <returns></returns>
        private ApiRequestResponseEntity SentValidation(int userID, string token, string operatorCode, long orderID, string customerNo, string consumerNo, string accountNo, string authenticator)
        {
            ApiRequestResponseEntity _response = new ApiRequestResponseEntity();
            _response.ApiRequest = "BillerFetchDetails?token=" + token + "&number=" + consumerNo + "&apiCode=" + operatorCode + "&accountNumber=" + accountNo + "&authenticator=" + authenticator;
            _response.ApiUrl = URL;
            _response.ApiResponse = ClsMethods.ApiRequest(_response.ApiUrl + _response.ApiRequest);
            ClsMethods.ResponseLogs(userID, consumerNo, clsVariables.ServiceType.BBPS, _response.ApiUrl, _response.ApiRequest, _response.ApiResponse, orderID, clsVariables.RechargeAPI.MoneyArt, "");
            return _response;
        }

        /// <summary>
        /// Bill Payment
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="token"></param>
        /// <param name="operatorCode"></param>
        /// <param name="orderID"></param>
        /// <param name="customerNo"></param>
        /// <param name="consumerNo"></param>
        /// <param name="accountNo"></param>
        /// <param name="authenticator"></param>
        /// <param name="amount"></param>
        /// <param name="dueDate"></param>
        /// <param name="txnID"></param>
        /// <returns></returns>
        private ApiRequestResponseEntity SendRequestBillPayment(int userID, string token, string operatorCode, long orderID, string customerNo, string consumerNo, string accountNo, string authenticator, decimal amount, string dueDate)
        {
            ApiRequestResponseEntity _response = new ApiRequestResponseEntity();
            _response.ApiRequest = "BillPayment?token=" + token + "&number=" + consumerNo + "&apiCode=" + operatorCode + "&amount=" + amount + "&accountNumber=" + accountNo + "&authenticator=" + authenticator + "&customerNumber=" + customerNo + "&dueDate=" + dueDate + "&transactionRef=" + orderID;
            _response.ApiUrl = URL;
            _response.ApiResponse = ClsMethods.ApiRequest(_response.ApiUrl + _response.ApiRequest);
            ClsMethods.ResponseLogs(userID, consumerNo, clsVariables.ServiceType.BBPS, _response.ApiUrl, _response.ApiRequest, _response.ApiResponse, orderID, clsVariables.RechargeAPI.Cyberplet, "");
            return _response;
        }

        /// <summary>
        /// Bill Payment
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="token"></param>
        /// <param name="operatorCode"></param>
        /// <param name="orderID"></param>
        /// <param name="customerNo"></param>
        /// <param name="consumerNo"></param>
        /// <param name="accountNo"></param>
        /// <param name="authenticator"></param>
        /// <param name="amount"></param>
        /// <param name="dueDate"></param>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        public MArtBBPSBillReponseEntity BillPayment(int userID, string token, string operatorCode, long orderID, string customerNo, string consumerNo, string accountNo, string authenticator, decimal amount, string dueDate)
        {
            MArtBBPSBillReponseEntity _response = new MArtBBPSBillReponseEntity();
            try
            {
                var apiResponse = SendRequestBillPayment(userID, token, operatorCode, orderID, customerNo, consumerNo, accountNo, authenticator, amount, dueDate);
                _response = new JavaScriptSerializer().Deserialize<MArtBBPSBillReponseEntity>(apiResponse.ApiResponse);
                return _response;
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Pending;
                _response.Message = Message.Errorsm;
                return _response;
            }
        }
    }
}
