// using System;
// using Core;
// using UnityEngine;
//
// namespace IL.Game.Plot
// {
//     public enum PlotType
//     {
//         Talk,
//         Move,
//         Animation,
//     }
//
//     public class PlayPlotCtrl 
//     {
//         public int Priority { get; set; }
//
//         private PlotFactory PlotFactory;
//
//         /// <summary>
//         /// 解析剧情
//         /// </summary>
//         /// <param name="command"></param>
//         /// <returns></returns>
//         public PlotInfo ParsePlot(string command)
//         {
//             command = command.Replace("\r", "");
//             string[] commandAry = command.Split('\n');
//             PlotInfo pi = new PlotInfo(this);
//             for (int i = 0; i < commandAry.Length; i++)
//             {
//                 if (commandAry[i].Equals("")) continue;
//                 string[] commandStruct = commandAry[i].Split(':');
//                 PlotCommand pc = PlotFactory.Create((PlotCommandType)Enum.Parse(typeof(PlotCommandType),commandStruct[0]));
//                 if (pc != null)
//                 {
//                     pc.OnParse(pi, commandStruct[1]);
//                     pi.AddCommand(pc);
//                 }
//                 else
//                     Debug.Log($"创建剧情命令{commandStruct[0]}，失败！！");
//             }
//
//             return pi;
//         }
//
//         public PlotInfo ParsePlot(PlotConfig plotConfig)
//         {
//             PlotInfo plotInfo = new PlotInfo(this);
//
//             foreach (var current in plotConfig.PlotList)
//             {
//                 PlotCommand pc = PlotFactory.Create(current.Type);
//                 if (pc != null)
//                 {
//                     pc.OnParse(plotInfo, current.Plot);
//                     plotInfo.AddCommand(pc);
//                 }
//                 else
//                 {
//                     Debug.Log($"创建剧情命令{current.Type.ToString()}，失败！！");
//                 }
//             }
//
//             return plotInfo;
//         }
//
//         public void PlayPlot(string PlotID)
//         {
//             //PlotInfo plotInfo = ParsePlot();
//             plotInfo.Play();
//         }
//         
//         public void Init()
//         {
//             ApplicationKit.Ins.onUpdate += Update;
//             PlotFactory = new PlotFactory();
//             PlotFactory.Init();
//         }
//         
//         private void Update()
//         {
//             if (Input.GetKeyDown(KeyCode.K))
//             {
//                 PlayPlot("1111");
//                 Debug.LogError("开始剧情");
//             }
//         }
//
//         
//
//         /// <summary>
//         /// 解析剧情
//         /// </summary>
//         /// <param name="PlotId"></param>
//         /// <returns></returns>
//         public static PlotInfo ParsePlot(int PlotId)
//         {
//             return null;
//         }
//
//
//         public NPC LoadNPC(string ID)
//         {
//             NPC npc = GameMgr.Ins.NpcCtrl.GetNPC(ID);
//             npc.GameObject.SetActive(true);
//             return npc;
//         }
//
//         public NPC HideNPC(string ID)
//         {
//             NPC npc = GameMgr.Ins.NpcCtrl.GetNPC(ID);
//             npc.GameObject.SetActive(false);
//             return npc;
//         }
//
//         public NPC GetNPC(string ID)
//         {
//             return GameMgr.Ins.NpcCtrl.GetNPC(ID);
//         }
//
//
//   
//
//         public void SetDialog(string id, NPC npc, Action OnCommandComplete)
//         {
//             UIMgr.Ins.Open<DialogPanel>().StartDialog(id, npc.transform, OnCommandComplete);
//         }
//
//         public void SetDialog(string id, NPC npc1, NPC npc2, Action OnCommandComplete)
//         {
//             UIMgr.Ins.Open<DialogPanel>().StartDialog(id, npc1.transform, npc2.transform, OnCommandComplete);
//         }
//         
//         public void SetNPCPos(NPC npc, Pos pos)
//         {
//             npc.SetPosition(pos);
//             Debug.LogError("设置NPC位置");
//         }
//
//         public void SetNPCTargetPos(NPC npc, Pos pos, Action OnCommandComplete)
//         {
//             npc.SetTargetPos(pos, OnCommandComplete);
//             Debug.LogError("设置NPC目标位置");
//         }
//
//         public void SetPlayerTargetPos(Pos pos, Action onComplete)
//         {
//             Debug.Log("设置主角目标位置");
//             GameMgr.Ins.PlayerCtrl.SetTarget(GameUtils.GetPosition(pos), onComplete);
//         }
//
//         public void SetPlayerPos(Pos pos)
//         {
//             Debug.Log("设置主角位置");
//             GameMgr.Ins.PlayerCtrl.SetPosition(GameUtils.GetPosition(pos));
//         }
//
//
//       
//
//         public void Load()
//         {
//             
//         }
//
//         public void Save()
//         {
//         }
//
//       
//
//       
//     }
// }