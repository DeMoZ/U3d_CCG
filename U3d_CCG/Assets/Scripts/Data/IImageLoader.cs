using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Data
{
    public interface IImageLoader
    {
        public Task<Sprite> Load();
        //public Task<List<Sprite>> Load(int amount = 1);
    }
}