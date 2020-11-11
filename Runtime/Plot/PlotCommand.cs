// using UnityEngine;
//
// namespace IL.Game.Plot
// {
//     public class PlotCommand : IPlotCommand
//     {
//         public PlotInfo PlotInfo { get; set; }
//
//         public PlotCommand NextCommand { get; set; }
//
//         public PlotCommand LastCommand { get; set; }
//
//         /// <summary>
//         /// 解析
//         /// </summary>
//         /// <param name="plotInfo"></param>
//         /// <param name="command"></param>
//         /// <returns></returns>
//         public bool OnParse(PlotInfo plotInfo, string command)
//         {
//             PlotInfo = plotInfo;
//             return Parse(command);
//         }
//
//         /// <summary>
//         /// 解析
//         /// </summary>
//         /// <param name="command"></param>
//         /// <returns></returns>
//         public virtual bool Parse(string command)
//         {
//             return true;
//         }
//
//         /// <summary>
//         /// 执行
//         /// </summary>
//         /// <returns></returns>
//         public virtual bool DoAction()
//         {
//             return true;
//         }
//
//         /// <summary>
//         /// 剧情结束后的恢复
//         /// </summary>
//         public virtual void OnPlotInfoComplete()
//         {
//         }
//
//
//         /// <summary>
//         /// 命令结束
//         /// </summary>
//         public void OnCommandComplete()
//         {
//             if (NextCommand != null)
//                 NextCommand.DoAction();
//             else
//                 PlotInfo.Close();
//         }
//     }
//
//     /// <summary>
//     /// 设置NPC和NPC进行对话
//     /// </summary>
//     public class StartDialogNpcAndNpc : PlotCommand
//     {
//         private string id;
//         private NPC Npc1;
//         private NPC Npc2;
//
//         public override bool Parse(string command)
//         {
//             string[] param = command.Split(',');
//             id = param[0];
//             Npc1 = PlotInfo.PlayPlotCtrl.GetNPC(param[1]);
//             Npc2 = PlotInfo.PlayPlotCtrl.GetNPC(param[2]);
//
//             return true;
//         }
//
//         public override bool DoAction()
//         {
//             PlotInfo.PlayPlotCtrl.SetDialog(id, Npc1, Npc2, OnCommandComplete);
//             return false;
//         }
//
//         public override void OnPlotInfoComplete()
//         {
//         }
//     }
//
//
//     /// <summary>
//     /// 设置NPC和玩家对话
//     /// </summary>
//     public class StartDialogPlayerAndNpc : PlotCommand
//     {
//         private string id;
//         private NPC Npc;
//
//         public override bool Parse(string command)
//         {
//             string[] param = command.Split(',');
//             id = param[0];
//             Npc = PlotInfo.PlayPlotCtrl.GetNPC(param[1]);
//             return true;
//         }
//
//         public override bool DoAction()
//         {
//             PlotInfo.PlayPlotCtrl.SetDialog(id, Npc, OnCommandComplete);
//             return false;
//         }
//
//         public override void OnPlotInfoComplete()
//         {
//         }
//     }
//
//     /// <summary>
//     /// 创建角色 例如 10001,24|23
//     /// </summary>
//     public class LoadNpc : PlotCommand
//     {
//         private string NPCID;
//
//         private Pos Pos;
//
//         public override bool DoAction()
//         {
//             PlotInfo.PlayPlotCtrl.LoadNPC(NPCID);
//             OnCommandComplete();
//             return true;
//         }
//
//         public override void OnPlotInfoComplete()
//         {
//         }
//
//         public override bool Parse(string command)
//         {
//             string[] param = command.Split(',');
//             NPCID = param[0];
//             string posStr = param[1];
//             string[] param1 = posStr.Split('|');
//             Pos = new Pos() {X = int.Parse(param1[0]), Y = int.Parse(param1[1])};
//
//
//             return true;
//         }
//     }
//
//     public class HideNpc : PlotCommand
//     {
//         private string NpcId;
//
//
//         public override bool DoAction()
//         {
//             PlotInfo.PlayPlotCtrl.HideNPC(NpcId);
//             OnCommandComplete();
//             return true;
//         }
//
//         public override void OnPlotInfoComplete()
//         {
//         }
//
//         public override bool Parse(string command)
//         {
//             NpcId = command;
//             return true;
//         }
//     }
//
//     /// <summary>
//     /// 设置NPC位置
//     /// </summary>
//     public class SetNpcPos : PlotCommand
//     {
//         private NPC Npc;
//
//         private Pos Pos;
//
//         public override bool Parse(string command)
//         {
//             string[] param = command.Split(',');
//             Npc = GameMgr.Ins.NpcCtrl.GetNPC(param[0]);
//             string posStr = param[1];
//             string[] param1 = posStr.Split('|');
//             Pos = new Pos() {X = int.Parse(param1[0]), Y = int.Parse(param1[1])};
//             return true;
//         }
//
//         public override bool DoAction()
//         {
//             Npc.NpcToPause();
//             PlotInfo.PlayPlotCtrl.SetNPCPos(Npc, Pos);
//             OnCommandComplete();
//             return true;
//         }
//
//         public override void OnPlotInfoComplete()
//         {
//             Npc.NpcContinue();
//         }
//     }
//
//
//     /// <summary>
//     /// 设置NPC目标位置
//     /// </summary>
//     public class SetNPCTargetPos : PlotCommand
//     {
//         private NPC Npc;
//
//         private Pos Pos;
//
//
//         public override bool Parse(string command)
//         {
//             string[] param = command.Split(',');
//             Npc = GameMgr.Ins.NpcCtrl.GetNPC(param[0]);
//             string posStr = param[1];
//             string[] param1 = posStr.Split('|');
//             Pos = new Pos() {X = int.Parse(param1[0]), Y = int.Parse(param1[1])};
//             return true;
//         }
//
//         public override bool DoAction()
//         {
//             PlotInfo.PlayPlotCtrl.SetNPCTargetPos(Npc, Pos, OnCommandComplete);
//             return true;
//         }
//     }
//
//     //延时
//     public class Delay : PlotCommand
//     {
//         private float DelayTime;
//
//         public override bool DoAction()
//         {
//             Debug.Log($"延时：{DelayTime}");
//             PlotTools.Delay(DelayTime, OnCommandComplete);
//             return false;
//         }
//
//         public override void OnPlotInfoComplete()
//         {
//         }
//
//         public override bool Parse(string command)
//         {
//             DelayTime = float.Parse(command);
//             return true;
//         }
//     }
//
//     public class SetPlayerPos : PlotCommand
//     {
//         private Pos Pos;
//
//         public override bool Parse(string command)
//         {
//             string[] param1 = command.Split('|');
//             Pos = new Pos() {X = int.Parse(param1[0]), Y = int.Parse(param1[1])};
//             return true;
//         }
//
//         public override bool DoAction()
//         {
//             PlotInfo.PlayPlotCtrl.SetPlayerPos(Pos);
//             OnCommandComplete();
//             return true;
//         }
//     }
//
//     public class SetPlayerTargetPos : PlotCommand
//     {
//         private Pos Pos;
//
//         public override bool Parse(string command)
//         {
//             string[] param1 = command.Split('|');
//             Pos = new Pos() {X = int.Parse(param1[0]), Y = int.Parse(param1[1])};
//             return true;
//         }
//
//         public override bool DoAction()
//         {
//             PlotInfo.PlayPlotCtrl.SetPlayerTargetPos(Pos, OnCommandComplete);
//             return true;
//         }
//     }
//
//     /// <summary>
//     /// 接取任务
//     /// </summary>
//     public class AcceptTask : PlotCommand
//     {
//         private string taskId;
//
//         private int taskState;
//         //任务状态 0 接取 1 完成
//         
//         
//         public override bool Parse(string command)
//         {
//             string[] param = command.Split(',');
//             taskId = param[0];
//             taskState = int.Parse(param[1]);
//             return true;
//         }
//
//         public override bool DoAction()
//         {
//             return base.DoAction();
//             //GameMgr.Ins.TaskCtrl.BeforeAcceptTask();
//         }
//     }
//
//     /// <summary>
//     /// 设置角色方向 主角或者NPC
//     /// </summary>
//     public class SetRoleDir : PlotCommand
//     {
//         private int role;
//         
//         private int dir;
//
//         private string npcId;
//         
//         //0/1 为主角和NPC 1234 分别为前后左右 
//         //朝右NPC：1，1，110111
//         //朝左主角 0，-1
//         
//         public override bool Parse(string command)
//         {
//             string[] param = command.Split(',');
//             role = int.Parse(param[0]);
//             dir = int.Parse(param[1]);
//             if (role == 1)
//             {
//                 npcId = param[2];
//             }
//
//             return true;
//         }
//
//         public override bool DoAction()
//         {
//             if (role == 0)
//             {
//                 
//                 GameMgr.Ins.PlayerCtrl.SetBScale();
//             }
//
//             if (role == 1)
//             {
//                 
//             }
//
//             return true;
//         }
//         
//     }
//     
//     /// <summary>
//     /// 设置相机位置
//     /// </summary>
//     public class SetCameraPos : PlotCommand
//     {
//         private int Pos;
//         
//         public override bool Parse(string command)
//         {
//             return base.Parse(command);
//         }
//
//         public override bool DoAction()
//         {
//             return base.DoAction();
//         }
//     }
//
//     /// <summary>
//     /// 设置相机目标 主角或者NPC
//     /// </summary>
//     public class SetCameraTarget : PlotCommand
//     {
//         private bool IsPlayer;
//
//
//         public override bool Parse(string command)
//         {
//             return base.Parse(command);
//         }
//
//         public override bool DoAction()
//         {
//             return base.DoAction();
//         }
//     }
// }