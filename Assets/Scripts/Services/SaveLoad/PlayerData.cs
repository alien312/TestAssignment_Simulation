using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public FieldData FieldData;
}

[Serializable]
public class FieldData
{
    public int Size;
    public int AnimalsAmount;
    public int Speed;

    public AnimalData[] Animals;
}

[Serializable]
public class AnimalData
{
    public Color Color;
    public Vector2Int CurrentCell;
    public Vector2Int TargetCell;
}
