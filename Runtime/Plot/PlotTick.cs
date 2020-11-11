using System;
using UnityEngine;

namespace IL.Game
{
    public abstract class PlotTick
    {
        public Action OnComplete;
        public bool IsComplete { get; protected set; }
        public abstract void OnUpdate();
    }
    
    
    public class MoveTick : PlotTick
    {
        public float moveSpeed;
        public Vector3 moveEndPos;
        public Transform moveActor;

        public override void OnUpdate()
        {
            moveActor.localPosition = Vector3.MoveTowards(moveActor.localPosition, moveEndPos, moveSpeed * Time.deltaTime);
            if (moveActor.localPosition == moveEndPos)
            {
                IsComplete = true;
                OnComplete?.Invoke();
            }
        }
    }
}