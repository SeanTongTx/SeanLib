using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorMDImageProcessor : AssetPostprocessor
{

    void OnPreprocessTexture()
    {
        if(assetPath.Contains("/Doc/"))
        {
           var textureImporter= this.assetImporter as TextureImporter;
            if(textureImporter)
            {
                textureImporter.mipmapEnabled = false;
                textureImporter.isReadable = false;
                textureImporter.npotScale = TextureImporterNPOTScale.None;
            }
        }
    }
    void OnPostprocessTexture(Texture2D texture)
    {
    }
}
