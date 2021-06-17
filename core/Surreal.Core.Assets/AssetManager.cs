using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Surreal.Data;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;

namespace Surreal.Assets {
  public sealed class AssetManager : IDisposable, IAssetManager {
    private static readonly ILog      Log      = LogFactory.GetLog<AssetManager>();
    private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<AssetManager>();

    private readonly Dictionary<string, object> assets = new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<Type, IAssetLoader> loaders;
    private readonly AssetManager?                  parent;

    public AssetManager()
        : this(Enumerable.Empty<IAssetLoader>()) {
    }

    public AssetManager(AssetManager parent)
        : this(parent.loaders.Values) {
      this.parent = parent;
    }

    public AssetManager(IEnumerable<IAssetLoader> loaders) {
      this.loaders = loaders.ToDictionary(loader => loader.AssetType);
    }

    public int  Count         => assets.Count;
    public Size EstimatedSize => assets.Values.OfType<IHasSizeEstimate>().Select(asset => asset.Size).Sum();

    public void RegisterLoader(IAssetLoader loader) {
      loaders[loader.AssetType] = loader;
    }

    Task<TAsset> IAssetResolver.GetAsync<TAsset>(Path path) => GetOrLoadAsync<TAsset>(path);

    public bool Contains<TAsset>(Path path)    => assets.ContainsKey(GetCacheKey(typeof(TAsset), path));
    public bool Contains(Type type, Path path) => assets.ContainsKey(GetCacheKey(type, path));

    public async Task<TAsset> GetOrLoadAsync<TAsset>(Path path) {
      return (TAsset) await GetOrLoadAsync(typeof(TAsset), path);
    }

    public async Task<object> GetOrLoadAsync(Type type, Path path) {
      using var _ = Profiler.Track($"Loading:{type.Name}");

      Log.Trace($"Loading {type.Name} from {path}");

      return await GetOrLoadInnerAsync(type, path, new AssetLoaderContext(this));
    }

    private async Task<object> GetOrLoadInnerAsync(Type type, Path path, AssetLoaderContext context) {
      if (!loaders.ContainsKey(type)) {
        throw new UnsupportedAssetException($"An unsupported asset type was requested: {type}");
      }

      // if the parent exists and contains the asset already, delegate to it
      if (parent?.Contains(type, path) == true) {
        return await parent.GetOrLoadAsync(type, path);
      }

      var cacheKey = GetCacheKey(type, path);

      if (!assets.TryGetValue(cacheKey, out var result)) {
        var loader = loaders[type];
        var asset  = await loader.LoadAssetAsync(path, context);

        assets[cacheKey] = result = asset;
      }

      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetCacheKey(Type type, Path path) {
      return $"{type.Name}:{path}";
    }

    public void Dispose() {
      foreach (var asset in assets.Values) {
        if (asset is IDisposable disposable) {
          disposable.Dispose();
        }
      }

      foreach (var loader in loaders.Values) {
        if (loader is IDisposable disposable) {
          disposable.Dispose();
        }
      }

      assets.Clear();
    }

    public Dictionary<string, object>.ValueCollection.Enumerator GetEnumerator() => assets.Values.GetEnumerator();
    IEnumerator<object> IEnumerable<object>.                     GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                                     GetEnumerator() => GetEnumerator();

    private sealed class AssetLoaderContext : IAssetLoaderContext {
      private readonly AssetManager manager;

      public AssetLoaderContext(AssetManager manager) {
        this.manager = manager;
      }

      public async Task<TAsset> GetAsync<TAsset>(Path path) {
        return (TAsset) await manager.GetOrLoadInnerAsync(typeof(TAsset), path, this);
      }
    }
  }
}