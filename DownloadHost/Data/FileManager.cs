using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadHost.Data
{
    public class FileManager : IFileManager
    {
        private string GetDownloadPath(string downloadId)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Files", downloadId);
        }

        private string GetFolderPath(string downloadId, string filename)
        {
            return Path.Combine(GetDownloadPath(downloadId), filename);
        }

        public string GenerateRandomFilename(string downloadId)
        {
            string randomFileName;

            do
            {
                randomFileName = Path.GetRandomFileName();
            } while (Directory.Exists(GetFolderPath(downloadId, randomFileName)) || randomFileName.Any(x => char.IsDigit(x)));

            string path = GetFolderPath(downloadId, randomFileName);

            Directory.CreateDirectory(path);

            return randomFileName;
        }

        public async Task SaveFile(string downloadId, string filename, Stream stream)
        {
            string path = GetFolderPath(downloadId, filename);
            int partition = Directory.GetFiles(path).Length + 1;

            using (Stream fileStream = File.Create(Path.Combine(path, $"{filename}.part.{partition}")))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        public void ZipUpFile(string downloadId, string filename)
        {
            ZipFile.CreateFromDirectory(GetFolderPath(downloadId, filename), Path.Combine(GetDownloadPath(downloadId), $"{filename}.zip"));
        }
    }
}
