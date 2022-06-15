using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Scripts;
using MrMynner.Actors;

namespace Game
{
    /// <summary>
    /// Test Script.
    /// </summary>
    public class Test : Script
    {
        public CableActor spline;
        Vector3 targetPos;

        public float lerpIntensity = 0.2f;

        public override void OnUpdate()
        {
            if(spline == null){return;}
            targetPos = Vector3.Lerp(targetPos, Actor.Position, lerpIntensity);

            if(spline.SplinePointsCount > 2)
            {
                
                spline.MovePoint(1, targetPos);
            }            

        }


    }
}
