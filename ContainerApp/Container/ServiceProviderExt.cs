using ContainerApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
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

        public static ServiceProvider Register(this ServiceProvider provider, Type serviceType, Type serviceImplType, Lifetime lifetime)
        {
            Func<ServiceProvider, Type[], object> factory = (_, arguments) => Create(_, serviceImplType, arguments);
            provider.Register(new ServiceRegistry(serviceType, lifetime, factory));
            return provider;
        }

        public static ServiceProvider Register<TService, TServiceImpl>(this ServiceProvider provider, Lifetime lifetime) where TServiceImpl : TService
            => provider.Register(typeof(TService), typeof(TServiceImpl), lifetime);

        public static ServiceProvider Register<TService>(this ServiceProvider provider, Func<ServiceProvider, object> factory, Lifetime lifetime)
            => provider.Register(new ServiceRegistry(typeof(TService), lifetime, (_, arguments) => factory(_)));

        public static ServiceProvider Register(this ServiceProvider provider, Assembly assembly)
        {
            var typeAttributes = from type in assembly.GetExportedTypes()
                                 let attribute = type.GetCustomAttribute<MapToAttribute>()
                                 where attribute != null
                                 select new { ServiceImplType = type, Attribute = attribute };
            /*
            var typeAttributes1 = assembly.GetExportedTypes()
                    .Where(t => t.GetCustomAttribute<MapToAttribute>() != null)
                    .Select(t => new { ServiceImplType = t, Attribute = t.GetCustomAttribute<MapToAttribute>() });
            */        
            foreach (var typeAttr in typeAttributes)
            {
                provider.Register(typeAttr.Attribute.ServiceType, typeAttr.ServiceImplType, typeAttr.Attribute.Lifetime);
            }
            return provider;
        }

        public static T GetService<T>(this ServiceProvider provider) => (T)provider.GetService(typeof(T));

        public static IEnumerable<T> GetServices<T>(this ServiceProvider provider) => provider.GetService<IEnumerable<T>>();

    }
}
