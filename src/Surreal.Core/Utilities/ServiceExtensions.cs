using System;
using System.ComponentModel.Design;

namespace Surreal.Utilities {
  public static class ServiceExtensions {
    public static TService? GetService<TService>(this IServiceProvider services)
        where TService : class {
      return services.GetService(typeof(TService)) as TService;
    }

    public static TService GetRequiredService<TService>(this IServiceProvider services)
        where TService : class {
      var service = services.GetService<TService>();

      if (service == null) {
        throw new Exception($"Unable to locate service of type: {typeof(TService)}");
      }

      return service;
    }

    public static void AddService<TService>(this IServiceContainer container, TService instance)
        where TService : class {
      container.AddService(typeof(TService), instance);
    }

    public static void AddService<TService>(this IServiceContainer container, Func<TService> factory)
        where TService : class {
      container.AddService(typeof(TService), (_, type) => factory());
    }

    public static void AddService<TService, TInstance>(this IServiceContainer container, TInstance instance)
        where TService : class
        where TInstance : class, TService {
      container.AddService(typeof(TService), instance);
    }

    public static void AddService<TService, TInstance>(this IServiceContainer container, Func<TInstance> factory)
        where TService : class
        where TInstance : class, TService {
      container.AddService(typeof(TService), (_, type) => factory());
    }
  }
}