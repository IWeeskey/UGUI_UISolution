using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using IWDev.Tools;

namespace IWDev.UISolution
{
    /// <summary>
    /// Class with window essential references
    /// </summary>
    [Serializable]
    public class WindowAnimation_References
    {
        [Tooltip("This window animator which contains all possible animations, such as 'appearing window from bottom' and others")]
        public Animator ThisAnimator;

        [Tooltip("This window canvas. This component will be enabled or disabled to optimize render performance of single window")]
        public Canvas ThisCanvas;

        [Tooltip("All animated elements this window contains. Animated elements are fancy appeared when window opens")]
        public List<UIElementBase> AllAnimatedElements;

        [Tooltip("All additional canvases this window contains. They are handled the same way as this window canvas")]
        public List<Canvas> AdditionalCanvases;

        /// <summary>
        /// Get all essential references
        /// </summary>
        /// <param name="_go"> Window parent Gameobject </param>
        public void GetReferences(GameObject _go)
        {
            Debug.Log("Try to get window anim references on object: " + _go.name);

            if (_go.GetComponent<Animator>() != null)
            {
                ThisAnimator = _go.GetComponent<Animator>();
                Debug.Log("Animator successfully found");
            }
            else
            {
                Debug.LogError("Not found animator component on object: " + _go.name);
            }

            if (_go.GetComponent<Canvas>() != null)
            {
                ThisCanvas = _go.GetComponent<Canvas>();
                Debug.Log("Canvas successfully found");
            }
            else
            {
                Debug.LogError("Not found Canvas component on object: " + _go.name);
            }

            AllAnimatedElements = _go.GetComponentsInChildren<UIElementBase>(true).ToList();
            if (AllAnimatedElements != null && AllAnimatedElements.Count > 0)
            {
                Debug.Log("Successfully found: " + AllAnimatedElements.Count + " animated elements");
            }
            else 
            {
                Debug.Log("No animated elements were found");
            }

            AdditionalCanvases = _go.GetComponentsInChildren<Canvas>(true).ToList();

            //if additional canvase contains this window canvas - remove it from the list
            if (AdditionalCanvases.Contains(ThisCanvas)) AdditionalCanvases.Remove(ThisCanvas);

            if (AdditionalCanvases != null && AdditionalCanvases.Count > 0)
            {
                Debug.Log("Successfully found: " + AdditionalCanvases.Count + " additional canvases");
            }
            else
            {
                Debug.Log("No additional canvases were found");
            }
        }
    }

    /// <summary>
    /// Class which controls the way window is animated
    /// </summary>
    [Serializable]
    public class WindowAnimation_Settings
    {
        /// <summary>
        /// Controls whether gameobject will be disabled completely in addition to canvas component
        /// </summary>
        public bool DisableIfNotActive = false;

        /// <summary>
        /// Controls whether gameobject will be disabled on initialization
        /// </summary>
        public bool AwakeDisabled = false;

        /// <summary>
        /// Controls appear animation speed. Increase it to make it faster
        /// </summary>
        [Range(0.5f, 2.5f)]
        public float AppearSpeed = 1f;

        /// <summary>
        /// Controls disappear animation speed. Increase it to make it faster
        /// </summary>
        [Range(0.5f, 2.5f)]
        public float DisappearSpeed = 1f;

        /// <summary>
        /// Controls window animation appear type
        /// </summary>
        public UIWAppearTypes AppearType = UIWAppearTypes.AlphaOnly;

        /// <summary>
        /// Controls window animation disappear type
        /// </summary>
        public UIWDisappearTypes DisappearType = UIWDisappearTypes.AlphaOnly;

    }


    /// <summary>
    /// Class showing runtime parameters of the window
    /// </summary>
    [Serializable]
    public class WindowAnimation_RunTimeParameters
    {
        /// <summary>
        /// Is window currently opened?
        /// </summary>
        public bool IsActive = false;

        /// <summary>
        /// Shows the state of the window
        /// </summary>
        public UIWStateTypes CurrentState = UIWStateTypes.None;
    }


    /// <summary>
    /// Class which controls animation behavior
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class WindowAnimationBase : MonoBehaviour
    {
        public WindowAnimation_References WindowReferences;
        public WindowAnimation_Settings WindowSettings;
        public WindowAnimation_RunTimeParameters WindowRuntimeParameters;

        /// <summary>
        /// List of active DOTweens
        /// </summary>
        private List<Tween> ActiveTweens = new List<Tween>();

        /// <summary>
        /// Use this method from editor. Gathers all necessary info to work properly
        /// </summary>
        public void GetReferences()
        {
            WindowReferences.GetReferences(gameObject);
        }

        /// <summary>
        /// On Init we set this window to default state
        /// </summary>
        protected virtual void OnInit()
        {
            KillAllActiveTweens();
            if (CurrentStateCoro != null) StopCoroutine(CurrentStateCoro);

            CanvasesEnable(!WindowSettings.AwakeDisabled);
            EnableAnimator(!WindowSettings.AwakeDisabled);


            WindowRuntimeParameters.IsActive = !WindowSettings.AwakeDisabled;

            foreach (UIElementBase uiae in WindowReferences.AllAnimatedElements)
            {
                uiae.OnInit(false);
            }
        }

        /// <summary>
        /// Controls this window canvases
        /// </summary>
        /// <param name="_value"></param>
        private void CanvasesEnable(bool _value)
        {
            WindowReferences.ThisCanvas.enabled = _value;

            foreach (Canvas cnv in WindowReferences.AdditionalCanvases)
            {
                cnv.enabled = _value;
            }

            //if we enabling window
            if (_value)
            {
                gameObject.SetActive(true);
            }
            //if we disabling window
            else
            {
                //check if should disable gameobject
                if (WindowSettings.DisableIfNotActive) gameObject.SetActive(false);
                else gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Enables or disables animator component in order to avoid unnecesary render calls within canvas
        /// </summary>
        /// <param name="_value"></param>
        private void EnableAnimator(bool _value)
        {
            WindowReferences.ThisAnimator.enabled = _value;
        }


        /// <summary>
        /// Disables this window instantly without any animation and breaks all active animations
        /// </summary>
        public void InstantDisable()
        {
            WindowRuntimeParameters.IsActive = false;
            CanvasesEnable(false);
            EnableAnimator(false);

            KillAllActiveTweens();
            if (CurrentStateCoro != null) StopCoroutine(CurrentStateCoro);
        }

        /// <summary>
        /// Stops all DOTweens and Coroutines
        /// </summary>
        private void StopAllCoroutines()
        {
            KillAllActiveTweens();
            if (CurrentStateCoro != null) StopCoroutine(CurrentStateCoro);
        }

        /// <summary>
        /// Animate this window to disappear state
        /// </summary>
        public void AnimateDisAppear()
        {
            WindowRuntimeParameters.IsActive = false;
            SwitchAnimationTo(UIWStateTypes.Disappear);

            StopAllCoroutines();

            CurrentStateCoro = DisAppearCoro();

            StartCoroutine(CurrentStateCoro);
        }

        /// <summary>
        /// Animate this window to appear state
        /// </summary>
        public void AnimateAppear()
        {
            //hard enables gameobject
            gameObject.SetActive(true);
            CanvasesEnable(true);

            SwitchAnimationTo(UIWStateTypes.Appear);

            StopAllCoroutines();

            CurrentStateCoro = AppearCoro();

            StartCoroutine(CurrentStateCoro);

            WindowRuntimeParameters.IsActive = true;
        }


        /// <summary>
        /// Controls of animator state
        /// </summary>
        /// <param name="_value"></param>
        public void SwitchAnimationTo(UIWStateTypes _value)
        {
            if (WindowReferences.ThisAnimator != null)
            {
                EnableAnimator(true);
                ApplySettings();
                WindowReferences.ThisAnimator.SetInteger("State", (int)_value);
                WindowReferences.ThisAnimator.SetTrigger("ChangeState");

                WindowRuntimeParameters.CurrentState = _value;
            }
        }


        /// <summary>
        /// Apply all settings
        /// </summary>
        private void ApplySettings()
        {
            WindowReferences.ThisAnimator.SetFloat("AppearMultiplier", WindowSettings.AppearSpeed);
            WindowReferences.ThisAnimator.SetFloat("DisappearMultiplier", WindowSettings.DisappearSpeed);

            WindowReferences.ThisAnimator.SetInteger("AppearState", (int)WindowSettings.AppearType);
            WindowReferences.ThisAnimator.SetInteger("DisappearState", (int)WindowSettings.DisappearType);
        }



        /// <summary>
        /// Current coroutine of changing state
        /// </summary>
        private IEnumerator CurrentStateCoro;

        /// <summary>
        /// Disappear coroutine
        /// </summary>
        /// <returns></returns>
        IEnumerator DisAppearCoro()
        {
            Tween _tween = IndependentCoroutines.CallbackDelay_DoTween(0.5f / WindowSettings.DisappearSpeed, () => 
            {
                CanvasesEnable(false);
                EnableAnimator(false); 
            });

            ActiveTweens.Add(_tween);

            yield return _tween.WaitForCompletion();
        }

        
        /// <summary>
        /// Kills all active tweens and clears the list
        /// </summary>
        private void KillAllActiveTweens()
        {
            foreach (Tween tw in ActiveTweens)
            {
                if (tw != null)
                {
                    tw.Kill();
                }
            }
            ActiveTweens.Clear();
        }


        private float _MinAEAppearGap = .075f;
        private float _MinAECount = 5f;
        private float _ActualAEAppearGap = 0f;
        private int _ActiveAE = 1;
        /// <summary>
        /// Appear coroutine. Here we successively iterate through all animated elements
        /// and call appear animation in each
        /// </summary>
        /// <returns></returns>
        IEnumerator AppearCoro()
        {
            //run tween wich disables animator component
            Tween _DisableAnimatorTween = IndependentCoroutines.CallbackDelay_DoTween(0.5f / WindowSettings.AppearSpeed, () =>
            {
                EnableAnimator(false);
            });

            ActiveTweens.Add(_DisableAnimatorTween);

            foreach (UIElementBase uiae in WindowReferences.AllAnimatedElements)
            {
                uiae.SwitchAnimationTo(UIEBasicStates.BeforeAppear);
            }

            _ActiveAE = GetActiveAnimatedElementsCount();

            //calculating time gap between showing each animated element
            if (_ActiveAE <= _MinAECount)
            {
                _ActualAEAppearGap = _MinAEAppearGap/WindowSettings.AppearSpeed;
            }
            else
            {
                _ActualAEAppearGap = _MinAEAppearGap / WindowSettings.AppearSpeed 
                    * _MinAECount / (float)_ActiveAE;
            }

            yield return new WaitForEndOfFrame();
            foreach (UIElementBase uiae in WindowReferences.AllAnimatedElements)
            {
                if (uiae.gameObject.activeInHierarchy)
                {
                    uiae.SwitchAnimationTo(UIEBasicStates.Appear);

                    Tween _tween = IndependentCoroutines.CallbackDelay_DoTween(_ActualAEAppearGap, ()=> { });
                    ActiveTweens.Add(_tween);

                    yield return _tween.WaitForCompletion();
                }
            }
        }

        int GetActiveAnimatedElementsCount()
        {
            int _activeAE_count = 0;

            foreach (UIElementBase AE in WindowReferences.AllAnimatedElements)
            {
                if (AE.gameObject.activeSelf)
                {
                    _activeAE_count++;
                }
            }

            return _activeAE_count;
        }
    }
}
