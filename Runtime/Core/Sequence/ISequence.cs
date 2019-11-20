using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SeanLib.Core.Sequence
{
    public interface ISequence : ISequenceProcess
    {
        ISequence RegistProcess(ISequenceProcess process);
        ISequence Remove(ISequenceProcess process);
        void Move(int i = 1);
        void MoveTo(int index);
        void MoveTo(ISequenceProcess process);
        ISequenceProcess Current { get; }
        void Clear();
        void Next();
        void End();
    }
    public interface ISequenceProcess
    {
        float progress { get; }
        ISequence ParentSequence { get; set; }
        void Execute();
        void Complete();
        void Error();
    }
}
