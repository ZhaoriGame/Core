// using System;
// using System.Collections;
// using UnityEngine;
//
// namespace IL.Game.Plot
// {
//     public class PlotFactory
//     {
//         private Hashtable lookUpType = new Hashtable();
//
//         public void Init()
//         {
//             lookUpType[PlotCommandType.StartDialogNpcAndNpc] = new StartDialogNpcAndNpc();
//             lookUpType[PlotCommandType.StartDialogPlayerAndNpc] = new StartDialogPlayerAndNpc();
//             lookUpType[PlotCommandType.LoadNpc] = new LoadNpc();
//             lookUpType[PlotCommandType.HideNpc] = new HideNpc();
//             lookUpType[PlotCommandType.SetNpcPos] = new SetNpcPos();
//             lookUpType[PlotCommandType.SetNpcTargetPos] = new SetNPCTargetPos();
//             lookUpType[PlotCommandType.Delay] = new Delay();
//             lookUpType[PlotCommandType.SetPlayerPos] = new SetPlayerPos();
//             lookUpType[PlotCommandType.SetPlayerTargetPos] = new SetPlayerTargetPos();
//         }
//
//         public PlotCommand Create(PlotCommandType plotType)
//         {
//             PlotCommand c = (PlotCommand)lookUpType[plotType];
//             
//
//
//             if (c == null)
//             {
//                 Debug.Log($"创建剧情命令{plotType}，失败！！");
//             }
//
//             return c;
//         }
//     }
//
//     public enum PlotCommandType
//     {
//         StartDialogNpcAndNpc,
//         StartDialogPlayerAndNpc,
//         LoadNpc,
//         HideNpc,
//         SetNpcPos,
//         SetNpcTargetPos,
//         Delay,
//         SetPlayerPos,
//         SetPlayerTargetPos
//     }
// }