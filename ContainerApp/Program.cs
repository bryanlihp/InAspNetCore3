using ContainerApp.Container;
using ContainerApp.Interfaces;
using ContainerApp.Services;
using System;
using System.Reflection;

namespace ContainerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new ServiceProvider();
            root.Register<IFoo, Foo>(Lifetime.Transient)
                .Register<IBar>(_ => new Bar(), Lifetime.Scope)
                .Register<IBaz, Baz>(Lifetime.Root)
                .Register(Assembly.GetEntryAssembly());
            var scope1 = root.CreateScope();
            var scope2 = root.CreateScope();

            void GetServices<TService>(ServiceProvider provider)
            {
                provider.GetService<TService>();
                provider.GetService<TService>();
            }

            GetServices<IFoo>(root);
            GetServices<IBar>(scope1);
            GetServices<IBaz>(scope1);
            GetServices<IQux>(scope1);

            Console.WriteLine("");

            GetServices<IFoo>(scope2);
            GetServices<IBar>(scope2);
            GetServices<IBaz>(scope2);
            GetServices<IQux>(scope2);
        }
    }
}
