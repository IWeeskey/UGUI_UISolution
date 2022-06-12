using IWDev.Tools;
using System;
using UnityEngine.EventSystems;


namespace IWDev.UISolution
{

	/// <summary>
	/// Class which contains this window events subscription
	/// </summary>
	[Serializable]
	public class WindowController_Events
	{
		//Triggers when window is initialized
		public EventTrigger.TriggerEvent OnInit;

		//Triggers before window switching on. Use this to handle animated elements 
		public EventTrigger.TriggerEvent OnAppear_BeforeAnimation;

		//Triggers after window switching on
		public EventTrigger.TriggerEvent OnAppear_AfterAnimation;

		//Triggers when window starting to close
		public EventTrigger.TriggerEvent OnClose;
	}

	/// <summary>
	/// Class with common settings such as playing sounds on Open/Close
	/// </summary>
	[Serializable]
	public class WindowController_Settings
	{
		public bool PlaySoundOnOpen = true;
		public bool PlaySoundOnClose = true;
	}

		
	/// <summary>
	/// Derived class from WindowAnimationBase class. This class controls window behavior.
	/// Inherit your own window classes from this one.
	/// </summary>
	public class WindowController : WindowAnimationBase
	{
		/// <summary>
		/// Name of this window
		/// </summary>
		public WindowNames WindowName = WindowNames.None;
		public WindowController_Events WindowEvents;
		public WindowController_Settings WindowCommonSettings;


		/// <summary>
		/// Init this window
		/// </summary>
		public void WindowInit()
		{
			OnInit();
			WindowEvents.OnInit.Invoke((PointerEventData)null);
		}

		/// <summary>
		/// Closes this window directly. I do not recommend using this method. See CloseFromButton().
		/// </summary>
		public void CloseWindow()
		{
			if (!WindowRuntimeParameters.IsActive) return;

			WindowEvents.OnClose.Invoke((PointerEventData)null);
			OnClose();

			AnimateDisAppear();

			IndependentCoroutines.CallbackDelay_DoTween(.1f, () =>
			{
				if (WindowCommonSettings.PlaySoundOnClose)
				{
					//play some sound here
				}
			});
		}


		/// <summary>
		/// Opens this window directly
		/// </summary>
		public void OpenWindow()
		{
			WindowEvents.OnAppear_BeforeAnimation.Invoke((PointerEventData)null);
			OnAppear_BeforeAnimation();

			AnimateAppear();

			WindowEvents.OnAppear_AfterAnimation.Invoke((PointerEventData)null);
			OnAppear_AfterAnimation();

			IndependentCoroutines.CallbackDelay_DoTween(.1f, ()=>
			{
				if (WindowCommonSettings.PlaySoundOnOpen)
				{
					//play some sound here
				}
			});
		}


		/// <summary>
		/// Closes this window through WindowsManager. I strongly recommed call this instead of CloseWindow() 
		/// bacause in this way you can add some global check logic in WindowsManager
		/// </summary>
		public void CloseByButton()
		{
			WindowsManager.Instance.CloseWindowByName(WindowName);
		}

		/// <summary>
		/// Fires when window is initialized
		/// </summary>
		protected override void OnInit(bool isWorld = false)
		{
			base.OnInit(isWorld);
		}

		/// <summary>
		/// Fires before window switching on. Use this to handle animated elements 
		/// </summary>
		protected virtual void OnAppear_BeforeAnimation()
		{

		}

		/// <summary>
		/// Fires after window switching on
		/// </summary>
		protected virtual void OnAppear_AfterAnimation()
		{

		}

		/// <summary>
		/// Fires when window starting to close
		/// </summary>
		protected virtual void OnClose()
		{

		}
	}
}
