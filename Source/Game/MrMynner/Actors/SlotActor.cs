using System;
using System.Collections.Generic;
using FlaxEngine;

namespace MrMynner.Actors
{
    public class SlotActor : BoxCollider
    {
        #region Vars

        [NoSerialize]
        private bool _isFull = false;

        [NoSerialize]
        private HardwareActor _attached = null;

        [NoSerialize]
        private AudioSource audioSource = null;

        [EditorDisplay("Slot Actor")][Serialize]
        public AnimatedModel slotModel = null;

        [EditorDisplay("Slot Actor")][Serialize]
        public AudioClip slotFullAudioClip = null;

        [EditorDisplay("Slot Actor")][Serialize]
        public AudioClip slotEmptyAudioClip = null;

        [EditorDisplay("Slot Actor")][Serialize]
        public HardwareActor.HardwareType type;

        #endregion

        #region Props

        [EditorDisplay("Slot Actor")][Serialize]
        public bool IsFull
        {
            get => _isFull;
            set
            {
                if(value != _isFull)
                {
                    if(value)// Attach
                    {
                    }
                    else // Deattach
                    {
                        Deattach();
                    }


                    _isFull = value;
                }
            }
        }

        [EditorDisplay("Slot Actor")][Serialize]
        public HardwareActor Attached
        {
            get => _attached;

            set
            {
                if(value != _attached)
                {
                    if(value == null)// Deattach
                    {
                        Deattach();
                    }
                    else // Attach
                    {
                        Attach(value);
                    }
                }
            }
        }

        #endregion

        #region Funcs

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();

            IsTrigger = true;
            slotModel = GetOrAddChild<AnimatedModel>();
            audioSource = GetOrAddChild<AudioSource>();
        }

        public void Attach(HardwareActor hardware)
        {
            if(hardware != null && !_isFull && hardware.Type == type && !hardware.locked)
            {
                hardware.EnableGravity = false;
                hardware.IsKinematic = true;
                hardware.Parent = this;
                hardware.Constraints = RigidbodyConstraints.LockRotation;
                hardware.LinearVelocity = 
                hardware.AngularVelocity = 
                hardware.LocalEulerAngles = 
                hardware.LocalPosition = Vector3.Zero;
                _attached = hardware;
                _isFull = true;
            }
        }

        public void Deattach()
        {
            if(_isFull)
            {
                _attached.SetParent(Scene, true, true);
                _attached.EnableGravity = true;
                _attached.IsKinematic = false;
                _attached.Constraints = RigidbodyConstraints.None;
                _attached.LocalEulerAngles = _attached.LocalPosition = Vector3.Zero;
                _attached.locked = true;
                _attached.LinearVelocity = 
                _attached.AngularVelocity = 
                _attached.LocalEulerAngles = 
                _attached.LocalPosition = Vector3.Zero;
                _isFull = false;
                _attached = null;
            }
        }

        #endregion
    }
}
