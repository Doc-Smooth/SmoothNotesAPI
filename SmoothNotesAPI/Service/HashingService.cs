namespace SmoothNotesAPI.Service;

public class HashingService
{
    public string HashPW(string PW)
    {
        string HPW = BCrypt.Net.BCrypt.EnhancedHashPassword(PW, 12, BCrypt.Net.HashType.SHA512);

        return HPW;
    }

    public bool VerifyPW(string PW, string HPW)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(PW, HPW, BCrypt.Net.HashType.SHA512);
    }
}
