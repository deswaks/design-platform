using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace DesignPlatform.Utils {


    public static class AssetUtil {

        private static Dictionary<string, AssetBundle> LoadedBundles = new Dictionary<string, AssetBundle>();

        public static Material LoadMaterial(string bundleName, string materialName) {

            AssetBundle assetBundle = LoadBundle(bundleName);

            // Load asset from bundle
            Material asset = assetBundle.LoadAsset<Material>(materialName);

            // Check if asset was loaded successfully
            if (asset == null) {
                Debug.LogError("Failed to load" + materialName + "from" + bundleName + "assetbundle");
                return null;
            }

            return asset;
        }

        public static GameObject LoadGameObject(string bundleName, string assetName) {

            AssetBundle assetBundle = LoadBundle(bundleName);

            // Load asset from bundle
            GameObject asset = assetBundle.LoadAsset<GameObject>(assetName);

            // Check if asset was loaded successfully
            if (asset == null) {
                Debug.LogError("Failed to load" + assetName + "from" + bundleName + "assetbundle");
                return null;
            }

            return asset;
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