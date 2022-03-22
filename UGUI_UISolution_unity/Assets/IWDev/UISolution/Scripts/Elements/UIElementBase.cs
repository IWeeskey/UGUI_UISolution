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
        /// <param name="_go"> Window parent Gameobject </param>
        public void CheckReferences(GameObject _go)
        {
            if (ThisAnimator == null)
            {
                if (_go.GetComponent<Animator>() != null)
                {
                    ThisAnimator = _go.GetComponent<Animator>();
                }
                else
                {
                    Debug.LogError("Not found animator component on object: " + _go.name);
                }
            }

            if (ThisNonDrawingGraphic == null)
            {
                if (_go.GetComponent<NonDrawingGraphic>() != null)
                {
                    ThisNonDrawingGraphic = _go.GetComponent<NonDrawingGraphic>();
                }
                else
                {
                    //Debug.Log("Not found ThisNonDrawingGraphic component on object: " + _go.name);
                }
            }

            if (ThisCanvas == null)
            {
                if (_go.GetComponent<Canvas>() != null)
                {
                    ThisCanvas = _go.GetComponent<Canvas>();
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
    }

    /// <summary>
    /// Class with some runtime info
    /// </summary>
    [Serializable]
    public class UIElement_RuntimeParameters
    {
        public bool NotClickable = false;
        public UIEBasicStates LastAnimationType = UIEBasicStates.Idle;
    }

    /// <summary>
    /// Class which controls animation behavior of an ui element
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class UIElementBase : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {

        private UIElement_References ElementReferences;
        public UIElement_ScaleSettings AnimationScaleSettings;
        public UIElement_RuntimeParameters RuntimeParameters;

        /// <summary>
        /// List of active DOTweens
        /// </summary>
        private List<Tween> ActiveTweens = new List<Tween>();

        /// <summary>
        /// List of active IEnumerator
        /// </summary>
        private List<IEnumerator> ActiveIEnumerators = new List<IEnumerator>();

        /// <summary>
        /// Locks or unlocks this ui element so it can not be clickable
        /// </summary>
        /// <param name="_val"></param>
        public void SetLock(bool _val)
        {
            RuntimeParameters.NotClickable = _val;
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
        /// <param name="_enableAnimators"></param>
        public virtual void OnInit(bool _enableAnimators = false)
        {
            ElementReferences = new UIElement_References();
            ElementReferences.CheckReferences(gameObject);
            EnableAnimator(_enableAnimators);
        }


        /// <summary>
        /// Refreshes all animator parameters
        /// </summary>
        private void RefreshAnimatorSettings()
        {
            ElementReferences.ThisAnimator.SetInteger("BeforeAppearState", (int)AnimationScaleSettings.BeforeAppear);
            ElementReferences.ThisAnimator.SetInteger("AppearState", (int)AnimationScaleSettings.Appear);
            ElementReferences.ThisAnimator.SetInteger("IdleState", (int)AnimationScaleSettings.Idle);
            ElementReferences.ThisAnimator.SetInteger("ClickedState", (int)AnimationScaleSettings.Clicked);
            ElementReferences.ThisAnimator.SetInteger("PressedState", (int)AnimationScaleSettings.Pressed);
            ElementReferences.ThisAnimator.SetInteger("DisappearState", (int)AnimationScaleSettings.Disappear);
            ElementReferences.ThisAnimator.SetInteger("WhileIdleState", (int)AnimationScaleSettings.Kick);
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
        /// <param name="_value"></param>
        private void EnableAnimator(bool _value)
        {
            ElementReferences.ThisAnimator.enabled = _value;
        }

        /// <summary>
        /// Kills all active tweens and coroutines
        /// </summary>
        private void KillAllCoroutines()
        {
            foreach (Tween tw in ActiveTweens)
            {
                if (tw != null)
                {
                    tw.Kill();
                }
            }
            ActiveTweens.Clear();

            foreach (IEnumerator _IEnum in ActiveIEnumerators)
            {
                if (_IEnum != null) StopCoroutine(_IEnum);
            }

            ActiveIEnumerators.Clear();
        }


        /// <summary>
        /// Change animation state of this element
        /// </summary>
        /// <param name="_value"></param>
        public void SwitchAnimationTo(UIEBasicStates _value)
        {
            EnableAnimator(true);
            KillAllCoroutines();

            RuntimeParameters.LastAnimationType = _value;
            ElementReferences.CheckReferences(gameObject);
            RefreshAnimatorSettings();

            ElementReferences.ThisAnimator.SetInteger("State", (int)_value);
            ElementReferences.ThisAnimator.SetTrigger("ChangeState");


            //handling animator component 
            if (AnimationScaleSettings.Idle == UIEStates_Idle.None)
            {
                IEnumerator _DisableAnimatorCoro = IndependentCoroutines.CallbackDelay_IEnumerator(0.5f, () =>
                {
                    EnableAnimator(false);
                });
                StartCoroutine(_DisableAnimatorCoro);
                ActiveIEnumerators.Add(_DisableAnimatorCoro);


                /*
                Tween _DisableAnimatorTween = IndependentCoroutines.CallbackDelay_DoTween(0.5f, () =>
                {
                    EnableAnimator(false);
                });

                ActiveTweens.Add(_DisableAnimatorTween);
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
