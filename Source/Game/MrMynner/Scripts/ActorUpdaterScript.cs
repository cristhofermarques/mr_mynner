using System;
using System.Collections.Generic;
using FlaxEngine;

namespace MrMynner.Scripts
{
    public class ActorUpdaterScript : Script
    {
        #region Vars

        [NoSerialize]
        public Action startCall;

        [NoSerialize]
        public Action updateCall;

        [NoSerialize]
        public Action fixedUpdateCall;

        #endregion

        #region Funcs
    
        public override void OnStart()
        {
            if(startCall != null)
            {
                startCall();
            }
        }
    
        public override void OnUpdate()
        {  
            if(updateCall != null)
            {
                updateCall();
            }
        }

        public override void OnFixedUpdate()
        {
            if(fixedUpdateCall != null)
            {
                fixedUpdateCall();
            }
        }

        public void SetFuncs(Action start = null, Action update = null, Action fixedUpdate = null)
        {
            if(start != null){startCall = start;}
            if(update != null){updateCall = update;}
            if(fixedUpdate != null){fixedUpdateCall = fixedUpdate;}
        }

        #endregion
    }
}
