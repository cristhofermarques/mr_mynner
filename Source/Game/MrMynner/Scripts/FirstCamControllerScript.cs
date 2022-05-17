using System;
using System.Collections.Generic;
using FlaxEngine;

namespace MrMynner.Scripts
{
    public class FirstCamControllerScript : Script
    {
        #region Vars

        [EditorOrder(0)]
        [Serialize]
        public float sens = 1f;

        [EditorOrder(1)]
        [Serialize]
        public Vector2 inv = Vector2.One;

        [EditorOrder(2)]
        [Serialize]
        public Vector2 yArc = new Vector2(-90f, 90f);

        [EditorOrder(3)]
        [Limit(0f, 1f, 0.01f)]
        [Serialize]
        public float smoothness = 0f;

        [EditorOrder(4)]
        [Serialize]
        public string xAxisRawName = "Mouse X";

        [EditorOrder(5)]
        [Serialize]
        public string yAxisRawName = "Mouse Y";


        #endregion

        #region Props

        [NoSerialize]
        public bool Focused
        {
            get => Screen.CursorVisible == false && Screen.CursorLock == CursorLockMode.Locked;
            set
            {
                if(value != Focused)
                {
                    if(value)
                    {
                        Screen.CursorLock = CursorLockMode.Locked;
                        Screen.CursorVisible = false;
                    }
                    else
                    {
                        Screen.CursorLock = CursorLockMode.None;
                        Screen.CursorVisible = true;
                    }
                }
            }
        }

        #endregion

        #region Funcs

        public override void OnStart()
        {
            Focused = true;
        }

        public override void OnUpdate()
        {
            Vector2 axis = Vector2.Zero;
            axis.X = Input.GetAxisRaw(xAxisRawName);
            axis.Y = Input.GetAxisRaw(yAxisRawName);

            axis *= inv * sens * Time.DeltaTime * 10f;

            Vector3 euler = Actor.EulerAngles;

            euler.X += axis.Y;
            euler.Y += axis.X;

            euler.X = Mathf.Clamp(euler.X, yArc.X, yArc.Y);
            
            Actor.EulerAngles = Vector3.Lerp(Actor.EulerAngles, euler, 1.1f - smoothness);
        }
        
        #endregion
    }
}
