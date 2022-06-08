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
		/// <param name="delta">time to wait</param>
		/// <returns></returns>
		public static IEnumerator WaitForExactTime(float delta)
        {
            float endTime = Time.time + delta;

            while (Time.time < endTime)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
		/// Simple IEnumerator that checks 
		/// </summary>
		/// <param name="delta">time to wait</param>
		/// <returns></returns>
		public static IEnumerator WaitForTime(float delta)
        {
            yield return new WaitForSeconds(delta);
        }


        /// <summary>
        /// IEnumerator that uses DOTween logic to precisely calculate time
        /// </summary>
        /// <param name="delta">time to wait</param>
        /// <returns></returns>
        public static IEnumerator WaitForExactTime_DoTween(float delta)
        {
            Tween tween = DOVirtual.Float(0f, 1f, delta, (float val) => { });
            yield return tween.WaitForCompletion();
        }



        /// <summary>
        /// This method "invokes" a given callback after given delta time
        /// and return DOTween Tween to handle it (e.g. kill or smth else).
        /// Note that dotween actually performs even while unity is lagging - it is bad in terms of connection with animator component.
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Tween CallbackDelay_DoTween(float delta, Action callback)
        {
            Tween tween = DOVirtual.Float(0f, 1f, delta, (float val) => { }).OnComplete(()=> { callback(); });
            return tween;
        }

        /// <summary>
        /// This method "invokes" a given callback after given delta time
        /// and return IEnumerator to handle it (e.g. kill or smth else)
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator CallbackDelay_IEnumerator(float delta, Action callback)
        {
            float endTime = Time.time + delta;

            while (Time.time < endTime)
            {
                yield return new WaitForEndOfFrame();
            }

            callback();
        }


    }
}
