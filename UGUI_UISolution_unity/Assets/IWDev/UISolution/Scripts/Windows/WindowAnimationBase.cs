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
        /// <param name="go"> Window parent Gameobject </param>
        public void GetReferences(GameObject go)
        {
            Debug.Log("Try to get window anim references on object: " + go.name);

            if (go.GetComponent<Animator>() != null)
            {
                ThisAnimator = go.GetComponent<Animator>();
                Debug.Log("Animator successfully found");
            }
            else
            {
                Debug.LogError("Not found animator component on object: " + go.name);
            }

            if (go.GetComponent<Canvas>() != null)
            {
                ThisCanvas = go.GetComponent<Canvas>();
                Debug.Log("Canvas successfully found");
            }
            else
            {
                Debug.LogError("Not found Canvas component on object: " + go.name);
            }

            AllAnimatedElements = go.GetComponentsInChildren<UIElementBase>(true).ToList();

            for (int i = AllAnimatedElements.Count - 1; i >= 0; i--)
            {
                if (AllAnimatedElements[i].RuntimeParameters.IgnoreToWindowAnimateSearch) AllAnimatedElements.RemoveAt(i);
            }

            if (AllAnimatedElements != null && AllAnimatedElements.Count > 0)
            {
                Debug.Log("Successfully found: " + AllAnimatedElements.Count + " animated elements");
            }
            else 
            {
                Debug.Log("No animated elements were found");
            }

            AdditionalCanvases = go.GetComponentsInChildren<Canvas>(true).ToList();

            //if additional canvas contains this window canvas - remove it from the list
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

        public bool InstantDisappear = false;

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
        /// Controls whether animation speed will be multilplied by timescale
        /// </summary>
        public bool TimeScaleSpeed = true;

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
        /// Is window in WORLD UI
        /// </summary>
        public bool IsWorldUI = false;

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
        private List<Tween> _activeTweens = new List<Tween>();

        /// <summary>
        /// List of active IEnumerator
        /// </summary>
        private List<IEnumerator> _activeIEnumerators = new List<IEnumerator>();

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
        protected virtual void OnInit(bool isWorld = false)
        {
            if (CurrentStateCoro != null) StopCoroutine(CurrentStateCoro);

            CanvasesEnable(!WindowSettings.AwakeDisabled);
            EnableAnimator(!WindowSettings.AwakeDisabled);

            WindowRuntimeParameters.IsWorldUI = isWorld;
            WindowRuntimeParameters.IsActive = !WindowSettings.AwakeDisabled;

            foreach (UIElementBase uiae in WindowReferences.AllAnimatedElements)
            {
                uiae.OnInit(false);
            }
        }

        /// <summary>
        /// Controls this window canvases
        /// </summary>
        /// <param name="value"></param>
        private void CanvasesEnable(bool value)
        {
            WindowReferences.ThisCanvas.enabled = value;

            foreach (Canvas cnv in WindowReferences.AdditionalCanvases)
            {
                cnv.enabled = value;
            }

            //if we enabling window
            if (value)
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
        /// <param name="value"></param>
        private void EnableAnimator(bool value)
        {
            WindowReferences.ThisAnimator.enabled = value;
        }


        /// <summary>
        /// Disables this window instantly without any animation and breaks all active animations
        /// </summary>
        public void InstantDisable()
        {
            WindowRuntimeParameters.IsActive = false;
            CanvasesEnable(false);
            EnableAnimator(false);

            KillAllCoroutines();
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
        /// <param name="value"></param>
        public void SwitchAnimationTo(UIWStateTypes value)
        {
            if (WindowReferences.ThisAnimator != null)
            {
                EnableAnimator(true);
                ApplySettings();
                WindowReferences.ThisAnimator.SetInteger("State", (int)value);
                WindowReferences.ThisAnimator.SetTrigger("ChangeState");

                WindowRuntimeParameters.CurrentState = value;
            }
        }


        /// <summary>
        /// Apply all settings
        /// </summary>
        private void ApplySettings()
        {
            WindowReferences.ThisAnimator.SetFloat("AppearMultiplier", WindowSettings.TimeScaleSpeed ? (WindowSettings.AppearSpeed / Time.timeScale) : WindowSettings.AppearSpeed);
            WindowReferences.ThisAnimator.SetFloat("DisappearMultiplier", WindowSettings.TimeScaleSpeed ? (WindowSettings.DisappearSpeed / Time.timeScale) : WindowSettings.DisappearSpeed);

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
            if (WindowSettings.InstantDisappear)
            {
                CanvasesEnable(false);
                EnableAnimator(false);
                yield break;
            }

            IEnumerator disableAnimatorCoro = IndependentCoroutines.CallbackDelay_IEnumerator(0.5f / 
                (WindowSettings.TimeScaleSpeed ? (WindowSettings.DisappearSpeed / Time.timeScale) : WindowSettings.DisappearSpeed)
                , () =>
            {
                CanvasesEnable(false);
                EnableAnimator(false);
            });
            _activeIEnumerators.Add(disableAnimatorCoro);

           

            /*
            Tween _tween = IndependentCoroutines.CallbackDelay_DoTween(0.5f / WindowSettings.DisappearSpeed, () => 
            {
                CanvasesEnable(false);
                EnableAnimator(false); 
            });

            ActiveTweens.Add(_tween);
            yield return _tween.WaitForCompletion();
            */

            yield return StartCoroutine(disableAnimatorCoro);
        }

        
        /// <summary>
        /// Kills all active tweens and coroutines
        /// </summary>
        private void KillAllCoroutines()
        {
            foreach (Tween tw in _activeTweens)
            {
                if (tw != null)
                {
                    tw.Kill();
                }
            }
            _activeTweens.Clear();

            foreach (IEnumerator _IEnum in _activeIEnumerators)
            {
                if (_IEnum != null) StopCoroutine(_IEnum);
            }

            _activeIEnumerators.Clear();

            if (CurrentStateCoro != null) StopCoroutine(CurrentStateCoro);
        }


        private float _minAEAppearGap = .075f;
        private float _minAECount = 5f;
        private float _actualAEAppearGap = 0f;
        private int _activeAE = 1;
        /// <summary>
        /// Appear coroutine. Here we successively iterate through all animated elements
        /// and call appear animation in each
        /// </summary>
        /// <returns></returns>
        IEnumerator AppearCoro()
        {
            
            IEnumerator disableAnimatorCoro = IndependentCoroutines.CallbackDelay_IEnumerator(0.5f / 
                (WindowSettings.TimeScaleSpeed ? (WindowSettings.AppearSpeed / Time.timeScale) : WindowSettings.AppearSpeed)
                , () =>
            {
                EnableAnimator(false);
            });
            StartCoroutine(disableAnimatorCoro);
            _activeIEnumerators.Add(disableAnimatorCoro);

            /*
            //run tween which disables animator component
            Tween disableAnimatorTween = IndependentCoroutines.CallbackDelay_DoTween(0.5f / WindowSettings.AppearSpeed, () =>
            {
                EnableAnimator(false);
            });

            ActiveTweens.Add(disableAnimatorTween);
            */

            foreach (UIElementBase uiae in WindowReferences.AllAnimatedElements)
            {
                if (uiae.gameObject.activeInHierarchy) uiae.SwitchAnimationTo(UIEBasicStates.BeforeAppear);
            }

            _activeAE = GetActiveAnimatedElementsCount();

            //calculating time gap between showing each animated element
            if (_activeAE <= _minAECount)
            {
                _actualAEAppearGap = _minAEAppearGap/WindowSettings.AppearSpeed;
            }
            else
            {
                _actualAEAppearGap = _minAEAppearGap / WindowSettings.AppearSpeed 
                    * _minAECount / (float)_activeAE;
            }

            _actualAEAppearGap = WindowSettings.TimeScaleSpeed ? (_actualAEAppearGap * Time.timeScale) : _actualAEAppearGap;

            yield return new WaitForEndOfFrame();
            foreach (UIElementBase uiae in WindowReferences.AllAnimatedElements)
            {
                if (uiae.gameObject.activeInHierarchy)
                {
                    uiae.SwitchAnimationTo(UIEBasicStates.Appear);

                    Tween tween = IndependentCoroutines.CallbackDelay_DoTween(_actualAEAppearGap, ()=> { });
                    _activeTweens.Add(tween);

                    yield return tween.WaitForCompletion();
                }
            }
        }

        int GetActiveAnimatedElementsCount()
        {
            int activeAE_count = 0;

            foreach (UIElementBase AE in WindowReferences.AllAnimatedElements)
            {
                if (AE.gameObject.activeSelf && AE.gameObject.activeInHierarchy)
                {
                    activeAE_count++;
                }
            }

            return activeAE_count;
        }
    }
}
