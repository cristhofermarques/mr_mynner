using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Actors;

namespace MrMynner.Scripts
{
    public class PlayerJumpScript : Script
    {
        #region Vars

        [NoSerialize]
        private PlayerActor player = null;

        [NoSerialize]
        private bool _isJumping = false;

        [Serialize]
        public KeyboardKeys jumpKey = KeyboardKeys.Spacebar;

        [Serialize]
        public float jumpHeight = 60f;

        public float test = 10f;

        #endregion

        #region Funcs

        public override void OnStart()
        {
            if(Actor is PlayerActor){player = Actor.As<PlayerActor>();}
        }
        
        public override void OnUpdate()
        {
            if(player == null){return;}
            if(player.IsGrounded)
            {
                if(_isJumping)
                {
                    _isJumping = false;
                }
                else if(Input.GetKey(jumpKey))
                {
                    _isJumping = true;
                    player.moveVel.Y += player.charGravityScript.gravity * Mathf.Sqrt(jumpHeight) * 0.1f;
                }
            }
        }

        #endregion
    }
}
