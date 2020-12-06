using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace Configuration
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConfigLoader.LoadOptionsFromConfigSource();
            ConfigBinder.LoadOptions();
        }
    }
}
