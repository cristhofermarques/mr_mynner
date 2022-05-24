using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Actors;

namespace MrMynner.Scripts
{
    public class GrabberScript : Script
    {   
        #region Vars

        AimScript aimScript = null;

        #endregion

        #region Funcs

        public override void OnStart()
        {
            aimScript = Actor.GetScript<AimScript>();
        }
        
        public override void OnEnable()
        {
            // Here you can add code that needs to be called when script is enabled (eg. register for events)
        }

        public override void OnDisable()
        {
            // Here you can add code that needs to be called when script is disabled (eg. unregister from events)
        }

        public override void OnUpdate()
        {
            if(Input.GetKeyDown(KeyboardKeys.E))
            {
                RayCastHit hit;
                if(Physics.RayCast(Actor.Position, Actor.Transform.Forward, out hit))
                {
                    if(hit.Collider.HasParent && hit.Collider.Parent is HardwareActor)
                    {
                        HardwareActor compAct = hit.Collider.Parent.As<HardwareActor>();
                        if(compAct.locked)
                        {
                            aimScript.SetAimColorForMiliseconds(Color.Red, 1000, 0.4f);
                        }
                    }
                }
            }

            if(Input.GetKeyDown(KeyboardKeys.L))
            {
                RayCastHit hit;
                if(Physics.RayCast(Actor.Position, Actor.Transform.Forward, out hit))
                {
                    if(hit.Collider.HasParent && hit.Collider.Parent is HardwareActor)
                    {
                        HardwareActor compAct = hit.Collider.Parent.As<HardwareActor>();
                        compAct.locked = !compAct.locked;
                    }
                }
            }
        }

        #endregion
    }
}
