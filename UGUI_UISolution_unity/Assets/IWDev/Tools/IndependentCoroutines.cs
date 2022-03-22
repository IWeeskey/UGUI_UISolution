using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;


namespace IWDev.Tools
{

    /// <summary>
    /// This class contains some useful coroutines which are independent from FPS
    /// because unity's WaitForSeconds usually not precise and wrong when it comes to low FPS.
    /// Note that dotween actually performs even while unity is lagging - it is bad in terms of connection with animator component.
    /// </summary>
    public class IndependentCoroutines : MonoBehaviour
    {

		/// <summary>
		/// Simple IEnumerator that checks time every frame
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
		/// Simple IEnumerator that checks 
		/// </summary>
		/// <param name="_delta">time to wait</param>
		/// <returns></returns>
		public static IEnumerator WaitForTime(float _delta)
        {
            yield return new WaitForSeconds(_delta);
        }


        /// <summary>
        /// IEnumerator that uses DOTween logic to precisely calculate time
        /// </summary>
        /// <param name="_delta">time to wait</param>
        /// <returns></returns>
        public static IEnumerator WaitForExactTime_DoTween(float _delta)
        {
            Tween _tween = DOVirtual.Float(0f, 1f, _delta, (float _val) => { });
            yield return _tween.WaitForCompletion();
        }



        /// <summary>
        /// This method "invokes" a given callback after given delta time
        /// and return DOTween Tween to handle it (e.g. kill or smth else).
        /// Note that dotween actually performs even while unity is lagging - it is bad in terms of connection with animator component.
        /// </summary>
        /// <param name="_delta"></param>
        /// <param name="_callback"></param>
        /// <returns></returns>
        public static Tween CallbackDelay_DoTween(float _delta, Action _callback)
        {
            Tween _tween = DOVirtual.Float(0f, 1f, _delta, (float _val) => { }).OnComplete(()=> { _callback(); });
            return _tween;
        }

        /// <summary>
        /// This method "invokes" a given callback after given delta time
        /// and return IEnumerator to handle it (e.g. kill or smth else)
        /// </summary>
        /// <param name="_delta"></param>
        /// <param name="_callback"></param>
        /// <returns></returns>
        public static IEnumerator CallbackDelay_IEnumerator(float _delta, Action _callback)
        {
            float EndTime = Time.time + _delta;

            while (Time.time < EndTime)
            {
                yield return new WaitForEndOfFrame();
            }

            _callback();
        }


    }
}
