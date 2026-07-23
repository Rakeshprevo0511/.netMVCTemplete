using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class Configuration
{
    #region DateTime

    public static DateTime ConvertToIST(DateTime utcDateTime)
    {
        TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istZone);
    }

    public static DateTime GetISTNow()
    {
        return ConvertToIST(DateTime.UtcNow);
    }

    public static DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }

    #endregion

    #region Base64 Encode/Decode

    public static string Base64Encode(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
    }

    public static string Base64Decode(string encodedText)
    {
        if (string.IsNullOrEmpty(encodedText))
            return string.Empty;

        return Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
    }

    #endregion

    #region Password Hash (PBKDF2)

    public static string HashPassword(string password)
    {
        using (var deriveBytes = new Rfc2898DeriveBytes(
            password,
            16,
            100000,
            HashAlgorithmName.SHA256))
        {
            byte[] salt = deriveBytes.Salt;
            byte[] hash = deriveBytes.GetBytes(32);

            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash))
            return false;

        string[] parts = storedHash.Split('.');

        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        using (var deriveBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256))
        {
            byte[] newHash = deriveBytes.GetBytes(32);

            return FixedTimeEquals(hash, newHash);
        }
    }
    private static bool FixedTimeEquals(byte[] left, byte[] right)
    {
        if (left == null || right == null || left.Length != right.Length)
            return false;

        int diff = 0;

        for (int i = 0; i < left.Length; i++)
        {
            diff |= left[i] ^ right[i];
        }

        return diff == 0;
    }
    #endregion

    #region Random String

    public static string GenerateRandomString(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[length];
            rng.GetBytes(bytes);

            var sb = new StringBuilder(length);

            foreach (byte b in bytes)
                sb.Append(chars[b % chars.Length]);

            return sb.ToString();
        }
    }

    #endregion

    #region OTP

    public static string GenerateOTP(int length = 6)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[4];
            rng.GetBytes(bytes);

            int number = BitConverter.ToInt32(bytes, 0) & int.MaxValue;

            return (number % (int)Math.Pow(10, length))
                .ToString()
                .PadLeft(length, '0');
        }
    }

    #endregion

    #region GUID

    public static string GenerateUniqueId()
    {
        return Guid.NewGuid().ToString();
    }

    #endregion

    #region File Extension

    public static string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName)?.ToLowerInvariant();
    }

    #endregion

    #region SHA256

    public static string ComputeSHA256(string text)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }

    #endregion
}