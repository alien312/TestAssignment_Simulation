using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TeamObject : ColoredObject
{
    [HideInInspector] public int Id;
    
    public static Dictionary<int, Color> Teams = new Dictionary<int, Color>();
    
    
    public void Initialise()
    {
        var color = Random.ColorHSV(0, 1, 0.5f, 1, 1, 1);
        var colorHash = color.GetHashCode();
        while (Teams.ContainsKey(colorHash))
        {
            color = Random.ColorHSV(0, 1, 0.5f, 1, 1, 1);
            colorHash = color.GetHashCode();
        }
        base.Initialise(color);
        Id = colorHash;
        Teams.Add(Id, color);
    }

    public new void Initialise(Color color)
    {
        base.Initialise(color);
        Id = color.GetHashCode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Animal")) return;
        if (other.gameObject.TryGetComponent<TeamObject>(out var animal))
        {
            if (!animal.Id.Equals(Id)) return;
            gameObject.SetActive(false);
            ObjectPool.Instance.SpawnFromPool(VisualEffect.VisualEffectObject, transform.position);
        }
        else
        {
            Debug.Log("Отсутсвует компонент TeamObject на объекте " + other.gameObject.name);
        }
    }
}
