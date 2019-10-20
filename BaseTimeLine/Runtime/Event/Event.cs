using SeanLib.Core.Event;
using ServiceTools;
using System.Collections.Generic;
namespace TimeLine
{
    public class Event 
    {
        public static Event Instace
        {
            get
            {
                return Singleton<Event>.Instance;
            }
        }
        public Event()
        {
            OnResume.AddEventListener(DefaultResume);
            OnAwait.AddEventListener(DefaultAwait);
        }

        public Signal<Behaviour.Context> OnAwait = new Signal<Behaviour.Context>();
        public Signal<Behaviour.Context> OnResume = new Signal<Behaviour.Context>();

        public Signal<Behaviour.Context> OnClipPlay = new Signal<Behaviour.Context>();
        public Signal<Behaviour.Context> OnClipPause= new Signal<Behaviour.Context>();
        public Dictionary<string, Signal<Behaviour.Context>> CustomEvents = new Dictionary<string, Signal<Behaviour.Context>>();


        #region Await Sequnence

        public HashSet<Behaviour.Context> Awaittings = new HashSet<Behaviour.Context>();
        public void ResumeAll()
        {
            foreach (var awaitting in Awaittings)
            {
                OnResume.Dispatch(awaitting);
            }
            Awaittings.Clear();
        }
        public void ResumeAwaiting(Behaviour.Context waitingObj)
        {
            if(Awaittings.Contains(waitingObj))
            {
                OnResume.Dispatch(waitingObj);
                Awaittings.Remove(waitingObj);
            }
        }
        private void DefaultAwait(Behaviour.Context obj)
        {
            Awaittings.Add(obj);
            obj.director.Pause();
        }
        private void DefaultResume(Behaviour.Context obj)
        {
            if (obj.director&&obj.director.state == UnityEngine.Playables.PlayState.Paused)
            {
                obj.director.Resume();
            }
        }
        #endregion
    }
}
