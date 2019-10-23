using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SeanLib.Core
{
    public interface IAssetManager
    {
        string LoadString(string assetGUID);
        byte[] LoadFile(string assetGUID);
        T LoadAsset<T>(string assetGUID) where T : UnityEngine.Object;
        string LoadStringAsync(string assetGUID, Action<string> CallBack);
        byte[] LoadFileAsync(string assetGUID, Action<byte[]> CallBack);
        void LoadAssetAsync<T>(string assetGUID, Action<T> CallBack) where T : UnityEngine.Object;
    }
}