using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

public static class ExtensionConvert
{
    #region Binary

    public static int BinaryToInteger(this int binary)
        => Convert.ToInt32(binary.ToString(), 2);
    
    public static int BinaryToInteger(this string content)
        => Convert.ToInt32(content, 2);

    public static string ToBinaryString(this int number)
        => Convert.ToString(number, 2);
    
    public static int ToBinaryInteger(this int number)
        => Convert.ToInt32(ToBinaryString(number), 2);
    
    public static bool[] BinaryToBools(this string content)
    {
        var size = content.Length;
        bool[] bools = new bool[size];
        
        for (int i = 0; i < size; i++)
        {
            bools[i] = content[i] == '1';
        }

        return bools;
    }
    
    public static string ToBinaryString(this bool[] boolArray)
    {
        var result = "";
        foreach (var b in boolArray)
        {
            result += b ? "1" : "0";
        }
        return result;
    }

    #endregion
    
    #region Byte
    
    // It also can convert string to bytes
    //
    // But when you input chinese word it will get wrong byte
    // 1, return System.Text.Encoding.UTF8.GetBytes(str);
    // 1, return System.Text.Encoding.Default.GetBytes(str);

    // 這會有 warning
    // 2, return System.Text.ASCIIEncoding.UTF8.GetBytes(str);

    // 這樣可以, 但會 new 一個 object, 好似有D多
    // 3, return new System.Text.ASCIIEncoding().GetBytes(str);
    
    public static byte[] ToBytes(this string str)
    {
        // if you input chinese word it will get 63 byte, is ?
        return Encoding.ASCII.GetBytes(str);
    }
    
    public static byte[] ToBytes(this string str, string intervalString)
    {
        var strArray = str.Split(intervalString);
        var bytesSize = strArray.Length;
        var bytes = new byte[bytesSize];
        
        for(int i = 0; i < bytesSize; i++)
        {
            bytes[i] = (byte)Convert.ToInt32(strArray[i], 16);
        }

        return bytes;
    }
    
    public static string ToString(this byte[] bytes)
    {
        return Encoding.ASCII.GetString(bytes);
    }
    
    public static string ToString_HEX(this byte[] bytes)
    {
        return BitConverter.ToString(bytes);
    }
    
    #endregion

    #region Json

    ///<see cref="https://gist.github.com/frankhu-2021/b6750185b19fd4ada4ba36b099985813"/>>
    public static string ToPrettyPrintJsonString(this string json) =>
        JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);

    ///<see cref="https://stackoverflow.com/questions/4580397/json-formatter-in-c"/>>
    public static string ToFormatJsonString(this string json, string indent = "  ")
    {
        var indentation = 0;
        var quoteCount = 0;
        var escapeCount = 0;

        var result =
            from ch in json ?? string.Empty
            let escaped = (ch == '\\' ? escapeCount++ : escapeCount > 0 ? escapeCount-- : escapeCount) > 0
            let quotes = ch == '"' && !escaped ? quoteCount++ : quoteCount
            let unquoted = quotes % 2 == 0
            let colon = ch == ':' && unquoted ? ": " : null
            let nospace = char.IsWhiteSpace(ch) && unquoted ? string.Empty : null
            let lineBreak = ch == ',' && unquoted ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indent, indentation)) : null
            let openChar = (ch == '{' || ch == '[') && unquoted ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indent, ++indentation)) : ch.ToString()
            let closeChar = (ch == '}' || ch == ']') && unquoted ? Environment.NewLine + string.Concat(Enumerable.Repeat(indent, --indentation)) + ch : ch.ToString()
            select colon ?? nospace ?? lineBreak ?? (
                openChar.Length > 1 ? openChar : closeChar
            );
        return string.Concat(result);
    }
    
    #endregion
}
