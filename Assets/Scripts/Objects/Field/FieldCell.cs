using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCell : MonoBehaviour
{
    public bool IsOccupiedByAnimal => AnimalOnCell != null;
    public bool IsOccupiedByFood => FoodOnCell != null;

    public Vector2Int Coordinates { get; private set; }

    public TeamObject AnimalOnCell { get; set; }
    public TeamObject FoodOnCell { get; set; }

    private void Awake()
    {
        Coordinates = new Vector2Int((int)transform.position.z, (int) transform.position.x);
    }

    public float GetSqrDistance(FieldCell fieldCell)
    {
        var currentCellPos = new Vector2(transform.position.x, transform.position.z);
        var targetCellPos = new Vector2(fieldCell.transform.position.x, fieldCell.transform.position.z);
        return (targetCellPos - currentCellPos).sqrMagnitude;
    }
}
