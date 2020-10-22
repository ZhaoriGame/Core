using System;
using UnityEngine;


namespace Core.Timer
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class TimerExtensions
    {
        /// <summary>
        /// 添加一个Timer到Behaviour,如果被销毁了，Callback将不会执行
        /// </summary>
        public static Timer AttachTimer(this MonoBehaviour behaviour, float duration, Action onComplete,
            Action<float> onUpdate = null, bool isLooped = false, bool useRealTime = false)
        {
            return Timer.Register(duration, onComplete, onUpdate, isLooped, useRealTime, behaviour);
        }
    }
}