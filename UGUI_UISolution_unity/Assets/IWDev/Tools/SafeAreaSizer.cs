using System;
using UnityEngine;

public enum ScreenAspects
{
    None = 0,
    Portrait_Normal_9_16,
    Portrait_Wide_10_15,//ipad
    Portrait_Narrow_11_24,//modern iphone and other sausage-like forms

    Landscape_Normal_9_16,
    Landscape_Wide_10_15,//ipad
    Landscape_Narrow_11_24,//modern iphone and other sausage-like forms
}


namespace IWDev.Tools
{
    /// <summary>
    /// Class that helps to arrange RectTransforms to narrow devices
    /// </summary>
    public class SafeAreaSizer : MonoBehaviour
    {

        public RectTransform TargetToFix;
        public float ScreenAspect = 1f;
        public ScreenAspects CurrentAspect = ScreenAspects.None;

        /// <summary>
        /// Top value which represents normal top anchor
        /// </summary>
        public int TopValueNormal = 0;

        /// <summary>
        /// Top value which represents narrow top anchor. For example iphone with top borders
        /// </summary>
        public int TopValueFixed = 0;

        public bool PerformLogic_Portrait = true;
        public bool PerformLogic_Landscape = false;


        private void Awake()
        {
            RecalculateAll();
        }

        public void RecalculateAll()
        {
            ScreenAspect = GetCurrentAspectRatio();
            CurrentAspect = GetScreenAspect();

            if ((CurrentAspect == ScreenAspects.Portrait_Narrow_11_24
                || CurrentAspect == ScreenAspects.Portrait_Normal_9_16
                || CurrentAspect == ScreenAspects.Portrait_Wide_10_15)
                && PerformLogic_Portrait)
            {
                if (CurrentAspect == ScreenAspects.Portrait_Narrow_11_24)
                {
                    SetTop(TopValueFixed);
                }
                else
                {
                    SetTop(TopValueNormal);
                }
            }

            if ((CurrentAspect == ScreenAspects.Landscape_Narrow_11_24
               || CurrentAspect == ScreenAspects.Landscape_Normal_9_16
               || CurrentAspect == ScreenAspects.Landscape_Wide_10_15)
               && PerformLogic_Landscape)
            {
                if (CurrentAspect == ScreenAspects.Landscape_Narrow_11_24)
                {
                    SetLeft(TopValueFixed);
                }
                else
                {
                    SetLeft(TopValueNormal);
                }
            }

        }

        private ScreenAspects GetScreenAspect()
        {
            //landscape
            if (ScreenAspect <= 1f)
            {
                //ipad
                if (ScreenAspect >= 0.74f)
                {
                    return ScreenAspects.Landscape_Wide_10_15;
                }
                //normal
                else if (ScreenAspect >= 0.49f)
                {
                    return ScreenAspects.Landscape_Normal_9_16;
                }

                //narrow
                return ScreenAspects.Landscape_Narrow_11_24;
            }
            //portrait
            else
            {
                if (ScreenAspect >= 2.1f)
                {
                    return ScreenAspects.Portrait_Narrow_11_24;
                }

                if (ScreenAspect <= 1.5f)
                {
                    return ScreenAspects.Portrait_Wide_10_15;
                }
                return ScreenAspects.Portrait_Normal_9_16;
            }
        }

        private float GetCurrentAspectRatio()
        {
            ScreenAspect = (float)Screen.height / (float)Screen.width;
            ScreenAspect = (float)Math.Round(ScreenAspect, 2);

            return ScreenAspect;
        }


        public void SetLeft(float left)
        {
            TargetToFix.offsetMin = new Vector2(left, TargetToFix.offsetMin.y);
        }

        public void SetRight(float right)
        {
            TargetToFix.offsetMax = new Vector2(-right, TargetToFix.offsetMax.y);
        }

        public void SetTop(float top)
        {
            TargetToFix.offsetMax = new Vector2(TargetToFix.offsetMax.x, -top);
        }

        public void SetBottom(float bottom)
        {
            TargetToFix.offsetMin = new Vector2(TargetToFix.offsetMin.x, bottom);
        }


    }
}
