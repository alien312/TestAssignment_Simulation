using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class VisualEffect : MonoBehaviour, IPoolingObject
{
    public static GameObject VisualEffectObject;
    private ParticleSystem m_ParticleSystem;
    void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }
    
    public bool IsFree => m_ParticleSystem.isStopped;
    public void OnObjectSpawn()
    {
        gameObject.SetActive(true);
        m_ParticleSystem.Play();
    }

    public static void LoadVisualEffect(int poolSize)
    {
        VisualEffectObject = Resources.Load<GameObject>("VisualEffect");
        ObjectPool.Instance.SetPoolCapacity(VisualEffectObject.GetHashCode(), poolSize);
    }
}
