using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IWDev.UISolution;
using IWDev.Tools;

public class ExampleBottomPanel : MonoBehaviour
{
	private WindowController _thisWindowController;

	LerpUnit _clickBlockLerp;

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

		_clickBlockLerp = new LerpUnit(0f, 0f, 0.5f, ()=>
		{ 
			//start
		}, ()=> 
		{
			//end
		}, (float lerp)=> 
		{ 
			//lerp from 0 to 1f
		}, ()=>
		{
			//after lerp
		}, 
		false,//apply global pause
		0,//repeats count
		0);//steps count

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

	CoroutineTask _clickBlock;
	public void ShowHide_ExampleWindow_Derived()
	{
		//if (_clickBlock != null && _clickBlock.Running) return;
		//_clickBlock = new CoroutineTask(IndependentCoroutines.WaitForExactTime(0.5f));

		if (_clickBlockLerp.IsRunning) return;
		_clickBlockLerp.Run();

		WindowsManager.Instance.SwitchWindowByName( WindowNames.ExampleWindow_Derived);
	}

	public void ShowHide_ExampleWindow_Detached()
	{
		//it closes immediately coz of click on ExampleWindow_Detached background
		//Debug.Log("0");
		//if (_clickBlock != null && _clickBlock.Running)
		//{
		//	Debug.Log("0 - 1");
		//	return;
		//}
		//_clickBlock = new CoroutineTask(IndependentCoroutines.WaitForExactTime(1.5f));
		//Debug.Log("1");

		WindowsManager.Instance.SwitchWindowByName(WindowNames.ExampleWindow_Detached);
	}
}
