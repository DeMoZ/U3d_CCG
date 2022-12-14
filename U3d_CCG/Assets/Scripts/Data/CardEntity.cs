using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Data
{
    public class CardEntity
    {
        public CardView view;
        public List<CardValue> parameters;
        public Sprite sprite;
        public string title;
        public string description;
        public Vector3 position;
        public ReactiveProperty<int> index;
    }
}