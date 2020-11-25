using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Utils {


    public static class AssetUtil {

        private static Dictionary<string, AssetBundle> LoadedBundles = new Dictionary<string, AssetBundle>();

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