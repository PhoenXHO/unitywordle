using Extensions;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Row[] rows { get; private set; }
    public int rowCount { get; private set; }

    public Tile.TileState emptyState;

    private void Awake()
    {
        // Order children by number if they are not in order already
        rowCount = transform.childCount;
        rows = transform.GetChildrenByOrder<Row>();
    }

    public void Clear()
    {
        foreach (Row row in rows)
        {
            foreach (Tile tile in row.tiles)
            {
                tile.Clear();
                StartCoroutine(tile.SetState(emptyState));
            }
        }
    }
}
