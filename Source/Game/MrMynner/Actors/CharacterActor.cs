using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Scripts;

namespace MrMynner.Actors
{
    public class CharacterActor : CharacterController
    {
        #region Vars

        [EditorDisplay("Character Actor")]
        [Serialize]
        public AnimatedModel animModel;

        [HideInEditor]
        [NoSerialize]
        public Vector3 moveVel = Vector3.Zero;

        [HideInEditor]
        [NoSerialize]
        public ActorUpdaterScript actUpdaterScript;

        [HideInEditor]
        [NoSerialize]
        public CharacterGravityScript charGravityScript;

        #endregion

        #region Props

        [EditorDisplay("Character Actor")]
        [NoSerialize]
        public float CharHeight
        {
            get => Height + Radius * 2f;

            set
            {
                Height = value - Radius * 2f;
            }
        }

        [EditorDisplay("Character Actor")]
        [NoSerialize]
        public float CharFat
        {
            get => Radius;

            set
            {
                float oldHeight = CharHeight;
                Radius = value;
                CharHeight = oldHeight;
            }
        }

        #endregion

        #region Funcs

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();
        
            actUpdaterScript = Helper.GetOrAddScriptToActor<ActorUpdaterScript>(this);
            actUpdaterScript.SetFuncs(null, null, FixedUpdateActor);    
            
            charGravityScript = Helper.GetOrAddScriptToActor<CharacterGravityScript>(this);
        }

        private void FixedUpdateActor()
        {
            Move(moveVel);
        }

        #endregion
    }
}
