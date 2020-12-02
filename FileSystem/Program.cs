using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using FileSystem.Interface;
using FileSystem.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace FileSystem
{
    class Program
    {
        static void Print(int layer, string name) => Console.WriteLine(($"{new string(' ', layer * 4)}{name}"));
        static async Task Main(string[] args)
        {
            await ShowStructure();
            await ShowResourceContent();
            await FileWatcher();
        }

        static async Task ShowStructure()
        {
            var testPath = @"D:\Dev\InAspNetCore\InAspNetCore3\FileSystem\Test";
            var testFile = $"{testPath}\\data.txt";
            var text = await File.ReadAllTextAsync(testFile);
            await using var serviceProvider = new ServiceCollection()
                .AddSingleton<IFileProvider>(new PhysicalFileProvider(testPath))
                .AddSingleton<IFileManager, FileManager>()
                .BuildServiceProvider();

            var fileManager = serviceProvider.GetService<IFileManager>();
            if (fileManager != null)
            {
                fileManager.ShowStructure(Print);
                var content = await fileManager.ReadAllTextAsync("data.txt");
                Debug.Assert(content == text);
            }
        }

        static async Task ShowResourceContent()
        {
            var assembly = Assembly.GetExecutingAssembly();
            //var resourceNames = assembly.GetManifestResourceNames();
            var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Test.data.txt");
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            var text = Encoding.Default.GetString(buffer);

            await using var serviceProvider = new ServiceCollection()
                .AddSingleton<IFileProvider>(new EmbeddedFileProvider(assembly))
                .AddSingleton<IFileManager, FileManager>()
                .BuildServiceProvider();
            var fileManager = serviceProvider.GetService<IFileManager>();
            var content = await fileManager.ReadAllTextAsync("Test.data.txt");
            Debug.Assert(content == text);
        }

        static async Task FileWatcher()
        {
            using var fileProvider = new PhysicalFileProvider(@"D:\Dev\InAspNetCore\InAspNetCore3\FileSystem\Test");
            string original = null;
            ChangeToken.OnChange(() => fileProvider.Watch("data.txt"), Callback);
            while (true)
            {
                await File.WriteAllTextAsync(@"D:\Dev\InAspNetCore\InAspNetCore3\FileSystem\Test\data.txt",DateTime.Now.ToString(CultureInfo.InvariantCulture));
                await Task.Delay(5000);
            }

            async void Callback()
            {
                await using var stream = fileProvider.GetFileInfo("data.txt").CreateReadStream();
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                string current = Encoding.Default.GetString(buffer);
                if (current != original)
                {
                    original = current;
                    Console.WriteLine(original);
                }
            }
        }
    }
}
