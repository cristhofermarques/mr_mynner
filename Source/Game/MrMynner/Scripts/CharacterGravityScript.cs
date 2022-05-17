using FlaxEngine;
using MrMynner.Actors;

namespace MrMynner.Scripts
{
    public class CharacterGravityScript : Script
    {
        #region Vars

        [NoSerialize]
        private CharacterActor chr = null;

        [EditorOrder(0)]
        [Serialize]
        public float gravity = 9.8f;

        [EditorOrder(1)]
        [Limit(0f, 1f)]
        [Serialize]
        public Vector3 axis = Vector3.Up;
        
        #endregion

        #region Funcs

        public override void OnStart()
        {
            if(Actor is CharacterActor){chr = Actor.As<CharacterActor>();}
        }
        
        public override void OnFixedUpdate()
        {
            if(chr == null){return;}

            if(chr.IsGrounded){chr.moveVel.Y = 0f;}
            else{chr.moveVel.Y -= gravity * Mathf.Pi * Time.DeltaTime;}
        }

        #endregion
    }
}
