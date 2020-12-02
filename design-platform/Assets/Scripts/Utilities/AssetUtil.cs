using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Utils {

    /// <summary>
    /// Helper functions for loading unity assets.
    /// </summary>
    public static class AssetUtil {

        /// <summary>List of loaded asset bundles to make sure each bundle is only loaded once.</summary>
        private static Dictionary<string, AssetBundle> LoadedBundles = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// Load an asset of a given type from a specific asset bundle.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load, eg. GameObject.</typeparam>
        /// <param name="bundleName">Name of the asset bundle wherein the asset is located.</param>
        /// <param name="assetName">Name of the asset to load.</param>
        /// <returns>The asset that were loaded.</returns>
        public static T LoadAsset<T>(string bundleName, string assetName) {

            AssetBundle assetBundle = LoadBundle(bundleName);

            // Load asset from bundle
            Object asset = assetBundle.LoadAsset<Object>(assetName);

            // Check if asset was loaded successfully
            if (asset == null) {
                Debug.LogError("Failed to load" + assetName + "from" + bundleName + "assetbundle");
                return default;
            }

            try { return (T)System.Convert.ChangeType(asset, typeof(T)); }
            catch { return default; }
        }

        /// <summary>
        /// Load an asset bundle into memory.
        /// </summary>
        /// <param name="bundleName">Name of the asset bundle to load.</param>
        /// <returns>The asset bundle that were loaded.</returns>
        public static AssetBundle LoadBundle(string bundleName) {

            // Load asset bundle if it has not already been loaded
            AssetBundle assetBundle;
            if (LoadedBundles.Keys.Contains(bundleName)) {
                assetBundle = LoadedBundles[bundleName];
            }
            else {
                assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
                LoadedBundles.Add(bundleName, assetBundle);
            }

            // Check if asset bundle was loaded successfully
            if (assetBundle == null) {
                Debug.LogError("Failed to load" + bundleName + "assetbundle");
                return null;
            }

            return assetBundle;

        }
    }
}