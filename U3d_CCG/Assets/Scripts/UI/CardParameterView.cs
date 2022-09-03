using TMPro;
using UnityEngine;

public class CardParameterView : MonoBehaviour
{
    [SerializeField] private ParamTypes type;
    [SerializeField] private TextMeshProUGUI text;

    public ParamTypes GetParamType => type;

    public void SetActive(bool value) => gameObject.SetActive(value);

    public void SetValue(int value) => text.text = value.ToString();
}