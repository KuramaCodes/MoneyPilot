using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

public class Encrypt
{
    private string passphrase = File.ReadAllText(@"../../Keys/Passphrase.key");

    public string EncryptToBase64(string Input)
    {
        var userBytes = Encoding.UTF8.GetBytes(Input);
        var userHash = MD5.Create().ComputeHash(userBytes);
        SymmetricAlgorithm crypt = Aes.Create();
        crypt.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(passphrase));
        crypt.IV = new byte[16];
        var memorystream = new MemoryStream();
        var cryptostream = new CryptoStream(memorystream, crypt.CreateEncryptor(), CryptoStreamMode.Write);
        cryptostream.Write(userBytes, 0, userBytes.Length);
        cryptostream.Write(userHash, 0, userHash.Length);
        cryptostream.FlushFinalBlock();
        var resultString = Convert.ToBase64String(memorystream.ToArray());
        return resultString;
    }
}