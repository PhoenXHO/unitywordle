using Extensions;
using UnityEngine;

public class Row : MonoBehaviour
{
    public Tile[] tiles { get; private set; }
    public int tileCount { get; private set; }
    public int filledTileCount { get; private set; }

    public RectTransform rectTransform { get; private set; }

    private void Awake()
    {
        // Order children by number if they are not in order already
        tileCount = transform.childCount;
        tiles = transform.GetChildrenByOrder<Tile>();

        rectTransform = GetComponent<RectTransform>();
    }

    public void AppendLetter(char letter, int at)
    {
        if (at < 0 && tileCount <= at)
        {
            Debug.LogError("Index out of bounds.", gameObject);
            return;
        }

        tiles[at].SetLetter(letter);
        filledTileCount++;
    }
    public void RemoveLetter(int at)
    {
        if (at < 0 && tileCount <= at)
        {
            Debug.LogError("Index out of bounds.", gameObject);
            return;
        }

        tiles[at].Clear();
        filledTileCount--;
    }

    public void Clear()
    {
        foreach (Tile tile in tiles)
            tile.Clear();
    }
}
