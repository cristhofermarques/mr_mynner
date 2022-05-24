using System.Timers;
using FlaxEngine;
using FlaxEngine.GUI;

namespace MrMynner.Scripts
{
    public class AimScript : Script
    {   
        #region Vars

        [NoSerialize]
        private UICanvas canvas = null;

        [NoSerialize]
        private Control aimQuadCtrl = null;

        [NoSerialize]
        private Image aimShadowImg = null;

        [NoSerialize]
        private Material aimShadowMat = null;

        [NoSerialize]
        public Color targetAimColor = Color.White;

        [NoSerialize]
        private Timer timer;

        [NoSerialize]
        private Color recColor = Color.White;

        [NoSerialize][HideInEditor]
        public bool isColorAnimation = false;

        [NoSerialize]
        private float colorAnimationLerpIntensity = 0.3f;

        #endregion

        #region Props

        [NoSerialize]
        public float AimSize
        {
            get
            {
                if(aimQuadCtrl != null)
                {
                    return (aimQuadCtrl.Size.X + aimQuadCtrl.Size.Y) * 0.5f;
                }
                else{return -1;}
            }

            set
            {
                if(AimSize != value && value > 0f)
                {
                    aimQuadCtrl.Size = Vector2.One * value;
                    aimQuadCtrl.Location = canvas.Size * 0.5f - aimQuadCtrl.Size * 0.5f;
                }
            }
        }

        [NoSerialize]
        public float AimShadowSize
        {
            get
            {
                if(aimShadowImg != null)
                {
                    return (aimShadowImg.Size.X + aimShadowImg.Size.Y) * 0.5f;
                }
                else{return -1;}
            }

            set
            {
                if(AimSize != value && value > 0f)
                {
                    aimShadowImg.Size = Vector2.One * value;
                    aimShadowImg.Location = canvas.Size * 0.5f - aimShadowImg.Size * 0.5f;
                }
            }
        }

        #endregion

        #region Funcs

        public override void OnStart()
        {
            aimShadowMat = Helper.PickAsset<Material>("mat_src_data", "aim_shadow_mat");

            canvas = Scene.GetOrAddChild<UICanvas>();
            canvas.RenderMode = CanvasRenderMode.ScreenSpace;

            aimShadowMat.SetParameterValue("color", Color.Black);
            aimShadowMat.SetParameterValue("shadow_exponent", 2f);

            aimShadowImg = canvas.GUI.AddChild<Image>();
            aimShadowImg.KeepAspectRatio = false;
            aimShadowImg.AnchorPreset = AnchorPresets.MiddleCenter;
            aimShadowImg.Brush = new MaterialBrush(aimShadowMat);
            AimShadowSize = 5f;

            aimQuadCtrl = canvas.GUI.AddChild<Control>();
            aimQuadCtrl.AnchorPreset = AnchorPresets.MiddleCenter;
            aimQuadCtrl.BackgroundColor = Color.White;
            AimSize = 1f; 

            timer = new Timer();
            timer.AutoReset = false;
            timer.Elapsed += OnTimerElapsed;
        }
        
        public override void OnUpdate()
        {
            aimQuadCtrl.BackgroundColor = Color.Lerp(aimQuadCtrl.BackgroundColor, targetAimColor, colorAnimationLerpIntensity);
        }

        public void SetAimColorForMiliseconds(Color color, int halfMs, float lerpIntensity)
        {
            if(!isColorAnimation)
            {
                colorAnimationLerpIntensity = lerpIntensity;
                timer.Interval = halfMs;
                recColor = aimQuadCtrl.BackgroundColor;
                targetAimColor = color;
                isColorAnimation = true;
                timer.Start();
            }
        }

        private void OnTimerElapsed(System.Object obj, ElapsedEventArgs args)
        {
            timer.Stop();
            targetAimColor = recColor;
            isColorAnimation = false;
            recColor = Color.Black;
        }

        public override void OnDestroy()
        {
            aimQuadCtrl.Dispose();
            aimQuadCtrl = null;
            base.OnDestroy();
        }
    
        #endregion
    }
}
