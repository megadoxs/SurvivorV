using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTextureExporter : MonoBehaviour
{
        [MenuItem("Tools/Export Selected TilePrefab To Sprite")]
    public static void ExportSelectedTilePrefab()
    {
        TilePrefab tilePrefab = Selection.activeObject as TilePrefab;
        if (tilePrefab == null)
        {
            Debug.LogError("Please select a TilePrefab asset.");
            return;
        }

        ExportTilePrefabToSprite(tilePrefab);
    }

    public static void ExportTilePrefabToSprite(TilePrefab tilePrefab)
    {
        int tileSize = 16; // Change if using 32x32 or custom size
        int width = tilePrefab.Rows.Length > 0 ? tilePrefab.Rows[0].Tiles.Length : 0;
        int height = tilePrefab.Rows.Length;

        if (width == 0 || height == 0)
        {
            Debug.LogWarning("TilePrefab has no tile data.");
            return;
        }

        Texture2D texture = new Texture2D(width * tileSize, height * tileSize);
        texture.filterMode = FilterMode.Point; // crisp pixels

        for (int y = 0; y < height; y++)
        {
            TileBase[] row = tilePrefab.Rows[height - 1 - y].Tiles; // flip Y for Unity's texture orientation
            for (int x = 0; x < row.Length; x++)
            {
                if (row[x] is Tile tile && tile.sprite != null)
                {
                    Texture2D tileTex = GetTextureFromSprite(tile.sprite);
                    Color[] pixels = tileTex.GetPixels();
                    texture.SetPixels(x * tileSize, y * tileSize, tileSize, tileSize, pixels);
                }
            }
        }

        texture.Apply();

        string path = $"Assets/Exported_{tilePrefab.name}.png";
        File.WriteAllBytes(path, texture.EncodeToPNG());
        AssetDatabase.Refresh();

        Debug.Log($"TilePrefab exported to sprite: {path}");
    }

    private static Texture2D GetTextureFromSprite(Sprite sprite)
    {
        Rect rect = sprite.rect;
        Texture2D source = sprite.texture;

        Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height);
        Color[] pixels = source.GetPixels(
            Mathf.RoundToInt(rect.x),
            Mathf.RoundToInt(rect.y),
            Mathf.RoundToInt(rect.width),
            Mathf.RoundToInt(rect.height)
        );

        newTex.SetPixels(pixels);
        newTex.Apply();
        return newTex;
    }
}
