using System;
using System.Collections.Generic;
using FlaxEngine;
using MrMynner.Actors;

namespace MrMynner.Scripts
{
    public class AttachableHardwareScript : Script
    {      
        #region Vars

        [NoSerialize]
        private HardwareActor hardware;
        
        [Tooltip("cpu:0|gpu:100|psu:200|ram:300|fan:400|mobo:500|Cooler:600|storage:700|case:800|monitor:900")][Serialize]
        public Dictionary<int, SlotActor> slots = new Dictionary<int, SlotActor>(25);

        [Serialize]
        public Dictionary<HardwareActor.HardwareType, int> slotsCapacity = new Dictionary<HardwareActor.HardwareType, int>(10);

        [Serialize]
        private AttachableHardwareType _type;

        #endregion

        #region Props

        public AttachableHardwareType Type
        {
            get => _type;

            set
            {
                if(value != _type)
                {
                    _type = value;
                    UpdateAttachableHardwareType(value);
                }
            }
        }

        #endregion

        #region Funcs

        public override void OnStart()
        {
            if(Actor is HardwareActor){hardware = Actor.As<HardwareActor>();}
            UpdateAttachableHardwareType(_type);
        }
        
        public override void OnUpdate()
        {
        }

        public int GetSlotKey(HardwareActor.HardwareType type, int idx){ return (int)type + idx;}

        public SlotActor GetSlot(HardwareActor.HardwareType type, int idx)
        {
            int slotKey = GetSlotKey(type, idx);
            if(slots != null && slots.ContainsKey(slotKey))
            {
                return slots[slotKey];
            }

            return null;
        }

        public HardwareActor GetSlotHardware(HardwareActor.HardwareType type, int idx)
        {
            SlotActor slot = GetSlot(type, idx);
            if(slot != null && slot.IsFull && slot.Attached != null){return slot.Attached;}

            return null;
        }

        public bool IsSlotFull(HardwareActor.HardwareType type, int idx)
        {
            HardwareActor hardware = GetSlotHardware(type, idx);
            return hardware != null;
        }

        public bool IsSomeOfSlotsFull(HardwareActor.HardwareType type, int initIdx, int endIdx)
        {
            for(int idx = initIdx; idx < endIdx; idx++)
            {
                if(IsSlotFull(type, idx)){return true;}
            }

            return false;
        }

        public bool CheckSlots(HardwareActor.HardwareType type)
        {
            if(!slotsCapacity.ContainsKey(type)){return false;}
            int capacity = slotsCapacity[type];
            if(capacity < 1){return false;}

            return IsSomeOfSlotsFull(type, 0, capacity);
        }

        public bool MoboCheck()
        {
            bool result = true;

            result &= CheckSlots(HardwareActor.HardwareType.Cpu);
            result &= CheckSlots(HardwareActor.HardwareType.Cooler);
            result &= CheckSlots(HardwareActor.HardwareType.Ram);


            return result;
        }

        public bool CaseCheck()
        {
            bool result = true;

            result &= CheckSlots(HardwareActor.HardwareType.Mobo);
            result &= CheckSlots(HardwareActor.HardwareType.Psu);
            result &= CheckSlots(HardwareActor.HardwareType.Storage);

            return result;
        }

        private void UpdateAttachableHardwareType(AttachableHardwareType type)
        {
            switch(type)
            {
                case AttachableHardwareType.Mobo:
                    slotsCapacity.Clear();
                    slotsCapacity.Add(HardwareActor.HardwareType.Cpu, 1);
                    slotsCapacity.Add(HardwareActor.HardwareType.Cooler, 1);
                    slotsCapacity.Add(HardwareActor.HardwareType.Ram, 2);
                    slotsCapacity.Add(HardwareActor.HardwareType.Gpu, 1);
                break;

                case AttachableHardwareType.Case:
                    slotsCapacity.Clear();
                    slotsCapacity.Add(HardwareActor.HardwareType.Mobo, 1);
                    slotsCapacity.Add(HardwareActor.HardwareType.Psu, 1);
                    slotsCapacity.Add(HardwareActor.HardwareType.Storage, 4);
                    slotsCapacity.Add(HardwareActor.HardwareType.Fan, 2);
                break;
            }
        }

        #endregion

        #region Classes

        public enum AttachableHardwareType
        {
            Mobo,
            Case,
        }

        #endregion
    }
}
