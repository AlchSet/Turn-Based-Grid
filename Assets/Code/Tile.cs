using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedGrid
{
    public class Tile : MonoBehaviour
    {
        //public GameObject g;
        public BoxCollider2D cellBounds;
        public int X;
        public int Y;
        public bool isWalkable;
        public bool isOccupied;

        public void InitTile(int x, int y, Vector3 position, LayerMask layer, LayerMask obstacles)
        {
            X = x;
            Y = y;
            //g = new GameObject("Tile"+X+","+Y);
            //g.AddComponent<Tile>();
            transform.position = position;
            gameObject.AddComponent<BoxCollider2D>();
            gameObject.layer = GameGrid.layermask_to_layer(layer);

            isWalkable = !Physics2D.OverlapPoint(position, obstacles);

        }
    }
}