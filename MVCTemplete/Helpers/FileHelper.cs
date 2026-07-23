using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;


public static class FileHelper
{
    /// <summary>
    /// Check if uploaded file exists.
    /// </summary>
    public static bool IsFileAvailable(HttpPostedFileBase file)
    {
        return file != null && file.ContentLength > 0;
    }

    /// <summary>
    /// Validate allowed extensions.
    /// </summary>
    public static bool IsValidExtension(HttpPostedFileBase file, params string[] allowedExtensions)
    {
        if (file == null)
            return false;

        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions
            .Select(x => x.ToLowerInvariant())
            .Contains(extension);
    }

    /// <summary>
    /// Validate maximum file size in MB.
    /// </summary>
    public static bool IsValidFileSize(HttpPostedFileBase file, int maxSizeMB)
    {
        if (file == null)
            return false;

        return file.ContentLength <= (maxSizeMB * 1024 * 1024);
    }

    /// <summary>
    /// Generate unique file name.
    /// </summary>
    public static string GenerateUniqueFileName(string originalFileName)
    {
        string extension = Path.GetExtension(originalFileName);

        return $"{Guid.NewGuid():N}{extension}";
    }

    /// <summary>
    /// Save uploaded file.
    /// </summary>
    public static string SaveFile(HttpPostedFileBase file, string folderPath)
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = GenerateUniqueFileName(file.FileName);

        string fullPath = Path.Combine(folderPath, fileName);

        file.SaveAs(fullPath);

        return fileName;
    }

    /// <summary>
    /// Delete file if exists.
    /// </summary>
    public static bool DeleteFile(string fullPath)
    {
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Read file bytes.
    /// </summary>
    public static byte[] ReadFile(string fullPath)
    {
        return File.Exists(fullPath)
            ? File.ReadAllBytes(fullPath)
            : null;
    }

    /// <summary>
    /// Copy file.
    /// </summary>
    public static void CopyFile(string source, string destination)
    {
        File.Copy(source, destination, true);
    }

    /// <summary>
    /// Move file.
    /// </summary>
    public static void MoveFile(string source, string destination)
    {
        File.Move(source, destination);
    }

    /// <summary>
    /// Get file extension.
    /// </summary>
    public static string GetExtension(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant();
    }

    /// <summary>
    /// Get file size in MB.
    /// </summary>
    public static double GetFileSizeMB(HttpPostedFileBase file)
    {
        return Math.Round(file.ContentLength / 1024d / 1024d, 2);
    }

    /// <summary>
    /// Check if file exists.
    /// </summary>
    public static bool Exists(string fullPath)
    {
        return File.Exists(fullPath);
    }

    /// <summary>
    /// Get MIME type.
    /// </summary>
    public static string GetMimeType(HttpPostedFileBase file)
    {
        return file?.ContentType;
    }

    // Get filename without extension
    public static string GetFileNameWithoutExtension(string fileName)
    {
        return Path.GetFileNameWithoutExtension(fileName);
    }

    // Create directory if not exists
    public static void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    // File to Base64
    public static string ConvertToBase64(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        return Convert.ToBase64String(bytes);
    }

    // Base64 to Bytes
    public static byte[] Base64ToBytes(string base64)
    {
        return Convert.FromBase64String(base64);
    }

    // Download file bytes
    public static byte[] DownloadFile(string filePath)
    {
        return File.ReadAllBytes(filePath);
    }

    // Resize Image
    public static void ResizeImage(string inputPath, string outputPath, int width, int height)
    {
        using (Image image = Image.FromFile(inputPath))
        using (Bitmap bitmap = new Bitmap(width, height))
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, 0, 0, width, height);

            bitmap.Save(outputPath, ImageFormat.Jpeg);
        }
    }

    // Compress Image (JPEG Quality)
    public static void CompressImage(string inputPath, string outputPath, long quality = 60L)
    {
        using (Image image = Image.FromFile(inputPath))
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] =
    new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            image.Save(outputPath, jpgEncoder, encoderParams);
        }
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
                return codec;
        }

        return null;
    }

    // Create ZIP
    public static void CreateZip(string sourceFolder, string zipFilePath)
    {
        if (File.Exists(zipFilePath))
            File.Delete(zipFilePath);

        ZipFile.CreateFromDirectory(sourceFolder, zipFilePath);
    }

    // Extract ZIP
    public static void ExtractZip(string zipFilePath, string destinationFolder)
    {
        ZipFile.ExtractToDirectory(zipFilePath, destinationFolder);
    }

    // SHA256 File Hash
    public static string GetFileHashSHA256(string filePath)
    {
        using (SHA256 sha = SHA256.Create())
        using (FileStream stream = File.OpenRead(filePath))
        {
            byte[] hash = sha.ComputeHash(stream);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }

    // Remove invalid filename characters
    public static string SanitizeFileName(string fileName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }

        return fileName;
    }

    // Get Relative Path
    public static string GetRelativePath(string rootPath, string fullPath)
    {
        Uri root = new Uri(rootPath.EndsWith("\\") ? rootPath : rootPath + "\\");
        Uri full = new Uri(fullPath);

        return Uri.UnescapeDataString(
            root.MakeRelativeUri(full)
                .ToString()
                .Replace('/', '\\'));
    }

    // Get Absolute Path
    public static string GetAbsolutePath(string relativePath)
    {
        return Path.GetFullPath(relativePath);
    }
}