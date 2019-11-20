using System;

namespace SeanLib.Core
{
    public class EmptyDebugConsole:IDebugConsole
    {
        public void Log(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Log(params object[] msgs)
        {
            foreach (var item in msgs)
            {
                UnityEngine.Debug.Log(item);
            }
        }

        public void LogStackTrace()
        {
            UnityEngine.Debug.Log("");
        }

        public void LogToChannel(string channel, string msg)
        {
            Log(msg);
        }

        public void LogToChannel(string channel, params object[] msgs)
        {
            Log(msgs);
        }

        public void LogToChannel(int channel, string msg)
        {
            Log(msg);
        }

        public void LogToChannel(int channel, params object[] msgs)
        {
            Log(msgs);
        }

        public void AddButton(string name, Action clickHandler)
        {
            
        }

        public void RemoveButton(string name)
        {
            
        }

        public void AddTopString(string name, string content)
        {
            
        }

        public void AddTopObject(string ObjectName, object obj)
        {
            
        }

        public void RemoveTopObject(string ObjectName)
        {
        }

        public void RemoveTopString(string name)
        {
            
        }

        public void SetConsoleActive(bool consoleActive)
        {
        }


        public void Info(string channel, string actionName, string info, string state = "")
        {
            Log(info);
        }

        public void Warning(string channel, string actionName, string info, string state = "")
        {
            UnityEngine.Debug.LogWarning(info);
        }

        public void Error(string channel, string actionName, string info, string state = "")
        {
            UnityEngine.Debug.LogError(info);
        }
    }
}