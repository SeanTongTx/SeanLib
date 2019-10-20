using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MaterialPropertyBlockHelper
{
    static MaterialPropertyBlockHelper()
    {
        SceneManager.activeSceneChanged += SceneChanged;
    }

    private static void SceneChanged(Scene arg0, Scene arg1)
    {
        RenderBlocks.Clear();
        MatBlocks.Clear();
    }

    public static Dictionary<Renderer, MaterialPropertyBlock> RenderBlocks = new Dictionary<Renderer, MaterialPropertyBlock>();
    public static Dictionary<Renderer,Dictionary<int, MaterialPropertyBlock>> MatBlocks = new Dictionary<Renderer, Dictionary<int, MaterialPropertyBlock>>();

    public static MaterialPropertyBlock GetPropertyBlock(this Renderer renderer)
    {
        MaterialPropertyBlock block = null;
        if (!RenderBlocks.TryGetValue(renderer, out block))
        {
            block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            RenderBlocks[renderer] = block;
        }
        return block;
    }
    public static MaterialPropertyBlock GetPropertyBlock(this Renderer renderer,int matIndex)
    {
        Dictionary<int, MaterialPropertyBlock> matblocks = null;
        if (!MatBlocks.TryGetValue(renderer, out matblocks))
        {
            matblocks = new Dictionary<int, MaterialPropertyBlock>();
            MatBlocks[renderer] = matblocks;
        }
        MaterialPropertyBlock block = null;
        if(!matblocks.TryGetValue(matIndex,out block))
        {
            block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block,matIndex);
            matblocks[matIndex] = block;
        }
        return block;
    }
}
