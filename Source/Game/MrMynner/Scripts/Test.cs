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
        public float dist = 0f;
        public float safeSubDistOffset = 10f;

        public int mask = int.MaxValue;

        public float lerpIntensity = 0.2f;

        public override void OnUpdate()
        {
            if(Input.GetMouseButtonDown(MouseButton.Right))
            {
                RayCastHit hit;

                if(Physics.SphereCast(Actor.Position, 1f, Actor.Direction, out hit, float.MaxValue, 4U, false))
                {

                    Debug.Log(hit.Collider.Name);

                    if(hit.Collider.HasParent && hit.Collider.Parent is CableActor)
                    {
                        isHolding = true;
                        cable = hit.Collider.Parent.As<CableActor>();
                        int pointIdx = 0;
                        cable.AddNearPoint(hit.Point, out pointIdx);
                    }
                }
            }


            if(Input.GetMouseButtonDown(MouseButton.Left))
            {
                Debug.Log(new LayersMask(mask).Mask);

                if(isHolding)
                {
                    isHolding = false;
                    cable.SmoothSpline();
                    cable.collider.IsActive = true;
                    cable.UpdateSpline();
                    cable = null;
                }
                else
                {

                    RayCastHit hit;
                    if(Physics.SphereCast(Actor.Position, 1f, Actor.Direction, out hit, float.MaxValue, 4U, false))
                    {

                        Debug.Log(hit.Collider.Name);

                        if(hit.Collider.HasParent && hit.Collider.Parent is CableActor)
                        {
                            isHolding = true;
                            cable = hit.Collider.Parent.As<CableActor>();
                            
                            dist = hit.Distance;
                            float time = cable.GetSplineTimeClosestToPoint(hit.Point);
                            idxToMove = cable.GetSplineIndexPointClosestToTime(time);
                            dist = Vector3.Distance(cable.GetSplinePoint(time), Actor.Position) - safeSubDistOffset;
                            cable.collider.IsActive = false;

                            Debug.Log(time + " " + idxToMove);
                        }
                    }
                }
            }


            if(isHolding)
            {
                cable.MovePoint(idxToMove, Actor.Position + Actor.Direction * dist);
                cable.SmoothSpline();
                cable.UpdateSpline();
            }
        }
    }
}
