using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{

    private Renderer rend = null;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public Color Color
    {
        set
        {
            rend.material.SetColor("Color_58E0201D", value);
        }
    }
}
