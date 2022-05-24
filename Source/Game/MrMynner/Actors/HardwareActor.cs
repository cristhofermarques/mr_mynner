using System;
using System.Collections.Generic;
using FlaxEngine;

namespace MrMynner.Actors
{
    public class HardwareActor : RigidBody
    {
        #region Vars

        [NoSerialize]
        private bool _broken = false;

        [EditorDisplay("Component Actor")][NoSerialize]
        public bool locked = false;

        [NoSerialize]
        private bool _isAttached = false;

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
        public bool IsAttached
        {
            get => _isAttached;

            set
            {
                if(value != _isAttached)
                {
                    _isAttached = value;
                    if(value)
                    {
                        IsKinematic = true;
                        EnableGravity = false;
                        Constraints = RigidbodyConstraints.LockRotation;
                        LinearVelocity = Vector3.Zero;
                        AngularVelocity = Vector3.Zero;
                    }
                    else
                    {
                        IsKinematic = false;
                        EnableGravity = true;
                        Constraints = RigidbodyConstraints.None;
                        LinearVelocity = Vector3.Zero;
                        AngularVelocity = Vector3.Zero;
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
            if(act is SlotActor){act.As<SlotActor>()?.Attach(this);}
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

        #endregion

        #region Classes

        public enum HardwareType
        {
            Cpu,
            Gpu,
            Psu,
            Ram,
            Fan,
            Mobo,
            Cooler,
            Storage,
            Case,
            Monitor,
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
