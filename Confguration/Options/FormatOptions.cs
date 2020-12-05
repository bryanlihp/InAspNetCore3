using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Configuration.Options
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

}
