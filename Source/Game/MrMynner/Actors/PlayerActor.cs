using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Scripts;

namespace MrMynner.Actors
{
    public class PlayerActor : CharacterActor
    {
        #region Vars

        [EditorDisplay("Player Actor")]
        [NoSerialize]
        public Camera cam;

        #endregion

        #region Props

        [EditorDisplay("Player Actor")]
        [NoSerialize]
        public float Fov
        {
            get
            {
                if(cam != null){return cam.FieldOfView;}
                else{return -1f;}
            }

            set
            {
                if(cam != null){cam.FieldOfView = value;}
            }

        }

        #endregion

        #region Funcs

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();

            if(cam == null)
                cam = Scene.GetOrAddChild<Camera>();

            Helper.GetOrAddScriptToActor<MrMynner.Scripts.FirstCamControllerScript>(cam);

            Helper.GetOrAddScriptToActor<MrMynner.Scripts.PlayerMovementControllerScript>(this);
            Helper.GetOrAddScriptToActor<MrMynner.Scripts.PlayerJumpScript>(this);
            Helper.GetOrAddScriptToActor<MrMynner.Scripts.PlayerCamLinkScript>(this).OrderInParent = ScriptsCount - 1;
        }


        #endregion   
    }
}
