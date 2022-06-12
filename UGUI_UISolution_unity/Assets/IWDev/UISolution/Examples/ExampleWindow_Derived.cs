using IWDev.UISolution;
using UnityEngine;

/// <summary>
/// Example of class inherited from WindowController class.
/// Here you can override basic events such as OnInit, OnClose and OnAppear.
/// Drawback of this way to make a window class - is that u have to get window references by your own:
/// - write your own class editor script and inherit it from WindowControllerEditor. 
///		But I wouldn't recommend this method because in case of a big game there will be hundreds of windows and there is no point to create editor versions for all of them
///	- use some self coded logic to call method WindowAnimationBase.GetReferences() (e.g. use a bool variable as trigger and check it while in unity editor)
///	- use Odin inspector to create custom button for method GetReferences()
/// </summary>
[ExecuteInEditMode]
public class ExampleWindow_Derived : WindowController
{
	/// <summary>
	///  Instance of this window. So u can reach this window from anywhere at anytime, ofc if it exists in the scene
	/// </summary>
	public static ExampleWindow_Derived Instance;

	/// <summary>
	/// Here is an example of how easily call GetReferences() method in editor
	/// </summary>
#if UNITY_EDITOR
	public bool GetWindowReferences = false;
	private void Update()
	{
		if (GetWindowReferences)
		{
			GetWindowReferences = false;
			base.GetReferences();
		}
	}
#endif


	/// <summary>
	/// Awake method. You should not use it because windows can be disabled at start of an app. Check out OnInit instead
	/// </summary>
	private void Awake()
	{

	}

	/// <summary>
	/// Init method. It is called when WindowsManager is initialized.
	/// Write here some singleton logic or what you wish to
	/// </summary>
	protected override void OnInit(bool isWorld = false)
	{
		base.OnInit(isWorld);
		Instance = this;
		Debug.Log("Init of a window: " + base.WindowName);
	}

	/// <summary>
	/// Fires before window switching on. Use this to handle animated elements 
	/// </summary>
	protected override void OnAppear_BeforeAnimation()
	{
		base.OnAppear_BeforeAnimation();
	}

	/// <summary>
	/// Fires after window switching on
	/// </summary>
	protected override void OnAppear_AfterAnimation()
	{
		base.OnAppear_AfterAnimation();
	}

	/// <summary>
	/// Fires when window starting to close
	/// </summary>
	protected override void OnClose()
	{
		base.OnClose();
	}
}
