using EditorPlus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEditor;
using UnityEngine;
using SeanLib.Core;

public class GUIGifDrawer
{
    private Image img;
    private List<byte[]> textures = new List<byte[]>();
    private EditorCoroutine decoding;
    private EditorCoroutine playing;
    private Action repaint;

    private Texture2D icon_play;
    private Texture2D icon_pause;
    private Rect drawRect;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool Playing => playing != null;
    public Texture2D Currrent;
    public int Index = -1;
    public int Count => textures.Count;
    public int frameCount;

    public float FrameRate=1/10f;
    public bool Controller;
    public GUIGifDrawer()
    {
        icon_play = AssetDBHelper.LoadAsset<Texture2D>(this.GetType(), "../ic_play.png");
        icon_pause = AssetDBHelper.LoadAsset<Texture2D>(this.GetType(), "../ic_pause.png");
    }
    public void OnGUI(Action repaint)
    {
        this.repaint = repaint;
        Texture2D texture = Currrent;
        //自适应分辨率
        if (texture)
        {
            int w = texture.width;
            int h = texture.height;
            if (w > 1024)
            {
                h = (int)((float)h / w * 1024f);
                w = 1024;
            }
            if (h > 1024)
            {
                w = (int)((float)w / h * 1024f);
                h = 1024;
            }
            GUILayout.Box("", GUILayout.Width(w), GUILayout.Height( h));
            if (Event.current.type == EventType.Repaint)
            {
                drawRect = GUILayoutUtility.GetLastRect();
                DrawControl();
                GUI.DrawTexture(drawRect, texture);
            }
            ControlEvent();
        }
    }
    private void DrawControl()
    {
        if (!Controller) return;
        if (drawRect.Contains(Event.current.mousePosition))
        {
            EditorGUI.DrawRect(drawRect.DeltaCenter(Vector2.one),OnGUIUtility.Colors.blue);
        }
    }
    private void ControlEvent()
    {
        if (!Controller) return;
        if (Event.current.type==EventType.Layout&&drawRect.size.x > 0 && drawRect.size.y > 0)
        {
            if (drawRect.Contains(Event.current.mousePosition))
            {
                if (!Playing)
                {
                    Play();
                }
                /*if (Event.current.type == EventType.MouseUp)
                {
                    if (Playing)
                    {
                        Stop();
                    }
                    else
                    {
                        Play();
                    }
                }*/
            }
            else
            {
                if (Playing)
                {
                    Stop();
                }
            }
        }
    }
    public void LoadGIF(string filePath)
    {
        Clean();
        img = System.Drawing.Image.FromFile(filePath);
        if (img == null)
        {
            Debug.LogError(filePath + " cant load");
            return;
        }
        decoding = EditorCoroutine.Start(DecodeGif());
    }
    public void Play()
    {
        Stop();
        //Index = -1;
        playing = EditorCoroutine.Start(playSequence());
    }
    public void Seek(int newIndex)
    {
        if (frameCount != textures.Count)
        {
            //未加载完
            Index = newIndex >= textures.Count ? Index : newIndex;
        }
        else
        {
            Index = newIndex % textures.Count;
        }
        if (Index >= 0)
        {
            if (!Currrent)
            {
                Currrent = new Texture2D(Width, Height, TextureFormat.ARGB32, true);
            }
            Currrent.LoadImage(textures[Index]);
        }
        repaint?.Invoke();
    }
    public void Stop()
    {
        if (playing != null)
        {
            playing.Stop();
            playing = null;
        }
    }
    private IEnumerator playSequence()
    {
        while (true)
        {
            Seek(Index + 1);
            yield return new EditorYields.WaitForSeconds(FrameRate);
        }
    }
    public void Clean()
    {
        Stop();
        if (decoding != null)
        {
            decoding.Stop();
            decoding = null;
        }
        if (Currrent)
        {
            GameObject.DestroyImmediate(Currrent);
            Currrent = null;
        }
        textures.Clear();
        Index = 0;
        if (img != null)
        {
            img.Dispose();
            img = null;
        }
    }
    private IEnumerator DecodeGif()
    {
        FrameDimension frame = new FrameDimension(img.FrameDimensionsList[0]);
        frameCount = img.GetFrameCount(frame);//帧数
        Width = img.Width;
        Height = img.Height;
        for (int i = 0; i < frameCount; i++)
        {
            img.SelectActiveFrame(frame, i);
            Bitmap framebmp = new Bitmap(img.Width, img.Height);
            using (System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(framebmp))
            {
                graphic.DrawImage(img, Point.Empty);
            }
            textures.Add(GetBmpBytes(framebmp));
            yield return null;
        }
        if (img != null)
        {
            img.Dispose();
            img = null;
        }

    }
    private byte[] GetBmpBytes(Bitmap bmp)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            // 将bitmap 以png格式保存到流中       
            bmp.Save(stream, ImageFormat.Png);
            // 创建一个字节数组，长度为流的长度    
            byte[] data = new byte[stream.Length];
            // 重置指针          
            stream.Seek(0, SeekOrigin.Begin);
            // 从流读取字节块存入data中       
            stream.Read(data, 0, Convert.ToInt32(stream.Length));
            return data;
        }
    }
}