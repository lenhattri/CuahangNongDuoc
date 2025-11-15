using System;
using System.Collections.Concurrent;

namespace CuahangNongduoc.Utils
{
    /// <summary>
    /// Very small service locator used to emulate dependency injection in the
    /// WinForms application without introducing a heavy dependency.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly ConcurrentDictionary<Type, Lazy<object>> _registrations =
            new ConcurrentDictionary<Type, Lazy<object>>();

        public static void Register<TService>(Func<TService> factory) where TService : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var lazy = new Lazy<object>(() => factory());
            _registrations[typeof(TService)] = lazy;
        }

        public static TService Resolve<TService>() where TService : class
        {
            if (_registrations.TryGetValue(typeof(TService), out var lazy))
            {
                return (TService)lazy.Value;
            }

            throw new InvalidOperationException($"Service {typeof(TService).FullName} has not been registered.");
        }
    }
}
