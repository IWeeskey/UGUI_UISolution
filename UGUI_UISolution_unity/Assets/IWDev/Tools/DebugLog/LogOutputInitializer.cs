using UnityEngine;

namespace IWDev.Tools
{
	/// <summary>
	/// I got this somewhere in internet. Originally created by VironIT.
	/// But I made a few changes.
	/// </summary>
	public class LogOutputInitializer : MonoBehaviour
	{
		public bool InitializeDebugLog = false;

		private void Awake()
		{
			if (InitializeDebugLog)
			{
				DebugHandler.Initialize();
				LogOutput.Initialize();
			}

		}
	}
}
