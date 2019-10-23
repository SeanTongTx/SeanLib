using UnityEngine;
using UnityEditor;
using System;

namespace EditorPlus
{
    [CustomSeanLibEditor("Other/InternalIcon", order = 100)]
    public class ListInternalIconWindow : SeanLibEditor
    {
        private static string[] text = new[]
                                           {
                                           "TreeEditor.AddLeaves", "TreeEditor.AddBranches", "TreeEditor.Trash",
                                           "TreeEditor.Duplicate", "TreeEditor.Refresh", "editicon.sml",
                                           "tree_icon_branch_frond", "tree_icon_branch", "tree_icon_frond",
                                           "tree_icon_leaf", "tree_icon", "animationvisibilitytoggleon",
                                           "animationvisibilitytoggleoff", "MonoLogo", "AgeiaLogo",
                                           "AboutWindow.MainHeader", "Animation.AddEvent", "lightMeter/greenLight",
                                           "lightMeter/lightRim", "lightMeter/orangeLight", "lightMeter/redLight",
                                           "Animation.PrevKey", "Animation.NextKey", "Animation.AddKeyframe",
                                           "Animation.EventMarker", "Animation.Play", "Animation.Record",
                                           "AS Badge Delete","AS Badge New",
                                           "WelcomeScreen.AssetStoreLogo", "preAudioAutoPlayOff", "preAudioAutoPlayOn",
                                           "preAudioPlayOff", "preAudioPlayOn", "preAudioLoopOff", "preAudioLoopOn",
                                           "AvatarInspector/BodySilhouette", "AvatarInspector/HeadZoomSilhouette",
                                           "AvatarInspector/LeftHandZoomSilhouette",
                                           "AvatarInspector/RightHandZoomSilhouette", "AvatarInspector/Torso",
                                           "AvatarInspector/Head", "AvatarInspector/LeftArm",
                                           "AvatarInspector/LeftFingers", "AvatarInspector/RightArm",
                                           "AvatarInspector/RightFingers", "AvatarInspector/LeftLeg",
                                           "AvatarInspector/RightLeg", "AvatarInspector/HeadZoom",
                                           "AvatarInspector/LeftHandZoom", "AvatarInspector/RightHandZoom",
                                           "AvatarInspector/DotFill", "AvatarInspector/DotFrame",
                                           "AvatarInspector/DotFrameDotted", "AvatarInspector/DotSelection", "SpeedScale",
                                           "AvatarPivot", "Avatar Icon", "Mirror", "AvatarInspector/BodySIlhouette",
                                           "AvatarInspector/BodyPartPicker", "AvatarInspector/MaskEditor_Root",
                                           "AvatarInspector/LeftFeetIk", "AvatarInspector/RightFeetIk",
                                           "AvatarInspector/LeftFingersIk", "AvatarInspector/RightFingersIk",
                                           "BuildSettings.SelectedIcon", "SocialNetworks.UDNLogo",
                                           "SocialNetworks.LinkedInShare", "SocialNetworks.FacebookShare",
                                           "SocialNetworks.Tweet", "SocialNetworks.UDNOpen", "Clipboard", "Toolbar Minus",
                                           "ClothInspector.PaintValue", "EditCollider", "EyeDropper.Large",
                                           "ColorPicker.CycleColor", "ColorPicker.CycleSlider", "PreTextureMipMapLow",
                                           "PreTextureMipMapHigh", "PreTextureAlpha", "PreTextureRGB", "Icon Dropdown",
                                           "UnityLogo", "Profiler.PrevFrame", "Profiler.NextFrame", "GameObject Icon",
                                           "Prefab Icon", "PrefabModel Icon", "ScriptableObject Icon",
                                           "sv_icon_none", "PreMatLight0", "PreMatLight1", "Toolbar Plus", "Camera Icon",
                                           "PreMatSphere", "PreMatCube", "PreMatCylinder", "PreMatTorus", "PlayButton",
                                           "PauseButton", "HorizontalSplit", "VerticalSplit", "BuildSettings.Web.Small",
                                           "cs Script Icon",  "Shader Icon",
                                           "TextAsset Icon", "AnimatorController Icon", "AudioMixerController Icon",
                                           "RectTransformRaw", "RectTransformBlueprint", "MoveTool", "MeshRenderer Icon",
                                           "Terrain Icon", "SceneviewLighting", "SceneviewFx", "SceneviewAudio",
                                           "SettingsIcon", "TerrainInspector.TerrainToolRaise",
                                           "TerrainInspector.TerrainToolSetHeight",
                                           "TerrainInspector.TerrainToolSmoothHeight", "TerrainInspector.TerrainToolSplat",
                                           "TerrainInspector.TerrainToolTrees", "TerrainInspector.TerrainToolPlants",
                                           "TerrainInspector.TerrainToolSettings", "RotateTool", "ScaleTool", "RectTool",
                                           "MoveTool On", "RotateTool On", "ScaleTool On", "RectTool On", "ViewToolOrbit",
                                           "ViewToolMove", "ViewToolZoom", "ViewToolOrbit On", "ViewToolMove On",
                                           "ViewToolZoom On", "StepButton", "PlayButtonProfile", "PlayButton On",
                                           "PauseButton On", "StepButton On", "PlayButtonProfile On",
                                           "Toolbar Plus More"
                                       };
        public Vector2 scrollPosition;
        public override void OnGUI()
        {

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            //鼠标放在按钮上的样式
            foreach (MouseCursor item in Enum.GetValues(typeof(MouseCursor)))
            {
                GUILayout.Button(Enum.GetName(typeof(MouseCursor), item));
                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), item);
                GUILayout.Space(10);
            }


            //内置图标
            for (int i = 0; i < text.Length; i += 8)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < 8; j++)
                {
                    int index = i + j;
                    if (index < text.Length)
                    {
                        try
                        {
                            if (GUILayout.Button(
                                EditorGUIUtility.IconContent(text[index]),
                                GUILayout.Width(50),
                                GUILayout.Height(30)))
                            {
                                EditorGUIUtility.systemCopyBuffer = text[index];
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            //


            GUILayout.EndScrollView();
        }
    }
}