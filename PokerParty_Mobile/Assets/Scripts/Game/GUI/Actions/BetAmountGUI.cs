using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PokerParty_SharedDLL;

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

    [HideInInspector] public int minValue;
    [HideInInspector] public int maxValue;

    private PossibleAction originalAction;

    public void Initialize(int minValue, int maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    private void Start()
    {
        plus.onClick.AddListener(Plus);
        minus.onClick.AddListener(Minus);
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
            submitBtnText.text = $"{submitText} ({slider.value} $)";
            submitBtn.action = originalAction;
        }
    }

    private void Plus()
    {
        int current = (int)slider.value;
        
        if (current >= maxValue) return;
        
        current += (int)(maxValue * 0.1f);
        
        if (current > maxValue)
            current = maxValue;
        
        SetSliderValue(current);
    }

    private void Minus()
    {
        int current = (int)slider.value;
        
        if (current <= minValue) return;
        
        current -= (int)(maxValue * 0.1f);
        
        if (current < minValue)
            current = minValue;
        
        SetSliderValue(current);
    }

    private void SetSliderValue(int value)
    {
        slider.value = value;
        moneyInput.text = value.ToString() + " $";
        submitBtnText.text = $"{submitText} ({value} $)";
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
            moneyInput.text = slider.value.ToString() + " $";
    }

}