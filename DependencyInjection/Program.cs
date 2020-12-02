using System;
using System.Diagnostics;
using System.Linq;
using DependencyInjection.Interfaces;
using DependencyInjection.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            Test2();
            Test3();
            Test4();
            Test5();
            Test6();
            Test7();
            Test8();
            Test9();
            //TestN();
        }

        static void Test1()
        {
            Console.WriteLine("Test1 - IFoo, IBar and IBaz");
            var provider = new ServiceCollection()
                .AddTransient<IFoo, Foo>()
                .AddTransient<IBar>(_ => new Bar())
                .AddSingleton<IBaz, Baz>()
                .AddTransient(typeof(IFooBar<,>), typeof(FooBar<,>))
                .BuildServiceProvider();
            Debug.Assert(provider.GetService<IFoo>() is Foo);
            Debug.Assert(provider.GetService<IBar>() is Bar);
            Debug.Assert(provider.GetService<IBaz>() is Baz);
            Console.WriteLine("");
        }

        static void Test2()
        {
            Console.WriteLine("Test2 - IFoo, IBar and IFooBar");
            var provider = new ServiceCollection()
                .AddTransient<IFoo, Foo>()
                .AddTransient<IBar, Bar>()
                .AddTransient(typeof(IFooBar<,>), typeof(FooBar<,>))
                .BuildServiceProvider();

            var instance = provider.GetService<IFooBar<IFoo, IBar>>();
            Debug.Assert(instance is FooBar<IFoo, IBar>);
            var foobar = instance as FooBar<IFoo, IBar>;
            Debug.Assert(foobar.Foo is Foo);
            Debug.Assert(foobar.Bar is Bar);
            Console.WriteLine("");
        }

        static void Test3()
        {
            Console.WriteLine("Test3 - Base as Foo, Bar and Baz");
            var provider = new ServiceCollection()
                .AddTransient<Base, Foo>()
                .AddTransient<Base, Bar>()
                .AddTransient<Base, Baz>()
                .BuildServiceProvider();

            var services = provider.GetServices<Base>();

            Debug.Assert(services.OfType<Foo>().Any());
            Debug.Assert(services.OfType<Bar>().Any());
            Debug.Assert(services.OfType<Baz>().Any());

            Console.WriteLine();
        }

        static void Test4()
        {
            Console.WriteLine("Test4 - IServiceProvider");
            var provider = new ServiceCollection().BuildServiceProvider();
            var p1 = provider.GetService<IServiceProvider>();
            Debug.Assert(p1.GetHashCode() != provider.GetHashCode());
            Console.WriteLine();
        }
        static void Test5()
        {
            Console.WriteLine("Test5 - IFoo, IBar, IBaz lifetime");
            var root = new ServiceCollection()
                .AddTransient<IFoo, Foo>()
                .AddScoped<IBar>(_ => new Bar())
                .AddSingleton<IBaz, Baz>()
                .BuildServiceProvider();

            var provider1 = root.CreateScope().ServiceProvider;
            var provider2 = root.CreateScope().ServiceProvider;

            GetService<IFoo>(provider1); // two instantiations for the transient instances
            GetService<IBar>(provider1); // one instantiation for provider1 scope
            GetService<IBaz>(provider1); // one instantiation for the singleton (int root)

            Console.WriteLine();

            GetService<IFoo>(provider2); // two instantiations for the transient instances
            GetService<IBar>(provider2); // one instantiation for provider2 scope
            GetService<IBaz>(provider2); // on instantiation, use the singleton created in root

            Console.WriteLine();
        }

        static void Test6()
        {
            Console.WriteLine("Test5 - IFoo, IBar, IBaz lifetime and disposal");
            using (var root = new ServiceCollection()
                .AddTransient<IFoo, Foo>()
                .AddScoped<IBar>(_ => new Bar())
                .AddSingleton<IBaz, Baz>()
                .BuildServiceProvider())
            {
                using (var scope1 = root.CreateScope())
                {
                    var provider1 = scope1.ServiceProvider;
                    GetService<IFoo>(provider1); // two instantiations for the transient instances
                    GetService<IBar>(provider1); // one instantiation for provider1 scope
                    GetService<IBaz>(provider1); // one instantiation for the singleton (int root)
                }

                Console.WriteLine("out of scope1");
                Console.WriteLine();
                using (var scope2 = root.CreateScope())
                {
                    var provider2 = scope2.ServiceProvider;
                    GetService<IFoo>(provider2); // two instantiations for the transient instances
                    GetService<IBar>(provider2); // one instantiation for provider2 scope
                    GetService<IBaz>(provider2); // on instantiation, use the singleton created in root
                }

                Console.WriteLine("out of scope2");
            }
            Console.WriteLine("out of root scope");
            Console.WriteLine();
        }

        static void GetService<T>(IServiceProvider provider)
        {
            provider.GetService<T>();
            provider.GetService<T>();
        }

        static void Test7()
        {
            Console.WriteLine("Test7 - Lifetime and scope");
            Console.WriteLine();
            using (var root = new ServiceCollection()
                .AddSingleton<IQux, Qux>()
                .AddScoped<IBar, Bar>()
                .BuildServiceProvider(false)
            )
            {
                T ResolveService<T>(IServiceProvider provider)
                {
                    T svc = default(T);
                    var isRootContainer = root == provider ? "Yes" : "No";
                    try
                    {
                        svc = provider.GetService<T>();
                        Console.WriteLine($"Status: Success; Service Type: {typeof(T).Name}; Root:{isRootContainer}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Status: Failed; Service Type: {typeof(T).Name}; Root:{isRootContainer}");
                        Console.WriteLine($"Error: {e.GetBaseException().Message}");
                    }
                    return svc;
                }
                Console.WriteLine("Root scope");
                var qux0 = ResolveService<IQux>(root); // instantiate  Qux as singleton and Bar( referenced by Qux)
                Console.WriteLine("IQux instance acquired.");
                var bar = ResolveService<IBar>(root);  // using the same instance as the one that is referenced by Qux 
                var bar0 = ResolveService<IBar>(root); // using the same instance as the one that is referenced by Qux 
                Debug.Assert(bar.GetHashCode() == bar0.GetHashCode());
                Console.WriteLine("IBar instance acquired.");

                using (var scope = root.CreateScope())
                {
                    Console.WriteLine();
                    Console.WriteLine("Child scope");
                    var child = scope.ServiceProvider;

                    var qux1 = ResolveService<IQux>(child);
                    var bar1 = ResolveService<IBar>(child);
                    Debug.Assert(qux0.GetHashCode() == qux1.GetHashCode());
                    Debug.Assert(bar0.GetHashCode() != bar1.GetHashCode());
                    Console.WriteLine("out of child scope");
                }
                Console.WriteLine();
            }
            Console.WriteLine("out of root scope");
            Console.WriteLine();
        }

        static void Test8()
        {
            Console.WriteLine("Test8 - IServiceProvider with scope verification");
            var root = new ServiceCollection()
                .AddSingleton<IQux, Qux>()
                .AddScoped<IBar, Bar>()
                .BuildServiceProvider(true);

            var child = root.CreateScope().ServiceProvider;

            void ResolveService<T>(IServiceProvider provider)
            {
                var IsRootContainer = root == provider ? "Yes" : "No";
                try
                {
                    provider.GetService<T>();
                    Console.WriteLine($"Status: Success; Service Type: {typeof(T).Name}; Root:{IsRootContainer}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Status: Failed; Service Type: {typeof(T).Name}; Root:{IsRootContainer}");
                    Console.WriteLine($"Error: {e.GetBaseException().Message}");
                }
            }

            ResolveService<IQux>(root);
            ResolveService<IBar>(root);
            ResolveService<IQux>(child);
            ResolveService<IBar>(child); // only this one can succeed

            Console.WriteLine();
        }
        static void Test9()
        {
            Console.WriteLine("Test9 - ServiceProviderOptions");
            BuildServiceProvider(false);
            BuildServiceProvider(true); // will fail, service provider cannot create service

            void BuildServiceProvider(bool validateOnBuild)
            {
                try
                {
                    var options = new ServiceProviderOptions
                    {
                        ValidateOnBuild = validateOnBuild,
                    };
                    new ServiceCollection()
                        .AddSingleton<IBaz, BazEx>()
                        .BuildServiceProvider(options);
                    Console.WriteLine($"Status: Success; ValidateOnBuild = {validateOnBuild}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Status: Failed; ValidateOnBuild = {validateOnBuild}");
                    Console.WriteLine($"Error: {e.GetBaseException().Message}");
                }
            }
            Console.WriteLine();
        }
    }
}
