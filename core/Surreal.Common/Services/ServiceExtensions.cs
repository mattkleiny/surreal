using System;
using System.ComponentModel.Design;

namespace Surreal.Services {
  public static class ServiceExtensions {
    public static T? GetService<T>(this IServiceProvider provider) {
      return (T?) provider.GetService(typeof(T));
    }

    public static T GetRequiredService<T>(this IServiceProvider provider) {
      var service = provider.GetService<T>();

      if (service == null) {
        throw new Exception($"Unable to locate service {typeof(T).Name}");
      }

      return service;
    }

    public static bool TryGetService<T>(this IServiceProvider provider, out T result) {
      var service = provider.GetService(typeof(T));

      if (service != null) {
        result = (T) service;
        return true;
      }

      result = default!;
      return false;
    }

    public static void AddService<T>(this IServiceContainer services, T service, bool promote = false)
        where T : class {
      services.AddService(typeof(T), service, promote);
    }
  }
}