using System;
using System.Collections.Generic;

/* 示例代码
public class TestFSM
{
    public enum ET
    {
        A,
        B,
        C,
    }

    FiniteStateMachine<ET> fsm = new FiniteStateMachine<ET>();
    
    public TestFSM()
    {
        fsm.RegistState(ET.A, OnEnter, OnExit, OnUpdate, SwitchEnable);
        fsm.RegistState(ET.B, OnEnter, OnExit, OnUpdate, SwitchEnable);
        fsm.SwitchState(ET.A);
        fsm.Update();
        fsm.SwitchState(ET.B);
        fsm.Update();
        fsm.SwitchState(ET.C);
        fsm.Update();
    }

    public void OnEnter(ET from)
    {

    }

    public void OnExit(ET to)
    {

    }

    public void OnUpdate()
    {
        Debug.Log("state: " + fsm.curState);
    }

    public bool SwitchEnable(ET toState)
    {
        return true;
    }
}
*/

namespace Core.FSM
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class FiniteStateMachine<T>
    {
        /// <summary>
        /// 状态控制器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class StateController<T>
        {
            /// <summary>
            /// 进入状态的委托
            /// </summary>
            public Action<T, object> OnEnter;

            /// <summary>
            /// 退出状态的委托
            /// </summary>
            public Action<T> OnExit;

            /// <summary>
            /// 更新状态的委托
            /// </summary>
            public Action<T> OnUpdate;

            /// <summary>
            /// 切换状态检查的委托
            /// </summary>
            public Func<T, bool> CheckSwitchEnable;

            /// <summary>
            /// 状态
            /// </summary>
            public T State;

            /// <summary>
            /// 配置的能切换到的状态，null表示不限制
            /// </summary>
            public HashSet<T> RoleSwitch = null;

            public StateController(T state)
            {
                this.State = state;
            }
        }

        /// <summary>
        /// 在当前状态下经过的时间(根据Update传入的dt值累计）
        /// </summary>
        public float StateStayTime { get; private set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public T CurState { get; private set; }

        /// <summary>
        /// 状态字典
        /// </summary>
        Dictionary<T, StateController<T>> stateDic = new Dictionary<T, StateController<T>>();

        /// <summary>
        /// 状态机名称
        /// </summary>
        public string Name { get; private set; }

        public FiniteStateMachine(string name = null)
        {
            this.Name = name;
        }

        /// <summary>
        /// 注册一个状态，不适用的方法可以传递Null
        /// </summary>
        public void RegistState(T state, Action<T, object> onEnter = null, Action<T> onExit = null,
            Action<T> onUpdate = null, Func<T, bool> checkSwitchEnable = null)
        {
            StateController<T> sc = new StateController<T>(state);
            sc.OnEnter = onEnter;
            sc.OnExit = onExit;
            sc.OnUpdate = onUpdate;
            sc.CheckSwitchEnable = checkSwitchEnable;

            if (null == CurState)
            {
                //设置为第一个状态
                CurState = state;
            }

            stateDic[state] = sc;
        }

        /// <summary>
        /// 注销一个状态
        /// </summary>
        public void UnregistState(T state)
        {
            if (stateDic.ContainsKey(state))
            {
                stateDic.Remove(state);
            }

            if (CurState.Equals(state))
            {
                CurState = default(T);
            }
        }

        /// <summary>
        /// 添加一个合法的状态转换规则
        /// </summary>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        public void AddSwitchRole(T fromState, T toState)
        {
            if (false == stateDic.ContainsKey(fromState))
            {
                return;
            }


            if (null == stateDic[fromState].RoleSwitch)
            {
                stateDic[fromState].RoleSwitch = new HashSet<T>();
            }

            stateDic[fromState].RoleSwitch.Add(toState);
        }

        /// <summary>
        /// 移除一个合法的状态转换规则
        /// </summary>
        /// <param name="fromState"></param>
        public void RemoveSwitchRole(T fromState, T toState)
        {
            if (false == stateDic.ContainsKey(fromState) || null == stateDic[fromState].RoleSwitch)
            {
                return;
            }

            stateDic[fromState].RoleSwitch.Remove(toState);
        }

        /// <summary>
        /// 进入一个状态
        /// </summary>
        public bool SwitchState(T toState, object data = null)
        {
            if (false == stateDic.ContainsKey(toState))
            {
                return false;
            }

            var oldSc = stateDic[CurState];

            if (oldSc.RoleSwitch != null && !oldSc.RoleSwitch.Contains(toState))
            {
                return false;
            }

            var newSc = stateDic[toState];

            if (oldSc.CheckSwitchEnable != null && !oldSc.CheckSwitchEnable.Invoke(toState))
            {
                return false;
            }

            oldSc.OnExit?.Invoke(toState);

            CurState = toState;
            StateStayTime = 0;
            newSc.OnEnter?.Invoke(oldSc.State, data);

            return true;
        }

        /// <summary>
        /// 状态更新
        /// </summary>
        /// <param name="dt">距离上次状态更新的间隔，如果传入，可以统计状态持续的时间</param>
        public void Update(float dt = 0f)
        {
            StateStayTime += dt;
            var nowSc = stateDic[CurState];
            nowSc.OnUpdate?.Invoke(CurState);
        }
    }
}