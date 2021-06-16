using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private int glassMaterialIndex, glassTextureMaterialIndex;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public Color Color
    {
        set
        {
            rend.materials[glassMaterialIndex].color = value;
            rend.materials[glassTextureMaterialIndex].color = value;
        }
    }
}
