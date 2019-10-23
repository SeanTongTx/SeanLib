using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: isequnce
/// </summary>
public class InitSequnce : MonoBehaviour
{
    public List<InitProcess> process = new List<InitProcess>();

    private void Awake()
    {
        foreach (var p in process)
        {
            p.Init();
        }
    }

}
