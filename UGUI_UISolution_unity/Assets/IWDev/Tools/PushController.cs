using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif


#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
namespace IWDev.Tools
{

    /// <summary>
    /// Class which handles pushes logic 
    /// </summary>
    public class PushController : MonoBehaviour
    {
        /// <summary>
        /// Singleton of PushController 
        /// </summary>
        public static PushController Instance;

        /// <summary>
        /// Variable to check whether Android Channel is Created
        /// </summary>
        private bool AndroidChannelCreated = false;

        /// <summary>
        /// Universal android channel id
        /// </summary>
        private string AndroidChannelID = "push_channel_id";


        /// <summary>
        /// if of test live push for android
        /// </summary>
        private int LivesPushID = 123321;

        /// <summary>
        /// if of test live push for ios
        /// </summary>
        private static string iOSLivesIdentifier = "test_push_id";


        /// <summary>
        /// Awake init if singleton
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Test start method to lauch test push
        /// </summary>
        private void Start()
        {
            StartTestPush();
        }


        public void StartTestPush()
        {
            CheckCreateChannel();
            ScheduleLivesPushNotification(60);
        }


        /// <summary>
        /// Method that checks whether android channel created and if not creates it
        /// </summary>
        void CheckCreateChannel()
        {

#if UNITY_ANDROID
            if (!AndroidChannelCreated)
            {

                var c = new AndroidNotificationChannel()
                {
                    Id = AndroidChannelID,
                    Name = "IWDev Push",
                    Importance = Importance.High,
                    Description = "Generic notifications",
                };
                AndroidNotificationCenter.RegisterNotificationChannel(c);

                AndroidChannelCreated = true;
            }
#endif

        }

        /// <summary>
        /// Breaks scheduled test lives pushes 
        /// </summary>
        void BreakOldLivesPushNotification()
        {
#if UNITY_ANDROID
            var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(LivesPushID);

            if (notificationStatus == NotificationStatus.Scheduled)
            {
                AndroidNotificationCenter.CancelNotification(LivesPushID);
            }
            else if (notificationStatus == NotificationStatus.Delivered)
            {
                AndroidNotificationCenter.CancelNotification(LivesPushID);
            }
            else if (notificationStatus == NotificationStatus.Unknown)
            {
            }
#endif


#if UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification(iOSLivesIdentifier);
#endif
        }


        /// <summary>
        /// Requesting push permission specifically for ios
        /// </summary>
        void IOS_RequestPermission()
        {
            if (_inIOSRequest) return;
            StartCoroutine(RequestAuthorization());
        }

        bool _inIOSRequest = false;

        IEnumerator RequestAuthorization()
        {
            _inIOSRequest = true;

#if UNITY_IOS

        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Sound;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log(res);
        }

#endif


            yield return null;
            _inIOSRequest = false;
        }


        /// <summary>
        /// Schedule test lives push notification
        /// </summary>
        /// <param name="_seconds"></param>
        void ScheduleLivesPushNotification(double _seconds)
        {
#if UNITY_ANDROID


            var notification = new AndroidNotification();
            notification.Title = "Test Lives Title";
            notification.Text = "Test Lives text";
            notification.SmallIcon = "icon_0";
            notification.LargeIcon = "icon_1";



            notification.FireTime = System.DateTime.Now.AddSeconds(_seconds);
            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, AndroidChannelID, LivesPushID);
#endif

#if UNITY_IOS

        IOS_RequestPermission();

        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0,0, _seconds),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            Identifier = iOSLivesIdentifier,
            Title = "Test Lives Title",
            Subtitle = "",
            Body = "Test Lives text",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
#endif
        }




        /// <summary>
        /// Breaks all scheduled pushes
        /// </summary>
        public void BreakStopAllPushes()
        {
            Debug.Log("BREAK ALL PUSHES");
            BreakOldLivesPushNotification();
        }
    }
}
