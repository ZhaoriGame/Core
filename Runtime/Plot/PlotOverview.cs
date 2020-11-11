// using Sirenix.OdinInspector;
// using UnityEngine.Serialization;
//
//
// namespace  IL.Game.Plot
// {
//     using Sirenix.Utilities;
//     using System.Linq;
//
// #if UNITY_EDITOR
//     using UnityEditor;
// #endif
//
//     [GlobalConfig("Resources/hot_res/data/plots", UseAsset = true)]
//     public class PlotOverview : GlobalConfig<PlotOverview> 
//     {
//         [ReadOnly]
//         [ListDrawerSettings(Expanded = true)]
//         public PlotConfig[] AllItems;
//
// #if UNITY_EDITOR
//         [Button(ButtonSizes.Medium), PropertyOrder(-1)]
//         public void UpdateOverview()
//         {
//             this.AllItems = AssetDatabase.FindAssets("t:PlotConfig")
//                 .Select(guid => AssetDatabase.LoadAssetAtPath<PlotConfig>(AssetDatabase.GUIDToAssetPath(guid)))
//                 .ToArray();
//         }
// #endif
//     }
// }