using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Configuration.UserTypes
{
    [TypeConverter(typeof(PointTypeConverter))]
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class PointTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var coords = value.ToString().Split(',');
            var t1 = coords[0].Trim().TrimStart('(');
            var t2 = coords[1].Trim().TrimEnd(')');
            var x = double.Parse(coords[0].Trim().TrimStart('('));
            var y = double.Parse(coords[1].Trim().TrimEnd(')'));
            return new Point
            {
                X = x,
                Y = y
            };
        }
    }
}
