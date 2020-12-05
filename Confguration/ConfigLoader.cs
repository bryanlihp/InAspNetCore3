using System;
using System.Collections.Generic;
using System.Text;
using Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace Configuration
{
    public static class ConfigLoader
    {
        public static void LoadOptionsFromConfigSource()
        {

            ConfigLoader.MemorySource();
            ConfigLoader.FileSource();
            ConfigLoader.EnvironmentConfiguration("develop");
            ConfigLoader.EnvironmentConfiguration("staging");
            ConfigLoader.FileSource(true);
        }

        private static void MemorySource()
        {
            Console.WriteLine("Load format options from memory key-value pairs");
            var source = new Dictionary<string, string>
            {
                ["Format:Datetime:LongDatePattern"] = "dddd, MMM d, yyyy",
                ["Format:Datetime:LongTimePattern"] = "h:mm:ss tt",
                ["Format:Datetime:ShortDatePattern"] = "M/d/yyyy",
                ["Format:Datetime:ShortTimePattern"] = "h:mm tt",

                ["Format:CurrencyDecimal:Digits"] = "2",
                ["Format:CurrencyDecimal:Symbol"] = "$",
            };

            var configuration = new ConfigurationBuilder()
                .Add(new MemoryConfigurationSource { InitialData = source })
                .Build();
            LoadFormatOptions(configuration);
            Console.WriteLine();


        }
        private static void FileSource(bool reloadOnChange = false)
        {
            Console.WriteLine("Load format options from appsettings.json");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: reloadOnChange)
                .Build();
            LoadFormatOptions(configuration, false);
            Console.WriteLine("Update appsettings.json.");
            if (reloadOnChange)
            {
                Console.Read();
                LoadFormatOptions(configuration, false);
                Console.WriteLine("Reloaded.");
                Console.Read();
            }
            Console.WriteLine();
        }

        private static void EnvironmentConfiguration(string environment)
        {
            Console.WriteLine($"Load format options - env ={environment}");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environment}.json", true)
                .Build();
            LoadFormatOptions(configuration, false);
            Console.WriteLine();
        }

        private static void LoadFormatOptions(IConfigurationRoot configuration, bool testAll = true)
        {
            var formatSection = configuration.GetSection("Format");
            if (testAll)
            {
                var options1 = new FormatOptions(formatSection);
                Console.WriteLine("Show options1");
                ShowOptions(options1);
            }
            var options2 = formatSection.Get<FormatOptions>();
            Console.WriteLine("Show options2");
            ShowOptions(options2);
        }

        private static void ShowOptions(FormatOptions options)
        {
            var dateTime = options.DateTime;
            var currencyDecimal = options.CurrencyDecimal;

            Console.WriteLine("DateTime:");
            Console.WriteLine($"\tLongDatePattern: {dateTime.LongDatePattern}");
            Console.WriteLine($"\tLongTimePattern: {dateTime.LongTimePattern}");
            Console.WriteLine($"\tShortDatePattern: {dateTime.ShortDatePattern}");
            Console.WriteLine($"\tShortTimePattern: {dateTime.ShortTimePattern}");

            Console.WriteLine("CurrencyDecimal:");
            Console.WriteLine($"\tDigits:{currencyDecimal.Digits}");
            Console.WriteLine($"\tSymbol:{currencyDecimal.Symbol}");
        }
    }
}
