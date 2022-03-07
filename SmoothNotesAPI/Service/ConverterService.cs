using System.Text;

namespace SmoothNotesAPI.Service;

public static class ConverterService
{
    public static byte[] ConvertToByteArray(string input)
    {
        return input.Select(Convert.ToByte).ToArray();
    }

    public static string ConvertToString(byte[] bytes)
    {
        return new string(bytes.Select(Convert.ToChar).ToArray());
    }

    public static string ByteArrayToHexString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", "");
    }

    public static byte[] HexStringToByteArray(string hex)
    {
        byte[] ba = Enumerable.Range(0, hex.Length / 2).Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)).ToArray();
        return ba;
    }



    public static string EncodeKey(string arg)
    {
        return ByteArrayToHexString(Encoding.UTF8.GetBytes(arg));
    }
    public static string ReadEncodedKey(string arg)
    {
        return Encoding.UTF8.GetString(HexStringToByteArray(arg));
    }
}
