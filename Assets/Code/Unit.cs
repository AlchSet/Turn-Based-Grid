using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedGrid
{
    public class Unit : MonoBehaviour
    {
        public enum Faction { Player, Enemy }

        public Faction faction;
        public string unitName = "Unit Name";

        public int moves = 5;

        public Vector2Int gridPosition;



        public void UpdateUnit(int x, int y)
        {
            gridPosition.x = x;
            gridPosition.y = y;
        }


    }
}