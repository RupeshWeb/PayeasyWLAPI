using System;
using System.Configuration;
using System.Text;

namespace BusinessServices.Pancard
{
    public class UTIAPIServices
    {
        #region Private Variable
        private readonly string UTI_PassKey = ConfigurationManager.AppSettings["Pan_UTI_KEY"];
        private readonly string UTI_PassID = ConfigurationManager.AppSettings["Pan_UTI_ID"];
        #endregion

        public string UtiGenerateChecksum(string mobileNo, long orderId)
        {
            try
            {
                string strLoginName = mobileNo; // e.g. demovle
                string iLoginID = "MASFPL" + orderId; // e.g. 1234

                string strLoginID = iLoginID.ToString();
                string strPasskey = UTI_PassKey; // e.g. 

                string value = strLoginName + strLoginID + strPasskey;
                string strSCACode = UTI_PassID;
                string strChecksum = "";

                byte[] asciiBytes = Encoding.ASCII.GetBytes(value);

                Int64 iTotalA = 1;
                Int64 iTotalB = 0;
                Int64 iAdler32 = 0;

                foreach (byte b in asciiBytes)
                {
                    iTotalA = iTotalA + b;
                    iTotalB = iTotalB + iTotalA;
                }

                iAdler32 = ((iTotalB % 65521) * 65536) + (iTotalA % 65521);

                strChecksum = iAdler32.ToString();
                //string asas = "iTotalA : " + iTotalA + "<br/>iTotalB : " + iTotalB + "<br/>userHandle : " + strLoginName + "<br/>transId : " + strLoginID + "<br/>strSCACode : " + strSCACode + "<br/>strChecksum : " + strChecksum;
                return "userHandle=" + strLoginName + "&transId=" + strLoginID + "&checksum=" + strChecksum + "&entityId=A45";
            }
            catch (Exception) { return null; }
        }
    }
}
