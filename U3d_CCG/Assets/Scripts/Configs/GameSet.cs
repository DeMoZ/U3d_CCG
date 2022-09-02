using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu]
    public class GameSet : ScriptableObject
    {
        [SerializeField] private int minCards = 4;
        [SerializeField] private int maxCards = 6;

        [SerializeField] private int minParamValue = 1;
        [SerializeField] private int maxParamValue = 6;

        [SerializeField] private List<string> titles;
        [SerializeField] private List<string> descriptions;
        
        public CardView cardPrefab;

        private int? _cardAmount;
        
        public int GetCardsAmount()
        {
            if(!_cardAmount.HasValue)
                _cardAmount = Random.Range(minCards, maxCards);

            return _cardAmount.Value;
        }
        
        public int GetRandomParamValue()
        {
            return Random.Range(minParamValue, maxParamValue);
        }

        public string GetRandomTitle()
        {
            return titles[Random.Range(0, titles.Count)];
        }
        
        public string GetRandomDescription()
        {
            return descriptions[Random.Range(0, descriptions.Count)];
        }
    }
}