using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour
{
    [HideInInspector] public FieldCell[][] FieldCells;
    [HideInInspector] public List<FieldCell> FreeCells;

    public void Initialize(int fieldSize)
    {
        FieldCells = new FieldCell[fieldSize][];
        for (var index = 0; index < FieldCells.Length; index++)
            FieldCells[index] = new FieldCell[fieldSize];
        FreeCells = new List<FieldCell>();
        EventManager.Instance.AddListener(EventType.AgentGotCell, OnAgentGotCell);
    }
    
    public FieldCell GetRandomFreeCell()
    {
        if (FreeCells == null || FreeCells.Count == 0) return null;
        
        var cell = FreeCells[Random.Range(0, FreeCells.Count - 1)];
        FreeCells.Remove(cell);
        return cell;
    }

    public FieldCell GetRandomCell()
    {
        return FieldCells[Random.Range(0, FieldCells.Length - 1)][Random.Range(0, FieldCells.Length - 1)];
    }
    
    private void OnAgentGotCell(EventType eventType, Component sender, object param)
    {
        var agent = (MovingAgent) sender;
        var currentAgentCell = agent.CurrentCell;
        float distanceToTarget;
        if (currentAgentCell.Equals(agent.TargetCell))
        {
            EventManager.Instance.PostNotification(EventType.AgentGotTargetCell, agent.gameObject.GetComponent<TeamObject>());
        }
        else
        {
            var minimalDistance = Mathf.Infinity;
            FieldCell newTarget = null;
            for (var dx = -1; dx < 2; dx++)
            {
                for (var dy = -1; dy < 2; dy++)
                {
                    if(dy==dx && dx == 0) continue;
                    
                    var coordinates = currentAgentCell.Coordinates;
                    var newX = coordinates.x + dx;
                    var newY = coordinates.y + dy;
                    if (newX >= 0 && newX < FieldCells.Length &&
                        newY >= 0 && newY < FieldCells.Length)
                    {
                        var cell = FieldCells[newX][newY];
                        if(cell.IsOccupiedByAnimal) continue;
                        if (cell.Equals(agent.TargetCell))
                        {
                            agent.CurrentCell.AnimalOnCell = null;
                            agent.CurrentCell = cell;
                            agent.CurrentCell.AnimalOnCell = agent.gameObject.GetComponent<TeamObject>();
                            return;
                        }
                        var distance = cell.GetSqrDistance(agent.TargetCell);
                        if (distance < minimalDistance)
                        {
                            minimalDistance = distance;
                            newTarget = FieldCells[newX][newY];
                            distanceToTarget = Mathf.Sqrt(distance);
                        }
                    }
                }
            }

            if (newTarget != null)
            {
                agent.CurrentCell.AnimalOnCell = null;
                agent.CurrentCell = newTarget;
                agent.CurrentCell.AnimalOnCell = agent.gameObject.GetComponent<TeamObject>();
            }
            else
            {
                Debug.Log("Невозможно установить новую цель для объекта " + agent.gameObject.name);
            }
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventType.AgentGotCell, OnAgentGotCell);
    }
}
