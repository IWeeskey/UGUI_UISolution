using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace IWDev.UISolution
{
    public class WindowsManager : MonoBehaviour
    {
        public static WindowsManager Instance;
        public List<WindowController> AllWindows;
        private bool _locked = false;


        private void Awake()
        {
            Instance = this;
            InitWindows();


        }

        public bool InterfaceLocked()
        {
            return _locked;
        }

        /// <summary>
        /// Simple logic to prevent 
        /// </summary>
        public void SmallLock()
        {
            _locked = true;

            Tween _tween = DOVirtual.Float(0f, 1f, 0.5f, (float val) =>
            { }).OnComplete(() =>
            {
                _locked = false;
            });
        }

        /// <summary>
        /// Init all windows
        /// </summary>
        public void InitWindows()
        {
            foreach (WindowController wind in AllWindows)
            {
                wind.WindowInit();
            }
        }


        public void GetAllWindows()
        {
            AllWindows = gameObject.GetComponentsInChildren<WindowController>(true).ToList();
            Debug.Log("Successfully found: " + AllWindows.Count + " windows");
        }

        public bool IsWindowOpened(WindowNames _wName)
        {
            bool _opnd = false;

            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == _wName && wind.WindowRuntimeParameters.IsActive)
                {
                    _opnd = true;
                }
            }

            return _opnd;
        }

        public void OpenWindowByName(WindowNames _wName)
        {
            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == _wName)
                {
                    wind.OpenWindow();
                    return;
                }
            }



            Debug.LogError("Not found window with name: " + _wName);
        }

        public void CloseWindowByName(WindowNames _wName)
        {
            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == _wName)
                {
                    wind.CloseWindow();
                    return;
                }
            }

            Debug.LogError("Not found window with name: " + _wName);
        }

        public WindowController GetWindowByName(WindowNames _wName)
        {
            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == _wName)
                {
                    return wind;
                }
            }

            Debug.LogError("Not found window with name: " + _wName);
            return null;
        }

        public void SwitchWindowByName(WindowNames _wName)
        {
            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == _wName)
                {
                    if (wind.WindowRuntimeParameters.IsActive)
                    {
                        wind.CloseWindow();
                    }
                    else
                    {
                        wind.OpenWindow();
                    }
                    return;
                }
            }

            Debug.LogError("Not found window with name: " + _wName);
        }
    }
}
