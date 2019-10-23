using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MarkDownConst
{
    public const string Version_Internal_rich= @"<color=""red"">(Internal)</color>";
    public const string Version_Preview_rich = @"<color=""orange"">(Preview)</color>";
    public const string Version_Alpha_rich = @"<color=""yellow"">(Alpha)</color>";
    public const string Version_Beta_rich = @"<color=""olive"">(Beta)</color>";
    public const string Version_Release_rich = @"<color=""green"">(Release)</color>";
    public const string Version_LTS_rich = @"<color=""lime"">(LTS)</color>";


    public static Color Internal_col = Color.red;
    public static Color Preview_col = new Color(1,165f/255f,0);
    public static Color Alpha_col = Color.yellow;
    public static Color Beta_col = new Color(0.5f,0.5f,0);
    public static Color Release_col = new Color(0,0.5f,0);
    public static Color LTS_col = Color.green;

    public static string GetVersionRichText(int versionType)
    {
        switch(versionType)
        {
            case 0:return Version_Internal_rich;
            case 1: return Version_Preview_rich;
            case 2: return Version_Alpha_rich;
            case 3: return Version_Beta_rich;
            case 4: return Version_Release_rich;
            case 5: return Version_LTS_rich;
        }
        return string.Empty;
    }
}
