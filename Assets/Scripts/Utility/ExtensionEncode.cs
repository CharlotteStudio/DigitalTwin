using System;
using System.Text;
using System.Security.Cryptography;

public static class ExtensionEncode
{
    public static string ToSHA1ToSum2(this string str)
    {
        var bytes = Encoding.ASCII.GetBytes(str);
        var sha1Provider = new SHA1CryptoServiceProvider();
        return BitConverter.ToString(sha1Provider.ComputeHash(bytes));
    }

    public static string ToSHA256(this string str)
    {
        var bytes = Encoding.ASCII.GetBytes(str);
        SHA256Managed crypt = new SHA256Managed();
        StringBuilder hash = new StringBuilder();
        
        byte[] crypto = crypt.ComputeHash(bytes, 0, bytes.Length);
        
        foreach (byte bit in crypto)
        {
            hash.Append(bit.ToString("x2"));
        }
        
        return hash.ToString().ToLower();
    }
    
    public static string ToMD5(this string str)
    {
        var bytes = Encoding.ASCII.GetBytes(str);
        var md5Provider = new MD5CryptoServiceProvider();
        return BitConverter.ToString(md5Provider.ComputeHash(bytes));
    }
    
    // This is use using()
    public static string ToMD5WithUpper(this string str)
    {
        using (var cryptoMD5 = MD5.Create())
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            var hash = cryptoMD5.ComputeHash(bytes);

            return BitConverter.ToString(hash).Replace("-", String.Empty).ToUpper();
        }
    }
}
