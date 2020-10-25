//example
//Timer.Register(10, () => { Debug.LogError("完成"); }, (f => Debug.LogError("进度:" + f)));
//更新 https://github.com/akbiggs/UnityTimer

using UnityEngine;
using System;


namespace Core.Timer {
public class Timer
{

    /// <summary>
    /// 计时器持续时间
    /// </summary>
    public float Duration { get; private set; }

    /// <summary>
    /// 是否循环
    /// </summary>
    public bool IsLooped { get; set; }

    /// <summary>
    /// 计时是否结束，当被取消是也为False
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// 是否使用真实时间或者游戏时间，真实时间不受TimeScale影响，游戏时间受TimeScale影响
    /// </summary>
    public bool UsesRealTime { get; private set; }

    /// <summary>
    /// 是否暂停
    /// </summary>
    public bool IsPaused => timeElapsedBeforePause.HasValue;

    /// <summary>
    /// 是否被取消
    /// </summary>
    public bool IsCancelled => timeElapsedBeforeCancel.HasValue;

    /// <summary>
    /// 是否完成（完成/取消/销毁）
    /// </summary>
    public bool IsDone => IsCompleted || IsCancelled || IsOwnerDestroyed;


   

    /// <summary>
    /// 注册一个计时器
    /// </summary>
    public static Timer Register(float duration, Action onComplete, Action<float> onUpdate = null,
        bool isLooped = false, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
    {
        Debug.LogError(1);
        if (manager == null)
        {
            Debug.LogError(2);
            //确保场景里TimerManager唯一
            TimerManager managerInScene = UnityEngine.Object.FindObjectOfType<TimerManager>();
            if (managerInScene != null)
            {
                manager = managerInScene;
            }
            else
            {
                Debug.LogError(3);
                GameObject managerObject = new GameObject { name = "TimerManager" };
                manager = managerObject.AddComponent<TimerManager>();
            }
        }

        Timer timer = new Timer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
        manager.RegisterTimer(timer);
        return timer;
    }

    /// <summary>
    /// 取消一个计时器
    /// </summary>
    /// <param name="timer"></param>
    public static void Cancel(Timer timer)
    {
        timer?.Cancel();
    }

    /// <summary>
    /// 暂停一个计时器
    /// </summary>
    /// <param name="timer"></param>
    public static void Pause(Timer timer)
    {
        timer?.Pause();
    }

    /// <summary>
    /// 继续一个计时器
    /// </summary>
    /// <param name="timer"></param>
    public static void Resume(Timer timer)
    {
        timer?.Resume();
    }

    /// <summary>
    /// 取消所有注册计时器
    /// </summary>
    public static void CancelAllRegisteredTimers()
    {
        if (manager != null)
        {
            manager.CancelAllTimers();
        }
    }

    /// <summary>
    /// 暂停所有计时器
    /// </summary>
    public static void PauseAllRegisteredTimers()
    {
        if (manager != null)
        {
            manager.PauseAllTimers();
        }
    }

    /// <summary>
    /// 继续所有计时器
    /// </summary>
    public static void ResumeAllRegisteredTimers()
    {
        if (Timer.manager != null)
        {
            Timer.manager.ResumeAllTimers();
        }
    }

   
 

    /// <summary>
    /// 取消一个计时器，完成时的任务不会触发
    /// </summary>
    public void Cancel()
    {
        if (IsDone)
        {
            return;
        }
        timeElapsedBeforeCancel = GetTimeElapsed();
        timeElapsedBeforePause = null;
    }

    /// <summary>
    /// 暂停计时器
    /// </summary>
    public void Pause()
    {
        if (IsPaused || IsDone)
        {
            return;
        }

        timeElapsedBeforePause = GetTimeElapsed();
    }

    /// <summary>
    /// 继续计时器
    /// </summary>
    public void Resume()
    {
        if (!IsPaused || IsDone)
        {
            return;
        }
        timeElapsedBeforePause = null;
    }

    /// <summary>
    /// 获得时间消逝的秒数 在当轮计时器
    /// </summary>
    public float GetTimeElapsed()
    {
        if (IsCompleted || GetWorldTime() >= GetFireTime())
        {
            return Duration;
        }
        //这里如果取消 或者 暂停 或者还在进行
        return timeElapsedBeforeCancel ??
               timeElapsedBeforePause ??
               GetWorldTime() - startTime;
    }

    /// <summary>
    /// 获得剩余秒数
    /// </summary>
    /// <returns></returns>
    public float GetTimeRemaining()
    {
        return Duration - GetTimeElapsed();
    }

    /// <summary>
    /// 获得完成比例 （0~1）
    /// </summary>
    public float GetRatioComplete()
    {
        return GetTimeElapsed() / Duration;
    }

    /// <summary>
    /// 获得剩余完成比例（0~1）
    /// </summary>
    public float GetRatioRemaining()
    {
        return GetTimeRemaining() / Duration;
    }

    

    /// <summary>
    /// 负责Update所有的Timer
    /// </summary>
    private static TimerManager manager;

    

    /// <summary>
    /// 是否被销毁
    /// </summary>
    private bool IsOwnerDestroyed => _hasAutoDestroyOwner && _autoDestroyOwner == null;

    private readonly Action onComplete;
    private readonly Action<float> onUpdate;
    private float startTime;
    private float lastUpdateTime;

   //缓存暂停和取消之前的时间
    private float? timeElapsedBeforeCancel;
    private float? timeElapsedBeforePause;

    //当owner物体销魂时也跟着销毁
    private readonly MonoBehaviour _autoDestroyOwner;
    private readonly bool _hasAutoDestroyOwner;

   

    

    private Timer(float duration, Action onComplete, Action<float> onUpdate,
        bool isLooped, bool usesRealTime, MonoBehaviour autoDestroyOwner)
    {
        this.Duration = duration;
        this.onComplete = onComplete;
        this.onUpdate = onUpdate;

        this.IsLooped = isLooped;
        this.UsesRealTime = usesRealTime;

        this._autoDestroyOwner = autoDestroyOwner;
        this._hasAutoDestroyOwner = autoDestroyOwner != null;

        this.startTime = this.GetWorldTime();
        this.lastUpdateTime = this.startTime;
    }

   

   
    /// <summary>
    /// 获得世界时间
    /// </summary>
    /// <returns></returns>
    private float GetWorldTime()
    {
        return UsesRealTime ? Time.realtimeSinceStartup : Time.time;
    }

    /// <summary>
    /// 获得目标时间
    /// </summary>
    /// <returns></returns>
    private float GetFireTime()
    {
        return startTime + Duration;
    }

    private float GetTimeDelta()
    {
        return GetWorldTime() - lastUpdateTime;
    }

    public void Update()
    {
        if (IsDone)
        {
            return;
        }

        //暂停时
        if (IsPaused)
        {
            startTime += GetTimeDelta();
            lastUpdateTime = GetWorldTime();
            return;
        }

        lastUpdateTime = GetWorldTime();

        onUpdate?.Invoke(GetTimeElapsed());

        if (GetWorldTime() >= GetFireTime())
        {
            onComplete?.Invoke();

            if (IsLooped)
            {
                startTime = GetWorldTime();
            }
            else
            {
                IsCompleted = true;
            }
        }
    }
}
}
