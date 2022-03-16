using UnityEngine;
using UnityEditor;

namespace IWDev.UISolution
{
	[CustomEditor(typeof(WindowAnimationBase))]
	public class WindowAnimationBaseEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            WindowAnimationBase thisWindowAnimationBase = (WindowAnimationBase)target;

            if (GUILayout.Button("Get References"))
            {
                thisWindowAnimationBase.GetReferences();
            }

            DrawDefaultInspector();
        }
    }
}
