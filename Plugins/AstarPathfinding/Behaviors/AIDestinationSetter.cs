using System;
using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    enum TargetPosType
    {
        Transform,
        Position
    }

    /// <summary>
    /// Sets the destination of an AI to the position of a specified object.
    /// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
    /// This component will then make the AI move towards the <see cref="target"/> set on this component.
    ///
    /// See: <see cref="Pathfinding.IAstarAI.destination"/>
    ///
    /// [Open online documentation to see images]
    /// </summary>
    [UniqueComponent(tag = "ai.destination")]
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
    public class AIDestinationSetter : VersionedMonoBehaviour
    {
        /// <summary>The object that the AI should move to</summary>
        public Transform target;

        private Vector3 position;

        private IAstarAI ai;

        private TargetPosType targetPosType;
        

        private bool IsArrive => ai.reachedDestination;

        private bool IsFind;

        private Action OnArrive;
        
        private float TargetDistance()
        {
            if (targetPosType == TargetPosType.Position)
            {
                return Vector3.Distance(
                    position, transform.position);
            }
            else
            {
                return Vector3.Distance(
                    target.position, transform.position);
            }
          
        }

        

        public void SetTarget(Transform target,Action arrive= null)
        {
            IsFind = true;
            targetPosType = TargetPosType.Transform;
            this.target = target;
            this.OnArrive = arrive;
        }

        public void SetPosition(Vector3 position,Action arrive = null)
        {
            IsFind = true;
            targetPosType = TargetPosType.Position;
            this.position = position;
            this.OnArrive = arrive;
        }

        public void SetFindEnable(bool value)
        {
            this.enabled = value;
        }

        public void SetStop(bool value)
        {
            ai.isStopped = value;
        }

        void OnEnable()
        {
            ai = GetComponent<IAstarAI>();
            // Update the destination right before searching for a path as well.
            // This is enough in theory, but this script will also update the destination every
            // frame as the destination is used for debugging and may be used for other things by other
            // scripts as well. So it makes sense that it is up to date every frame.
            if (ai != null) ai.onSearchPath += Update;
        }

        void OnDisable()
        {
            if (ai != null) ai.onSearchPath -= Update;
        }

        /// <summary>Updates the AI's destination every frame</summary>
        void Update()
        {
            if (ai != null)
            {
                if (IsFind)
                {
                    if (IsArrive)
                    {
                        IsFind = false;
                        OnArrive?.Invoke();
                        Debug.LogError("到达");
                    }
                }
                
                if (targetPosType == TargetPosType.Position)
                {
                    ai.destination = position;
                }
                else
                {
                    if (target!=null)
                    {
                        ai.destination = target.position;
                    }
                    
                }
            }

          
        }
    }
}