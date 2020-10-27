using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerApp.Container
{
    public class ServiceRegistry
    {
        public Type ServiceType { get; }
        public Lifetime Lifetime { get; }
        public Func<ServiceProvider, Type[], object> Factory { get; }
        internal ServiceRegistry Next { get; set; }

        public ServiceRegistry(Type serviceType, Lifetime lifetime, Func<ServiceProvider, Type[], object> factory)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
            Factory = factory;
        }

        internal IEnumerable<ServiceRegistry> AsEnumerable()
        {
            var list = new List<ServiceRegistry>();
            for(var registry = this; registry !=null; registry = registry.Next )
            {
                list.Add(registry);
            }
            return list;
        }

    }
}
