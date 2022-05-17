using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Actors;

namespace MrMynner.Scripts
{
    public class PlayerCamLinkScript : Script
    {
        #region Vars

        [NoSerialize]
        private PlayerActor player = null;

        [Serialize]
        [Limit(0f, 1f, 0.01f)]
        public float forwardLerpIntensity = 0.8f;

        [Serialize]
        [Limit(0f, 1f, 0.01f)]
        public float camYPosLerpIntensity = 1f;

        [Serialize]
        public bool lerpYCamPos = true;

        [Serialize]
        public float camYPosOffset = -7f;

        #endregion

        #region Funcs

        public override void OnStart()
        {
            if(Actor is PlayerActor)
            {
                player = Actor.As<PlayerActor>();
            }
        }
        
        public override void OnFixedUpdate()
        {
            if(player == null || player.cam == null){return;}

            Vector3 playerEuler = player.EulerAngles;
            playerEuler.Y = Mathf.LerpAngle(playerEuler.Y, player.cam.EulerAngles.Y, forwardLerpIntensity);
            player.EulerAngles = playerEuler;

            Vector3 targetCamPos = player.Position;
            float targetYCamPos = player.Position.Y + player.Height * 0.5f + player.Radius + camYPosOffset;
            targetCamPos.Y = lerpYCamPos? Mathf.Lerp(player.cam.Position.Y, targetYCamPos, camYPosLerpIntensity) : targetYCamPos;
            player.cam.Position = targetCamPos;
        }
    
        #endregion
    }
}
