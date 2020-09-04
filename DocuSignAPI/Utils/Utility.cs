using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DocuSign.Utils
{
    public static class Utility
    {
        public static bool IsEventLogged= Convert.ToBoolean(ConfigurationManager.AppSettings["EnableEventLogging"]);
        public static string DocuSignAccountID= string.Empty;
        public static void LogAction(string action)
        {
            string logFilePath = ConfigurationManager.AppSettings["LogFileLocation"];
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
                DirectoryInfo directoryInfo = new DirectoryInfo(logFilePath);
                DirectorySecurity accessControl = directoryInfo.GetAccessControl();
                accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                directoryInfo.SetAccessControl(accessControl);
            }
            using (StreamWriter streamWriter = new StreamWriter(string.Concat(logFilePath, "\\", "Log.txt"), true))
            {
                streamWriter.WriteLine(string.Concat(new string[] { DateTime.Now.ToString(), ":", DateTime.Now.Millisecond.ToString(), " -@: ", action }));
                streamWriter.Close();
            }
        }
        public static string DecryptText(string CipherText)
        {
            string password = ConfigurationManager.AppSettings["passwordEnc"];
            string HashAlgorithm = ConfigurationManager.AppSettings["HashAlgorithm"];
            int PasswordIterations = Convert.ToInt32(ConfigurationManager.AppSettings["PasswordIterations"]);
            string InitialVector = ConfigurationManager.AppSettings["InitialVector"];
            int KeySize = Convert.ToInt32(ConfigurationManager.AppSettings["KeySize"]);
            string Salt = ConfigurationManager.AppSettings["SaltKey"];
            if (string.IsNullOrEmpty(CipherText))
                return "";

            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);

            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);

            byte[] CipherTextBytes = Convert.FromBase64String(CipherText);

            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(password, SaltValueBytes, HashAlgorithm, PasswordIterations);

            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);

            RijndaelManaged SymmetricKey = new RijndaelManaged();

            SymmetricKey.Mode = CipherMode.CBC;
            //SymmetricKey.Padding = PaddingMode.None;

            byte[] PlainTextBytes = new byte[CipherTextBytes.Length];

            int ByteCount = 0;

            using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
            {

                using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
                {

                    using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
                    {

                        ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);

                        MemStream.Close();

                        CryptoStream.Close();

                    }

                }

            }

            SymmetricKey.Clear();

            return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);

        }

        public static string EncryptText(string PlainText, string Salt)
        {
            string Password = ConfigurationManager.AppSettings["passwordEnc"];
            string HashAlgorithm = ConfigurationManager.AppSettings["HashAlgorithm"];
            int PasswordIterations = Convert.ToInt32(ConfigurationManager.AppSettings["PasswordIterations"]);
            string InitialVector = ConfigurationManager.AppSettings["InitialVector"];
            int KeySize = Convert.ToInt32(ConfigurationManager.AppSettings["KeySize"]);

            #region New
            if (string.IsNullOrEmpty(PlainText))

                return "";

            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);

            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);

            byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);

            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);

            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);

            RijndaelManaged SymmetricKey = new RijndaelManaged();

            SymmetricKey.Mode = CipherMode.CBC;
            //SymmetricKey.Padding = PaddingMode.None;

            byte[] CipherTextBytes = null;

            using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
            {

                using (MemoryStream MemStream = new MemoryStream())
                {

                    using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
                    {

                        CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);

                        CryptoStream.FlushFinalBlock();

                        CipherTextBytes = MemStream.ToArray();

                        MemStream.Close();

                        CryptoStream.Close();

                    }

                }

            }

            SymmetricKey.Clear();

            return Convert.ToBase64String(CipherTextBytes);
            #endregion

        }
    }
}
