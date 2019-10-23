using System;
using System.Collections.Generic;

namespace SeanLib.Core
{
    public class RunningTimeLog
    {
        private static readonly Dictionary<string, DateTime> _tickTime = new Dictionary<string, DateTime>();
        /// <summary>
        /// 获得方法耗时
        /// </summary>
        /// <param name="a">方法</param>
        /// <param name="times">运行次数</param>
        /// <returns>耗时</returns>
        public static double Run(Action a,int times=1)
        {
            DateTime startTime = DateTime.Now;

            for (int i = 0; i < times; i++)
            {
                a();
            }
            
            DateTime endTime = DateTime.Now;
            TimeSpan timeSpan = endTime - startTime;
            return timeSpan.TotalMilliseconds;
        }
        /// <summary>
        /// 获得方法耗时，并打印
        /// </summary>
        /// <param name="a">方法</param>
        /// <param name="times">运行次数</param>
        /// <returns>耗时</returns>
        public static double RunAndPrint(Action a, int times=1)
        {
            double totalMilliseconds = Run(a, times);
            DebugConsole.Log("a:RunningTime ", totalMilliseconds);
            return totalMilliseconds;
        }
        /// <summary>
        /// 对比方法，获得（方法a耗时-方法b耗时）
        /// </summary>
        /// <param name="a">方法a</param>
        /// <param name="b">方法b</param>
        /// <param name="times">运行次数</param>
        /// <returns>方法a耗时-方法b耗时的差</returns>
        public static double RunAndContrast(Action a, Action b, int times = 1)
        {
            double runA = Run(a, times);
            double runB = Run(b, times);
            return runA - runB;
        }
        /// <summary>
        /// 执行frame*times次方法，并记录时间
        /// 这些执行会被分散在frame个帧数内
        /// </summary>
        /// <param name="testFunctions">被检测的方法</param>
        /// <param name="times">一帧内运行次数</param>
        /// <param name="frame">执行帧数</param>
        public static void RunAndPrintInDisperseFrame(Action[] testFunctions, int times, int frame)
        {
            double[] totalMilliseconds=new double[testFunctions.Length];
            double currFrame = 0;
            FrameCall.Call(() =>
            {
                for (int i = 0; i < testFunctions.Length; i++)
                {
                    totalMilliseconds[i] += Run(testFunctions[i], times);
                }

                currFrame++;
                bool b = currFrame < frame;
                if (!b)
                {
                    for (int i = 0; i < testFunctions.Length; i++)
                    {
                        DebugConsole.Log(i + ":RunningTime ", totalMilliseconds[i]);
                    }
                }
                return b;
            });
        }

        /// <summary>
        /// 记录当前时间
        /// </summary>
        /// <param name="tag">这个时间的标记</param>
        public static void TickTime(string tag)
        {
            _tickTime[tag] = DateTime.Now;
        }
        /// <summary>
        /// 当前时间和上一次记录的时间比较输出（Milliseconds）
        /// </summary>
        /// <param name="tag">时间的标记</param>
        public static void LogDistanceTickTime(string tag)
        {
            DebugConsole.Log(tag + ":" + (DateTime.Now - _tickTime[tag]).TotalMilliseconds);
            _tickTime.Remove(tag);
        }
    }
}
