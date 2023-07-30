using Extensions;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Row[] rows { get; private set; }
    public int rowCount { get; private set; }

    private void Awake()
    {
        // Order children by number if they are not in order already
        rowCount = transform.childCount;
        rows = transform.GetChildrenByOrder<Row>();
    }

    public void Clear()
    {
        foreach (Row row in rows)
            row.Clear();
    }
}
