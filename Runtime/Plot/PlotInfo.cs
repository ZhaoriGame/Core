// using System.Collections.Generic;
//
// namespace IL.Game.Plot
// {
//     public class PlotInfo
//     {
//         //public PlayLineCtrl PlayLineCtrl;
//
//         private List<PlotCommand> PlotCommands = new List<PlotCommand>();
//
//         public PlotInfo()
//         {
//         }
//
//         public PlayPlotCtrl PlayPlotCtrl;
//
//         public PlotInfo(PlayPlotCtrl playPlotCtrl)
//         {
//             PlayPlotCtrl = playPlotCtrl;
//         }
//
//         /// <summary>
//         /// 开始剧情
//         /// </summary>
//         public void Play()
//         {
//             //PlotManager.ClearPlotActor();
//             if (Count > 0)
//                 PlotCommands[0].DoAction();
//         }
//
//         /// <summary>
//         /// 停止剧情
//         /// </summary>
//         public void Stop()
//         {
//         }
//
//         /// <summary>
//         /// 关闭剧情
//         /// </summary>
//         public void Close()
//         {
//             foreach (var current in PlotCommands)
//             {
//                 current.OnPlotInfoComplete();
//             }
//         }
//
//         public void AddCommand(PlotCommand PlotCommand)
//         {
//             if (Count > 0)
//             {
//                 PlotCommand.LastCommand = PlotCommands[Count - 1];
//                 PlotCommands[Count - 1].NextCommand = PlotCommand;
//             }
//
//             PlotCommands.Add(PlotCommand);
//         }
//
//         private int Count => PlotCommands.Count;
//     }
// }