using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ui_MainController : MonoBehaviour
{
    private Ui_SettingsWindowController m_SettingsWindowController;
    void Start()
    {
        m_SettingsWindowController = GetComponentInChildren<Ui_SettingsWindowController>(true);
    }
    
    public void OnNewSimulationButtonClick()
    {
        m_SettingsWindowController.gameObject.SetActive(true);
    }

    public void OnLoadSimulationButtonClick()
    {
        if(PlayerPrefs.HasKey(GameManager.SaveKey))
            GameManager.Instance.LoadSimulation();
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }
}
