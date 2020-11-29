using System;
using System.Collections.Generic;
using System.Text;
using DependencyInjection.Interfaces;

namespace DependencyInjection.Services
{
    public class Base: IDisposable
    {
        private bool _disposed = false;
        public Base()
        {
            Console.WriteLine($"An instance of {GetType().Name} is created.");
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                // Dispose of unmanaged resources
            }
            Console.WriteLine($"The instance of {GetType().Name} is disposed.");
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }


    public class Foo : Base, IFoo, IDisposable
    {
    }
    public class Bar : Base, IBar, IDisposable
    {
    }
    public class Baz : Base, IBaz, IDisposable
    {
    }
    public class FooBar<T1,T2> : Base, IFooBar<T1, T2>, IDisposable
    {
        public T1 Foo { get; }
        public T2 Bar { get; }

        public FooBar(T1 foo, T2 bar)
        {
            Foo = foo;
            Bar = bar;
        }
    }
}
