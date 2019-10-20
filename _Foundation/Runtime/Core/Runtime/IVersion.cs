
using System;
namespace SeanLib.Core
{
    public interface IVersion
    {
        string VersionKey { get; set; }
        string LocalVersion { get; set; }
        string RemoteVersion { get; set; }
        void Update();
        void Merge();
        bool CheckChange();
        IVersionRepostory Repostory { get; set; }
    }
    public interface IVersionRepostory
    {
        /// <summary>
        /// check out on version
        /// </summary>
        /// <param name="version"></param>
        void CheckOut(IVersion version);
        /// <summary>
        /// pull Remote respostory to local
        /// </summary>
        void Pull();
        /// <summary>
        /// Merge memory diff
        /// </summary>
        void Merge(IVersion version);
        /// <summary>
        /// commit memory diff to local
        /// </summary>
        void Commit();

    }
    public class VersionFileIO
    {
        public Action<IVersion> OnCheckOut;
        public void CheckOut(IVersion version)
        {
            if (OnCheckOut != null)
                OnCheckOut(version);
        }

        public Action<string> OnClone;
        public Action OnCommit;
        public void Commit()
        {
            if (OnCommit != null)
                OnCommit();
        }

        public Action<IVersion> OnMerge;
        public void Merge(IVersion version)
        {
            if (OnMerge != null)
                OnMerge(version);
        }
    }
    /*  public Action<DownLoadAgent, DownLoadAgent, Action> OnPull;
      public void Pull(DownLoadAgent Local, DownLoadAgent Remote,Action complete)
      {
          if (OnPull != null)
              OnPull(Local, Remote, complete);
      }
      */
}

