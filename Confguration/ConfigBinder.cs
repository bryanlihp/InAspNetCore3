using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Configuration.UserTypes;
using Microsoft.Extensions.Configuration;

namespace Configuration
{
    public static class ConfigBinder
    {
        public static void LoadOptions()
        {
            SimpleBinder();
            ClassBinder();
            CollectionBinder();
        }

        private static void SimpleBinder()
        {
            var source = new Dictionary<string, string>
            {
                ["foo"] = null,
                ["bar"] = "",
                ["baz"] = "123",
                ["point"] = "(1.2,5.6)"
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData: source)
                .Build();

            // object type, return the raw value or null
            Debug.Assert(config.GetValue<object>("foo") == null);
            Debug.Assert("".Equals(config.GetValue<object>("bar")));
            Debug.Assert("123".Equals(config.GetValue<object>("baz")));

            // Not Nullable<T>, convert using TypeConverter<T> 
            Debug.Assert(config.GetValue<int>("foo") == 0);
            Debug.Assert(config.GetValue<int>("baz") == 123);

            // Nullable<T>, using TypeConverter if not null or null
            Debug.Assert(config.GetValue<int?>("foo") == null);
            Debug.Assert(config.GetValue<int?>("bar") == null);

            var p = config.GetValue<Point>("point");
            Debug.Assert(Math.Abs(p.X - 1.2) <= 0.01);
            Debug.Assert(Math.Abs(p.Y - 5.6) <= 0.01);
        }

        private static void ClassBinder()
        {

            var source = new Dictionary<string, string>
            {
                ["profile:gender"] = "Male",
                ["profile:age"] = "21",
                ["profile:contactInfo:email"] = "test@hotmail.com",
                ["profile:contactInfo:phone"] = "123-000-4567"
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData: source)
                .Build();

            var profile = config.GetSection("Profile").Get<Profile>();

            Debug.Assert(profile != null);
            Debug.Assert(profile
                .Equals(new Profile
                {
                    Gender = Gender.Male, 
                    Age = 21,
                    ContactInfo = new ContactInfo
                    {
                        Email = "test@hotmail.com", 
                        Phone = "123-000-4567"
                    }

                })
            );
        }
        private static void CollectionBinder()
        {
            var source = new Dictionary<string, string>
            {
                ["foo:gender"] = "Male",
                ["foo:age"] = "21",
                ["foo:contactInfo:email"] = "foo@hotmail.com",
                ["foo:contactInfo:phone"] = "123-000-4567",

                ["bar:gender"] = "Male",
                ["bar:age"] = "25",
                ["bar:contactInfo:email"] = "bar@hotmail.com",
                ["bar:contactInfo:phone"] = "123-001-4567",

                ["baz:gender"] = "Female",
                ["baz:age"] = "26",
                ["baz:contactInfo:email"] = "baz@hotmail.com",
                ["baz:contactInfo:phone"] = "123-002-4567"
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData: source)
                .Build();

            var profiles = config.Get<IEnumerable<Profile>>();

            Debug.Assert(profiles != null);
            Debug.Assert(profiles.Count()==3);
        }
    }
}
