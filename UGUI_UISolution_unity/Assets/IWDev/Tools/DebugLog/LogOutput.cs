using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace IWDev.Tools
{
    /// <summary>
    /// I got this somewhere in internet. Originally created by VironIT.
    /// But I made a few changes.
    /// </summary>
    public class LogOutput : MonoBehaviour
    {
        private class ListItem
        {
            public GUIContent Content { get; set; }
            public Rect Position { get; set; }
            public Color Color { get; set; }
        }



        private class LogItem
        {
            public DateTime Time { get; set; }
            public LogType Type { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }

			public override string ToString()
			{
				string msg = string.Format("{0} [{1}] {2}", Time.ToString("HH:mm:ss.fff"), Type.ToString().ToUpper(), Message);
				if (!string.IsNullOrEmpty(StackTrace))
				{
					switch (Type)
					{
					case LogType.Exception:
					case LogType.Error:
						msg += "\r\n" + StackTrace;
						break;
					}
				}
				
				return msg.Trim();
			}
        }


        private bool _isVisible = false;

        public bool skip = false;

        private List<LogItem> _toAdd = new List<LogItem>();
        //private string _strLog = string.Empty;
        private Vector2 _scrollPosition = Vector2.zero;

        private Vector2 _lastMousePos = Vector2.zero;
        private bool _isMousePressed = false;

        private List<ListItem> _list = new List<ListItem>();


		private Rect _buttonRect = new Rect();

        private Rect _rect = new Rect();
        private Rect _viewRect = new Rect();
        private Rect _showHideRect = new Rect();


        public const string GAME_OBJECT_NAME = "LogGameObject";

        public static void Initialize()
        {
            GameObject logGameObject = GameObject.Find(GAME_OBJECT_NAME);
            if (logGameObject == null)
            {
                logGameObject = new GameObject(GAME_OBJECT_NAME);
                logGameObject.AddComponent<LogOutput>();

                GameObject.DontDestroyOnLoad(logGameObject);
            }
            else
            {
                LogOutput log = logGameObject.GetComponent<LogOutput>();
                if (log == null)
                    logGameObject.AddComponent<LogOutput>();
            }
        }


        void Awake()
        {

            float buttonWidth = Screen.width * 0.3f;
            float buttonHeight = Screen.height * 0.1f;
            _buttonRect = new Rect((Screen.width - buttonWidth) * 0.5f, Screen.height - buttonHeight - 8, buttonWidth, buttonHeight);

            //float width = (Screen.height > Screen.width) ? (Screen.width - 8) : (Screen.width / 2);
            float width = Screen.width - 40;
			float height = Screen.height/2;//Screen.height
			_rect = new Rect((Screen.width - width) * 0.5f, height, width - 8, height - buttonHeight - 20);
            //_rect = new Rect((Screen.width - width) * 0.5f, height, width - 8, height - 40);

            float viewWidth = width - 16;
            //float viewHeight = style.CalcHeight(content, viewWidth);
            _viewRect = new Rect(0, 0, viewWidth - 8, 0);

			height = Screen.height / 16;
            _showHideRect = new Rect(8, Screen.height - height, width, height);


        }

        void OnDestroy()
        {

        }


        void Update()
        {
			Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

            if (_isVisible)
            {
                Rect testRect = new Rect(_rect);
                testRect.width -= 24;

                if (Input.GetMouseButtonDown(0) && testRect.Contains(mousePos))
                {
                    _isMousePressed = true;
                    _lastMousePos = mousePos;
                }
                
                if (Input.GetMouseButtonUp(0))
                {
                    _isMousePressed = false;
                }


                if (_isMousePressed && mousePos != _lastMousePos)
                {
                    _scrollPosition -= new Vector2(0, mousePos.y - _lastMousePos.y);
                    _lastMousePos = mousePos;
                }                
            }
        }


        // scr - что, dst - в чем
        public static bool IsContains(Rect rect1, Rect rect2)
        {
            return ((rect1.xMin <= rect2.xMin && rect2.xMin <= rect1.xMax) || (rect2.xMin <= rect1.xMin && rect1.xMin <= rect2.xMax)) &&
                ((rect1.yMin <= rect2.yMin && rect2.yMin <= rect1.yMax) || (rect2.yMin <= rect1.yMin && rect1.yMin <= rect2.yMax));
        }

        private int FindVisibleItemIndex()
        {
            int first = 0;
            int last = _list.Count - 1;

            if (first == last)
            {
                Rect rect = _list[first].Position;
                rect.x += (_rect.x - _scrollPosition.x);
                rect.y += (_rect.y - _scrollPosition.y);

                if (IsContains(_rect, rect))
                    return first;

                return -1;
            }

            while (first < last)
            {
                int middle = first + (last - first) / 2;

                Rect rect = _list[middle].Position;
                rect.x += (_rect.x - _scrollPosition.x);
                rect.y += (_rect.y - _scrollPosition.y);

                if (IsContains(_rect, rect))
                {
                    return middle;
                }
                else if (rect.yMin < _rect.y)
                {
                    first = middle + 1;
                }
                else
                {
                    last = middle;
                }
            }

            return -1;
        }

        private ListItem[] GetVisibleItems()
        {
            List<ListItem> items = new List<ListItem>();

            int index = FindVisibleItemIndex();

            if (index >= 0)
            {
                //UnityDebug.Log("{0}", index);

                int first = index;
                while (first >= 0)
                {
                    Rect rect = _list[first].Position;
                    rect.x += (_rect.x - _scrollPosition.x);
                    rect.y += (_rect.y - _scrollPosition.y);

                    if (IsContains(_rect, rect))
                    {
                        first--;
                        if (first <= 0)
                        {
                            first = 0;
                            break;
                        }
                    }
                    else
                        break;
                }

                int last = index;
                while (last <= _list.Count - 1)
                {
                    Rect rect = _list[last].Position;
                    rect.x += (_rect.x - _scrollPosition.x);
                    rect.y += (_rect.y - _scrollPosition.y);

                    if (IsContains(_rect, rect))
                    {
                        last++;
                        if (last >= _list.Count - 1)
                        {
                            last = _list.Count - 1;
                            break;
                        }
                    }
                    else
                        break;
                }

                for (int i = first; i <= last; i++)
                    items.Add(_list[i]);

            }
            else
            {
                //UnityDebug.Log("{0}", index);
            }

            return items.ToArray();
        }



        void AddToList(LogItem item)
        {
			if (item.Type == LogType.Warning) return;
            string log = item.ToString();
            LogType type = item.Type;
            if (string.IsNullOrEmpty(log))
            {
                type = LogType.Warning;
                log = "Empty log message!";
            }

            ListItem listItem = new ListItem();
            listItem.Content = new GUIContent(log);

            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                    listItem.Color = Color.red;
                    break;
                case LogType.Warning:
                    listItem.Color = Color.yellow;
                    break;
                case LogType.Log:
                case LogType.Assert:
                    listItem.Color = Color.gray;
                    break;
                default:
                    listItem.Color = Color.white;
                    break;
            }

            GUIStyle style = GUI.skin.label;
            style.wordWrap = true;

            float x = 0;
            float y = (_list.Count > 0) ? _list[_list.Count - 1].Position.yMax : 0;
            float width = _viewRect.width;
            float height = style.CalcHeight(listItem.Content, width);

            listItem.Position = new Rect(x, y, width, height);
            _list.Add(listItem);


            bool scrollToBottom = _scrollPosition.y >= (_viewRect.height - _rect.height);
            _viewRect.height += listItem.Position.height;
            if (scrollToBottom)
                _scrollPosition = new Vector2(0, _viewRect.height - _rect.height);
        }

        void AddExists()
        {
            lock (_toAdd)
            {
                while (_toAdd.Count > 0)
                {
                    AddToList(_toAdd[0]);
                    _toAdd.RemoveAt(0);
                }
            }
        }

        GUIStyle ButtonGuiStyle;
        void OnGUI()
        {

            AddExists();
            ButtonGuiStyle = new GUIStyle(GUI.skin.button);
            ButtonGuiStyle.fontSize =  (int) (Screen.width*0.03f);

            if (GUI.Button(_buttonRect, _isVisible ? "hide console" : "show console", ButtonGuiStyle))
			{
				_isVisible = !_isVisible;
			}

            if (_isVisible)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.wordWrap = true;


                GUI.Box(Rect.MinMaxRect(_rect.xMin - 2, _rect.yMin - 2, _rect.xMax + 2, _rect.yMax + 2), GUIContent.none);
                _scrollPosition = GUI.BeginScrollView(_rect, _scrollPosition, _viewRect);

                ListItem[] visibleItems = GetVisibleItems();
                for (int i = 0; i < visibleItems.Length; i++)
                {
                    Color color = visibleItems[i].Color;
                    color.a = 0.25f;

                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, color);
                    tex.Apply();

                    color.a = 1.0f;
                    color.r += 0.5f;
                    color.g += 0.5f;
                    color.b += 0.5f;
                    style.normal.textColor = color;
                    style.normal.background = tex;

                    GUI.Label(visibleItems[i].Position, visibleItems[i].Content, style);

                    Texture.Destroy(tex);
                }

                GUI.EndScrollView();
            }
        }


        void OnEnable()
        {
            //Application.RegisterLogCallback(HandleLog);

            DebugHandler.MessageReceived += HandleLog;
        }

        void OnDisable()
        {
            //Application.RegisterLogCallback(null);

            DebugHandler.MessageReceived -= HandleLog;
        }


#if UNITY_STANDALONE_WIN
    private FileStream _file = null;
    private TextWriter _writer = null;
#endif

        void HandleLog(string message, string stackTrace, LogType type)
        {
            if (skip)
                return;

#if UNITY_STANDALONE_WIN
        if (_file == null)
        {
			_file = File.Open(Application.persistentDataPath + "\\" + "debug_output.txt", FileMode.Create, FileAccess.Write);
            _writer = new StreamWriter(_file);
        }
#endif

            LogItem item = new LogItem()
            {
                Time = DateTime.Now,
                Type = type,
                Message = message,
                StackTrace = stackTrace
            };

#if UNITY_STANDALONE_WIN
        _writer.WriteLine(item.ToString());
        _writer.Flush();
#endif

            lock (_toAdd)
                _toAdd.Add(item);
        }


        public void Log(string msg)
        {
            Debug.Log(msg);
        }

        public void LogWarning(string msg)
        {
            Debug.LogWarning(msg);
        }

        public void LogError(string msg)
        {
            Debug.LogError(msg);
        }
    }
}