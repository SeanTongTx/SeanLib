using SeanLib.Core.Event;

namespace SeanLib.Core.Sequence
{
    public class CommonProcess : ISequenceProcess
    {
        public bool IgnoreError;
        public string ErrorStr;
        public Signal<CommonProcess> OnComplete = new Signal<CommonProcess>();
        public Signal<CommonProcess> OnError = new Signal<CommonProcess>();
        public virtual float progress { get; set; }
        public virtual ISequence ParentSequence { get; set; }
        public virtual void Complete()
        {
            OnComplete.Dispatch(this);
            if (ParentSequence != null)
            {
                ParentSequence.Next();
            }
        }

        public virtual void Error()
        {
            OnError.Dispatch(this);
            if (ParentSequence != null)
            {
                if (IgnoreError)
                {
                    ParentSequence.Next();
                }
                else
                {
                    ParentSequence.Error();
                }
            }
        }

        public virtual void Execute()
        {
            Complete();
        }
    }
}