using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerApp.Container
{
    internal class ServiceKey
    {
        public ServiceRegistry Registry { get; }
        public Type[] GenericArguments { get; }

        public ServiceKey(ServiceRegistry registry, Type[] genericArguments)
        {
            Registry = registry;
            GenericArguments = genericArguments;
        }
    }
}
