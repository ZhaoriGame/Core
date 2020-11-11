// using System.Collections.Generic;
//
// namespace IL.Game.Plot
// {
//     public class PlotTools
//     {
//         private List<PlotTick> plotTick = new List<PlotTick>();
//         private List<PlotTick> removePlotTick = new List<PlotTick>();
//         
//         
//         public void Update()
//         {
//             removePlotTick.Clear();
//             for (int i = 0; i < plotTick.Count; i++)
//             {
//                 if (!plotTick[i].IsComplete)
//                 {
//                     plotTick[i].OnUpdate();
//                     continue;
//                 }
//                 removePlotTick.Add(plotTick[i]);
//             }
//             for (int i = 0; i < removePlotTick.Count; i++)
//             {
//                 RemovePlotTick(removePlotTick[i]);
//             }
//         }
//         
//         
//         public void AddPlotTick(PlotTick tick)
//         {
//             plotTick.Add(tick);
//         }
//
//         public void RemovePlotTick(PlotTick tick)
//         {
//             if(plotTick.Contains(tick))
//                 plotTick.Remove(tick);
//         }
//         
//         /// <summary>
//         /// 延时执行某个方法
//         /// </summary>
//         /// <param name="time"></param>
//         /// <param name="OnComplete"></param>
//         public static void Delay(float time, System.Action OnComplete)
//         {
//             Timer.AddTimer(time).OnCompleted(OnComplete);
//         }
//         
//         public static Pos ParseVector3(string content)
//         {
//             string[] data = content.Split('|');
//             return new Pos(){X = int.Parse(data[0]), Y = int.Parse(data[1])};
//         }
//
//        
//     }
// }