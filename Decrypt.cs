using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Reflection;

public class Decrypt
{
    private string passphrase = File.ReadAllText(@"../../Keys/Passphrase.key");

    public string DecryptFromBase64(string Input)
    {
        var encryptedBytes = Convert.FromBase64String(Input);
        SymmetricAlgorithm crypt = Aes.Create();
        crypt.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(passphrase));
        crypt.IV = new byte[16];
        var memoryStream = new MemoryStream();
        var cryptoStream = new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Write);
        cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
        cryptoStream.FlushFinalBlock();
        var allBytes = memoryStream.ToArray();
        var userLen = allBytes.Length - 16;
        if (userLen < 0) throw new Exception("Invalid Len");
        var userHash = new byte[16];
        Array.Copy(allBytes, userLen, userHash, 0, 16);
        var decryptHash = MD5.Create().ComputeHash(allBytes, 0, userLen);
        if (userHash.SequenceEqual(decryptHash) == false) throw new Exception("Invalid Hash");
        var resultString = Encoding.UTF8.GetString(allBytes, 0, userLen);
        return resultString;
    }
}