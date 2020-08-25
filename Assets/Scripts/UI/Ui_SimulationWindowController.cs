using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ui_SimulationWindowController : MonoBehaviour
{
    [SerializeField] private Button SaveButton;

    [SerializeField] private Slider SimulationSpeedSlider;
    [SerializeField] private Text SimulationSpeedText;

    void Start()
    {
        SaveButton.onClick.AddListener(GameManager.Instance.SaveData);
        SimulationSpeedSlider.onValueChanged.AddListener(OnSpeedValueChanged);
    }

    private void OnSpeedValueChanged(float value)
    {
#if UNITY_EDITOR
        if (value > 100)
            value = 100;
#endif
        Time.timeScale = value;
        SimulationSpeedText.text = value.ToString();
    }

    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene(0);
    }
}