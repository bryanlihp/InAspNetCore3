using ContainerApp.Attributes;
using ContainerApp.Container;
using ContainerApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerApp.Services
{
    public class Base : IDisposable
    {
        private bool _disposedValue;

        public Base() => Console.WriteLine($"Instance of {GetType().Name} is created.");

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            Console.WriteLine($"Instance of {GetType().Name} is disposed.");
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Base()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }


    public class Foo : Base, IFoo
    {

    }

    public class Bar : Base, IBar
    {

    }

    public class Baz : Base, IBaz
    {

    }

    [MapTo(typeof(IQux),Lifetime.Root)]
    public class Qux :Base, IQux
    {

    }
    public class Foobar<T1, T2> : Base, IFoobar<T1, T2>
    {
        public T1 Foo { get; }
        public T2 Bar { get; }

        public Foobar(T1 foo, T2 bar)
        {
            Foo = foo;
            Bar = bar;
        }
    }
}
