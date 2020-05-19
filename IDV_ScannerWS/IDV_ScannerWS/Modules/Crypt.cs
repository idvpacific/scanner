using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IDV_ScannerWS.Modules
{
    public class Crypt
    {
        public string Encrypt(string PlainText,string SecurityKey)

        {
            try
            {
                byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);
                MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
                byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
                objMD5CryptoService.Clear();
                var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
                objTripleDESCryptoService.Key = securityKeyArray;
                objTripleDESCryptoService.Mode = CipherMode.ECB;
                objTripleDESCryptoService.Padding = PaddingMode.PKCS7;
                var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
                byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
                objTripleDESCryptoService.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string Decrypt(string CipherText,string SecurityKey)
        {
            try
            {
                byte[] toEncryptArray = Convert.FromBase64String(CipherText);
                MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
                byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
                objMD5CryptoService.Clear();
                var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
                objTripleDESCryptoService.Key = securityKeyArray;
                objTripleDESCryptoService.Mode = CipherMode.ECB;
                objTripleDESCryptoService.Padding = PaddingMode.PKCS7;
                var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
                byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                objTripleDESCryptoService.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}