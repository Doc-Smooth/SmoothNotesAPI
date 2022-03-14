using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace SmoothNotesAPI.Service;

public class RSAService
{
    private string containerName = "RSAContainer";

    private UnicodeEncoding ByteConverter = new UnicodeEncoding();

    public static string RSAEncrypt(string DataToEncrypt, string Key, bool DoOAEPPadding)
    {
        try
        {
            byte[] data = ConverterService.ConvertToByteArray(DataToEncrypt);

            byte[] encryptedData;
            //Create a new instance of RSACryptoServiceProvider.
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromXmlString(Key);
                //Import the RSA Key information. This only needs
                //toinclude the public key information.
                //RSA.ImportParameters(RSAKeyInfo);

                //Encrypt the passed byte array and specify OAEP padding.  
                //OAEP padding is only available on Microsoft Windows XP or
                //later.  
                encryptedData = RSA.Encrypt(data, DoOAEPPadding);
            }
            return Convert.ToBase64String(encryptedData);
        }
        //Catch and display a CryptographicException  
        //to the console.
        catch (CryptographicException e)
        {
            Console.WriteLine(e.Message);

            return null;
        }
    }

    public static string RSADecrypt(string DataToDecrypt, string Key, bool DoOAEPPadding)
    {
        try
        {
            byte[] data = Convert.FromBase64String(DataToDecrypt);
            byte[] decryptedData;
            //Create a new instance of RSACryptoServiceProvider.
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                //Import the RSA Key information. This needs
                //to include the private key information.
                //RSA.ImportParameters(RSAKeyInfo);
                RSA.FromXmlString(Key);

                //Decrypt the passed byte array and specify OAEP padding.  
                //OAEP padding is only available on Microsoft Windows XP or
                //later.  
                decryptedData = RSA.Decrypt(data, DoOAEPPadding);
            }
            return ConverterService.ConvertToString(decryptedData);
        }
        //Catch and display a CryptographicException  
        //to the console.
        catch (CryptographicException e)
        {
            Console.WriteLine(e.ToString());

            return null;
        }
    }

    //private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);

    //public string Encrypt(string plainText, )
    //{
    //    csp = new RSACryptoServiceProvider();
    //    csp.ImportParameters(_publicKey);
    //    var data = Encoding.Unicode.GetBytes(plainText);
    //    var cypher = csp.Encrypt(data, false);
    //    return Convert.ToBase64String(cypher);
    //}

    //public string Decrypt(string cypherText)
    //{
    //    var dataBytes = Convert.FromBase64String(cypherText);
    //    csp.ImportParameters(_privateKey);
    //    var plainText = csp.Decrypt(dataBytes, false);
    //    return Encoding.Unicode.GetString(plainText);
    //}



    public List<string> GenKeyPair()
    {
        // Create the CspParameters object and set the key container
        // name used to store the RSA key pair.
        var parameters = new CspParameters
        {
            KeyContainerName = containerName
        };

        // Create a new instance of RSACryptoServiceProvider that accesses
        // the key container MyKeyContainerName.
        using var rsa = new RSACryptoServiceProvider(parameters)
        {
            PersistKeyInCsp = false
        };
        List<string> result = new List<string>();
        //Private Key
        result.Add(ConverterService.EncodeKey(rsa.ToXmlString(true)));
        //Public Key
        result.Add(ConverterService.EncodeKey(rsa.ToXmlString(false)));

        // Call Clear to release resources and delete the key from the container.
        rsa.Clear();

        return result;
    }

    //private static void GenKey_SaveInContainer(string containerName)
    //{
    //    // Create the CspParameters object and set the key container
    //    // name used to store the RSA key pair.
    //    var parameters = new CspParameters
    //    {
    //        KeyContainerName = containerName
    //    };

    //    // Create a new instance of RSACryptoServiceProvider that accesses
    //    // the key container MyKeyContainerName.
    //    using var rsa = new RSACryptoServiceProvider(parameters);

    //    // Display the key information to the console.
    //    //Console.WriteLine($"Key added to container: \n  {rsa.ToXmlString(true)}");
    //}

    //private static string GetKeyFromContainerPrivate(string containerName)
    //{
    //    // Create the CspParameters object and set the key container
    //    // name used to store the RSA key pair.
    //    var parameters = new CspParameters
    //    {
    //        KeyContainerName = containerName
    //    };

    //    // Create a new instance of RSACryptoServiceProvider that accesses
    //    // the key container MyKeyContainerName.
    //    using var rsa = new RSACryptoServiceProvider(parameters);

    //    // Display the key information to the console.
    //    //Console.WriteLine($"Key retrieved from container : \n {rsa.ToXmlString(true)}");
    //    return rsa.ToXmlString(true);
    //}

    //private static string GetKeyFromContainerPublic(string containerName)
    //{
    //    // Create the CspParameters object and set the key container
    //    // name used to store the RSA key pair.
    //    var parameters = new CspParameters
    //    {
    //        KeyContainerName = containerName
    //    };

    //    // Create a new instance of RSACryptoServiceProvider that accesses
    //    // the key container MyKeyContainerName.
    //    using var rsa = new RSACryptoServiceProvider(parameters);

    //    // Display the key information to the console.
    //    //Console.WriteLine($"Key retrieved from container : \n {rsa.ToXmlString(true)}");
    //    return rsa.ToXmlString(false);
    //}

    //private static void DeleteKeyFromContainer(string containerName)
    //{
    //    // Create the CspParameters object and set the key container
    //    // name used to store the RSA key pair.
    //    var parameters = new CspParameters
    //    {
    //        KeyContainerName = containerName
    //    };

    //    // Create a new instance of RSACryptoServiceProvider that accesses
    //    // the key container.
    //    using var rsa = new RSACryptoServiceProvider(parameters)
    //    {
    //        // Delete the key entry in the container.
    //        PersistKeyInCsp = false
    //    };

    //    // Call Clear to release resources and delete the key from the container.
    //    rsa.Clear();

    //    Console.WriteLine("Key deleted.");
    //}
}
