using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Scripts;

namespace MrMynner.Actors
{
    public class HardwareActor : RigidBody
    {
        #region Vars

        [NoSerialize]
        private bool _broken = false;

        [HideInEditor][Serialize]
        public bool _isAttachable = false;

        [EditorDisplay("Component Actor")][NoSerialize]
        public bool locked = false;

        [EditorDisplay("Component Actor")][Serialize]
        private HardwareType _type;

        [EditorDisplay("Component Actor")][Serialize]
        public HardwareConfig config;

        [EditorDisplay("Component Actor")][Serialize]
        public ParticleEmitter brokeParticleEffect;

        #endregion

        #region Props
        
        [EditorDisplay("Component Actor")][NoSerialize]
        public bool Broken
        {
            get => _broken;

            set
            {
                if(value != _broken)
                {
                    _broken = value;
                    if(value)
                    {
                        brokeParticleEffect.Spawn(Position);
                    }
                }
            }
        }

        [EditorDisplay("Component Actor")][NoSerialize]
        public HardwareType Type
        {
            get => _type;
            set
            {
                if(value != _type)
                {
                    _type = value;
                    UpdateHardwareConfig(value);
                }
            }
        }

        [EditorDisplay("Component Actor")][Serialize]
        public bool IsAttachable
        {
            get => _isAttachable;

            set
            {
                if(_isAttachable != value)
                {
                    _isAttachable = value;
                    if(value)
                    {
                        Helper.GetOrAddScriptToActor<MrMynner.Scripts.AttachableHardwareScript>(this);
                    }
                    else
                    {
                        MrMynner.Scripts.AttachableHardwareScript attachable;
                        if(TryGetScript<MrMynner.Scripts.AttachableHardwareScript>(out attachable))
                        {
                            Destroy<MrMynner.Scripts.AttachableHardwareScript>(ref attachable);
                            attachable = null;
                        }
                    }
                }
            }
        }

        [EditorDisplay("Component Actor")][NoSerialize]
        public AttachableHardwareScript Attachable
        {
            get
            {
                AttachableHardwareScript attachable;
                if(TryGetScript<AttachableHardwareScript>(out attachable))
                {
                    return attachable;
                }

                return null;
            }
        }

        #endregion

        #region Funcs

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();
            this.TriggerEnter += OnTriggerEnter;
        }

        public override void OnEndPlay()
        {
            this.TriggerEnter -= OnTriggerEnter;
            base.OnEndPlay();
        }

        private void OnTriggerEnter(PhysicsColliderActor act)
        {
            if(act is SlotActor && act.HasParent && act.Parent != this){act.As<SlotActor>()?.Attach(this);}
        }

        private void UpdateHardwareConfig(HardwareType type)
        {
            switch(type)
            {
                case HardwareType.Cpu:
                    config = new CpuConfig();
                break;

                case HardwareType.Gpu:
                    config = null;
                break;

                case HardwareType.Psu:
                    config = null;
                break;

                case HardwareType.Ram:
                    config = null;
                break;

                case HardwareType.Fan:
                    config = null;
                break;

                case HardwareType.Cooler:
                    config = null;
                break;

                case HardwareType.Storage:
                    config = null;
                break;

                case HardwareType.Case:
                    config = null;
                break;

                case HardwareType.Monitor:
                    config = null;
                break;
            }
        }

        public void SetHardwareToFreeMode()
        {
            this.LinearVelocity = Vector3.Zero;
            this.AngularVelocity = Vector3.Zero;
            this.IsKinematic = false;
            this.EnableGravity = true;
            this.Constraints = RigidbodyConstraints.None;
            SetCollidersActive(true);
        }

        public void SetHardwareToAttachMode()
        {
            this.LinearVelocity = Vector3.Zero;
            this.AngularVelocity = Vector3.Zero;
            this.IsKinematic = true;
            this.EnableGravity = false;
            this.Constraints = RigidbodyConstraints.LockAll;
            SetCollidersActive(true);
        }

        public void SetHardwareToHoldMode()
        {
            this.LinearVelocity = Vector3.Zero;
            this.AngularVelocity = Vector3.Zero;
            this.IsKinematic = true;
            this.EnableGravity = false;
            this.Constraints = RigidbodyConstraints.LockAll;
            SetCollidersActive(false);
        }

        public void SetCollidersActive(bool state)
        {
            Collider[] colliders = this.GetChildren<Collider>();
            foreach(Collider collider in colliders)
            {
                if(collider is SlotActor)
                {
                    SlotActor slot = collider as SlotActor;
                    if(slot != null && slot.IsFull && slot.Attached != null)
                    {
                        slot.Attached.SetCollidersActive(state);
                    }
                }
                else
                {
                    collider.IsActive = state;
                }
            }
        }


        #endregion

        #region Classes

        public enum HardwareType
        {
            Cpu = 0,
            Gpu = 100,
            Psu = 200,
            Ram = 300,
            Fan = 400,
            Mobo = 500,
            Cooler = 600,
            Storage = 700,
            Case = 800,
            Monitor = 900,
        }

        public class HardwareConfig{}

        public class CpuConfig : HardwareConfig
        {
            [Serialize]
            public float baseClock = 3.0f;

            [Serialize]
            public float tdp = 70.0f;

            [Serialize]
            public float temperature = 0.0f;

            [Serialize]
            public float temperatureScalar = 1.0f;
        }
        

        #endregion
    }
}
