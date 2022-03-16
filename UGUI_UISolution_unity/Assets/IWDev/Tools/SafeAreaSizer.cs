using System;
using UnityEngine;

public enum ScreenAspects
{
    None = 0,
    Normal_9_16,
    Wide_10_15,
    Narrow_11_24
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


        private void Awake()
        {

            ScreenAspect = GetCurrentAspectRatio();
            CurrentAspect = GetScreenAspect();


            if (CurrentAspect == ScreenAspects.Narrow_11_24)
            {
                SetTop(TopValueFixed);
            }
            else
            {
                SetTop(TopValueNormal);
            }

        }

        private ScreenAspects GetScreenAspect()
        {
            if (ScreenAspect >= 2.1f)
            {
                return ScreenAspects.Narrow_11_24;
            }

            if (ScreenAspect <= 1.5f)
            {
                return ScreenAspects.Wide_10_15;
            }

            return ScreenAspects.Normal_9_16;
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
