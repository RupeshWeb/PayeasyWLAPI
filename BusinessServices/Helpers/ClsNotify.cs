using DataModel;
using DataModel.UnitOfWork;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace BusinessServices
{
    public class ClsNotify
    {
        private readonly UnitOfWork _unitOfWork;

        public ClsNotify(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Recipient Verification
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileNumber"></param>
        /// <param name="otp"></param>
        /// <returns></returns>
        public bool RecipientVerifyOtp(int userId, string mobileNumber, string otp)
        {
            string message = otp + " is the One Time Password(OTP) for recipient registration. PLS Do Not Share The OTP With Anyone.";
            return SendSms(userId, mobileNumber, message, clsVariables.SmsType.RecipientVerify);
        }

        public bool RemitterVerifyOtp(int userId, string mobileNumber, string otp)
        {
            string message = otp + " is the One Time Password(OTP) for remitter registration. PLS Do Not Share The OTP With Anyone.";
            return SendSms(userId, mobileNumber, message, clsVariables.SmsType.RecipientVerify);
        }

        public bool RecipientRemoveOtp(int userId, string mobileNumber, string otp)
        {
            string message = otp + " is the One Time Password(OTP) for recipient remove. PLS Do Not Share The OTP With Anyone.";
            return SendSms(userId, mobileNumber, message, clsVariables.SmsType.RecipientRemove);
        }

        public bool ChangePassword(int userId, string mobileNumber, string otp)
        {
            string message = otp + " is the One Time Password(OTP) for charge password. PLS Do Not Share The OTP With Anyone.";
            return SendSms(userId, mobileNumber, message, clsVariables.SmsType.ChangePassword);
        }

        public bool DmtTransfer(int userId, string mobileNumber, string amount, string recipientName, string bankName, string accountNo, string refNo, string paymentType)
        {
            string message = "Dear Customer Rs " + amount + " transferred successfully to " + recipientName + ", " + bankName + ", Account No. " + accountNo + ", " + paymentType + " ref No " + refNo + ", Thanks.";
            return SendSms(userId, mobileNumber, message, clsVariables.SmsType.MoneyTransfer);
        }

        public bool BillPayemnt(int userId, string mobileNumber, string amount, string consumnerNo, string refNo)
        {
            string message = "Thanks you for bill Payment of Rs." + amount + " against PSPCL. Consumner No." + consumnerNo + " Txn. Ref. ID " + refNo + " recived on " + DateTime.Now.ToShortDateString() + " Paypanel.";
            return SendSms(userId, mobileNumber, message, clsVariables.SmsType.BillPayment);
        }

        public bool ComplaintRegistration(int userId, string mobileNumber, string transactionId, string complaintId)
        {
            string message = "Your complaint has been registered successfully for Txn. Ref. ID " + transactionId + ". Your complaint ID is " + complaintId + ". Track your complaint using this ID.";
            return SendSms(userId, mobileNumber, message, clsVariables.SmsType.ComplaintRegistration);
        }

        /// <summary>
        /// send Sms
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendSms(int userId, string mobileNumber, string message, string smsType)
        {
            return SmsRequest(userId, mobileNumber, message, smsType);
        }

        /// <summary>
        /// sms request
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileNumber"></param> 
        /// <param name="message"></param>
        /// <returns></returns>
        private bool SmsRequest(int userId, string mobileNumber, string message, string smsType)
        {
            try
            {
                SMSApi getApi = new SMSApi();
                //if (new[] { clsVariables.SmsType.RecipientVerify, clsVariables.SmsType.RecipientRemove, clsVariables.SmsType.MoneyTransfer, clsVariables.SmsType.AccountValidate }.Contains(smsType))
                //getApi = _unitOfWork.SmaApiRepository.GetFirst(x => x.Status == true && x.SenderId == "MASFPL");
                //else
                getApi = _unitOfWork.SmaApiRepository.GetFirst(x => x.Status == true);
                if (getApi != null)
                {
                    if (getApi.APIName == clsVariables.SmsAPI.Bulk)
                    {
                        string requestUrl = "https://sms6.rmlconnect.net:8443/bulksms/bulksms?";
                        string body = "username=" + getApi.Username + "&password=" + getApi.Password + "&type=0&dlr=1&destination=" + mobileNumber + "&source=" + getApi.SenderId + "&message=" + message;
                        string response = APIConsume(requestUrl + body);
                        SmsStore(userId, mobileNumber, smsType, requestUrl + body, response, getApi.APIName);
                        return true;
                    }
                    else if (getApi.APIName == clsVariables.SmsAPI.MobiComm)
                    {
                        string requestUrl = "http://mobicomm.dove-sms.com//submitsms.jsp?";
                        string body = "user=" + getApi.Username + "&key=" + getApi.Password + "&mobile=+91" + mobileNumber + "&message=" + message + "&senderid=" + getApi.SenderId + "&accusage=1";
                        string response = SiteContents(requestUrl + body);
                        SmsStore(userId, mobileNumber, smsType, requestUrl + body, response, getApi.APIName);
                        return true;
                    }
                    else if (getApi.APIName == clsVariables.SmsAPI.SmsVille)
                    {
                        string requestUrl = "http://www.smsville.com/api/sendhttp.php?";
                        string body = "authkey=" + getApi.Password + "&mobiles=91" + mobileNumber + "&message=" + message + "&sender=" + getApi.SenderId + "&route=4&country=91";
                        string response = SiteContents(requestUrl + body);
                        SmsStore(userId, mobileNumber, smsType, requestUrl + body, response, getApi.APIName);
                        return true;
                    }
                    else if (getApi.APIName == clsVariables.SmsAPI.SmsCountry)
                    {
                        string requestUrl = "http://api.smscountry.com/SMSCwebservice_bulk.aspx?";
                        string body = "User=" + getApi.Username + "&passwd=" + getApi.Password + "&mobilenumber=91" + mobileNumber + "&message=" + message + "&sid=" + getApi.SenderId + "&mtype=N&DR=Y";
                        string response = SiteContents(requestUrl + body);
                        SmsStore(userId, mobileNumber, smsType, requestUrl + body, response, getApi.APIName);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Http Request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string SiteContents(string url)
        {
            try
            {
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
            catch (Exception ex)
            {
                ClsMethods.RequestLogs(1, "Notification", "Exception", "Get", url, ex.Message, 0, string.Empty, string.Empty);
                return ex.Message;
            }
        }

        /// <summary>
        /// Stored sms log
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="number"></param>
        /// <param name="smsType"></param>
        /// <param name="message"></param>
        /// <param name="response"></param>
        /// <param name="apiName"></param>
        /// <returns></returns>
        private bool SmsStore(int userId, string number, string smsType, string message, string response, string apiName)
        {
            try
            {
                SmsLog objsms = new SmsLog();
                objsms.SenderMobile = number;
                objsms.SmsMessage = message;
                objsms.AddDate = DateTime.Now;
                objsms.SmsType = smsType;
                objsms.IpAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                objsms.Status = true;
                objsms.Response = response;
                objsms.Extra = apiName;
                objsms.UserID = 0;
                _unitOfWork.SmsLogRepository.Insert(objsms);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// http request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string APIConsume(string url)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
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
            catch (Exception ex)
            {
                ClsMethods.RequestLogs(1, "Notification", "Exception", "Get", url, ex.Message, 0, string.Empty, string.Empty);
                return ("Error");
            }
        }
    }
}
