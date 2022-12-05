using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot : MonoBehaviour
{
    public Transform BG;
    public Transform Common;
    public Transform Top;
    public Transform Pop;
    public Transform Cover;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
