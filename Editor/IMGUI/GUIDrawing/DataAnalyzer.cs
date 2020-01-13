using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class DataClass
{
    public int[] data = new int[4];//假设每一个采样有四种数据
}

public class DataAnalyzer : EditorWindow
{
    //[MenuItem("DataAnalyzer/数据分析")]
    public static void ShowWindow()
    {
        DataAnalyzer window = new DataAnalyzer();
        window.wantsMouseMove = true;
        window.titleContent = new GUIContent("数据分析");
        window.Show();
        window.Focus();
    }

    const int LAYERS = 4;

    //绘制参数
    private GUIStyle _headStyle;
    private Material _graphMaterial;
    private Rect _axisRect = new Rect(150, 250, 800, 300);
    private Rect _graphRect = new Rect(170, 270, 760, 280);
    private Rect _graphContentRect = new Rect(170, 270, 760, 280);
    private Color[] _layerColor = new Color[LAYERS]
    {
            new Color(190f / 255f, 192f / 255f, 41f / 255f),
            new Color(53f / 255f, 136f / 255f, 167f / 255f),
            new Color(204f / 255f, 113f / 255f, 0f / 255f),
            new Color(136f / 255f, 41f / 255f, 7f / 255f),
    };

    private Vector3[][] _points = new Vector3[LAYERS][];
    private int _current;
    private bool _clickGraph;

    //采样数据
    private int _sampleCount;
    private List<DataClass> _samples = new List<DataClass>();

    private void OnEnable()
    {
        _samples = GenFackeData();
        _sampleCount = _samples.Count;
    }

    private void OnGUI()
    {
        if(_headStyle == null)
        {
            _headStyle = new GUIStyle();
            _headStyle.fontSize = 20;
            _headStyle.alignment = TextAnchor.MiddleCenter;
            _headStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        }

        if(_graphMaterial == null)
        {
            _graphMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            _graphMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _graphMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _graphMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _graphMaterial.SetInt("_ZWrite", 0);
        }
        if (_samples.Count > 0)
            DrawGraph();

        HandleEvent();
    }

    private void DrawGraph()
    {
        if(_points[0] == null || _points[0].Length != _sampleCount)
        {
            for (int layer = 0; layer < LAYERS; ++layer)
                _points[layer] = new Vector3[_sampleCount]; 
        }

        float maxValue = GetListMaxValue(_samples);
        float avgValue = GetListAverageValue(_samples);
        for(int i =0;i<_samples.Count;++i)
        {
            for (int layer = 0; layer < LAYERS; layer++)
            {
                _points[layer][i].x = (float)i / _sampleCount * _graphContentRect.width + _graphContentRect.xMin;
                _points[layer][i].y = _graphContentRect.yMax - _samples[i].data[layer] / maxValue * _graphContentRect.height;
            }
        }

        //标题
        string head = string.Format("   Samples:{0:N0}   Max: {1:N0}   Avg: {2:N1}",  _samples.Count, maxValue, avgValue);
        EditorGUI.LabelField(new Rect(_axisRect.center.x - 400, _axisRect.y - 50, 800, 50), head, _headStyle);

        //填充曲线
        _graphMaterial.SetPass(0);
        for (int layer = 0; layer < LAYERS; ++layer)
        {
            GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(_layerColor[layer]);
            for (int i = 0; i < _samples.Count; ++i)
            {
                if (_graphRect.Contains(_points[layer][i]))
                {
                    GL.Vertex(_points[layer][i]);
                    if (layer == LAYERS - 1)
                        GL.Vertex3(_points[layer][i].x, _graphContentRect.yMax, 0);
                    else
                        GL.Vertex(_points[layer + 1][i]);
                }
            }
            GL.End();
        }

        //画边(这里的作用是去锯齿)
        Handles.BeginGUI();
        for (int layer = 0; layer < LAYERS; ++layer)
        {
            Handles.color = _layerColor[layer];
            Handles.DrawAAPolyLine(_points[layer].Where(p => _graphRect.Contains(p)).ToArray());
        }
        Handles.EndGUI();

        //定位线
        if (_graphRect.Contains(_points[0][_current]))
        {
            Handles.BeginGUI();
            Handles.color = Color.white;
            Handles.DrawAAPolyLine(3, new Vector2(_points[0][_current].x, _axisRect.yMin), new Vector2(_points[0][_current].x, _axisRect.yMax));
            Handles.DrawAAPolyLine(2, new Vector2(_axisRect.x, _points[0][_current].y), _points[0][_current]);
            Handles.EndGUI();
            EditorGUI.LabelField(new Rect(_points[0][_current].x - 10, _axisRect.yMax + 5, 50, 20), _current.ToString());
            EditorGUI.LabelField(new Rect(_axisRect.xMin - 50, _points[0][_current].y - 10, 50, 20), _samples[_current].data[0].ToString());

            //详细数据
            string detail = string.Format("<color={0}>Total:{1:N0}</color>   <color={2}>Data1:{3:N0}</color>   " +
                "<color={4}>Data2:{5:N0}</color>   <color={6}>Data3:{7:N0}</color>   <color={8}>Data4:{9:N0}</color>",
                Color2String(Color.white), _samples[_current].data[0],
                Color2String(_layerColor[0]), _samples[_current].data[0] - _samples[_current].data[1],
                Color2String(_layerColor[1]), _samples[_current].data[1] - _samples[_current].data[2],
                Color2String(_layerColor[2]), _samples[_current].data[2] - _samples[_current].data[3],
                Color2String(_layerColor[3]), _samples[_current].data[3]);
            EditorGUI.LabelField(new Rect(_axisRect.center.x - 400, _axisRect.yMax + 20, 800, 50), detail, _headStyle);
        }

        //坐标轴
        DrawArrow(new Vector2(_axisRect.xMin, _axisRect.yMax), new Vector2(_axisRect.xMin, _axisRect.yMin), Color.white);
        DrawArrow(new Vector2(_axisRect.xMin, _axisRect.yMax), new Vector2(_axisRect.xMax, _axisRect.yMax), Color.white);

    }

    private void HandleEvent()
    {
        var point = Event.current.mousePosition;
        switch (Event.current.type)
        {
            case EventType.MouseDrag:
                if (Event.current.button == 0 && _clickGraph)
                {
                    UpdateCurrentSample();
                    Repaint();
                }

                if (Event.current.button == 2 && _clickGraph)
                {
                    _graphContentRect.x += Event.current.delta.x;
                    if (_graphContentRect.x > _graphRect.x)
                        _graphContentRect.x = _graphRect.x;
                    if (_graphContentRect.xMax < _graphRect.xMax)
                        _graphContentRect.x = _graphRect.xMax - _graphContentRect.width;
                    Repaint();
                }
                break;

            case EventType.MouseDown:
                _clickGraph = _graphRect.Contains(point);
                if (_clickGraph)
                    EditorGUI.FocusTextInControl(null);

                if (Event.current.button == 0 && _clickGraph)
                {
                    UpdateCurrentSample();
                    Repaint();
                }
                if (Event.current.button == 1)
                {
                    //DrawFloatMenu();
                    Repaint();
                }
                break;

            case EventType.KeyDown:
                if (Event.current.keyCode == KeyCode.LeftArrow)
                    SetCurrentIndex(_current - 1);
                if (Event.current.keyCode == KeyCode.RightArrow)
                    SetCurrentIndex(_current + 1);
                Repaint();
                break;

            case EventType.ScrollWheel:
                if (_graphRect.Contains(point))
                {
                    if (Event.current.delta.y < 0)
                    {
                        float factor = 0.9f;
                        float maxWidth = _graphRect.width * _sampleCount / Mathf.Min(100, _sampleCount);
                        if (_graphContentRect.width / factor > maxWidth)
                            factor = _graphContentRect.width / maxWidth;
                        _graphContentRect.x = (_graphContentRect.x - point.x) / factor + point.x;
                        _graphContentRect.width /= factor;
                    }
                    if (Event.current.delta.y > 0)
                    {
                        float factor = 0.9f;
                        if (_graphContentRect.width * factor < _graphRect.width)
                            factor = _graphRect.width / _graphContentRect.width;
                        _graphContentRect.x = (_graphContentRect.x - point.x) * factor + point.x;
                        _graphContentRect.width *= factor;
                    }
                    if (_graphContentRect.x > _graphRect.x)
                        _graphContentRect.x = _graphRect.x;
                    if (_graphContentRect.xMax < _graphRect.xMax)
                        _graphContentRect.x = _graphRect.xMax - _graphContentRect.width;
                    Repaint();
                }
                break;
        }
    }

    private void UpdateCurrentSample()
    {
        float x = Event.current.mousePosition.x;
        float distance = float.MaxValue;
        int index = 0;
        for (int i = 0; i < _points[0].Length; ++i)
        {
            if (_graphRect.Contains(_points[0][i]) && Mathf.Abs(x - _points[0][i].x) < distance)
            {
                distance = Mathf.Abs(x - _points[0][i].x);
                index = i;
            }
        }
        SetCurrentIndex(index);
    }

    private void SetCurrentIndex(int i)
    {
        _current = Mathf.Clamp(i, 0, _samples.Count - 1);
    }


    private string Color2String(Color color)
    {
        string c = "#";
        c += ((int)(color.r * 255)).ToString("X2");
        c += ((int)(color.g * 255)).ToString("X2");
        c += ((int)(color.b * 255)).ToString("X2");
        return c;
    }

    private void DrawArrow(Vector2 from, Vector2 to, Color color)
    {
        Handles.BeginGUI();
        Handles.color = color;
        Handles.DrawAAPolyLine(3, from, to);
        Vector2 v0 = from - to;
        v0 *= 10 / v0.magnitude;
        Vector2 v1 = new Vector2(v0.x * 0.866f - v0.y * 0.5f, v0.x * 0.5f + v0.y * 0.866f);
        Vector2 v2 = new Vector2(v0.x * 0.866f + v0.y * 0.5f, v0.x * -0.5f + v0.y * 0.866f); ;
        Handles.DrawAAPolyLine(3, to + v1, to, to + v2);
        Handles.EndGUI();
    }


    private int GetListMaxValue(List<DataClass> list)
    {
        List<int> newList = new List<int>();
        for(int i =0;i<list.Count;i++)
        {
            for(int j=0;j<4;j++)
            {
                newList.Add(list[i].data[j]);
            }
        }
        return newList.Max();
    }

    private float GetListAverageValue(List<DataClass> list)
    {
        List<int> newList = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                newList.Add(list[i].data[j]);
            }
        }
        return (float)newList.Average();
    }


    private List<DataClass> GenFackeData()
    {
        List<DataClass> list = new List<DataClass>();
        DataClass data = null;
        for (int i = 0; i < 100; i++)
        {
            data = new DataClass();
            int value = UnityEngine.Random.Range(60, 100);
            data.data[0] = value;

            value = UnityEngine.Random.Range(40, 60);
            data.data[1] = value;

            value = UnityEngine.Random.Range(20,40);
            data.data[2] = value;

            value = UnityEngine.Random.Range(0, 20);
            data.data[3] = value;

            list.Add(data);
        }


        return list;
    }
}
