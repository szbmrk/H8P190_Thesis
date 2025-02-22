using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PokerParty_SharedDLL;
using UnityEngine.EventSystems;

public class BetAmountGUI : MonoBehaviour
{
    [SerializeField] private ActionButton submitBtn;
    [SerializeField] private Button plus;
    [SerializeField] private Button minus;
    [SerializeField] private Button _25;
    [SerializeField] private Button _50;
    [SerializeField] private Button _75;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField moneyInput;
    [SerializeField] private TextMeshProUGUI submitBtnText;
    [SerializeField] private string submitText;

    public int minValue;
    public int maxValue;

    private bool isPlusHeld = false;
    private bool isMinusHeld = false;
    private float holdTimer = 0f;
    [SerializeField] private float initialDelay = 0.3f;
    [SerializeField] private float changeInterval = 0.1f;

    private PossibleAction originalAction;

    public void Initialize(int minValue, int maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    private void Start()
    {
        _25.onClick.AddListener(() => SetSliderValue(minValue + (maxValue - minValue) / 4));
        _50.onClick.AddListener(() => SetSliderValue(minValue + (maxValue - minValue) / 2));
        _75.onClick.AddListener(() => SetSliderValue(minValue + (maxValue - minValue) * 3 / 4));
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        SetSliderValue(minValue);
        moneyInput.onEndEdit.AddListener((value) => OnInputEndEdit(value));
        slider.onValueChanged.AddListener((value) => SetSliderValue((int)value));
        originalAction = submitBtn.action;
    }

    private void Update()
    {
        minus.interactable = slider.value != minValue;

        if (slider.value == maxValue)
        {
            plus.interactable = false;
            submitBtnText.text = "All in";
            submitBtn.action = PossibleAction.AllIn;
        }
        else
        {
            plus.interactable = true;
            submitBtnText.text = $"{submitText} {slider.value}$";
            submitBtn.action = originalAction;
        }

        if (isPlusHeld || isMinusHeld)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= (holdTimer == 0 ? initialDelay : changeInterval))
            {
                if (isPlusHeld) Plus();
                if (isMinusHeld) Minus();
                holdTimer = 0f;
            }
        }
    }

    private void Plus()
    {
        int current = (int)slider.value;

        if (current >= maxValue) return;

        int nextValue = GetNextStep(current);

        nextValue = RoundToNearestMultipleOf5Or10(nextValue);

        if (nextValue > maxValue)
            nextValue = maxValue;

        SetSliderValue(nextValue);
    }

    private void Minus()
    {
        int current = (int)slider.value;

        if (current <= minValue) return;

        int prevValue = GetPreviousStep(current);

        prevValue = RoundToNearestMultipleOf5Or10(prevValue);

        if (prevValue < minValue)
            prevValue = minValue;

        SetSliderValue(prevValue);
    }

    private int GetNextStep(int current)
    {
        int range = maxValue - minValue;
        int stepSize = range / 20;

        stepSize = stepSize switch
        {
            < 1 => 1,
            < 2 => 2,
            < 5 => 5,
            _ => (stepSize / 5) * 5
        };

        int nextValue = current + stepSize;

        return nextValue;
    }

    private int GetPreviousStep(int current)
    {
        int range = maxValue - minValue;
        int stepSize = range / 20;

        stepSize = stepSize switch
        {
            < 1 => 1,
            < 2 => 2,
            < 5 => 5,
            _ => (stepSize / 5) * 5
        };

        int prevValue = current - stepSize;

        return prevValue;
    }

    private int RoundToNearestMultipleOf5Or10(int value)
    {
        if (maxValue - minValue < 99)
            return value;
        
        return maxValue - minValue < 249 ? (int)(Math.Round(value / 5.0) * 5) : (int)(Math.Round(value / 10.0) * 10);
    }

    public void OnPlusHeldDown()
    {
        isPlusHeld = true;
        Plus();
        holdTimer = initialDelay;
    }

    public void OnMinusHeldDown()
    {
        isMinusHeld = true;
        Minus();
        holdTimer = initialDelay;
    }


    public void OnPlusHeldUp()
    {
        isPlusHeld = false;
        holdTimer = 0f;
    }

    public void OnMinusHeldUp()
    {
        isMinusHeld = false;
        holdTimer = 0f;
    }

    private void SetSliderValue(int value)
    {
        slider.value = value;
        moneyInput.text = value.ToString() + " $";
        submitBtnText.text = $"{submitText} {value}$";
        submitBtn.amount = value;
    }

    private void OnInputEndEdit(string value)
    {
        if (int.TryParse(value, out int result))
        {
            if (result >= minValue && result <= maxValue)
                SetSliderValue(result);
            else
            {
                SetSliderValue(result < minValue ? minValue : maxValue);
            }
        }
        else
            moneyInput.text = slider.value.ToString() + "$";
    }

}