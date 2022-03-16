using UnityEngine;
using System;


namespace IWDev.Tools
{
	[ExecuteInEditMode]
	public class ScreenShotCapture : MonoBehaviour
	{

		[Tooltip("Path to save screenshots")]
		public string SavePath = "C:/";

		[Tooltip("Prefix to screenshot file name")]
		public string NamePrefix = "TestScreenShot";

		[Tooltip("KeyCode to capture screenshot")]
		public KeyCode CaptureKey = KeyCode.C;

		[Tooltip("KeyCode to pause game")]
		public KeyCode PauseKey = KeyCode.P;

		[Tooltip("Resolution scale of screenshots")]
		public int SuperSize = 1;

		[Tooltip("Bool value change it to capture screenshot while not in play mode")]
		public bool EditorCaptureScreen = false;

#if UNITY_EDITOR
		private void Update()
		{
			if (Input.GetKeyDown(CaptureKey) || EditorCaptureScreen)
			{
				EditorCaptureScreen = false;

				string path = SavePath;
				string screenName = NamePrefix + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "_" +
				DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + ".png";

				ScreenCapture.CaptureScreenshot(path + screenName, SuperSize);
				Debug.Log(path + screenName);
			}

			if (Input.GetKeyDown(PauseKey))
			{
				Debug.Break();
			}
		}
#endif

	}
}
