using System.Runtime.CompilerServices;
using FlaxEngine;
using MrMynner.Actors;

namespace MrMynner.Scripts
{
    public class PlayerMovementControllerScript : Script
    {
        #region Vars

        [NoSerialize]
        private PlayerActor player;

        [NoSerialize]
        private bool _isRunning = false;

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

        [Serialize][Limit(0f, 1f, 0.001f)]
        public float inertiaVelDampLerpIntensity = 0.01f;

        [Serialize][Limit(0f, 1f, 0.001f)]
        public float moveVelLerpIntensity = 0.7f;

        [Serialize]
        public KeyboardKeys runKey = KeyboardKeys.Shift;

        [HideInEditor][NoSerialize]
        public Vector3 inertialVel = Vector3.Zero;

        #endregion

        #region Funcs

        public override void OnStart()
        {
            if(Actor is PlayerActor){player = Actor.As<PlayerActor>();}
        }
        
        public override void OnUpdate()
        {
            if(player.IsGrounded)
            {   
                float epsOffset = 0.01f;
                Vector3 targetMoveVel = Vector3.Zero;

                float forwardAxisRaw = Input.GetAxisRaw(forwardAxisRawName);
                float strafeAxisRaw = Input.GetAxisRaw(strafeAxisRawName);

                bool isRunning = Input.GetKey(runKey) && forwardAxisRaw > 0f;
                float maxTargetMoveSpeed = strafeSpeed;

                if(forwardAxisRaw < -epsOffset)// backward move
                {
                    maxTargetMoveSpeed = strafeSpeed;
                    targetMoveVel += Actor.Transform.Backward * strafeSpeed;
                }
                else if(forwardAxisRaw > epsOffset)// forward move
                {
                    _isRunning = Input.GetKey(runKey);

                    if(_isRunning)
                    {
                        maxTargetMoveSpeed = runSpeed;
                        targetMoveVel += Actor.Transform.Forward * runSpeed;
                    }
                    else
                    {
                        maxTargetMoveSpeed = forwardSpeed;
                        targetMoveVel += Actor.Transform.Forward * forwardSpeed;
                    }
                }
                
                targetMoveVel += strafeAxisRaw * Actor.Transform.Right * strafeSpeed;

                float targetMoveVelLen =  targetMoveVel.Length;
                if(targetMoveVelLen > maxTargetMoveSpeed)
                {
                    targetMoveVel *= maxTargetMoveSpeed / targetMoveVelLen;
                }

                targetMoveVel *= Time.DeltaTime;
                targetMoveVel.Y = player.moveVel.Y;
                player.moveVel = inertialVel = Vector3.SmoothStep(player.moveVel, targetMoveVel, moveVelLerpIntensity);;
            }
            else
            {
                inertialVel.Y = player.moveVel.Y;
                player.moveVel = Vector3.SmoothStep(player.moveVel, inertialVel, moveVelLerpIntensity);
            }

            inertialVel = Vector3.Lerp(inertialVel, Vector3.Zero, inertiaVelDampLerpIntensity);
        }

        #endregion
    }
}
