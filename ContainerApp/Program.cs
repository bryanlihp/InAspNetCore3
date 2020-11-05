using ContainerApp.Container;
using ContainerApp.Interfaces;
using ContainerApp.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ContainerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //ScopeDemo();
            //GenericDemo();
            //MultiServiceDemo();
            DisposalDemo();
        }

        private static void ScopeDemo()
        {
            using var root = new ServiceProvider();
            root.Register<IFoo, Foo>(Lifetime.Transient);
            root.Register<IBar>(_ => new Bar(), Lifetime.Scope);
            root.Register<IBaz, Baz>(Lifetime.Root);
            root.Register(Assembly.GetEntryAssembly());

            using var scope1 = root.CreateScope();
            using var scope2 = root.CreateScope();

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

        private static void GenericDemo()
        {
            using var root = new ServiceProvider();

            root.Register<IFoo, Foo>(Lifetime.Transient);
            root.Register<IBar, Bar>(Lifetime.Transient);
            root.Register(typeof(IFoobar<,>), typeof(Foobar<,>), Lifetime.Transient);
            using var foobar = (Foobar<IFoo, IBar>)root.GetService<IFoobar<IFoo, IBar>>();
            Debug.Assert(foobar.Foo is IFoo);
            Debug.Assert(foobar.Bar is IBar);
        }

        private static void MultiServiceDemo()
        {
            using var root = new ServiceProvider()
                .Register<Base, Foo>(Lifetime.Transient)
                .Register<Base, Bar>(Lifetime.Transient)
                .Register<Base, Baz>(Lifetime.Transient);

            var service = root.GetService<Base>();
            Debug.Assert(service is Baz);

            var services = root.GetServices<Base>();
            Debug.Assert(services.OfType<Foo>().Any());
            Debug.Assert(services.OfType<Bar>().Any());
            Debug.Assert(services.OfType<Baz>().Any());

        }

        private static void DisposalDemo()
        {
            using var root = new ServiceProvider()
                .Register<IFoo, Foo>(Lifetime.Transient)
                .Register<IBar>(_ => new Bar(), Lifetime.Scope)
                .Register<IBaz, Baz>(Lifetime.Root)
                .Register(Assembly.GetEntryAssembly());
            {
                using var scope = root.CreateScope();
                scope.GetService<IFoo>();
                scope.GetService<IBar>();
                scope.GetService<IBaz>();
                scope.GetService<IQux>();
                Console.WriteLine("Service scope disposed.");
            }
            Console.WriteLine("Root container disposed.");
        }

    }
}
