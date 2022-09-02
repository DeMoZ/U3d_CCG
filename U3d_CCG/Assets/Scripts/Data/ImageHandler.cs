using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using UnityEngine;

public class ImageHandler : IDisposable
{
    private IImageLoader _loader;
    
    public ImageHandler(IImageLoader loader)
    {
        _loader = loader;
    }

    public async Task<Sprite> GetImage()
    {
        var sprite = await _loader.Load();
        return sprite;
    }
    
    public async Task<List<Sprite>> GetImages(int amount = 1)
    {
        var sprites = new List<Sprite>();
            
        for (var i = 0; i < amount; i++) 
            sprites.Add(await _loader.Load());

        return sprites;
    }

    public void Dispose()
    {
    }
}