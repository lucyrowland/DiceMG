using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DiceMG;

public class SpriteManager
{
    private Dictionary<string, Texture2D> _textures;
    private GraphicsDevice _graphicsDevice;
    
    public SpriteManager(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _textures = new Dictionary<string, Texture2D>();
    }

    public void LoadTexture(string key, Texture2D texture)
    {
        if (!_textures.ContainsKey(key))
        {
            _textures.Add(key, texture);
        }
    }

    public Texture2D GetTexture(string key)
    {
        if (_textures.ContainsKey(key))
        {
            return _textures[key];
        }
        return null;
    }
    public void Draw(SpriteBatch spriteBatch, string key, GameObject obj, Color color)
    {
        Texture2D texture = GetTexture(key);
        if (texture != null)
        {
            spriteBatch.Draw(texture, obj.Box, color);
        }
    }
    // Draw with rotation (useful for dice)
    public void Draw(SpriteBatch spriteBatch, string key,  GameObject obj, float rotation)
    {
        Texture2D texture = GetTexture(key);
        if (texture != null)
        {
            spriteBatch.Draw(
                texture,
                obj.Box, 
                null,
                Color.White,
                rotation,
                new Vector2(texture.Width / 2, texture.Height / 2),
                SpriteEffects.None,
                0f
            );
        }
    }

    public void Unload()
    {
        foreach (var texture in _textures)
        {
            texture.Value.Dispose();
        }
        _textures.Clear();
    }

}