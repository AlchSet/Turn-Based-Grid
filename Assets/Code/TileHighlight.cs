using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedGrid.Ui
{
    public class TileHighlight : MonoBehaviour
    {
        public void HighlightPosition(Vector3 pos)
        {
            transform.position = pos;
        }


        public void SetOffscreen()
        {

            transform.position = Vector3.right * -100;

        }
    }
}