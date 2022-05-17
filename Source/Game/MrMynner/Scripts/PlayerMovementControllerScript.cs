using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Actors;

namespace MrMynner.Scripts
{
    public class PlayerMovementControllerScript : Script
    {
        #region Vars

        [NoSerialize]
        private PlayerActor player;

        [Serialize]
        public float forwardSpeed = 250f;

        [Serialize]
        public float strafeSpeed = 150f;

        [Serialize]
        public float runSpeed = 450f;

        [Serialize]
        public string forwardAxisRawName = "Vertical";

        [Serialize]
        public string strafeAxisRawName = "Horizontal";

        [Serialize]
        public KeyboardKeys runKey = KeyboardKeys.Shift;

        #endregion


        #region Funcs

        public override void OnStart()
        {
            if(Actor is PlayerActor){player = Actor.As<PlayerActor>();}
        }
        
        public override void OnUpdate()
        {
            Vector3 targetMoveVel = Vector3.Zero;

            float forwardAxisRaw = Input.GetAxisRaw(strafeAxisRawName);
            bool isRunning = Input.GetKey(runKey) && forwardAxisRaw > 0f;
            float maxTargetMoveSpeed = 0f;

            if(isRunning)
            {
                maxTargetMoveSpeed = runSpeed;
                targetMoveVel += Input.GetAxisRaw(forwardAxisRawName) * Actor.Transform.Forward * runSpeed;
            }
            else
            {
                maxTargetMoveSpeed = forwardSpeed;
                targetMoveVel += Input.GetAxisRaw(forwardAxisRawName) * Actor.Transform.Forward * (forwardAxisRaw > 0f? forwardSpeed : strafeSpeed);
            }

            
            float strafeAxisRaw = Input.GetAxisRaw(strafeAxisRawName);
            targetMoveVel += strafeAxisRaw * Actor.Transform.Right * strafeSpeed;

            float moveVelLen = targetMoveVel.Length;
            if(moveVelLen > maxTargetMoveSpeed)
            {
                targetMoveVel *= maxTargetMoveSpeed / moveVelLen;
            }

            Debug.Log(targetMoveVel.Length);

            targetMoveVel *= Time.DeltaTime;

            player.moveVel = targetMoveVel;            
        }

        #endregion
    }
}
