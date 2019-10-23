using System;

namespace SeanLib.Core
{
    using System.Text;

    using UnityEngine;

    /// <summary>
    /// 32个并发状态机
    /// </summary>
    /// <typeparam name="T">状态枚举</typeparam>
    [Serializable]
    public class State
    {
        public int Current = 0;

        public Enum BindEnum;
        public State()
        {
        }
        public State(Enum t,bool None=false)
        {
            this.SetState(t);
            if (None)
            {
                Current = 0;
            }
        }

        public string GetEnumStr()
        {
            if (BindEnum != null)
            {
                StringBuilder sb=new StringBuilder();
               string[] names= Enum.GetNames(BindEnum.GetType());
                for (int i = 0; i < names.Length; i++)
                {
                    if ((this.Current & (1 << i)) > 0)
                    {
                        sb.Append(names[i]);
                        sb.Append(",");
                    }
                }
                return sb.ToString();
            }
            return Current.ToString();
        }
        public static int GetStateNumber(Enum statEnum)
        {
            int index = Convert.ToInt32(statEnum);
            return 1 << index;
        }
        public void SetState(Enum t)
        {
            this.Current = GetStateNumber(t);
            BindEnum = t;
        }

        public void EnableState(params Enum[] ts)
        {
            foreach (Enum t in ts)
            {
                this.Current |= (1 << Convert.ToInt32(t));
            }
        }

        public void DisableState(params Enum[] ts)
        {
            foreach (Enum t in ts)
            {
                this.Current &= ~(1 << Convert.ToInt32(t));
            }
        }
        /// <summary>
        /// 交集
        /// </summary>
        /// <param name="otherState"></param>
        /// <returns></returns>
        public State Intersection(State otherState)
        {
            State IntersectionState = new State();
            IntersectionState.Current = this.Current & otherState.Current;
            IntersectionState.BindEnum = this.BindEnum;
            return IntersectionState;
        }
        /// <summary>
        /// 同时满足
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public bool CheckState(params Enum[] ts)
        {
            foreach (Enum t in ts)
            {
                bool enable = ContainState(t);
                if (enable == false)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ContainState(Enum t)
        {
            return (this.Current & (1 << Convert.ToInt32(t))) > 0;
        }
        /// <summary>
        /// 全等
        /// </summary>
        /// <param name="state"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool operator ==(State state, Enum t)
        {
            if (t == null)
            {
                return false;
            }
            return state.Current == (1 << Convert.ToInt32(t));
        }

        public static bool operator !=(State state, Enum t)
        {
            return !(state == t);
        }

        public override bool Equals(object obj)
        {
            if (obj is Enum)
            {
                return this.Current == GetStateNumber(obj as Enum);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
