using System.Drawing.Text;
using AstraBlog.Services.Interfaces;

namespace AstraBlog.Services
{
    public class ImageService : IImageService
    {
        private readonly string _defaultUserImage = "/img/DefaultUserImage.png";
        private readonly string _defaultBlogImage = "/img/DefaultBlogImage.jpg";
        private readonly string _defaultCategoryImage = "/img/DefaultCategoryImage.png";

        public string ConvertByteArrayToFile(byte[] fileData, string extension, int defaultImage)
        {
            if (fileData == null || fileData.Length == 0)
            {
                switch (defaultImage)
                {
                    case 1: return _defaultUserImage;
                    case 2: return _defaultBlogImage;
                    case 3: return _defaultCategoryImage;
                    default: throw new ArgumentException("Invalid default image type", nameof(defaultImage));
                }
            }

            try
            {
                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = "image/jpeg"; // Default extension
                }

                string imageBase64Data = Convert.ToBase64String(fileData);
                return $"data:{extension};base64,{imageBase64Data}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error converting byte array to file: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File cannot be null");
            }

            if (file.Length == 0)
            {
                throw new ArgumentException("File is empty", nameof(file));
            }

            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                
                if (memoryStream.Length == 0)
                {
                    throw new Exception("Failed to read file content");
                }

                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();

                return byteFile;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error converting file to byte array: {ex.Message}", ex);
            }
        }
    }
}
