using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public struct Ctx
    {
        public Sprite sprite;
        public string title;
        public string description;
        public List<CardValue> values;
    }

    [SerializeField] private Image cardImage = null;
    [SerializeField] private GameObject glow = null;
    [SerializeField] private Image glowImage = null;
    [SerializeField] private TextMeshProUGUI title = null;
    [SerializeField] private TextMeshProUGUI description = null;
    [SerializeField] private List<CardParameterView> cardParameters = new List<CardParameterView>();

    private Ctx _ctx;
    private Coroutine _glowRoutine;
    
    public void SetCtx(Ctx ctx)
    {
        _ctx = ctx;
        cardImage.sprite = _ctx.sprite;
        title.text = _ctx.title;
        description.text = _ctx.description;
        glow.SetActive(false);

        foreach (var cardParameter in cardParameters)
        {
            cardParameter.SetActive(false);

            var param = _ctx.values.FirstOrDefault(p => p.type == cardParameter.GetType);
            if (param != null)
            {
                cardParameter.SetValue(param.value);
                cardParameter.SetActive(true);
            }
        }
    }

    private void OnChangeCardParam(ParamTypes param, int oldValue, int newValue)
    {
        var cardParameter = cardParameters.FirstOrDefault(p => p.GetType == param);

        int value = oldValue;

        DOTween.To(() => value, x => value = x, newValue, 5)
            .OnUpdate(() => { cardParameter.SetValue(value); });
    }

    private void OnSelect()
    {
        glow.SetActive(true);
        
        if(_glowRoutine!=null)
            StopCoroutine(_glowRoutine);

        _glowRoutine = StartCoroutine(GlowRoutine());
    }

    private IEnumerator GlowRoutine()
    {
        var color = glowImage.color;
        while (true)
        {
            var alpha = Mathf.PingPong(Time.time, 0.3f);
            color.a = alpha;
            glowImage.color = color;
            yield return null;
        }    
    }
    
    private void OnDeselect()
    {
        glow.SetActive(false);
    }
}