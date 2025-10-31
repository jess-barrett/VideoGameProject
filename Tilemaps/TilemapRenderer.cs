using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject2.Tilemaps
{
    public class TilemapRenderer
    {
        public static void DrawLayer(SpriteBatch spriteBatch, Tilemap tilemap, TileLayer layer, float layerDepth, float scale = 1f)
        {
            if (tilemap?.TilesetTexture == null || layer == null) return;

            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    int index = y * layer.Width + x;
                    int tileId = layer.Tiles[index];

                    if (tileId == 0) continue;

                    tileId -= 1;

                    int tileX = tileId % tilemap.TilesetColumns;
                    int tileY = tileId / tilemap.TilesetColumns;

                    Rectangle sourceRect = new Rectangle(
                        tileX * tilemap.TilesetTileWidth,
                        tileY * tilemap.TilesetTileHeight,
                        tilemap.TilesetTileWidth,
                        tilemap.TilesetTileHeight
                    );

                    Vector2 position = new Vector2(
                        x * tilemap.TileWidth * scale,
                        y * tilemap.TileHeight * scale
                    );

                    spriteBatch.Draw(
                        tilemap.TilesetTexture,
                        position,
                        sourceRect,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        scale,
                        SpriteEffects.None,
                        layerDepth
                    );
                }
            }
        }
    }
}