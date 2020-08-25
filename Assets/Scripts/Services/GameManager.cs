using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var gm = new GameObject("Game Manager");
                _instance = gm.AddComponent<GameManager>();
            }

            return _instance;
        }
    }

    private static GameManager _instance;

    private EventManager m_EventManager;

    private Generator m_Generator;

    private ObjectPool m_ObjectPool;
    
    public PlayerData PlayerData;

    public const string SaveKey = "Save";

    private void Awake()
    {
        if (_instance == null)
            _instance = FindObjectOfType<GameManager>();
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
        m_EventManager = new EventManager();
        m_ObjectPool = new ObjectPool();
        m_Generator = new Generator();
    }

    public void StartSimulation(int fieldSize, int animalsAmount, int speed)
    {
        PlayerData = new PlayerData
        {
            FieldData = new FieldData {Size = fieldSize, AnimalsAmount = animalsAmount, Speed = speed}
        };
        
        SceneManager.LoadScene(1);
    }

    public void LoadSimulation()
    {
        PlayerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(SaveKey));
        SceneManager.LoadScene(1);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex != 1) return;

        EventManager.Instance.PostNotification(EventType.SimulationSceneLoaded, this, PlayerData.FieldData);
        VisualEffect.LoadVisualEffect(PlayerData.FieldData.AnimalsAmount);
        
        var cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            cameraController.SetInitialCameraPosition(PlayerData.FieldData.Size);
        }
        else
        {
            Debug.Log("Не найден CameraController");
        }
    }

    public void SaveData()
    {
        PlayerData.FieldData.Animals = m_Generator.CreatAnimalData();
        var json = JsonUtility.ToJson(PlayerData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }
}
