using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class
    CardView : MonoBehaviour, IPointerDownHandler, IDragHandler,
        IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public struct Ctx
    {
        public Sprite sprite;
        public string title;
        public string description;
        public List<CardValue> values;
        public float changeDuration;
        public ReactiveProperty<int> index;
        public RectTransform dropArea;
        public ReactiveCommand<int> onCardReturn;
    }

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private Image cardImage = null;
    [SerializeField] private GameObject glow = null;
    [SerializeField] private Image glowImage = null;
    [SerializeField] private TextMeshProUGUI title = null;
    [SerializeField] private TextMeshProUGUI description = null;
    [SerializeField] private List<CardParameterView> cardParameters = new List<CardParameterView>();

    private Ctx _ctx;
    private Coroutine _glowRoutine;
    private int _initOrder;
    private Vector3 _dragOffset;

    public void SetCtx(Ctx ctx)
    {
        _ctx = ctx;
        _ctx.index.Subscribe(OnChangeIndex);
        cardImage.sprite = _ctx.sprite;
        title.text = _ctx.title;
        description.text = _ctx.description;
        glow.SetActive(false);

        foreach (var cardParameter in cardParameters)
        {
            cardParameter.SetActive(false);

            var param = _ctx.values.FirstOrDefault(p => p.type == cardParameter.GetParamType);
            if (param != null)
            {
                cardParameter.SetValue(param.value);
                cardParameter.SetActive(true);
            }
        }
    }

    private void OnChangeIndex(int index)
    {
        canvas.sortingOrder = index;
        _initOrder = index;
    }
    private void SetOrder(int order)
    {
        canvas.sortingOrder = order;
    }

    public void ChangeCardParam(ParamTypes param, int oldValue, int newValue)
    {
        var cardParameter = cardParameters.FirstOrDefault(p => p.GetParamType == param);

        int value = oldValue;

        DOTween.To(() => value, x => value = x, newValue, _ctx.changeDuration)
            .OnUpdate(() => { cardParameter.SetValue(value); });
    }

    private void OnSelect()
    {
        // SetOrder(100, false);
        glow.SetActive(true);

        if (_glowRoutine != null)
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
        // SetOrder(_initOrder, false);
        if (_glowRoutine != null)
        {
            StopCoroutine(_glowRoutine);
            _glowRoutine = null;
        }

        glow.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnSelect();
        _dragOffset = (Vector3) eventData.position - transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = (Vector3) eventData.position - _dragOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnDeselect();

        var a = (!_ctx.dropArea.rect.Contains(eventData.position));
        if (!_ctx.dropArea.WorldSpaceRect().Contains(eventData.position))//.Overlaps((transform as RectTransform).WorldSpaceRect()))
            _ctx.onCardReturn.Execute(_ctx.index.Value);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetOrder(100);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetOrder(_initOrder);
    }
}