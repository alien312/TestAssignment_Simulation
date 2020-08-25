using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredObject : MonoBehaviour
{
    private Renderer m_Renderer;
    private MaterialPropertyBlock m_PropertyBlock;
    private static readonly int ColorCached = Shader.PropertyToID("_Color");
    
    void Awake()
    {
        m_PropertyBlock = new MaterialPropertyBlock();
        m_Renderer = GetComponent<Renderer>();
    }
    
    public void Initialise(Color color)
    {
        m_Renderer.GetPropertyBlock(m_PropertyBlock);
        m_PropertyBlock.SetColor(ColorCached, color);
        m_Renderer.SetPropertyBlock(m_PropertyBlock);
    }
}
