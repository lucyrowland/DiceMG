using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Apos.Shapes;

namespace DiceMG;

public class SpriteManager
{
    private Dictionary<string, Texture2D> _textures;
    private GraphicsDevice _graphicsDevice;
    private Texture2D pixel;
    
   
    
    public SpriteManager(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _textures = new Dictionary<string, Texture2D>();
        pixel = new Texture2D(_graphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
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
    public void Draw(SpriteBatch spriteBatch, string key, GameObject obj, float rotation)
    {
        Texture2D texture = GetTexture(key);
        if (texture != null)
        {
            // Use simple rectangle draw when no rotation
            if (rotation < 1f)
            {
                spriteBatch.Draw(texture, obj.Box, Color.White);
            }
            else
            {
                // For rotation, use position-based drawing
                Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 position = new Vector2(obj.Position.X + obj.Width / 2, obj.Position.Y + obj.Height / 2);
                Vector2 scale = new Vector2(obj.Width / texture.Width, obj.Height / texture.Height);
                
                spriteBatch.Draw(
                    texture,
                    position, 
                    null,
                    Color.White,
                    rotation,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
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