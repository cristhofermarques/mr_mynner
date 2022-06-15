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
        Vector3 lastPos;

        public override void OnStart()
        {
            lastPos = Actor.Position;

        }

        public override void OnUpdate()
        {
            if(spline == null){return;}

            if(spline.SplinePointsCount > 2)
            {
                spline.MovePoint(1, Actor.Position);
            }            

            lastPos = Actor.Position;
        }


    }
}
