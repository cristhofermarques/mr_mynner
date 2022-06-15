using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Actors;

namespace MrMynner.Scripts
{
    public class GrabberScript : Script
    {   
        #region Vars

        [NoSerialize]
        private AimScript aimScript = null;

        [Serialize]
        public KeyboardKeys grabKey = KeyboardKeys.E;

        [Serialize]
        public KeyboardKeys lockKey = KeyboardKeys.L;

        [Serialize]
        public KeyboardKeys rotateKey = KeyboardKeys.R;

        [Serialize]
        public KeyboardKeys rotateXKey = KeyboardKeys.X;

        [Serialize]
        public KeyboardKeys rotateZKey = KeyboardKeys.Z;

        [Serialize]
        public KeyboardKeys invertRotationKey = KeyboardKeys.Shift;

        [Serialize]
        public KeyboardKeys resetRotationKey = KeyboardKeys.Q;

        [Serialize]
        public MouseButton throwButton = MouseButton.Left;

        [Serialize]
        public float holdingHardwareRotationAngle = 60f;

        [Serialize]
        public float grabDistance = 200f;

        [Serialize]
        public float throwForce = 2000f;

        [Serialize]
        public uint grabMask = 1;

        [Serialize]
        public float holdScrollSens = 50f;

        [Serialize]
        public float minHoldDistance = 100f;

        [NoSerialize]
        private HardwareActor holdingHardware;

        [NoSerialize]
        private bool _isHolding = false;

        [NoSerialize]
        private float holdDistance = 100;

        [NoSerialize]
        private Vector3 boxExtents = Vector3.Zero;

        #endregion

        #region Funcs

        public override void OnStart()
        {
            aimScript = Actor.GetScript<AimScript>();
        }
    
        public override void OnUpdate()
        {
            UpdateLock();
            UpdateGrab();
            UpdateHold();
        }

        public override void OnFixedUpdate()
        {
            if(Input.GetMouseButtonDown(throwButton))
            {
                if(_isHolding)
                {
                    if(!CheckHoldingHardwareBox())// To Avoid hardware be dropped inside of some collider
                    {
                        HardwareActor hardware = holdingHardware;
                        StopHold();
                        hardware.AddForce(Actor.Transform.Forward * throwForce * hardware.Mass - hardware.Mass * Mathf.Pi, ForceMode.Impulse);
                    }
                }
            }
        }

        private void UpdateGrab()
        {
            if(Input.GetKeyDown(grabKey))
            {
                if(_isHolding)
                {
                    if(!CheckHoldingHardwareBox())// To Avoid hardware be dropped inside of some collider
                    {
                        StopHold();
                    }
                }
                else
                {
                    HardwareActor hardware = SendLookRay(grabDistance, grabMask);

                    if(hardware == null){return;}// No hardware at look ray

                    if(hardware.locked){aimScript.SetAimColorForMiliseconds(Color.Red, 700, 0.4f); return;}

                    SlotActor slot;

                    if(IsHardwareAttached(hardware, out slot))// Attached
                    {
                        slot.Deattach();
                        StartHold(hardware);
                    }
                    else // Deattached
                    {
                        StartHold(hardware);
                    }
                }
            }
        }

        private void UpdateLock()
        {
            if(Input.GetKeyDown(lockKey))
            {
                HardwareActor hardware = SendLookRay(grabDistance, grabMask);

                if(hardware == null){return;}// No hardware at look ray

                hardware.locked = !hardware.locked;
            }
        }

        private void UpdateHold()
        {
            if(_isHolding)
            {
                holdDistance = Mathf.Clamp(holdDistance + Input.MouseScrollDelta * Time.DeltaTime * 10f * holdScrollSens, minHoldDistance, grabDistance);

                float rotationMultClock = Input.GetKey(invertRotationKey)? -1f : 1f;

                if(Input.GetKey(resetRotationKey))
                {
                    holdingHardware.EulerAngles = Vector3.Zero;
                }

                if(Input.GetKey(rotateKey))
                {
                    Vector3 euler = holdingHardware.EulerAngles;
                    euler.Y += Time.DeltaTime * holdingHardwareRotationAngle * rotationMultClock;
                    holdingHardware.EulerAngles = euler;
                }

                if(Input.GetKey(rotateXKey))
                {
                    Vector3 euler = holdingHardware.EulerAngles;
                    euler.X += Time.DeltaTime * holdingHardwareRotationAngle * rotationMultClock;
                    euler.X = Mathf.Clamp(euler.X, -90f, 90f);
                    holdingHardware.EulerAngles = euler;
                }

                if(Input.GetKey(rotateZKey))
                {
                    Vector3 euler = holdingHardware.EulerAngles;
                    euler.Z += Time.DeltaTime * holdingHardwareRotationAngle * rotationMultClock;
                    holdingHardware.EulerAngles = euler;
                }


                holdingHardware.Position = GetHoldPosition();

                while(CheckHoldingHardwareBox(grabMask) && Vector3.Distance(holdingHardware.Position, Actor.Position) >= boxExtents.Length)
                {
                    Vector3 toCamDir = Vector3.Normalize(holdingHardware.Position - Actor.Position);
                    holdingHardware.Position -= toCamDir * Time.DeltaTime * 10f;
                }

                DebugDraw.DrawWireBox(holdingHardware.BoxWithChildren, Color.Red, 0f, false);
            }
        }

        private HardwareActor SendLookRay(float dist, uint mask)
        {
            RayCastHit hit;
            if(Physics.RayCast(Actor.Position, Actor.Transform.Forward, out hit, dist, mask, false))
            {
                if(hit.Collider.HasParent && hit.Collider.Parent is HardwareActor)
                {
                    HardwareActor hardware = hit.Collider.Parent.As<HardwareActor>();
                    if(CheckPlayerOverHardware(hardware)){return null;}

                    return hardware;
                }
            }

            return null;
        }

        private bool IsHardwareAttached(HardwareActor hardware, out SlotActor slot)
        {
            slot = null;
            if(hardware.HasParent && hardware.Parent is SlotActor)
            {
                slot = hardware.Parent as SlotActor;
                return true;
            }

            return false;
        }

        private void StartHold(HardwareActor hardware)
        {
            if(hardware == null){return;}

            if(_isHolding){StopHold();}

            holdingHardware = hardware;
            holdDistance = Vector3.Distance(Actor.Position, holdingHardware.Position);
            boxExtents = GetBoxExtents(holdingHardware);
            holdingHardware.SetHardwareToHoldMode();

            _isHolding = true;
        }

        private void StopHold()
        {
            if(_isHolding)
            {
                _isHolding = false;
                holdingHardware.SetHardwareToFreeMode();
                boxExtents = Vector3.Zero;
                holdingHardware = null;
            }
        }

        private Vector3 GetHoldPosition()
        {
            return Actor.Position + Actor.Transform.Forward * holdDistance;
        }

        private bool CheckHoldingHardwareBox(uint mask = uint.MaxValue)
        {
            return Physics.CheckBox(holdingHardware.Position, boxExtents, Quaternion.RotationMatrix(holdingHardware.Rotation), mask, false);
        }

        private Vector3 GetBoxExtents(Actor act)
        {
            Vector3 euler = act.EulerAngles;
            act.EulerAngles = Vector3.Zero;
            Vector3 extents = act.BoxWithChildren.Maximum - act.BoxWithChildren.Center;
            act.EulerAngles = euler;
            return extents;
        }

        private bool CheckPlayerOverHardware(HardwareActor hardware)
        {
            RayCastHit hit;
            if(Physics.BoxCast(hardware.Position, GetBoxExtents(hardware), Vector3.Up, out hit, Quaternion.RotationMatrix(hardware.Rotation),float.MaxValue, 2, false))
            {
                return hit.Collider is PlayerActor;
            }

            return false;
        }

        #endregion
    }
}
