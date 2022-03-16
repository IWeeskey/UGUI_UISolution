using UnityEngine;
using UnityEditor;


namespace IWDev.UISolution
{
    [CustomEditor(typeof(WindowsManager))]
    public class WindowsManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            WindowsManager thisWindowsManager = (WindowsManager)target;

            if (GUILayout.Button("Get Windows"))
            {
                thisWindowsManager.GetAllWindows();
            }

            if (GUILayout.Button("Reset Windows"))
            {
                thisWindowsManager.InitWindows();
            }

            DrawDefaultInspector();
        }
    }
}
