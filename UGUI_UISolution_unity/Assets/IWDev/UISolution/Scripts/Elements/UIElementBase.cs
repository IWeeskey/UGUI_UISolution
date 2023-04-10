using DG.Tweening;
using IWDev.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;



namespace IWDev.UISolution
{
    /// <summary>
    /// Class with ui element essential references
    /// </summary>
    [Serializable]
    public class UIElement_References
    {
        [Tooltip("This ui element animator which contains all possible animations")]
        public Animator ThisAnimator;

        [Tooltip("Not necessary. This ui element animator which contains all possible animations")]
        public NonDrawingGraphic ThisNonDrawingGraphic;

        [Tooltip("Not necessary. This ui element canvas. This component will be enabled or disabled to optimize render performance of single window")]
        public Canvas ThisCanvas;

        /// <summary>
        /// Get all essential references
        /// </summary>
        /// <param name="go"> Window parent Gameobject </param>
        public void CheckReferences(GameObject go)
        {
            if (ThisAnimator == null)
            {
                if (go.GetComponent<Animator>() != null)
                {
                    ThisAnimator = go.GetComponent<Animator>();
                }
                else
                {
                    Debug.LogError("Not found animator component on object: " + go.name);
                }
            }

            if (ThisNonDrawingGraphic == null)
            {
                if (go.GetComponent<NonDrawingGraphic>() != null)
                {
                    ThisNonDrawingGraphic = go.GetComponent<NonDrawingGraphic>();
                }
                else
                {
                    //Debug.Log("Not found ThisNonDrawingGraphic component on object: " + _go.name);
                }
            }

            if (ThisCanvas == null)
            {
                if (go.GetComponent<Canvas>() != null)
                {
                    ThisCanvas = go.GetComponent<Canvas>();
                }
                else
                {
                    //Debug.Log("Not found Canvas component on object: " + _go.name);
                }
            }



        }
    }


    /// <summary>
    /// Class which controls the way ui element is animated
    /// </summary>
    [Serializable]
    public class UIElement_ScaleSettings
    {
        public UIEStates_BeforeAppear BeforeAppear = UIEStates_BeforeAppear.FromZeroScale;
        public UIEStates_Appear Appear = UIEStates_Appear.Fancy;

        public UIEStates_Idle Idle = UIEStates_Idle.None;

        public UIEStates_Pressed Pressed = UIEStates_Pressed.Fancy;
        public UIEStates_Clicked Clicked = UIEStates_Clicked.Fancy;

        public UIEStates_Disappear Disappear = UIEStates_Disappear.Fancy;

        public UIEStates_Kick Kick = UIEStates_Kick.Bump;

        public float AppearSpeedMultiplier = 2f;
        public float DisappearSpeedMultiplier = 1f;

        /// <summary>
        /// Controls whether animation speed will be multilplied by timescale
        /// </summary>
        public bool TimeScaleSpeed = true;
    }

    /// <summary>
    /// Class with some runtime info
    /// </summary>
    [Serializable]
    public class UIElement_RuntimeParameters
    {
        public bool NotClickable = false;
        public UIEBasicStates LastAnimationType = UIEBasicStates.Idle;
        public bool IgnoreToWindowAnimateSearch = false;
    }

    /// <summary>
    /// Class which controls animation behavior of an ui element
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class UIElementBase : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {

        private UIElement_References _elementReferences;
        public UIElement_ScaleSettings AnimationScaleSettings;
        public UIElement_RuntimeParameters RuntimeParameters;

        /// <summary>
        /// List of active DOTweens
        /// </summary>
        private List<Tween> _activeTweens = new List<Tween>();

        /// <summary>
        /// List of active IEnumerator
        /// </summary>
        private List<IEnumerator> _activeIEnumerators = new List<IEnumerator>();

        private bool _initialized = false;

        /// <summary>
        /// Locks or unlocks this ui element so it can not be clickable
        /// </summary>
        /// <param name="val"></param>
        public void SetLock(bool val)
        {
            RuntimeParameters.NotClickable = val;
        }

        /// <summary>
        /// Check whether this ui element is clickable
        /// </summary>
        /// <returns></returns>
        public bool IsClickable()
        {
            if (RuntimeParameters.NotClickable || RuntimeParameters.LastAnimationType == UIEBasicStates.BeforeAppear)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// On Init Fires from parent window class
        /// </summary>
        /// <param name="enableAnimators"></param>
        public virtual void OnInit(bool enableAnimators = false)
        {
            _elementReferences = new UIElement_References();
            _elementReferences.CheckReferences(gameObject);
            EnableAnimator(enableAnimators);
            _initialized = true;
        }

        public virtual void Awake()
        {
            if (!_initialized) OnInit();
        }


        /// <summary>
        /// Refreshes all animator parameters
        /// </summary>
        private void RefreshAnimatorSettings()
        {
            if (AnimationScaleSettings.TimeScaleSpeed)
            {
                _elementReferences.ThisAnimator.speed = 1f / Time.timeScale;
            }
            else
            {
                _elementReferences.ThisAnimator.speed = 1f;
            }

            _elementReferences.ThisAnimator.SetFloat("AppearSpeedMultiplier", AnimationScaleSettings.AppearSpeedMultiplier);
            _elementReferences.ThisAnimator.SetFloat("DisappearSpeedMultiplier", AnimationScaleSettings.DisappearSpeedMultiplier);

            _elementReferences.ThisAnimator.SetInteger("BeforeAppearState", (int)AnimationScaleSettings.BeforeAppear);
            _elementReferences.ThisAnimator.SetInteger("AppearState", (int)AnimationScaleSettings.Appear);
            _elementReferences.ThisAnimator.SetInteger("IdleState", (int)AnimationScaleSettings.Idle);
            _elementReferences.ThisAnimator.SetInteger("ClickedState", (int)AnimationScaleSettings.Clicked);
            _elementReferences.ThisAnimator.SetInteger("PressedState", (int)AnimationScaleSettings.Pressed);
            _elementReferences.ThisAnimator.SetInteger("DisappearState", (int)AnimationScaleSettings.Disappear);
            _elementReferences.ThisAnimator.SetInteger("WhileIdleState", (int)AnimationScaleSettings.Kick);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsClickable()) return;

            SwitchAnimationTo(UIEBasicStates.Clicked);
            OnClick(eventData);
        }

        public void OnPointerDown(PointerEventData pointerEventData)
        {
            if (!IsClickable()) return;
            SwitchAnimationTo(UIEBasicStates.Pressed);

            OnPress(pointerEventData);
        }

        public void OnPointerUp(PointerEventData pointerEventData)
        {
            if (!IsClickable()) return;

            SwitchAnimationTo(UIEBasicStates.Idle);

            OnUnPress(pointerEventData);
        }


        /// <summary>
        /// Enables or disables animator component in order to avoid unnecesary render calls within canvas
        /// </summary>
        /// <param name="value"></param>
        private void EnableAnimator(bool value)
        {
            _elementReferences.ThisAnimator.enabled = value;
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
        }


        /// <summary>
        /// Change animation state of this element
        /// </summary>
        /// <param name="value"></param>
        public void SwitchAnimationTo(UIEBasicStates value)
        {
            if (!gameObject.activeInHierarchy) return;

            EnableAnimator(true);
            KillAllCoroutines();

            RuntimeParameters.LastAnimationType = value;
            _elementReferences.CheckReferences(gameObject);
            RefreshAnimatorSettings();

            _elementReferences.ThisAnimator.SetInteger("State", (int)value);
            _elementReferences.ThisAnimator.SetTrigger("ChangeState");


            //handling animator component 
            if (AnimationScaleSettings.Idle == UIEStates_Idle.None)
            {
                IEnumerator disableAnimatorCoro = IndependentCoroutines.CallbackDelay_IEnumerator(
                    AnimationScaleSettings.TimeScaleSpeed ? 
                    (0.5f * Time.timeScale / AnimationScaleSettings.DisappearSpeedMultiplier) 
                    : 0.5f / AnimationScaleSettings.DisappearSpeedMultiplier
                    , () =>
                {
                    EnableAnimator(false);
                });
                StartCoroutine(disableAnimatorCoro);
                _activeIEnumerators.Add(disableAnimatorCoro);


                /*
                Tween disableAnimatorTween = IndependentCoroutines.CallbackDelay_DoTween(0.5f, () =>
                {
                    EnableAnimator(false);
                });

                ActiveTweens.Add(disableAnimatorTween);
                */
            }
        }



        public virtual void OnClick(PointerEventData eventData)
        {

        }

        public virtual void OnPress(PointerEventData eventData)
        {

        }

        public virtual void OnUnPress(PointerEventData eventData)
        {

        }
    }
}
