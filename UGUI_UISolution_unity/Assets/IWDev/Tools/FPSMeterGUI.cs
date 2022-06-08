using System.Collections;
using UnityEngine;

namespace IWDev.Tools
{
	public class FPSMeterGUI : MonoBehaviour
	{
		public static FPSMeterGUI Instance;

		public bool CalculateFPS = true;
		public int CurrentFps;
		public Color FPSColor = Color.black;
		public string Prefix = "FPS: ";


		private GUIStyle _fPSLabelStyle;
		private Rect _fPSRect = new Rect();

		private bool _isPortrait = true;

		private void Awake()
		{
			Instance = this;

			_isPortrait = true;

			if ((float)Screen.height / (float)Screen.width < 1f)
			{
				_isPortrait = false;
			}

			if (_isPortrait)
			{
				_fPSRect = new Rect(5,
				Screen.height - Screen.height * 0.025f - 20,
				(int)Screen.width * 0.25f,
				(int)Screen.height * 0.025f + 20);
			}
			else
			{
				_fPSRect = new Rect(5,
					Screen.height - Screen.height * 0.025f - 50,
					(int)Screen.width * 0.25f,
					(int)Screen.height * 0.025f + 50);
			}


		}



		private IEnumerator Start()
		{
			while (true)
			{
				if (CalculateFPS)
				{
					CurrentFps = (int)(1f / Time.unscaledDeltaTime);
				}
				else
				{ 
				
				}

				yield return new WaitForSeconds(.5f);
			}
		}

		private void OnGUI()
		{
			if (!CalculateFPS) return;


			_fPSLabelStyle = new GUIStyle(GUI.skin.label);

			if (_isPortrait) _fPSLabelStyle.fontSize = (int)(Screen.width * 0.05f);
			else  _fPSLabelStyle.fontSize = (int)(Screen.height * 0.05f);

			GUI.contentColor = FPSColor;
			GUI.Label(_fPSRect, Prefix + CurrentFps.ToString(), _fPSLabelStyle);
		}
		
	
	}
}
