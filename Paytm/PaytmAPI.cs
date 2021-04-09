using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace PaytmAPI
{
    public class PaytmAPIServices
    {
        public string Url = ConfigurationManager.AppSettings["Paytm_URL"];
        public string Paytm_MID = ConfigurationManager.AppSettings["Paytm_MID"];
        public string Paytm_KEY = ConfigurationManager.AppSettings["Paytm_KEY"];
        public string Paytm_Web_Name = ConfigurationManager.AppSettings["Paytm_Web_Name"];

        /// <summary>
        /// Do Transaction
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="IFSC"></param>
        /// <param name="accountType"></param>
        /// <param name="mobile"></param>
        /// <param name="accountHolderName"></param>
        /// <param name="amount"></param>
        /// <param name="orderID"></param>
        /// <returns>TransactionResponse</returns>
        public PayTransactionResponse DoTransaction(string accountNumber, string IFSC, string accountType, string mobile, string accountHolderName, string amount, string orderID)
        {
            PayTransactionResponse tempApiResponse = new PayTransactionResponse();
            JavaScriptSerializer jSerializer = new JavaScriptSerializer();
            tempApiResponse.apiRequest = SendPaytmRequest(accountNumber, IFSC, accountType, mobile, accountHolderName, amount, orderID);
            string response = GetPaytmRequestString(accountNumber, IFSC, accountType, mobile, accountHolderName, amount, orderID);
            tempApiResponse.apiResponse = response;
            TransactionResponse transactionResponse = jSerializer.Deserialize<TransactionResponse>(response);
            tempApiResponse.transactionResponse = transactionResponse;
            return tempApiResponse;
        }

        /// <summary>
        /// Get API Balance
        /// </summary>
        /// <returns></returns>
        public string GetAPIBalance()
        {
            Dictionary<string, string> innerrequest = new Dictionary<string, string>();
            innerrequest.Add("mid", Paytm_MID);
            innerrequest.Add("channel", "WEB");

            try
            {
                string Check = paytm.CheckSum.generateCheckSum(Paytm_KEY, innerrequest);
                string correct_check = Check; //Server.UrlEncode(Check);

                String First_jason = new JavaScriptSerializer().Serialize(innerrequest);
                First_jason = First_jason.Replace("\\", "").Replace(":\"{", ":{").Replace("}\",", "},");

                String url = "https://secure.paytm.in/oltp/HANDLER_INTERNAL/poolAccountBalance?JsonData=" + First_jason;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter2.Write(First_jason);
                }
                //  This actually does the request and gets the response back;

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    var obj1 = JObject.Parse(responseData);

                    responseData = paytm.security.Crypto.Decrypt(obj1["response"].ToString(), Paytm_KEY);

                    var obj2 = JObject.Parse(responseData);

                    return obj2["effectiveBalance"].ToString();
                }
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        /// <summary>
        /// Check Transaction Status
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public CheckStatusResponse CheckTransactionStatus(string orderId)
        {
            CheckStatusResponse transactionResponse = new CheckStatusResponse();

            Dictionary<string, string> innerrequest = new Dictionary<string, string>();
            innerrequest.Add("MID", Paytm_MID);
            innerrequest.Add("ORDERID", orderId);

            try
            {
                string Check = paytm.CheckSum.generateCheckSum(Paytm_KEY, innerrequest);
                string correct_check = Check; //Server.UrlEncode(Check);

                String First_jason = new JavaScriptSerializer().Serialize(innerrequest);
                First_jason = First_jason.Replace("\\", "").Replace(":\"{", ":{").Replace("}\",", "},");

                String url = "https://secure.paytm.in/oltp/HANDLER_INTERNAL/TXNSTATUS?JsonData=" + First_jason;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter2.Write(First_jason);
                }

                //  This actually does the request and gets the response back;

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    transactionResponse = js.Deserialize<CheckStatusResponse>(responseData);
                    return transactionResponse;
                }
            }
            catch (Exception ex)
            {
                return transactionResponse;
            }
        }

        /// <summary>
        /// Send Paytm Request
        /// </summary>
        /// <param name="BANK_ACC_NO"></param>
        /// <param name="IFSC_CODE"></param>
        /// <param name="ACC_TYPE"></param>
        /// <param name="MOBILE_NO"></param>
        /// <param name="SENDER_NAME"></param>
        /// <param name="AMOUNT"></param>
        /// <param name="ORDER_ID"></param>
        /// <returns></returns>
        private string SendPaytmRequest(string BANK_ACC_NO, string IFSC_CODE, string ACC_TYPE, string MOBILE_NO, string SENDER_NAME, string AMOUNT, string ORDER_ID)
        {
            Dictionary<string, string> innerrequest = new Dictionary<string, string>();
            innerrequest.Add("BANK_ACC_NO", BANK_ACC_NO);
            innerrequest.Add("IFSC_CODE", IFSC_CODE);
            innerrequest.Add("ACC_TYPE", ACC_TYPE);
            innerrequest.Add("MOBILE_NO", MOBILE_NO);
            innerrequest.Add("SENDER_NAME", SENDER_NAME);
            innerrequest.Add("AMOUNT", AMOUNT);
            innerrequest.Add("CURRENCY", "INR");
            innerrequest.Add("MID", Paytm_MID);
            innerrequest.Add("ORDER_ID", ORDER_ID);
            innerrequest.Add("REQUEST_TYPE", "P2B_S2S");
            innerrequest.Add("REMARKS", "NA");

            try
            {
                string Check = paytm.CheckSum.generateCheckSum(Paytm_KEY, innerrequest);
                string correct_check = Check; //Server.UrlEncode(Check);
                innerrequest.Add("CHECKSUM", correct_check);
                String Second_jason = new JavaScriptSerializer().Serialize(innerrequest);
                Second_jason = Second_jason.Replace("\\", "").Replace(":\"{", ":{").Replace("}\",", "},");

                String url = "https://secure.paytm.in/oltp/P2B?JsonData=" + Second_jason;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (StreamWriter requestWriter2 = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter2.Write(Second_jason);
                }
                //  This actually does the request and gets the response back;

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    return responseData;
                }

            }
            catch (Exception ex)
            {
                return ("Error$" + ex.Message.ToString());
            }
        }

        private string GetPaytmRequestString(string BANK_ACC_NO, string IFSC_CODE, string ACC_TYPE, string MOBILE_NO, string SENDER_NAME, string AMOUNT, string ORDER_ID)
        {
            Dictionary<string, string> innerrequest = new Dictionary<string, string>();
            innerrequest.Add("BANK_ACC_NO", BANK_ACC_NO);
            innerrequest.Add("IFSC_CODE", IFSC_CODE);
            innerrequest.Add("ACC_TYPE", ACC_TYPE);
            innerrequest.Add("MOBILE_NO", MOBILE_NO);
            innerrequest.Add("SENDER_NAME", SENDER_NAME);
            innerrequest.Add("AMOUNT", AMOUNT);
            innerrequest.Add("CURRENCY", "INR");
            innerrequest.Add("MID", Paytm_MID);
            innerrequest.Add("ORDER_ID", ORDER_ID);
            innerrequest.Add("REQUEST_TYPE", "P2B_S2S");
            innerrequest.Add("REMARKS", "NA");

            try
            {
                string Check = paytm.CheckSum.generateCheckSum(Paytm_KEY, innerrequest);
                string correct_check = Check; //Server.UrlEncode(Check);
                innerrequest.Add("CHECKSUM", correct_check);
                String Second_jason = new JavaScriptSerializer().Serialize(innerrequest);
                Second_jason = Second_jason.Replace("\\", "").Replace(":\"{", ":{").Replace("}\",", "},");

                String url = "https://secure.paytm.in/oltp/P2B?JsonData=" + Second_jason;
                return url;
            }
            catch (Exception ex)
            {
                return ("Error$" + ex.Message.ToString());
            }
        }


    }

    public class PayTransactionResponse
    {
        public string apiRequest { get; set; }
        public string apiResponse { get; set; }
        public TransactionResponse transactionResponse { get; set; }
    }

    public class TransactionResponse
    {
        public string PG_TXN_ID { get; set; }
        public string BANK_TXN_ID { get; set; }
        public string BANK_ACC_NO { get; set; }
        public string MOBILE_NO { get; set; }
        public string AMOUNT { get; set; }
        public string CURRENCY { get; set; }
        public string MID { get; set; }
        public string ORDER_ID { get; set; }
        public string REQUEST_TYPE { get; set; }
        public string TXN_STATUS { get; set; }
        public string RESPCODE { get; set; }
        public string TXN_MSG { get; set; }
        public string BENEFICIARY_NAME { get; set; }
        public string CHECKSUM { get; set; }
    }

    public class CheckTransactionResponse
    {
        public string apiRequest { get; set; }
        public string apiResponse { get; set; }
        public CheckStatusResponse checkStatusResponse { get; set; }
    }

    public class CheckStatusResponse
    {
        public string TXNID { get; set; }
        public string BANKTXNID { get; set; }
        public string ORDERID { get; set; }
        public string TXNAMOUNT { get; set; }
        public string STATUS { get; set; }
        public string TXNTYPE { get; set; }
        public string GATEWAYNAME { get; set; }
        public string RESPCODE { get; set; }
        public string RESPMSG { get; set; }
        public string BANKNAME { get; set; }
        public string MID { get; set; }
        public string PAYMENTMODE { get; set; }
        public string REFUNDAMT { get; set; }
        public string TXNDATE { get; set; }
    }
}
