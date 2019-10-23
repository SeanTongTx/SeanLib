using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SeanLib.Core
{
    public interface IMoudule
    {
        ModuleConfig Config { get; }
        void Load();
        void UnLoad();
    }
}