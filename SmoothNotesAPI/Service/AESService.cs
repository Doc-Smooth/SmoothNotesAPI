using System.Security.Cryptography;
using System.Text;

namespace SmoothNotesAPI.Service;

public class AESService
{
    private byte[] GetKeyBytes(string arg)
    {
        //string conv;

        //conv = arg.Substring(16, 16);

        List<string> filler = new List<string> { "0", "j", "f", "i", "5", "o", "j", "f", "i", "e", "7", "j", "i", "f", "j", "a", "o", "e", "8", "f", "k", "n", "u", "9", "u", "e", "1", "h", "d", "a", "k", "d" };
        Console.WriteLine(filler.Count);
        int i = 1;
        if(arg.Length > 32)
        {
            arg = arg.Substring(0, 32);
        }

        while (arg.Length < 32)
        {
            arg += filler[i];
            i++;
        }

        return ConverterService.ConvertToByteArray(arg);
    }
    //public byte[] Encrypt(string Text, string Key, byte[] IV)
    //{
    //    //Check values
    //    if (Text == null || Text.Length <= 0) throw new ArgumentNullException("Text");
    //    if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");
    //    if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");

    //    byte[] eData;

    //    // CLosed scoped using statements version
    //    #region Closed scoped using statments
    //    //Create an Aes object with the specified key and IV
    //    using (Aes aes = Aes.Create())
    //    {
    //        aes.Key = GetKeyBytes(Key);
    //        aes.IV = IV;

    //        using (MemoryStream ms = new MemoryStream())
    //        {
    //            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
    //            {
    //                using (StreamWriter sw = new StreamWriter(cs))
    //                {
    //                    //Writing the data to the stream
    //                    sw.Write(Text);
    //                }
    //                eData = ms.ToArray();
    //            }
    //        }

    //    }
    //    #endregion

    //    // Return the encrypted bytes from memory stream
    //    return eData;
    //}
    public string Encrypt(string Text, string Key)
    {
        //Check values
        if (Text == null || Text.Length <= 0) throw new ArgumentNullException("Text");
        if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");

        byte[] eData;
        string final;

        //Create an Aes object with the specified key and IV
        using (Aes aes = Aes.Create())
        {
            aes.Key = GetKeyBytes(Key);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        //Writing the data to the stream
                        sw.Write(Text);
                    }
                    eData = ms.ToArray();
                }
            }

            final = Convert.ToBase64String(ConverterService.HexStringToByteArray(ConverterService.ByteArrayToHexString(aes.IV) + ConverterService.ByteArrayToHexString(eData)));
        }


        // Return IV and the encrypted data as hex string
        return final;
    }

    public string Decrypt(string cText, string Key)
    {
        //Check values
        if (cText == null || cText.Length <= 0) throw new ArgumentNullException("cText");
        if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");

        string dData;
        string iv = ConverterService.ByteArrayToHexString(Convert.FromBase64String(cText)).Substring(0, 32);
        byte[] data = ConverterService.HexStringToByteArray(ConverterService.ByteArrayToHexString(Convert.FromBase64String(cText)).Substring(32));

        using (Aes aes = Aes.Create())
        {
            aes.Key = GetKeyBytes(Key);
            aes.IV = ConverterService.HexStringToByteArray(iv);

            using (MemoryStream ms = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        // Read the decrypted bytes and place them in a string
                        dData = sr.ReadToEnd();
                    }
                }
            }
        }

        // Return the decrypted string
        return dData;
    }
}
