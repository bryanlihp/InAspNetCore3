using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FileSystem.Interface;
using Microsoft.Extensions.FileProviders;

namespace FileSystem.Services
{
    public class FileManager : IFileManager
    {
        private readonly IFileProvider _fileProvider;
        public FileManager(IFileProvider fileProvider) => _fileProvider = fileProvider;
        public void ShowStructure(Action<int, string> render)
        {
            var indent = -1;
            Render("");

            
            void Render(string subPath)
            {
                indent++;
                foreach (var fileInfo in _fileProvider.GetDirectoryContents(subPath))
                {
                    render(indent, fileInfo.Name);
                    if (fileInfo.IsDirectory)
                    {
                        Render($"{subPath}\\{fileInfo.Name}".TrimStart('\\'));
                    }
                }
                indent--;
            }
        }

        public async Task<string> ReadAllTextAsync(string path)
        {
            var fileInfo = _fileProvider.GetFileInfo(path);
            await using var stream = _fileProvider.GetFileInfo(path).CreateReadStream();
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.Default.GetString(buffer);
        }
    }
}
