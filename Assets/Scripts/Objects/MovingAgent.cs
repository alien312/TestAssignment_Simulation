using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAgent : MonoBehaviour
{
    public FieldCell TargetCell { get; set; }
    public static float Speed;

    public FieldCell CurrentCell
    {
        get => m_CurrentCell;
        set
        {
            m_CurrentCell = value;
            var position = m_CurrentCell.transform.position;
            m_CurrentCellPos = new Vector2(position.x, position.z);
        }
    }

    private FieldCell m_CurrentCell;
    private Vector2 m_CurrentCellPos;
    private float m_Time = 0;
    private float m_TimeOnOneUnit => 1f / Speed;
    private Vector3 m_StartPos;
    void Start()
    {
        EventManager.Instance.PostNotification(EventType.AgentGotCell, this);
    }
    
    void Update()
    {
        var currentPos2D = new Vector2(transform.position.x, transform.position.z);
        if (Vector2.Distance(m_CurrentCellPos, currentPos2D) > 0.1f)
        {
            var t = Mathf.InverseLerp(0, m_TimeOnOneUnit, m_Time);
            var value = Mathf.Lerp(0, 1, t);
            transform.position = Vector3.Lerp(m_StartPos, CurrentCell.transform.position + Vector3.up, value);
            m_Time += Time.deltaTime;
        }
        else
        {
            EventManager.Instance.PostNotification(EventType.AgentGotCell, this);
            m_Time = 0;
            m_StartPos = transform.position;
        }
    }
}
