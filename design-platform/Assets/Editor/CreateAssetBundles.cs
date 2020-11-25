using UnityEditor;
using System.IO;
using System.Windows.Forms;

public class CreateAssetBundles {
    [UnityEditor.MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles() {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(UnityEngine.Application.streamingAssetsPath)) {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                    BuildAssetBundleOptions.None,
                    EditorUserBuildSettings.activeBuildTarget);
    }
}