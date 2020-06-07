using System.IO;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data.CloudStorage
{
    public interface ICloudStorage
    {
        Task DeleteFileAsync(string bucket, string filename);
        Task<string> UploadFileAsync(Stream stream, string bucket, string filename);
    }
}