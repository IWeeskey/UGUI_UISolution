using UnityEngine;

namespace IWDev.Tools
{
    /// <summary>
    /// I got this somewhere in internet. Originally created by VironIT.
    /// But I made a few changes.
    /// </summary>
    public class DebugHandler : MonoBehaviour
    {
        private const string GAME_OBJECT_NAME = "DebugHandler";

        public static void Initialize()
        {
            GameObject gameObject = GameObject.Find(GAME_OBJECT_NAME);
            if (gameObject == null)
            {
                gameObject = new GameObject(GAME_OBJECT_NAME);
                GameObject.DontDestroyOnLoad(gameObject);

                gameObject.AddComponent<DebugHandler>();
            }
        }


        public static event Application.LogCallback MessageReceived;
        private static void OnMessageReceived(string message, string stackTrace, LogType type)
        {
            if (MessageReceived != null)
                MessageReceived(message, stackTrace, type);
        }


        void OnEnable()
        {
            Application.RegisterLogCallback(HandleLog);
        }

        void OnDisable()
        {
            Application.RegisterLogCallback(null);
        }

        void HandleLog(string message, string stackTrace, LogType type)
        {
            OnMessageReceived(message, stackTrace, type);
        }
    }
}
