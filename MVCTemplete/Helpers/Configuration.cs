using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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
    public static string ToDate(DateTime? date)
    {
        return date?.ToString("dd/MM/yyyy") ?? "";
    }

    public static string ToDateTime(DateTime? date)
    {
        return date?.ToString("dd/MM/yyyy hh:mm tt") ?? "";
    }

    public static string ToShortDate(DateTime? date)
    {
        return date?.ToString("dd-MMM-yyyy") ?? "";
    }

    public static string ToLongDate(DateTime? date)
    {
        return date?.ToString("dddd, dd MMMM yyyy") ?? "";
    }

    public static string ToTime(DateTime? date)
    {
        return date?.ToString("hh:mm tt") ?? "";
    }

    public static string To24HourTime(DateTime? date)
    {
        return date?.ToString("HH:mm") ?? "";
    }

    public static string ToIsoDate(DateTime? date)
    {
        return date?.ToString("yyyy-MM-dd") ?? "";
    }

    public static string ToIsoDateTime(DateTime? date)
    {
        return date?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
    }

    public static string ToMonthYear(DateTime? date)
    {
        return date?.ToString("MMM yyyy") ?? "";
    }

    public static string ToFullMonth(DateTime? date)
    {
        return date?.ToString("MMMM yyyy") ?? "";
    }

    public static string ToDayMonth(DateTime? date)
    {
        return date?.ToString("dd MMM") ?? "";
    }

    public static string ToFileName(DateTime? date)
    {
        return date?.ToString("yyyyMMdd_HHmmss") ?? "";
    }

    public static string ToUniversal(DateTime? date)
    {
        return date?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "";
    }

    public static string ToCustom(DateTime? date, string format)
    {
        return date?.ToString(format) ?? "";
    }

    public static DateTime? Parse(string value)
    {
        if (DateTime.TryParse(value, out DateTime date))
            return date;

        return null;
    }

    public static DateTime? ParseExact(string value, string format)
    {
        if (DateTime.TryParseExact(value,
                                   format,
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None,
                                   out DateTime date))
            return date;

        return null;
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

    #region Random 
    public static string AmountToWords(decimal amount)
    {
        if (amount == 0)
            return "Zero Only";

        long rupees = (long)Math.Floor(amount);
        int paise = (int)((amount - rupees) * 100);

        string result = NumberToWords(rupees) + " Rupees";

        if (paise > 0)
            result += " and " + NumberToWords(paise) + " Paise";

        return result + " Only";
    }

    private static string NumberToWords(long number)
    {
        if (number == 0)
            return "";

        if (number < 0)
            return "Minus " + NumberToWords(Math.Abs(number));

        string words = "";

        if ((number / 10000000) > 0)
        {
            words += NumberToWords(number / 10000000) + " Crore ";
            number %= 10000000;
        }

        if ((number / 100000) > 0)
        {
            words += NumberToWords(number / 100000) + " Lakh ";
            number %= 100000;
        }

        if ((number / 1000) > 0)
        {
            words += NumberToWords(number / 1000) + " Thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += NumberToWords(number / 100) + " Hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            if (words != "")
                words += "and ";

            string[] units =
            {
            "Zero","One","Two","Three","Four","Five","Six","Seven","Eight","Nine",
            "Ten","Eleven","Twelve","Thirteen","Fourteen","Fifteen",
            "Sixteen","Seventeen","Eighteen","Nineteen"
        };

            string[] tens =
            {
            "Zero","Ten","Twenty","Thirty","Forty","Fifty",
            "Sixty","Seventy","Eighty","Ninety"
        };

            if (number < 20)
                words += units[number];
            else
            {
                words += tens[number / 10];

                if ((number % 10) > 0)
                    words += " " + units[number % 10];
            }
        }

        return words.Trim();
    }

    public static string ToUpperCase(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? string.Empty : text.ToUpperInvariant();
    }

    public static string ToLowerCase(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? string.Empty : text.ToLowerInvariant();
    }

    public static string ToTitleCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
    }
    public static string FirstLetterCapital(string text)
    {
        if (string.IsNullOrWhiteSpace(text))return string.Empty;

        return char.ToUpper(text[0]) + text.Substring(1).ToLower();
    }

    public static string MaskMobile(string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < 10) return mobile;

        return mobile.Substring(0, 2) + "******" + mobile.Substring(mobile.Length - 2);
    }
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return "";

        int index = email.IndexOf('@');

        if (index <= 2) return email;

        return email.Substring(0, 2) + new string('*', index - 2) + email.Substring(index);
    }

    public static string MaskAadhaar(string aadhaar)
    {
        if (string.IsNullOrWhiteSpace(aadhaar) || aadhaar.Length != 12)return aadhaar;

        return "XXXXXXXX" + aadhaar.Substring(8);
    }

    public static string MaskPAN(string pan)
    {
        if (string.IsNullOrWhiteSpace(pan) || pan.Length != 10)return pan;

        return pan.Substring(0, 3) + "*****" + pan.Substring(8);
    }

    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";

        text = text.ToLowerInvariant();

        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

        text = Regex.Replace(text, @"\s+", "-");

        text = Regex.Replace(text, @"-+", "-");

        return text.Trim('-');
    }
    #endregion
}