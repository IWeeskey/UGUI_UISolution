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
        /// Simple logic to handle locking 
        /// </summary>
        public void SmallLock()
        {
            _locked = true;

            Tween tween = DOVirtual.Float(0f, 1f, 0.5f, (float val) =>
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
            bool opnd = false;

            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == _wName && wind.WindowRuntimeParameters.IsActive)
                {
                    opnd = true;
                }
            }

            return opnd;
        }

        public void OpenWindowByName(WindowNames wName)
        {
            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == wName)
                {
                    wind.OpenWindow();
                    return;
                }
            }



            Debug.LogError("Not found window with name: " + wName);
        }

        public void CloseWindowByName(WindowNames wName)
        {
            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == wName)
                {
                    wind.CloseWindow();
                    return;
                }
            }

            Debug.LogError("Not found window with name: " + wName);
        }

        public WindowController GetWindowByName(WindowNames wName)
        {
            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == wName)
                {
                    return wind;
                }
            }

            Debug.LogError("Not found window with name: " + wName);
            return null;
        }

        public void SwitchWindowByName(WindowNames wName)
        {
            foreach (WindowController wind in AllWindows)
            {
                if (wind.WindowName == wName)
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

            Debug.LogError("Not found window with name: " + wName);
        }
    }
}
