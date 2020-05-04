using System.IO;
using System.Threading.Tasks;

namespace DownloadHost.Data
{
    public interface IFileManager
    {
        string GenerateRandomFilename(string downloadId);
        Task SaveFile(string downloadId, string filename, Stream stream);
        void ZipUpFile(string downloadId, string filename);
    }
}