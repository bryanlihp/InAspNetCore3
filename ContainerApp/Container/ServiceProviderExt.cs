using ContainerApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ContainerApp.Container
{
    public static class ServiceProviderExt
    {

        private static object Create(ServiceProvider provider, Type type, Type[] genericArguments)
        {
            if (genericArguments.Length > 0)
            {
                type = type.MakeGenericType(genericArguments);
            }
            var constructors = type.GetConstructors();
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"Cannot create the instance of {type} which does not have an public constructor.");
            }
            var constructor = constructors.FirstOrDefault(it => it.GetCustomAttributes(false).OfType<InjectionAttribute>().Any());
            constructor ??= constructors.First();
            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(type);
            }
            var arguments = new object[parameters.Length];
            for (int index = 0; index < arguments.Length; index++)
            {
                arguments[index] = provider.GetService(parameters[index].ParameterType);
            }
            return constructor.Invoke(arguments);
        }

        public static ServiceProvider Register(this ServiceProvider provider, Type serviceType, Type instanceType, Lifetime lifetime)
        {
            Func<ServiceProvider, Type[], object> factory = (_, arguments) => Create(_, instanceType, arguments);
            provider.Register(new ServiceRegistry(serviceType,lifetime,factory));
            return provider;
        }

        public static ServiceProvider Register<TService, TServiceInstance>(this ServiceProvider provider, Lifetime lifetime) where TServiceInstance : TService
            => provider.Register(typeof(TService), typeof(TServiceInstance), lifetime);

        public static ServiceProvider Register<TService>(this ServiceProvider provider,Func<ServiceProvider,object> Factory, Lifetime lifetime)
        {
            return provider;
        }

        public static ServiceProvider Register(this ServiceProvider provider, Assembly assembly)
        {
            return provider;
        }

        public static T GetService<T>(this ServiceProvider provider) => (T)provider.GetService(typeof(T));
    }
}
