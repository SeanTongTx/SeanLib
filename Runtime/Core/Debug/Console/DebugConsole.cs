using System;

namespace SeanLib.Core
{
    public class DebugConsole
    {
        public static IDebugConsole consoleImpl
        {
            get { return _consoleImpl ?? (_consoleImpl = new EmptyDebugConsole()); }
            set { _consoleImpl = value; }
        }

        private static IDebugConsole _consoleImpl = null;
        /// <summary>
        /// 设置调试窗口可见状态
        /// </summary>
        /// <param name="active">可见性</param>
        public static void SetActive(bool active)
        {
            consoleImpl.SetConsoleActive(active);
        }
        /// <summary>
        /// 添加一个常驻调试字符
        /// </summary>
        /// <param name="stringName">调试字符名（用于移除）</param>
        /// <param name="content">字符内容</param>
        public static void AddTopString(string stringName, string content)
        {
            consoleImpl.AddTopString(stringName, content);
        }
        /// <summary>
        /// 移除常驻调试字符
        /// </summary>
        /// <param name="stringName">调试字符名</param>
        public static void RemoveTopString(string stringName)
        {
            consoleImpl.RemoveTopString(stringName);
        }
        /// <summary>
        /// 添加调试用按钮
        /// </summary>
        /// <param name="btnName">按钮名</param>
        /// <param name="clickHandler">按钮响应事件</param>
        public static void AddButton(string btnName, Action clickHandler)
        {
            consoleImpl.AddButton(btnName, clickHandler);
        }
        /// <summary>
        /// 按按钮名移除调试按钮
        /// </summary>
        /// <param name="btnName">按钮名</param>
        public static void RemoveButton(string btnName)
        {
            consoleImpl.RemoveButton(btnName);
        }
        /// <summary>
        /// 调用栈输出
        /// </summary>
        public static void LogStackTrace()
        {
            consoleImpl.LogStackTrace();
        }
        /// <summary>
        /// 输出内容到默认频道
        /// </summary>
        /// <param name="msg">输出信息</param>
        public static void Log(string msg)
        {
            consoleImpl.Log(msg);
        }
        /// <summary>
        /// ToString后空格分隔
        /// </summary>
        /// <param name="msgs">输出信息</param>
        public static void Log(params object[] msgs)
        {
            consoleImpl.Log(msgs);
        }
        /// <summary>
        /// log到指定频道
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="msg">输出信息</param>
        public static void LogToChannel(string channel, string msg)
        {
            consoleImpl.LogToChannel(channel, msg);
        }
        /// <summary>
        /// log到指定频道
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="msgs">输出信息</param>
        public static void LogToChannel(string channel, params object[] msgs)
        {
            consoleImpl.LogToChannel(channel, msgs);
        }
        /// <summary>
        /// 添加监视object
        /// </summary>
        /// <param name="ObjectName"></param>
        /// <param name="obj"></param>
        public static void AddObject(string ObjectName, object obj)
        {
            consoleImpl.AddTopObject(ObjectName, obj);
        }
        /// <summary>
        /// 移除监视object
        /// </summary>
        /// <param name="ObjectName"></param>
        /// <param name="obj"></param>
        public static void RemoveObject(string ObjectName)
        {
            consoleImpl.RemoveTopObject(ObjectName);
        }

        public static void Info(string channel, string actionName, string info, string state = "")
        {

            consoleImpl.Info(channel, actionName, info, state);
        }

        public static void Warning(string channel, string actionName, string info, string state = "")
        {

            consoleImpl.Warning(channel, actionName, info, state);
        }
        public static void Error(string channel, string actionName, string info, string state = "")
        {

            consoleImpl.Error(channel, actionName, info, state);
        }
    }

}