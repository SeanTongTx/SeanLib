using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SeanLib.Core
{
    public interface IMultiLanguage
    {
        string GetMessage(int code);
    }
    public class MultiLanguage
    {
        public static IMultiLanguage multiLanguage;
    }
}
