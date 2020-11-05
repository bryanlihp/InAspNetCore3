using ContainerApp.Container;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerApp.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class InjectionAttribute : Attribute{}


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MapToAttribute : Attribute 
    {
        public Type ServiceType { get; }
        public Lifetime Lifetime { get; }

        public MapToAttribute(Type serviceType, Lifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }
    }
}
