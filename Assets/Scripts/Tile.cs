using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<ITileObject> objects = new List<ITileObject>();

    public Vector2Int position;

}
