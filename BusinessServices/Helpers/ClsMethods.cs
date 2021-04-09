using DataModel;
using DataModel.UnitOfWork;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BusinessServices
{
    public class ClsMethods
    {
        #region Private Variable
        private static readonly string passPhrase = "ANDRGM";
        private static readonly string saltValue = "95RG61";
        private static readonly string initVector = "jsKidmshatyb4jdu";
        #endregion

        /// <summary>
        /// Return Unique Number
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenereateUniqueNumber(int length)
        {
            var chars = "0123456789";
            var stringChars = new char[length];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            string finalString = new String(stringChars);
            return finalString;
        }

        /// <summary>
        /// Http Get Request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string APIConsume(string url)
        {
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                StringBuilder sb = new StringBuilder();
                byte[] buf = new byte[8192];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                string tempString = null;
                int count = 0;
                do
                {
                    count = resStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                    {
                        tempString = Encoding.ASCII.GetString(buf, 0, count);
                        sb.Append(tempString);
                    }
                }
                while (count > 0);


                return sb.ToString();
            }
            catch (Exception EX)
            {
                RequestLogs(1, "Exception", "APIRequest", url, url, EX.Message, 0, "", "");
                return "Error";
            }
        }

        /// <summary>
        /// Http Get Request SecurityProtocolType
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string APIConsumeTls12(string url)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                byte[] buf = new byte[8192];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                string tempString = null;
                int count = 0;
                do
                {
                    count = resStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                    {
                        tempString = Encoding.ASCII.GetString(buf, 0, count);
                        sb.Append(tempString);
                    }
                }
                while (count > 0);


                return sb.ToString();
            }
            catch (Exception EX)
            {
                RequestLogs(1, "Exception", "APIRequest", url, url, EX.Message, 0, "", "");
                return "Error";
            }
        }

        /// <summary>
        /// Http Post method
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static string ApiRequest(string requestUrl)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "POST";
                request.Timeout = 1000000;
                request.ContentType = "application/x-www-form-urlencoded";
                using (var stream = request.GetRequestStream())
                {
                }
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    return responseString.ToString();
                }
                else
                {
                    RequestLogs(1, "Exception", "ApiRequest", requestUrl, requestUrl, response.StatusCode.ToString(), 0, "", "");
                    return "Error";
                }
            }
            catch (Exception EX)
            {
                RequestLogs(1, "Exception", "ApiRequest", requestUrl, requestUrl, EX.Message, 0, "", "");
                return "Error";
            }
        }

        /// <summary>
        /// Request Log
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="number"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="refNumber"></param>
        /// <param name="extra1"></param>
        /// <param name="extra2"></param>
        /// <returns></returns>
        public static bool RequestLogs(int userId, string number, string requestType, string url, string request, string response, long refNumber, string extra1, string extra2)
        {
            try
            {
                using (UnitOfWork _unitOfWork = new UnitOfWork())
                {
                    RequestLog add = new RequestLog()
                    {
                        UserId = userId,
                        Number = number,
                        RequestType = requestType,
                        AddDate = DateTime.Now,
                        RequestUrl = url,
                        Request = request,
                        Response = response,
                        IpAddress = HttpContext.Current.Request.UserHostAddress,
                        RefId = refNumber,
                        Extra1 = extra1,
                        Extra2 = extra2
                    };
                    _unitOfWork.RequestLogRepository.Insert(add);
                    _unitOfWork.Save();
                    return true;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Stored activity log
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="number"></param>
        /// <param name="requestType"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="extra1"></param>
        /// <param name="extra2"></param>
        /// <returns></returns>
        public static bool ActivityLog(int userId, string number, string requestType, string request, string response, string extra1, string extra2)
        {
            try
            {
                using (UnitOfWork _unitOfWork = new UnitOfWork())
                {
                    ActivityLog add = new ActivityLog()
                    {
                        UserId = userId,
                        Number = number,
                        RequestType = requestType,
                        AddDate = DateTime.Now,
                        Request = request,
                        Response = response,
                        IpAddress = HttpContext.Current.Request.UserHostAddress,
                        Extra1 = extra1,
                        Extra2 = extra2
                    };
                    _unitOfWork.ActivityRepository.Insert(add);
                    _unitOfWork.Save();
                    return true;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// stored response log
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="number"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="refNumber"></param>
        /// <param name="extra1"></param>
        /// <param name="extra2"></param>
        /// <returns></returns>
        public static bool ResponseLogs(int userId, string number, string requestType, string url, string request, string response, long refNumber, string extra1, string extra2)
        {
            try
            {
                using (UnitOfWork _unitOfWork = new UnitOfWork())
                {
                    ResponsesLog add = new ResponsesLog()
                    {
                        UserId = userId,
                        Number = number,
                        RequestType = requestType,
                        AddDate = DateTime.Now,
                        RequestUrl = url,
                        Request = request,
                        Response = response,
                        IpAddress = HttpContext.Current.Request.UserHostAddress,
                        RefId = refNumber,
                        Extra1 = extra1,
                        Extra2 = extra2
                    };
                    _unitOfWork.ResponseLogRepository.Insert(add);
                    _unitOfWork.Save();
                    return true;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Request Log
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="number"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="refNumber"></param>
        /// <param name="extra1"></param>
        /// <param name="extra2"></param>
        /// <returns></returns>
        public static bool ClentRequestLog(int userId, string number, string requestType, string url, string request, string response, long refNumber, string extra1, string extra2)
        {
            try
            {
                using (UnitOfWork _unitOfWork = new UnitOfWork())
                {
                    ClientRequestLog add = new ClientRequestLog()
                    {
                        UserId = userId,
                        Number = number,
                        RequestType = requestType,
                        AddDate = DateTime.Now,
                        RequestUrl = url,
                        Request = request,
                        Response = response,
                        IpAddress = HttpContext.Current.Request.UserHostAddress,
                        RefId = refNumber,
                        Extra1 = extra1,
                        Extra2 = extra2
                    };
                    _unitOfWork.ClientRequestLogRepository.Insert(add);
                    _unitOfWork.Save();
                    return true;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Request Log
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="number"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="refNumber"></param>
        /// <param name="extra1"></param>
        /// <param name="extra2"></param>
        /// <returns></returns>
        public static bool AEPSRequestLogs(int userId, string number, string requestType, string url, string request, string response, long refNumber, string extra1, string extra2)
        {
            try
            {
                using (UnitOfWork _unitOfWork = new UnitOfWork())
                {
                    AEPSRequestLog add = new AEPSRequestLog()
                    {
                        UserId = userId,
                        Number = number,
                        RequestType = requestType,
                        AddDate = DateTime.Now,
                        RequestUrl = url,
                        Request = request,
                        Response = response,
                        IpAddress = HttpContext.Current.Request.UserHostAddress,
                        RefId = refNumber,
                        Extra1 = extra1,
                        Extra2 = extra2
                    };
                    _unitOfWork.AEPSRequestLogRepository.Insert(add);
                    _unitOfWork.Save();
                    return true;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Request Log
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="number"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="refNumber"></param>
        /// <param name="extra1"></param>
        /// <param name="extra2"></param>
        /// <returns></returns>
        public static bool SDKAuthenticationLogs(int userId, string number, string requestType, string url, string request, string response, long refNumber, string extra1, string extra2)
        {
            try
            {
                using (UnitOfWork _unitOfWork = new UnitOfWork())
                {
                    SDKAuthRequestLog add = new SDKAuthRequestLog()
                    {
                        UserId = userId,
                        Number = number,
                        RequestType = requestType,
                        AddDate = DateTime.Now,
                        RequestUrl = url,
                        Request = request,
                        Response = response,
                        IpAddress = HttpContext.Current.Request.UserHostAddress,
                        RefId = refNumber,
                        Extra1 = extra1,
                        Extra2 = extra2
                    };
                    _unitOfWork.SDKAuthRequestLogRepository.Insert(add);
                    _unitOfWork.Save();
                    return true;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Http Post Request
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public static string POSTAPIConsume(string requestUrl, string requestData)
        {
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                var data = Encoding.ASCII.GetBytes(requestData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString.ToString();
            }
            catch (Exception EX)
            {
                RequestLogs(1, "Exception", "POSTAPIConsume", requestUrl, requestData, EX.Message, 0, "", "");
                return "Error";
            }
        }

        /// <summary>
        /// Http POST JSON API INVOKE
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public static string HttpRequestJson(string url, string requestBody)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var data = Encoding.ASCII.GetBytes(requestBody);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("feSessionId", "Sancxc2sre");
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString.ToString();
            }
            catch (Exception Ex)
            {
                return "Error$" + Ex.Message.ToString();
            }
        }

        /// <summary>
        /// Http POST JSON API INVOKE
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public static string HttpRequestJson(string url, object requestBody)
        {
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                #region Http Rest
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(requestBody);

                IRestResponse response = client.Execute(request);
                string responseData = response.Content.ToString();
                return responseData;
                #endregion
            }
            catch (Exception Ex)
            {
                return "Error$" + Ex.Message.ToString();
            }
        }

        /// <summary>
        /// Customer Transaction Ref
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="userID"></param>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        public static bool CustomerTransactionRef(UnitOfWork _unitOfWork, int userID, string transactionID)
        {
            try
            {
                Int16 flag = 0;
                var transactionResult = _unitOfWork.SqlQuery<CustomerTransactionRef_Result>("CustomerTransactionRef @UserId,@TransactionID,@Flag", new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = userID }, new SqlParameter("TransactionID", System.Data.SqlDbType.VarChar) { Value = transactionID }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = flag }).ToList();
                if (transactionResult.Count == 0)
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Customer Transaction Ref
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="userID"></param>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        public static CustomerTransactionRef_Result CustomerTransactionRefs(UnitOfWork _unitOfWork, int userID, string transactionID)
        {
            try
            {
                Int16 flag = 0;
                var transactionResult = _unitOfWork.SqlQuery<CustomerTransactionRef_Result>("CustomerTransactionRef @UserId,@TransactionID,@Flag", new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = userID }, new SqlParameter("TransactionID", System.Data.SqlDbType.VarChar) { Value = transactionID }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = flag }).ToList();
                return transactionResult.FirstOrDefault();
            }
            catch { return null; }
        }

        /// <summary>
        /// Customer Pending Transaction Details
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        public static CustomerTransactionRef_Result CustomerPendingTransaction(UnitOfWork _unitOfWork, string transactionID)
        {
            try
            {
                Int16 flag = 2;
                var transactionResult = _unitOfWork.SqlQuery<CustomerTransactionRef_Result>("CustomerTransactionRef @UserId,@TransactionID,@Flag", new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = 0 }, new SqlParameter("TransactionID", System.Data.SqlDbType.VarChar) { Value = transactionID }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = flag }).ToList();
                return transactionResult.FirstOrDefault();
            }
            catch { return null; }
        }

        /// <summary>
        /// Customer Statements Ref
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="userID"></param>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        public static List<CustomerStatementRef_Result> CustomerStatementsRefs(UnitOfWork _unitOfWork, int userID, string transactionID)
        {
            try
            {
                Int16 flag = 1;
                var transactionResult = _unitOfWork.SqlQuery<CustomerStatementRef_Result>("CustomerStatementRef @UserId,@TransactionID,@Flag", new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = userID }, new SqlParameter("TransactionID", System.Data.SqlDbType.VarChar) { Value = transactionID }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = flag }).ToList();
                return transactionResult;
            }
            catch { return null; }
        }

        /// <summary>
        /// Update Pending Transaction
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="transactionID"></param>
        /// <param name="operatorRef"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool CustomerPendingTransaction(UnitOfWork _unitOfWork, Int64 transactionID, string operatorRef, Int16 flag)
        {
            try
            {
                var transactionResult = _unitOfWork.SqlQuery<int>("CustomerTransactionupdate @TransactionID,@OperatorRef,@Flag", new SqlParameter("TransactionID", System.Data.SqlDbType.BigInt) { Value = transactionID }, new SqlParameter("OperatorRef", System.Data.SqlDbType.VarChar) { Value = operatorRef }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = flag }).ToList();
                if (transactionResult[0] == 1)
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Update Pending Transaction
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="transactionID"></param>
        /// <param name="operatorRef"></param>
        /// <param name="apiRef"></param>
        /// <param name="message"></param>
        /// <param name="apiRequestlog"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool CustomerPendingTransaction(UnitOfWork _unitOfWork, Int16 apiID, Int64 transactionID, string operatorRef, string apiRef, string message, string apiRequestlog, Int16 flag)
        {
            try
            {
                var transactionResult = _unitOfWork.SqlQuery<int>("CustomerReprocTransactionupdate @TransactionID,@OperatorRef,@APIID,@APIRef,@Remarks,@ApiReq,@Flag", new SqlParameter("TransactionID", System.Data.SqlDbType.BigInt) { Value = transactionID }, new SqlParameter("OperatorRef", System.Data.SqlDbType.VarChar) { Value = operatorRef }, new SqlParameter("APIID", System.Data.SqlDbType.SmallInt) { Value = apiID }, new SqlParameter("APIRef", System.Data.SqlDbType.VarChar) { Value = apiRef }, new SqlParameter("Remarks", System.Data.SqlDbType.VarChar) { Value = message }, new SqlParameter("ApiReq", System.Data.SqlDbType.VarChar) { Value = apiRequestlog }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = flag }).ToList();
                if (transactionResult[0] == 1)
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Update Pending Transaction
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="transactionID"></param>
        /// <param name="operatorRef"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool AEPSCustomerPendingTransaction(UnitOfWork _unitOfWork, Int64 transactionID, string operatorRef, string aadharNo, string bankName, string custMobileNo, string status, string txnLogs, Int16 flag)
        {
            try
            {
                var transactionResult = _unitOfWork.SqlQuery<int>("AEPSCustomerTransactionupdate @TransactionID,@OperatorRef,@aadharNo,@BankName,@CustMobileNo,@FNOStatus,@TxnLogs,@Flag", new SqlParameter("TransactionID", System.Data.SqlDbType.BigInt) { Value = transactionID }, new SqlParameter("OperatorRef", System.Data.SqlDbType.VarChar) { Value = operatorRef }, new SqlParameter("aadharNo", System.Data.SqlDbType.VarChar) { Value = aadharNo }, new SqlParameter("BankName", System.Data.SqlDbType.VarChar) { Value = bankName }, new SqlParameter("CustMobileNo", System.Data.SqlDbType.VarChar) { Value = custMobileNo }, new SqlParameter("FNOStatus", System.Data.SqlDbType.VarChar) { Value = status }, new SqlParameter("TxnLogs", System.Data.SqlDbType.VarChar) { Value = txnLogs }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = flag }).ToList();
                if (transactionResult[0] == 1)
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Validate customer txn 
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="userID"></param>
        /// <param name="number"></param>
        /// <param name="amount"></param>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        public static int TransactionValidate(UnitOfWork _unitOfWork, int userID, string number, decimal amount, string transactionID)
        {
            try
            {
                var transactionResult = _unitOfWork.SqlQuery<int>("CustomerTransactionValidate @userID,@number,@amount,@refNumber", new SqlParameter("userID", System.Data.SqlDbType.Int) { Value = userID }, new SqlParameter("number", System.Data.SqlDbType.VarChar) { Value = number }, new SqlParameter("amount", System.Data.SqlDbType.Decimal) { Value = amount }, new SqlParameter("refNumber", System.Data.SqlDbType.VarChar) { Value = transactionID }).ToList();
                if (transactionResult.Count > 0)
                    return transactionResult[0];
                else
                    return 2;
            }
            catch (Exception) { return 10; }
        }

        /// <summary>
        /// Customer Transaction Ref
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="userID"></param>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        public static CustomerTransactionRef_Result CustomerTransactionRefs(UnitOfWork _unitOfWork, int userID, string transactionID, short flag)
        {
            try
            {
                var transactionResult = _unitOfWork.SqlQuery<CustomerTransactionRef_Result>("CustomerTransactionRef @UserId,@TransactionID,@Flag", new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = userID }, new SqlParameter("TransactionID", System.Data.SqlDbType.VarChar) { Value = transactionID }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = flag }).ToList();
                return transactionResult.FirstOrDefault();
            }
            catch { return null; }
        }

        #region X method
        /// <summary>
        /// method
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="useHashing"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public static string RG5HashCrypto(string toEncrypt, bool useHashing, string hashKey)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                AppSettingsReader settingsReader = new AppSettingsReader();
                // Get the key from config file
                string key = hashKey;
                //System.Windows.Forms.MessageBox.Show(key);
                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch { return null; }
        }

        /// <summary>
        /// De
        /// </summary>
        /// <param name="cipherString"></param>
        /// <param name="useHashing"></param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        public static string RG5HashDecryCrypto(string cipherString, bool useHashing, string hashkey)
        {
            cipherString = cipherString.Replace(" ", "+");
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            AppSettingsReader settingsReader = new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = hashkey;

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static string GenerateHash(string objText)
        {
            try
            {
                int keySize = 256;
                int passwordIterations = 03;
                string hashAlgorithm = "MD5";
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(objText);
                PasswordDeriveBytes password = new PasswordDeriveBytes
                (
                passPhrase,
                saltValueBytes,
                hashAlgorithm,
                passwordIterations
                );
                byte[] keyBytes = password.GetBytes(keySize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor
                (
                keyBytes,
                initVectorBytes
                );
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream
                (
                memoryStream,
                encryptor,
                CryptoStreamMode.Write
                );
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                string cipherText = Convert.ToBase64String(cipherTextBytes);
                cipherText = HttpUtility.UrlEncode(cipherText);
                return cipherText;
            }
            catch (Exception) { return null; }
        }

        public static string DecryptionHash(string cipherText)
        {
            try
            {
                string plainText = "";
                int keySize = 256;
                int passwordIterations = 03;
                string hashAlgorithm = "MD5";
                try
                {
                    cipherText = HttpUtility.UrlDecode(cipherText);
                    cipherText = cipherText.Replace(" ", "+");
                    byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                    byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                    byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                    PasswordDeriveBytes password = new PasswordDeriveBytes
                    (
                    passPhrase,
                    saltValueBytes,
                    hashAlgorithm,
                    passwordIterations
                    );
                    byte[] keyBytes = password.GetBytes(keySize / 8);
                    RijndaelManaged symmetricKey = new RijndaelManaged();
                    symmetricKey.Mode = CipherMode.CBC;
                    ICryptoTransform decryptor = symmetricKey.CreateDecryptor
                    (
                    keyBytes,
                    initVectorBytes
                    );
                    MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                    CryptoStream cryptoStream = new CryptoStream
                    (
                    memoryStream,
                    decryptor,
                    CryptoStreamMode.Read
                    );
                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                    int decryptedByteCount = cryptoStream.Read
                    (
                    plainTextBytes,
                    0,
                    plainTextBytes.Length
                    );
                    memoryStream.Close();
                    cryptoStream.Close();
                    plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                }
                catch (Exception ex)
                {
                    plainText = "";
                }
                return plainText;
            }
            catch (Exception ex) { return null; }
        }

        static byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
        public static string Encrypt(string originalString)
        {
            //if (String.IsNullOrEmpty(originalString))
            //{
            //    throw new ArgumentNullException "The string which needs to be encrypted can not be null.");
            //}

            var cryptoProvider = new DESCryptoServiceProvider();
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes),
                CryptoStreamMode.Write);
            var writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        public static string Decrypt(string encryptedString)
        {
            //if (String.IsNullOrEmpty(encryptedString))
            //{
            //    throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
            //}

            var cryptoProvider = new DESCryptoServiceProvider();
            var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedString));
            var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes),
                CryptoStreamMode.Read);
            var reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }
        #endregion

        /// <summary>
        /// a
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string RGHash(string data)
        {
            return RG5HashCrypto(data, true, "AndYWeb34");
        }

        /// <summary>
        /// a
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DecryptionRGHash(string data)
        {
            return RG5HashDecryCrypto(data, true, "AndYWeb34");
        }

        private static string SmallNumberToWord(int number, string words)
        {
            if (number <= 0) return words;
            if (words != "")
                words += " ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
            return words;
        }

        public static string NumberToWords(double doubleNumber)
        {
            var beforeFloatingPoint = (int)Math.Floor(doubleNumber);
            var beforeFloatingPointWord = $"{NumberToWords(beforeFloatingPoint)}";
            var afterFloatingPointWord =
                $"{SmallNumberToWord((int)((doubleNumber - beforeFloatingPoint) * 100), "")}";
            if (!string.IsNullOrEmpty(afterFloatingPointWord))
                return $"{beforeFloatingPointWord} and {afterFloatingPointWord} Rupees only.";
            else
                return $"{beforeFloatingPointWord} Rupees only.";
        }

        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            var words = "";

            if (number / 1000000000 > 0)
            {
                words += NumberToWords(number / 1000000000) + " billion ";
                number %= 1000000000;
            }

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            words = SmallNumberToWord(number, words);

            return words;
        }

        public static string MacAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }

        /// <summary>
        /// Customer Pending Transaction Details
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        public static CustomerTransactionRef_Result CustomerPendingTransaction(UnitOfWork _unitOfWork, string transactionID, Int16 flag)
        {
            try
            {
                var transactionResult = _unitOfWork.SqlQuery<CustomerTransactionRef_Result>("CustomerTransactionRef @UserId,@TransactionID,@Flag", new SqlParameter("UserId", System.Data.SqlDbType.Int) { Value = 0 }, new SqlParameter("TransactionID", System.Data.SqlDbType.VarChar) { Value = transactionID }, new SqlParameter("Flag", System.Data.SqlDbType.SmallInt) { Value = flag }).ToList();
                return transactionResult.FirstOrDefault();
            }
            catch { return null; }
        }

    }
}
