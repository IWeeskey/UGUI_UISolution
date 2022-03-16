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


		private GUIStyle FPSLabelStyle;
		private Rect FPSRect = new Rect();

		private void Awake()
		{
			Instance = this;

			FPSRect = new Rect(5, 
				Screen.height - Screen.height * 0.025f-20, 
				(int)Screen.width*0.25f, 
				(int)Screen.height * 0.025f + 20);
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


			FPSLabelStyle = new GUIStyle(GUI.skin.label);
			FPSLabelStyle.fontSize = (int)(Screen.width * 0.05f);

			GUI.contentColor = FPSColor;
			GUI.Label(FPSRect, Prefix + CurrentFps.ToString(), FPSLabelStyle);
		}
		
	
	}
}
