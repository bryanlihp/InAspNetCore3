using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace Confguration
{

    public class DateTimeFormatOptions
    {
        public DateTimeFormatOptions()
        {

        }
        public DateTimeFormatOptions(IConfiguration config)
        {
            LongDatePattern = config["LongDatePattern"];
            LongTimePattern = config["LongTimePattern"];
            ShortDatePattern = config["ShortDatePattern"];
            ShortTimePattern = config["ShortTimePattern"];
        }
        public string LongDatePattern { get; set; }
        public string LongTimePattern { get; set; }
        public string ShortDatePattern { get; set; }
        public string ShortTimePattern { get; set; }
    }

    public class CurrencyDecimalFormatOptions
    {
        public int Digits { get; set; }
        public string Symbol { get; set; }
        public CurrencyDecimalFormatOptions(IConfiguration config)
        {
            Digits = int.Parse(config["Digits"]);
            Symbol = config["Symbol"];
        }

        public CurrencyDecimalFormatOptions()
        {

        }
    }

    public class FormatOptions
    {
        public FormatOptions()
        {
        }

        public FormatOptions(IConfiguration config)
        {
            DateTime = new DateTimeFormatOptions(config.GetSection("DateTime"));
            CurrencyDecimal = new CurrencyDecimalFormatOptions(config.GetSection("CurrencyDecimal"));
        }
        public DateTimeFormatOptions DateTime { get; set; }
        public CurrencyDecimalFormatOptions CurrencyDecimal { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            MemorySource();
            FileSource();
        }

        static void MemorySource()
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
                .Add(new MemoryConfigurationSource {InitialData = source})
                .Build();
            LoadFormatOptions(configuration);
            Console.WriteLine();
        }
        static void FileSource()
        {
            Console.WriteLine("Load format options from appsettings.json");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            LoadFormatOptions(configuration);
            Console.WriteLine();
        }

        private static void LoadFormatOptions(IConfigurationRoot configuration)
        {
            var formatSection = configuration.GetSection("Format");
            var options1 = new FormatOptions(formatSection);
            Console.WriteLine("Show options1");
            ShowOptions(options1);

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
