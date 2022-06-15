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

        [Serialize]
        private HardwareActor _attached = null;

        [NoSerialize]
        private AudioSource audioSource = null;

        [EditorDisplay("Slot Actor")][Serialize]
        public float attachOffset = 0f;

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
            if(hardware != null && !_isFull && hardware.Type == type && !hardware.locked && CanHardwareBoxFitInSlotBox(hardware))
            {
                hardware.Parent = this;
                hardware.SetHardwareToAttachMode();
                hardware.LocalEulerAngles = Vector3.Zero;
                hardware.LocalPosition = Transform.Up * attachOffset;
                _attached = hardware;
                _isFull = true;
            }
        }

        public void Deattach()
        {
            if(_isFull && _attached != null)
            {
                _attached.SetParent(Scene, true, true);
                _attached.SetHardwareToFreeMode();
                _isFull = false;
                _attached = null;
            }
        }

        private bool CanHardwareBoxFitInSlotBox(HardwareActor hardware)
        {
            Vector3 euler = hardware.EulerAngles;
            hardware.EulerAngles = Vector3.Zero;
            BoundingBox hardwareBox = hardware.BoxWithChildren;

            if(hardware.IsAttachable)
            {
                hardwareBox = BoundingBox.Empty;
                StaticModel[] models = hardware.GetChildren<StaticModel>();
                foreach(StaticModel model in models)
                {
                    BoundingBox.Merge(hardwareBox, model.Box);
                }

                Collider[] colliders = hardware.GetChildren<Collider>();
                foreach(Collider collider in colliders)
                {
                    if(!(collider is SlotActor))
                    {
                        BoundingBox.Merge(hardwareBox, collider.Box);
                    }
                }
            }

            Vector3 hardwareBoxSize = hardwareBox.Maximum - hardwareBox.Minimum;
            hardware.EulerAngles = euler;
            Vector3 slotBoxSize = BoxWithChildren.Maximum - BoxWithChildren.Minimum;

            return hardwareBoxSize.X <= slotBoxSize.X && hardwareBoxSize.Y <= slotBoxSize.Y && hardwareBoxSize.Z <= slotBoxSize.Z;
        }

        #endregion

        #region Classes

        public struct SlotKey
        {
            public HardwareActor.HardwareType type;
            public int index;
        }

        #endregion
    }
}
