using System;
using System.Configuration;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace BusinessServices.Pancard
{
    public class UTIEncryptions
    {
        public static string serverMac = ConfigurationManager.AppSettings["Pan_UTI_MAC"].ToString();

        public static string EncryptData(string content)
        {
            try
            {
                byte[] array = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Pan_UTI_KeyValue"]);
                return Encrypt(content.Trim(), array);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string DecryptData(string content)
        {
            try
            {
                byte[] array = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Pan_UTI_KeyValue"]);
                return Decrypt(content.Trim(), array);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static string Decrypt(string textToDecrypt, byte[] keyValue)
        {
            if (!VerifyMac())
            {
                throw new Exception("Authentication Failed");
            }
            RijndaelManaged managed = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 0x80,
                BlockSize = 0x80
            };
            int length = textToDecrypt.Length;
            byte[] inputBuffer = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
            byte[] sourceArray = keyValue;
            byte[] destinationArray = new byte[0x10];
            int num2 = sourceArray.Length;
            if (num2 > destinationArray.Length)
            {
                num2 = destinationArray.Length;
            }
            Array.Copy(sourceArray, destinationArray, num2);
            managed.Key = destinationArray;
            managed.IV = destinationArray;
            byte[] bytes = managed.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        private static string Encrypt(string textToEncrypt, byte[] keyValue)
        {
            if (!VerifyMac())
            {
                throw new Exception("Authentication Failed");
            }
            RijndaelManaged managed = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 0x80,
                BlockSize = 0x80
            };
            byte[] sourceArray = keyValue;
            byte[] destinationArray = new byte[0x10];
            int length = sourceArray.Length;
            if (length > destinationArray.Length)
            {
                length = destinationArray.Length;
            }
            Array.Copy(sourceArray, destinationArray, length);
            managed.Key = destinationArray;
            managed.IV = destinationArray;
            ICryptoTransform transform = managed.CreateEncryptor();
            byte[] bytes = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(transform.TransformFinalBlock(bytes, 0, bytes.Length));
        }

        private byte[] GetFileBytes(string filePath)
        {
            byte[] buffer;
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int num2;
                int length = (int)stream.Length;
                buffer = new byte[length];
                for (int i = 0; (num2 = stream.Read(buffer, i, length - i)) > 0; i += num2)
                {
                }
            }
            finally
            {
                stream.Close();
            }
            return buffer;
        }

        //public static string GetMac()
        //{
        //    ManagementObjectCollection instances = new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
        //    string str = string.Empty;
        //    foreach (ManagementObject obj2 in instances)
        //    {
        //        if ((str == string.Empty) && ((bool)obj2["IPEnabled"]))
        //        {
        //            str = obj2["MacAddress"].ToString();
        //        }
        //        obj2.Dispose();
        //    }
        //    return str;
        //}

        public string GetMD5Hash(string name)
        {
            MD5 md = new MD5CryptoServiceProvider();
            byte[] bytes = new ASCIIEncoding().GetBytes(name);
            byte[] buffer2 = md.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder(buffer2.Length * 2);
            foreach (byte num in buffer2)
            {
                builder.AppendFormat("{0:x2}", num);
            }
            return builder.ToString();
        }

        private static bool VerifyMac()
        {
            String SystemMac = getMac();
            return serverMac.Equals(SystemMac);
        }

        private static string getMac()
        {
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                String sMacAddress = string.Empty;
                foreach (NetworkInterface adapter in nics)
                {
                    if (sMacAddress == String.Empty)
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        sMacAddress = adapter.GetPhysicalAddress().ToString();//"1831BF272175";//
                    }
                }
                return sMacAddress;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
