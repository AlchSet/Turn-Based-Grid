using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TurnBasedGrid
{
    [DefaultExecutionOrder(100)]
    public class GameManager : MonoBehaviour
    {

        public Unit selectedUnit;
        public Tile selectedTile;


        public List<Unit> unitList = new List<Unit>();


        public GameGrid grid;

        public LayerMask unitLayer;

        public UnitEvent OnSelectUnit;
        public UnitEvent OnDeSelectUnit;

        public VectorEvent OnTileSelect;
        public VectorEvent OnUnitSelect;
        public VectorEvent OnUnitMoved;


        delegate void Process();

        Process _process;
        bool isProcessing;

        Seeker seeker;

        public Pathfinder pathfinder;





        [System.Serializable]
        public class Pathfinder
        {
            public Path path;

            public Unit unit;
            public Tile target;

            public bool hasPath;
            public bool endPath;
            public int currentWaypoint;

            public const float unitSpeed = 5;

            public void SetDestination(Seeker seeker, Unit u, Tile t)
            {
                seeker.StartPath(u.transform.position, t.transform.position, OnPathComplete);
                unit = u;
                target = t;
                endPath = false;
            }

            public void OnPathComplete(Path p)
            {
                Debug.Log("PATH FOUND");
                path = p;
                endPath = false;
                hasPath = true;
                currentWaypoint = 1;


            }

            public void MoveUnit()
            {



                if (path == null) return;
                //Debug.Log("MOVE UNIT");
                if (currentWaypoint > path.vectorPath.Count) return;

                if (currentWaypoint == path.vectorPath.Count)
                {
                    //Debug.Log("END PATH");
                    endPath = true;
                    currentWaypoint++;
                    return;
                }

                unit.transform.position = Vector3.MoveTowards(unit.transform.position, path.vectorPath[currentWaypoint], Time.deltaTime * unitSpeed);

                if ((unit.transform.position - path.vectorPath[currentWaypoint]).sqrMagnitude <= 0)
                {
                    currentWaypoint++;
                }

            }
        }


        // Start is called before the first frame update
        void Start()
        {
            grid = GetComponentInChildren<GameGrid>();

            unitList.AddRange(GameObject.FindObjectsOfType<Unit>());

            seeker = GetComponent<Seeker>();

            foreach (Unit u in unitList)
            {
                UpdateUnitGridPosition(u);
            }

        }

        // Update is called once per frame
        void Update()
        {
            //Process movement etc
            if (isProcessing)
            {
                _process();
                return;
            }


            ProcessInput();

            if (selectedUnit)
            {
                SelectTile();
            }

        }



        void ProcessInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedUnit)
                {
                    if (selectedTile == null)
                        return;
                    StartMoveUnitProcess(selectedUnit, selectedTile);


                }
                else
                {
                    SelectUnitFromMouse();
                }

            }

            if (Input.GetMouseButtonDown(1))
            {
                DeselectUnit();
            }

        }

        public void SelectUnitFromMouse()
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, unitLayer);

            if (hit.collider != null)
            {
                Debug.Log("Select Unit");
                selectedUnit = hit.collider.GetComponent<Unit>();
                //SelectedTile = grid.GetTileFromGrid(SelectedUnit.gridPosition.x, SelectedUnit.gridPosition.y);
                OnSelectUnit?.Invoke(selectedUnit);
                OnUnitSelect?.Invoke(selectedUnit.transform.position);
                // raycast hit this gameobject
            }
        }

        public void DeselectUnit()
        {
            OnDeSelectUnit?.Invoke(selectedUnit);
            selectedUnit = null;
            selectedTile = null;
            Debug.Log("DeSelect");
        }

        public void SelectTile()
        {
            //Scan for Tile
            Tile t = grid.GetTileFromMouse();

            if (t == null)
            {
                return;
            }

            //Debug.Log("On " + t.name);

            if (!t.isOccupied && t.isWalkable)
            {
                selectedTile = t;

                OnTileSelect?.Invoke(t.transform.position);
            }
        }


        public void StartMoveUnitProcess(Unit unit, Tile target)
        {
            Debug.Log("Begin Move");

            grid.GetTileFromGrid(unit.gridPosition.x, unit.gridPosition.y).isOccupied = false;


            pathfinder = new Pathfinder();
            pathfinder.SetDestination(seeker, unit, target);
            _process = delegate { _MoveUnit(selectedUnit, selectedTile); };
            isProcessing = true;
        }


        void _MoveUnit(Unit unit, Tile target)
        {
            //Debug.Log(selectedUnit.unitName + " move to " + selectedTile.name);

            pathfinder.MoveUnit();

            if (pathfinder.endPath)
            {
                UpdateUnitGridPosition(unit);
                selectedTile = null;
                isProcessing = false;
                OnUnitMoved?.Invoke(unit.transform.position);
            }

            //unit.transform.position = Vector3.MoveTowards(unit.transform.position, target.transform.position, Time.deltaTime);

            //float distance = (unit.transform.position - target.transform.position).sqrMagnitude;

            //if(distance<=0)
            //{
            //    Debug.Log("End Move");
            //    UpdateUnitGridPosition(unit);
            //    isProcessing = false;
            //}


        }

        void UpdateUnitGridPosition(Unit u)
        {
            Tile t = grid.GetTileFromPoint(u.transform.position);

            if (t == null)
            {
                Debug.LogError("Unit not on the grid", u);
                return;
            }

            u.UpdateUnit(t.X, t.Y);
            //Snap unit to position
            u.transform.position = t.transform.position;
            t.isOccupied = true;
        }

        [System.Serializable]
        public class UnitEvent : UnityEvent<Unit>
        {

        }
        [System.Serializable]
        public class VectorEvent : UnityEvent<Vector3>
        {

        }


    }
}