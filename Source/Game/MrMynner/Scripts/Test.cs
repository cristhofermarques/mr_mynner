using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Scripts;
using MrMynner.Actors;

namespace Game
{
    public class Test : Script
    {
        public CableActor cable;

        public bool isHolding = false;
        public int idxToMove = 0;
        float dist = 0f;

        public float lerpIntensity = 0.2f;

        public override void OnUpdate()
        {
            if(Input.GetMouseButtonDown(MouseButton.Left))
            {
                if(isHolding)
                {
                    isHolding = false;
                    cable = null;
                }
                else
                {
                    RayCastHit hit;
                    if(Physics.SphereCast(Actor.Position, 1f, Actor.Direction, out hit, float.MaxValue, uint.MaxValue, true))
                    {

                        if(hit.Collider.HasParent && hit.Collider.Parent is CableActor)
                        {
                            isHolding = true;
                            cable = hit.Collider.Parent.As<CableActor>();
                            bool isNext;
                            dist = hit.Distance;
                            float time = cable.GetSplineTimeClosestToPoint(hit.Point);
                            idxToMove = cable.GetSplineIndexClosestToTime(time, out isNext);
#if BUILD_DEBUG
#else
                            Debug.Log(idxToMove);
#endif
                        }
                    }
                }
            }


            if(isHolding)
            {
                cable.MovePoint(idxToMove, Actor.Position + Actor.Direction * dist);
            }
        }
    }
}
