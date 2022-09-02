using TMPro;
using UnityEngine;

public class CardParameterView : MonoBehaviour
{
    [SerializeField] private ParamTypes type;
    [SerializeField] private GameObject gameObject;
    [SerializeField] private TextMeshProUGUI text;

    public ParamTypes GetType => type;

    public void SetActive(bool value) => gameObject.SetActive(value);

    public void SetValue(int value) => text.text = value.ToString();
}