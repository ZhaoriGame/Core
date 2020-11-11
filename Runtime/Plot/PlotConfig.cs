// using System;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using UnityEngine;
// using System.Linq;
// using UnityEngine.Serialization;
//
// namespace IL.Game.Plot
// {
//     public class PlotConfig : ScriptableObject
//     {
//         private const string LEFT_VERTICAL_GROUP = "Split/Left";
//         private const string LEFT_GENERAL_SETTING = "Split/Left/General Settings";
//
//
//         [VerticalGroup(LEFT_VERTICAL_GROUP)] [LabelText("ID")]
//         public string ID;
//
//         [BoxGroup(LEFT_GENERAL_SETTING)] [LabelText("Plot")]
//         public List<PlotBase> PlotList;
//
//
//         [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
//         [BoxGroup("Split/Right/Description")]
//         [HideLabel, TextArea(4, 14)]
//         [Tooltip("描述")]
//         public string Description;
//
//         [VerticalGroup("Split/Right")]
//         [BoxGroup("Split/Right/Notes")]
//         [HideLabel, TextArea(4, 9)]
//         [Tooltip("笔记 （做一些记录）")]
//         public string Notes;
//     }
//
//
//     [Serializable]
//     public class PlotBase
//     {
//         private string GetSample;
//
//         //[SuffixLabel("$Sample")]
//         [Title("示例","$Sample")]
//          [LabelWidth(30)] [LabelText("类型")] [VerticalGroup()]
//         public PlotCommandType Type;
//
//         [LabelWidth(30)] [LabelText("数据")] [VerticalGroup()]
//         public string Plot;
//
//
//         public string Sample
//         {
//             get
//             {
//                 switch (Type)
//                 {
//                     case PlotCommandType.StartDialogNpcAndNpc:
//                         return "1101001(对话ID),1101000(NPC1),110111(NPC2)";
//                         break;
//                     case PlotCommandType.StartDialogPlayerAndNpc:
//                         return "1101001(对话ID),1101000(NPC)";
//                         break;
//                     case PlotCommandType.LoadNpc:
//                         return "1101000(NPC)";
//                         break;
//                     case PlotCommandType.HideNpc:
//                         return "1101000(NPC)";
//                         break;
//                     case PlotCommandType.SetNpcPos:
//                         return "1101000(NPC),12|23(位置)";
//                         break;
//                     case PlotCommandType.SetNpcTargetPos:
//                         return "1101000(NPC),12|23(位置)";
//                         break;
//                     case PlotCommandType.Delay:
//                         return "1(延时)";
//                         break;
//                     case PlotCommandType.SetPlayerPos:
//                         return "12|23(位置)";
//                         break;
//                     case PlotCommandType.SetPlayerTargetPos:
//                         return "12|23(位置)";
//                         break;
//                 }
//
//                 return "";
//             }
//         }
//       
//         // [ShowInInspector]
//        
//     }
// }