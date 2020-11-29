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
            TestN();
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
                .AddTransient<IBar,Bar>()
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
            Debug.Assert(p1.GetHashCode()!=provider.GetHashCode());
            Console.WriteLine();
        }
        static void Test5()
        {
            Console.WriteLine("Test5 - IFoo, IBar, IBaz lifetime");
            var root = new ServiceCollection()
                .AddTransient<IFoo, Foo>()
                .AddScoped<IBar>(_=>new Bar())
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

        static void TestN()
        {
            Console.WriteLine("TestX - IFoo, IBar and IFooBar");

            Console.WriteLine();
        }

    }
}
