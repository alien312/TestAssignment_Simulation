using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_SettingsWindowController : MonoBehaviour
{
    [SerializeField] private Slider FieldSideSlider;
    [SerializeField] private Text FieldSizeText;
    [SerializeField] private Slider AnimalsAmountSlider;
    [SerializeField] private Text AnimalAmountText;
    [SerializeField] private Slider SpeedSlider;
    [SerializeField] private Text SpeedText;
    void Start()
    {
        FieldSideSlider.onValueChanged.AddListener(OnFieldSizeChanged);
        AnimalsAmountSlider.onValueChanged.AddListener(OnAnimalsAmountChanged);
        SpeedSlider.onValueChanged.AddListener(OnSpeedValueChanged);

        var fieldSizeValue = FieldSideSlider.value;
        AnimalsAmountSlider.maxValue = fieldSizeValue * fieldSizeValue / 2;
    }

    public void OnBackButtonClick()
    {
        gameObject.SetActive(false);
    }

    private void OnFieldSizeChanged(float value)
    {
        FieldSizeText.text = value+" X "+value;
        AnimalsAmountSlider.maxValue = value * value / 2;
    }

    private void OnAnimalsAmountChanged(float value)
    {
        AnimalAmountText.text = value.ToString();
    }

    private void OnSpeedValueChanged(float value)
    {
        SpeedText.text = value.ToString();
    }

    public void OnStartSimulationClick()
    {
        GameManager.Instance.StartSimulation((int) FieldSideSlider.value, (int) AnimalsAmountSlider.value, (int) SpeedSlider.value);
    }
}
