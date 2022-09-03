using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu]
    public class GameSet : ScriptableObject
    {
        [SerializeField] private int minCards = 4;
        [SerializeField] private int maxCards = 6;
        [Space] 
        [SerializeField] private int minParamValue = 1;
        [SerializeField] private int maxParamValue = 6;
        [Space] 
        [SerializeField] private int minChangeValue = -2;
        [SerializeField] private int maxChangeValue = 9;
        [Space]
        public CardView cardPrefab;
        [Space]
        public float paramChangeDuration;
        public float cardAppearDuration = 0.4f;
        public float cardRemoveDuration;
        public float cardCompactDuration;
        public float cardReturnDuration = 0.4f;

        [Space] [SerializeField] private List<string> titles;
        [Space] [SerializeField] private List<string> descriptions;

        private int? _cardAmount;

        public int GetCardsAmount()
        {
            if (!_cardAmount.HasValue)
                _cardAmount = Random.Range(minCards, maxCards + 1);

            return _cardAmount.Value;
        }

        public int GetRandomParamValue() =>
            Random.Range(minParamValue, maxParamValue);

        public string GetRandomTitle() =>
            titles[Random.Range(0, titles.Count)];

        public string GetRandomDescription() =>
            descriptions[Random.Range(0, descriptions.Count)];

        public int GetRandomChangeParameterValue() =>
            Random.Range(0, maxChangeValue - minChangeValue) + minChangeValue;
    }
}