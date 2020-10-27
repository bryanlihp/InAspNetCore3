using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContainerApp.Container
{

    public class ServiceProvider : IServiceProvider, IDisposable
    {
        internal readonly ServiceProvider _root;
        internal readonly ConcurrentDictionary<Type, ServiceRegistry> _registries;
        internal readonly ConcurrentDictionary<ServiceKey, object> _services;
        private readonly ConcurrentBag<IDisposable> _disposables;
        private volatile bool _disposed;
        public ServiceProvider()
        {
            _root = this;
            _registries = new ConcurrentDictionary<Type, ServiceRegistry>();
            _services = new ConcurrentDictionary<ServiceKey, object>();
            _disposables = new ConcurrentBag<IDisposable>();
        }

        internal ServiceProvider(ServiceProvider parent)
        {
            _root = parent._root;
            _registries = _root._registries;
            _services = new ConcurrentDictionary<ServiceKey, object>();
            _disposables = new ConcurrentBag<IDisposable>();
        }

        private void EnsureNotDisposed()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException("ServiceProvider");
            }
        }

        public ServiceProvider Register(ServiceRegistry registry)
        {
            EnsureNotDisposed();
            if(_registries.TryGetValue(registry.ServiceType,out var existing))
            {
                _registries[registry.ServiceType] = registry;
                registry.Next = existing;
            }
            else
            {
                _registries[registry.ServiceType] = registry;
            }
            return this;
        }

        public ServiceProvider CreateScope() => new ServiceProvider(this);
        public object GetService(Type serviceType)
        {
            EnsureNotDisposed();
            // return this instance when requesting service provider
            if(serviceType==typeof(ServiceProvider)||serviceType== typeof(IServiceProvider))
            {
                return this;
            }
            ServiceRegistry registry;

            //IEnumerable<T>
            if(serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var elementType = serviceType.GetGenericArguments()[0];
                if(!_registries.TryGetValue(elementType,out registry))
                {
                    // return an empty array if not registered
                    return Array.CreateInstance(elementType, 0);
                }

                // return all registered services
                var registries = registry.AsEnumerable();
                var services = registries.Select(it => GetServiceCore(it, Type.EmptyTypes)).ToArray();
                Array array = Array.CreateInstance(elementType, services.Length);
                services.CopyTo(array, 0); 
                return array;
            }

            //Generic
            if(serviceType.IsGenericType && !_registries.ContainsKey(serviceType))
            {
                var definition = serviceType.GetGenericTypeDefinition();
                return _registries.TryGetValue(definition, out registry)
                    ? GetServiceCore(registry, serviceType.GetGenericArguments())
                    : null;
            }

            //Normal
            return _registries.TryGetValue(serviceType, out registry)
                    ? GetServiceCore(registry, new Type[0])
                    : null;
        }

        private object GetServiceCore(ServiceRegistry registry, Type[] genericArgumants)
        {

            var key = new ServiceKey(registry, genericArgumants);
            switch(registry.Lifetime)
            {
                case Lifetime.Root: return GetOrCreate(_root._services, _root._disposables);
                case Lifetime.Scope: return GetOrCreate(this._services, this._disposables);
                default:
                    {
                        var service = registry.Factory(this, genericArgumants);
                        if(service is IDisposable disposable && disposable != this)
                        {
                            _disposables.Add(disposable);
                        }
                        return service;
                    }
            }

            object GetOrCreate(ConcurrentDictionary<ServiceKey, object> services, ConcurrentBag<IDisposable> disposables)
            {
                if (services.TryGetValue(key, out var service))
                {
                    return service;
                }
                service = registry.Factory(this, genericArgumants);
                services[key] = service;
                if(service is IDisposable disposable)
                {
                    disposables.Add(disposable);
                }
                return service;
            }
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach(var disposable in _disposables)
                    {
                        disposable.Dispose();
                    }
                    _disposables.Clear();
                    _services.Clear();
                }
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ServiceProvider()
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
        #endregion IDisposable
    }
}
