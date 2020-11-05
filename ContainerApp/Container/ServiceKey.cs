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
        public override bool Equals(object obj) => obj is ServiceKey key ? Equals(key) : false;
        public override int GetHashCode()
        {
            var hashCode = Registry.GetHashCode();
            for (int index = 0; index < GenericArguments.Length; index++)
            {
                hashCode ^= GenericArguments[index].GetHashCode();
            }
            return hashCode;
        }

        public bool Equals(ServiceKey other)
        {
            if (Registry != other.Registry)
            {
                return false;
            }
            if (GenericArguments.Length != other.GenericArguments.Length)
            {
                return false;
            }
            for (int index = 0; index < GenericArguments.Length; index++)
            {
                if (GenericArguments[index] != other.GenericArguments[index])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
