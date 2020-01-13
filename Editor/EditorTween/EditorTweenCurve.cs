using EditorPlus;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace EditorPlus
{
    //[CreateAssetMenu(fileName = "aa", menuName = "aa")]
    public class EditorTweenCurve : ScriptableObject
    {
        public enum BuiltinCurve
        {
            linear,
            //in
            easeInSine,
            easeInCubic,
            easeInQuint,
            easeInCirc,
            easeInElastic,
            easeInQuad,
            easeInQuart,
            easeInExpo,
            easeInBack,
            easeInBounce,
            //out
            easeOutSine,
            easeOutCubic,
            easeOutQuint,
            easeOutCirc,
            easeOutElastic,
            easeOutQuad,
            easeOutQuart,
            easeOutExpo,
            easeOutBack,
            easeOutBounce,
            //in-out
            easeInOutSine,
            easeInOutCubic,
            easeInOutQuint,
            easeInOutCirc,
            easeInOutElastic,
            easeInOutQuad,
            easeInOutQuart,
            easeInOutExpo,
            easeInOutBack,
            easeInOutBounce,
            //ping-pong
            pingpongSine,
            pingpongStraight,
            pingpongInOut,
            //trigonometric
            Sin,
            Cos,

        }

        [Serializable]
        public class TweenCurve
        {
            public string name;
            public AnimationCurve curve;
        }
        public List<TweenCurve> Curves = new List<TweenCurve>();
        public static TweenCurve Get(string Name)
        {
            var curveAssets = AssetDBHelper.LoadAssets<EditorTweenCurve>(" t:EditorTweenCurve");
            foreach (var asset in curveAssets)
            {
                var curve = asset.Curves.Find(e => e.name == Name);
                if (curve != null)
                {
                    return curve;
                }
            }
            return null;
        }
        public static TweenCurve Get(BuiltinCurve curve)
        {
            return Get(curve.ToString());
        }
    }
}