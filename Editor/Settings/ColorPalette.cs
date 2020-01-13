using EditorPlus;
using SeanLib.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
//[CreateAssetMenu(fileName = "ColorPalette", menuName = "ColorPalette")]
public class ColorPalette: ScriptableObject
{
    public StringColor Light = new StringColor();
    public StringColor Dark = new StringColor();
    public Preset DefaultSetting;
    public Color Get(string key,Color ifNull)
    {
        Color color;
        if(EditorGUIUtility.isProSkin)
        {
            if(!Dark.TryGetValue(key, out color))
            {
                return ifNull;
            }
        }
        else
        {
            if(!Light.TryGetValue(key, out color))
            {
                return ifNull;
            }
        }
        return color;
    }
    [InspectorPlus.Button(Editor =true)]
    public void SetDefault()
    {
        if(DefaultSetting)
        {
            DefaultSetting.ApplyTo(this);
        }
    }
    private static Dictionary<string, ColorPalette> cache = new Dictionary<string, ColorPalette>();
    //static
    public static Color Get(string PaletteName,string ColorName,Color ifNull)
    {
        ColorPalette palette=null;
        if(!cache.TryGetValue(PaletteName,out palette))
        {
            if (AssetDBHelper.TryLoadAsset<ColorPalette>(PaletteName + " t: ColorPalette", out palette))
            {
                cache[PaletteName] = palette;
                return palette.Get(ColorName, ifNull);
            }
            return ifNull;
        }
        return palette.Get(ColorName, ifNull);
    }
} 