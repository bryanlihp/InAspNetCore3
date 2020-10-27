using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerApp.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class InjectionAttribute : Attribute{}
}
