using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace Data
{
    public class ImageLoader : IImageLoader, IDisposable
    {
        private string _url;

        public ImageLoader(string url)
        {
            _url = url;
        }

        public async Task<Sprite> Load()
        {
            using var webClient = new WebClient();

            try
            {
                var data = await webClient.DownloadDataTaskAsync(_url);
                var tex = new Texture2D(1, 1);
                tex.LoadImage(data);
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                    new Vector2(tex.width / 2, tex.height / 2));
            }
            catch (Exception e)
            {
                // ignored
                Debug.LogError(e);
                return null;
            }
        }
        
        /*public async Task<List<Sprite>> Load(int amount = 1)
        {
            var sprites = new List<Sprite>();
            
            for (var i = 0; i < amount; i++) 
                sprites.Add(await Load());

            return sprites;
        }*/
        
        /*public async Task<List<Sprite>> Load(int amount = 1)
        {
            var sprites = new List<Sprite>();
            using var webClient = new WebClient();
            
            for (int i = 0; i < amount; i++)
            {
                var data = await webClient.DownloadDataTaskAsync(_url);
                var tex = new Texture2D(1, 1);
                tex.LoadImage(data);
                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                    new Vector2(tex.width / 2, tex.height / 2));

                sprites.Add(sprite);
            }

            return sprites;
        }*/

        public void Dispose()
        {
        }
    }
}