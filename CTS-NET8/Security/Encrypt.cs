using CTS_NET8.Connection;
using System.Security.Cryptography;
using System.Text;

namespace CTS_NET8.Security
{
    public class Encrypt
    {
        public static string Encryption(string str)
        {
            var interfaceConfig = new InterfaceConfig();
            try
            {
                TripleDESCryptoServiceProvider des;
                MD5CryptoServiceProvider hashmd5;
                byte[] keyhash, buff;
                string stEncripted;
                hashmd5 = new MD5CryptoServiceProvider();
                keyhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(interfaceConfig.keySecurityEncryptionPassUsers));
                hashmd5 = null;
                des = new TripleDESCryptoServiceProvider();

                des.Key = keyhash;
                des.Mode = CipherMode.ECB;

                buff = ASCIIEncoding.ASCII.GetBytes(str);
                stEncripted = Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));

                return stEncripted;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Decryption(string str, string stkey)
        {
            try
            {
                TripleDESCryptoServiceProvider des;
                MD5CryptoServiceProvider hashmd5;
                byte[] keyhash, buff;
                string stDecripted;
                hashmd5 = new MD5CryptoServiceProvider();
                keyhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(stkey));
                hashmd5 = null;
                des = new TripleDESCryptoServiceProvider();

                des.Key = keyhash;
                des.Mode = CipherMode.ECB;

                buff = Convert.FromBase64String(str);
                stDecripted = ASCIIEncoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));

                return stDecripted;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
