using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;


namespace IWDev.Tools
{

    /// <summary>
    /// This class contains some useful coroutines which are independent from FPS
    /// because unity's WaitForSecond usually not precise and wrong when it comes to low FPS
    /// </summary>
    public class IndependentCoroutines : MonoBehaviour
    {
        /// <summary>
        /// Simple IEnumerator that check time every frame
        /// </summary>
        /// <param name="_delta">time to wait</param>
        /// <returns></returns>
        public static IEnumerator WaitForExactTime(float _delta)
        {
            float EndTime = Time.time + _delta;

            while (Time.time < EndTime)
            {
                yield return new WaitForEndOfFrame();
            }
        }


        /// <summary>
        /// IEnumerator that uses DOTween logic to precisely calculate time
        /// </summary>
        /// <param name="_delta">time to wait</param>
        /// <returns></returns>
        public static IEnumerator WaitForTime_DoTween(float _delta)
        {
            Tween _tween = DOVirtual.Float(0f, 1f, _delta, (float val) => { });
            yield return _tween.WaitForCompletion();
        }


        /// <summary>
        /// This method "invokes" a given callback after given delta time
        /// and return DOTween Tween to handle it (e.g. kill or smth else)
        /// </summary>
        /// <param name="_delta"></param>
        /// <param name="_callback"></param>
        /// <returns></returns>
        public static Tween CallbackDelay_DoTween(float _delta, Action _callback)
        {
            Tween _tween = DOVirtual.Float(0f, 1f, _delta, (float val) => { }).OnComplete(()=> { _callback(); });
            return _tween;
        }

    }
}
