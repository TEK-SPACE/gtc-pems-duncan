using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace RBACToolbox
{
    public class SymmetricEncryption
    {
        // Fields
        private string _initialIV;
        private SymmetricAlgorithm _symmetricAlgorithm;

        // Methods
        public SymmetricEncryption(EncryptionType Type)
        {
            this._initialIV = "12345678";
            switch (Type)
            {
                case EncryptionType.DES:
                    this._symmetricAlgorithm = new DESCryptoServiceProvider();
                    break;

                case EncryptionType.RC2:
                    this._symmetricAlgorithm = new RC2CryptoServiceProvider();
                    break;

                case EncryptionType.Rijndael:
                    this._symmetricAlgorithm = new RijndaelManaged();
                    break;

                case EncryptionType.TripleDES:
                    this._symmetricAlgorithm = new TripleDESCryptoServiceProvider();
                    break;
            }
        }

        public SymmetricEncryption(SymmetricAlgorithm ServiceProvider)
        {
            this._initialIV = "12345678";
            this._symmetricAlgorithm = ServiceProvider;
        }

        public string Decrypt(string contents, string key)
        {
            string result;
            byte[] buffer = Convert.FromBase64String(contents);
            using (MemoryStream memorystream = new MemoryStream(buffer, 0, buffer.Length))
            {
                this._symmetricAlgorithm.Key = this.GetLegalKey(key);
                this._symmetricAlgorithm.IV = this.GetLegalIV();
                using (ICryptoTransform transform = this._symmetricAlgorithm.CreateDecryptor())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memorystream, transform, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            return result;
        }

        public string Encrypt(string contents, string key)
        {
            string result;
            byte[] buffer = Encoding.UTF8.GetBytes(contents);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                this._symmetricAlgorithm.Key = this.GetLegalKey(key);
                this._symmetricAlgorithm.IV = this.GetLegalIV();
                using (ICryptoTransform transform = this._symmetricAlgorithm.CreateEncryptor())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(buffer, 0, buffer.Length);
                        cryptoStream.FlushFinalBlock();
                        memoryStream.Close();
                        result = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            return result;
        }

        private byte[] GetLegalIV()
        {
            string sTemp = this._initialIV;
            this._symmetricAlgorithm.GenerateIV();
            int IVLength = this._symmetricAlgorithm.IV.Length;
            if (sTemp.Length > IVLength)
            {
                sTemp = sTemp.Substring(0, IVLength);
            }
            else if (sTemp.Length < IVLength)
            {
                sTemp = sTemp.PadRight(IVLength, ' ');
            }
            return Encoding.ASCII.GetBytes(sTemp);
        }

        private byte[] GetLegalKey(string Key)
        {
            string sTemp = Key;
            this._symmetricAlgorithm.GenerateKey();
            int KeyLength = this._symmetricAlgorithm.Key.Length;
            if (sTemp.Length > KeyLength)
            {
                sTemp = sTemp.Substring(0, KeyLength);
            }
            else if (sTemp.Length < KeyLength)
            {
                sTemp = sTemp.PadRight(KeyLength, ' ');
            }
            return Encoding.ASCII.GetBytes(sTemp);
        }

        // Properties
        public string InitialIV
        {
            get
            {
                return this._initialIV;
            }
            set
            {
                this._initialIV = value;
            }
        }
    }

    public class MD5Class
    {
        // Methods
        public static string ConvertToMD5(string Password)
        {
            MD5 enc = MD5.Create();
            byte[] rescBytes = Encoding.ASCII.GetBytes(Password);
            byte[] hashBytes = enc.ComputeHash(rescBytes);
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                str.Append(hashBytes[i].ToString("X2"));
            }
            return str.ToString();
        }

        /*
        public static string ConvertToMD5_Silverlight(string Password)
        {
            MD5Managed_SL enc = new MD5Managed_SL();
            byte[] rescBytes = Encoding.ASCII.GetBytes(Password);
            byte[] hashBytes = enc.ComputeHash(rescBytes);
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                str.Append(hashBytes[i].ToString("X2"));
            }
            return str.ToString();
        }
        */

        public static string GenerateDigest(string input)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            return Encoding.ASCII.GetString(sha.ComputeHash(Encoding.ASCII.GetBytes(input)));
        }

        public static string Reverse(string InputString)
        {
            int Stringlength = InputString.Length;
            char[] arrString = new char[Stringlength];
            for (int i = 0; i < Stringlength; i++)
            {
                arrString[i] = InputString[(Stringlength - 1) - i];
            }
            return new string(arrString);
        }
    }

    public enum EncryptionType
    {
        DES,
        RC2,
        Rijndael,
        TripleDES
    }
}
