using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
[Serializable]
public class PluginList:IEnumerable<PluginInfo>
{
    public List<PluginInfo> Plugins = new List<PluginInfo>();

    public IEnumerator<PluginInfo> GetEnumerator()
    {
       return Plugins.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Plugins.GetEnumerator();
    }
}
public enum PluginType
{
    Packages,
    Local,
    Remote,
}
public enum PluginVersion
{
    Internal,
    Preview,
    Alpha,
    Beta,
    Release,
    LTS
}
[Serializable]
public class PluginInfo
{
    public string Name;
    public string URL;
    public PluginVersion version=PluginVersion.Internal;
    [NonSerialized]
    public PluginType type;
}
