using SeanLib.Core.Event;
using System.Collections.Generic;

namespace SeanLib.Core.Sequence
{
    public class CommonSequence : ISequence
    {
        protected List<ISequenceProcess> processes = new List<ISequenceProcess>();
        public int ptr = 0;

        public ISequenceProcess Current
        {
            get
            {
                if (processes.Count > ptr)
                {
                    return processes[ptr];
                }
                return null;
            }
        }

        public ISequence ParentSequence
        {
            get;

            set;
        }

        public float progress
        {
            get { return ptr / processes.Count; }
        }

        public Signal<CommonSequence> OnComplete = new Signal<CommonSequence>();
        public Signal<CommonSequence> OnError = new Signal<CommonSequence>();
        public virtual void Clear()
        {
            ptr = 0;
            processes.Clear();
        }

        public virtual void Move(int i = 1)
        {
            ptr += i;
        }

        public virtual void MoveTo(int index)
        {
            ptr = index;
        }

        public virtual void MoveTo(ISequenceProcess process)
        {
            ptr = processes.IndexOf(process);
        }

        public virtual ISequence RegistProcess(ISequenceProcess process)
        {
            process.ParentSequence = this;
            processes.Add(process);
            return this;
        }

        public virtual ISequence Remove(ISequenceProcess process)
        {
            processes.Remove(process);
            return this;
        }

        public virtual void Next()
        {
            ptr += 1;
            if (ptr >= processes.Count)
            {
                Complete();
            }
            else
            {
                Current.Execute();
            }
        }

        public virtual void Complete()
        {
            OnComplete.Dispatch(this);
        }

        public virtual void Error()
        {
            OnError.Dispatch(this);
        }

        public virtual void Execute()
        {
            if (ptr >= processes.Count)
            {
                Error();
                return;
            }
            else
            {
                Current.Execute();
            }
        }

        public virtual void End()
        {
            ptr = processes.Count;
        }
    }
}