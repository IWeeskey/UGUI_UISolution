using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IWDev.UISolution;

public class ExampleBottomPanel : MonoBehaviour
{
	private WindowController ThisWindowController;

	/// <summary>
	/// Fires when window is initialized
	/// Note that this will not be called while not in play mode
	/// </summary>
	public void OnInit()
	{
		if (gameObject.GetComponent<WindowController>())
		{
			ThisWindowController = gameObject.GetComponent<WindowController>();
		}
		else
		{
			Debug.LogError("No WindowController component were found on object: " + gameObject);
			return;
		}

		Debug.Log("Init of a window: " + ThisWindowController.WindowName);
	}


	/// <summary>
	/// Fires before window switching on. Use this to handle animated elements 
	/// </summary>
	public void OnAppear_BeforeAnimation()
	{

	}


	/// <summary>
	/// Fires after window switching on
	/// </summary>
	public void OnAppear_AfterAnimation()
	{

	}


	/// <summary>
	/// Fires when window starting to close
	/// </summary>
	public void OnClose()
	{

	}

	public void ShowHide_ExampleWindow_Derived()
	{
		WindowsManager.Instance.SwitchWindowByName( WindowNames.ExampleWindow_Derived);
	}

	public void ShowHide_ExampleWindow_Detached()
	{
		WindowsManager.Instance.SwitchWindowByName(WindowNames.ExampleWindow_Detached);
	}
}
