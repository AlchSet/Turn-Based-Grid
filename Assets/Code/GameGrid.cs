using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TurnBasedGrid
{
    [AddComponentMenu("Turn Based Game/Game Grid")]
    public class GameGrid : MonoBehaviour
    {


        public Material gridMat;

        public Renderer gridMesh;

        public Transform grid;

        public Vector3 topCorner;
        [Tooltip("Assign only 1 layer")]
        public LayerMask tileLayer;
        public LayerMask osbtacleLayer;

        public int X;
        public int Y;




        public List<Tile> tiles = new List<Tile>();

        public Tile[,] tileGird;

        // Start is called before the first frame update
        void Awake()
        {


            gridMat = GetComponent<Renderer>().material;

            Vector3 pos = transform.position;

            topCorner = pos - transform.localScale / 2;



            gridMat.SetTextureScale("_MainTex", transform.localScale);


            Vector3 offset = transform.right * 0.25f + transform.up * 0.25f;
            //Vector3 offset = Vector3.zero;

            tileGird = new Tile[Y, X];

            for (int y = 0; y < Y; y++)
            {
                for (int x = 0; x < X; x++)
                {
                    Tile t = CreateTile(x, y, topCorner + transform.right * x + offset + transform.up * y + offset);
                    t.transform.SetParent(transform);
                    tiles.Add(t);
                    tileGird[y, x] = t;
                }
            }


        }

        Tile CreateTile(int x, int y, Vector3 position)
        {
            GameObject g = new GameObject("Tile:" + x + "," + y);
            Tile t = g.AddComponent<Tile>();
            t.InitTile(x, y, position, tileLayer, osbtacleLayer);
            return t;


        }

        public static int layermask_to_layer(LayerMask layerMask)
        {
            int layerNumber = 0;
            int layer = layerMask.value;
            while (layer > 0)
            {
                layer = layer >> 1;
                layerNumber++;
            }
            return layerNumber - 1;
        }


        //2D
        public Tile GetTileFromMouse()
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, tileLayer);

            if (hit.collider != null)
            {
                return hit.collider.GetComponent<Tile>();
                // raycast hit this gameobject
            }
            return null;
        }


        public Tile GetTileFromPoint(Vector3 point)
        {

            Collider2D c = Physics2D.OverlapPoint(point, tileLayer);

            if (c != null)
            {
                return c.GetComponent<Tile>();
            }
            return null;
        }

        public Tile GetTileFromGrid(int X, int Y)
        {
            return tileGird[Y, X];
        }

        private void OnValidate()
        {

            X = (int)transform.localScale.x;
            Y = (int)transform.localScale.y;

            //Astar.position = transform.position;
            //transform.localScale = new Vector3((int)transform.localScale.x, (int)transform.localScale.y, (int)transform.localScale.z);
        }

        private void OnDrawGizmos()
        {
            Vector3 pos = transform.position;

            topCorner = pos - transform.localScale / 2;

            Gizmos.DrawWireSphere(topCorner, 0.5f);
        }
    }
}