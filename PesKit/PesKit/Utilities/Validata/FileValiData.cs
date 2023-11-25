using System.Runtime.Serialization.Formatters;

namespace PesKit.Utilities.Validata
{
    public static class FileValiData
    {
        public static bool ValiDataType(this IFormFile file, string type = "image/")
        {
            if (file.ContentType.Contains(type)) { return true; };

            return false;
        }
        public static bool ValiDataSize(this IFormFile file, int limitMb)
        {
            if (file.Length > limitMb) { return true; }

            return false;
        }
        public static async Task<string> CreateFile(this IFormFile file, string root, params string[] folders)
        {
            string originalFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string guidBasedName = ExtractGuidFileName(originalFileName);
            string fileFormat = GetFileFormat(originalFileName);
            string finalFileName = guidBasedName + fileFormat;
            
            string path = root;
            for(int i  = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }
            path = Path.Combine(path, finalFileName);
            using(FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return finalFileName;

        }
        public static async void DeleteFile(this string fileName, string root, params string[] folders)
        {
            string path = root;
            for (int i = 0;i< folders.Length;i++)
            {
                path += folders[i];
            }
            path += Path.Combine(path, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public static string ExtractGuidFileName(string fullFileName)
        {
            int scoreIndex = fullFileName.IndexOf('_');
            if (scoreIndex != -1)
            {
                string guidBasedFileName = fullFileName.Substring(0, scoreIndex);
                return guidBasedFileName;
            }
            return fullFileName;
        }
        public static string GetFileFormat(string fullFileName)
        {
            int lastDotIndex = fullFileName.LastIndexOf('.');
            if (lastDotIndex != -1)
            {
                string fileFormat = fullFileName.Substring(lastDotIndex);
                return fileFormat;
            }
            return string.Empty;
        }
    }
}
