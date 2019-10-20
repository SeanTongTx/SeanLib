using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SeanLib.Core
{
    [Serializable]
    public class ModuleConfig
    {
        public struct ModuleKeys
        {
            public const string ModuleName = "ModuleName";
            public const string ModuleVersion = "ModuleVersion";
        }
        public string ModuleName;
        public string ModuleVersion;
        public List<string> RefrencesModule = new List<string>();
    }
}