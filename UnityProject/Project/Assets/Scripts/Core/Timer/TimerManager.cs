using System.Collections.Generic;
using UnityEngine;

namespace Core.Timer
{
    /// <summary>
    /// 管理时间
    /// 当创建第一个Timer会被创建
    /// </summary>
    public class TimerManager : MonoBehaviour
    {
        private List<Timer> timers = new List<Timer>();

        // 缓冲计时器，能在迭代时编辑
        private List<Timer> timersToAdd = new List<Timer>();

        public void RegisterTimer(Timer timer)
        {
            this.timersToAdd.Add(timer);
        }

        /// <summary>
        /// 取消所有计时器
        /// </summary>
        public void CancelAllTimers()
        {
            foreach (Timer timer in timers)
            {
                timer.Cancel();
            }

            timers.Clear();
            timersToAdd.Clear();
        }

        /// <summary>
        /// 暂停所有计时器
        /// </summary>
        public void PauseAllTimers()
        {
            foreach (Timer timer in timers)
            {
                timer.Pause();
            }
        }

        /// <summary>
        /// 继续所有的计时器
        /// </summary>
        public void ResumeAllTimers()
        {
            foreach (Timer timer in timers)
            {
                timer.Resume();
            }
        }

   
        private void Update()
        {
            UpdateAllTimers();
        }

        private void UpdateAllTimers()
        {
            if (timersToAdd.Count > 0)
            {
                timers.AddRange(timersToAdd);
                timersToAdd.Clear();
            }

            foreach (Timer timer in timers)
            {
                timer.Update();
            }

            timers.RemoveAll(t => t.IsDone);
        }
    }
}