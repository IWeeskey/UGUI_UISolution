using UnityEngine;
using IWDev.UISolution;

/// <summary>
/// 
/// </summary>
public class ExampleWindow_Detached : MonoBehaviour
{
	private WindowController _thisWindowController;


	/// <summary>
	/// Awake method. U should not use it because windows can be disabled at start of an app. Check out OnInit instead
	/// </summary>
	private void Awake()
	{

	}

	/// <summary>
	/// Fires when window is initialized
	/// Note that this will not be called while not in play mode
	/// </summary>
	public void OnInit()
	{
		if (gameObject.GetComponent<WindowController>())
		{
			_thisWindowController = gameObject.GetComponent<WindowController>();
		}
		else
		{
			Debug.LogError("No WindowController component were found on object: " + gameObject);
			return;
		}

		Debug.Log("Init of a window: " + _thisWindowController.WindowName);
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
}
