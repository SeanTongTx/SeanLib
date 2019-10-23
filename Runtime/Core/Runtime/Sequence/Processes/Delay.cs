using System.Collections;
using UnityEngine;

namespace SeanLib.Core.Sequence
{
    public class Delay : CommonProcess
    {
        public float Secound;
        public override void Execute()
        {
            CoroutineCall.Call(WaitTime);
        }

        private IEnumerator WaitTime()
        {
            yield return new WaitForSeconds(Secound);
            Complete();
        }
    }
}