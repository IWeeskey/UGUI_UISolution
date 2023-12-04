using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IWDev.Tools
{



    [Serializable]
    public class LerpUnit
    {
        [SerializeField] private float _startDelay = 0f;
        [SerializeField] private float _endDelay = 0f;
        [SerializeField] private float _lerpLength = 0f;

        [SerializeField] private int _stepsCount = 0;
        [SerializeField] private int _repeatCount = 0;

        [SerializeField] private int _currentRepeat = 0;

        private Action<float> _lerpAction;

        private Action _afterStartAction;
        private Action _afterLerpAction;
        private Action _afterEndAction;

        [SerializeField] public bool ApplyGlobalPause = true;
        [SerializeField] private bool _selfPause = false;

        [SerializeField] private float _currentStartDelay_progress = 0;
        [SerializeField] private float _currentEndDelay_progress = 0;
        [SerializeField] private float _currentLerp_progress = 0;

        [SerializeField] private bool _startDelayDone = false;
        [SerializeField] private bool _lerpDone = false;
        [SerializeField] private bool _endDelayDone = false;

        [SerializeField] public bool IsRunning = false;
        [SerializeField] public bool Completed = false;

        [SerializeField] private float _step = 0;
        [SerializeField] private float _nextStep = 0;

        /// <summary>
        /// LerpUnit is a universal class that lerp from 0 to 1f every frame
        /// </summary>
        /// <param name="startDelay">delay in seconds to start this lerp unit</param>
        /// <param name="endDelay">delay in seconds after lerp logic is done</param>
        /// <param name="lerpLength">length ot the lerp in seconds</param>
        /// <param name="afterStartAction">action which triggers right after startDelay</param>
        /// <param name="afterEndAction">action which triggers right after startDelay+lerpLength+endDelay</param>
        /// <param name="lerpAction">action which iterates every frame and returns a float value from 0 to 1 representins a lerp progress</param>
        /// <param name="afterLerpAction">action which triggers right after startDelay+lerpLength </param>
        /// <param name="applyPause">if this is true all logic will be freezed during global pause</param>
        /// <param name="repeatCount">number of repeats, eg 0 - no repeats and only 1 iterations of lerp unit. For example 2 means 1 + 2 repeats</param>
        /// <param name="stepsCount">if this value is more then 0 it means lerp action will be called stepsCount times instead of every frame</param>
        public LerpUnit(float startDelay, float endDelay, float lerpLength,
            Action afterStartAction, Action afterEndAction, Action<float> lerpAction, Action afterLerpAction,
            bool applyPause = true, int repeatCount = 0, int stepsCount = 0)
        {
            _selfPause = false;
            _startDelay = startDelay;
            _endDelay = endDelay;
            _lerpLength = lerpLength;
            _stepsCount = stepsCount;
            _repeatCount = repeatCount;

            _afterStartAction = afterStartAction;
            _afterEndAction = afterEndAction;
            _lerpAction = lerpAction;
            _afterLerpAction = afterLerpAction;

            ApplyGlobalPause = applyPause;
            _currentStartDelay_progress = 0;
            _currentEndDelay_progress = 0;
            _currentLerp_progress = 0;
            _currentRepeat = 0;

            _startDelayDone = false;
            _lerpDone = false;
            _endDelayDone = false;

            Completed = false;
        }

        public void Reset()
        {
            SoftReset();
            _currentRepeat = 0;
            Completed = false;
        }

        private void SoftReset()
        {
            _startDelayDone = false;
            _lerpDone = false;
            _endDelayDone = false;

            _currentStartDelay_progress = 0;
            _currentEndDelay_progress = 0;
            _currentLerp_progress = 0;


            _step = 0;

            if (_stepsCount > 0) _step = _lerpLength / (float)_stepsCount;
            _nextStep = _step;
        }

        public void UpdateLerp(float deltaTime)
        {
            if (_selfPause) return;

            deltaTime *= Time.timeScale;

            if (!_startDelayDone)
            {
                _currentStartDelay_progress += deltaTime;
                if (_currentStartDelay_progress >= _startDelay)
                {
                    _startDelayDone = true;
                    _afterStartAction();
                }
                else
                {
                    return;
                }
            }

            if (!_lerpDone)
            {
                _currentLerp_progress += deltaTime;

                //if it is step lerp
                if (_stepsCount > 0)
                {
                    if (_currentLerp_progress >= _nextStep)
                    {
                        _nextStep += _step;
                        _lerpAction(_currentLerp_progress / _lerpLength);
                    }
                }
                //if it is linear lerp
                else 
                {
                    if (_currentLerp_progress > _lerpLength) _currentLerp_progress = _lerpLength;
                    _lerpAction(_currentLerp_progress / _lerpLength);
                }

                //check if lerp is done
                if (_currentLerp_progress >= _lerpLength)
                {
                    _lerpDone = true;
                    _afterLerpAction();
                }
                else
                {
                    return;
                }
            }

            if (_currentRepeat < _repeatCount)
            {
                _currentRepeat++;
                SoftReset();
                return;
            }


            if (!_endDelayDone)
            {
                _currentEndDelay_progress += deltaTime;
                if (_currentEndDelay_progress >= _endDelay)
                {
                    _endDelayDone = true;
                    IsRunning = false;
                    _afterEndAction();
                    Completed = true;
                    
                }
                else
                {
                    return;
                }
            }

        }


        public void Run()
        {
            SmartLerpManager.Instance.StopLerpUnit(this);
            Reset();
            IsRunning = true;
            SmartLerpManager.Instance.AddLerpUnit(this);
        }

        public void Stop()
        {
            IsRunning = false;
            SmartLerpManager.Instance.StopLerpUnit(this);
        }

        public void Pause(bool value)
        {
            _selfPause = value;
        }

    }

    [ExecuteInEditMode]
    public class SmartLerpManager : MonoBehaviour
    {
        private static SmartLerpManager instance = null;
        public static SmartLerpManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (SmartLerpManager)FindObjectOfType(typeof(SmartLerpManager));
                }
                return instance;
            }
        }


        [SerializeField] private List<LerpUnit> _lerpUnits = new List<LerpUnit>();
        //public bool Pause = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Update()
        {
            UpdateUnits();
        }


        bool IsGlobalPauseEnabled()
        {
            //return GameController.Instance.Paused;

            return false;
        }

        public void AddLerpUnit(LerpUnit unit)
        {
            if (_lerpUnits.Contains(unit)) return;
            _lerpUnits.Add(unit);

            if (unit.ApplyGlobalPause && IsGlobalPauseEnabled()) return;
            //update immediataly
            unit.UpdateLerp(Time.deltaTime);
        }

        void UpdateUnits()
        {
            //if (GameController.Instance != null && GameController.Instance.Paused) return;

            for (int index = _lerpUnits.Count - 1; index >= 0; index--)
            {
                //skip if it is global pause
                if (_lerpUnits[index].ApplyGlobalPause && IsGlobalPauseEnabled()) continue;

                _lerpUnits[index].UpdateLerp(Time.deltaTime);
                if (_lerpUnits[index].Completed)
                {
                    StopLerpUnit(_lerpUnits[index]);
                    continue;
                }
            }
        }

        public void StopLerpUnit(LerpUnit unit)
        {
            if (!_lerpUnits.Contains(unit)) return;
            _lerpUnits.Remove(unit);
        }
    }
}
