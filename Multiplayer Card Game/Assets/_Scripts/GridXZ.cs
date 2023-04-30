using System;
using UnityEngine;

public class GridXZ<TGridObject>
{

    private int _maxWidth;
    private int _maxHeight;
    private float _cellsize;
    private TGridObject[,] _gridArray;
    TextMesh[,] _debugTextArray;
    private Transform _tGrid;


    private readonly Func<GridXZ<TGridObject>, int, int, TGridObject> func;

    public GridXZ(int gridwidth, int gridheight, float cellsize, Transform tGrid, Func<GridXZ<TGridObject>, int, int, TGridObject> func)
    {
        this._maxWidth = gridwidth;
        this._maxHeight = gridheight;
        this._cellsize = cellsize;
        this.func = func;
        _tGrid = tGrid;
        _debugTextArray = new TextMesh[_maxWidth, _maxHeight];

        _gridArray = new TGridObject[_maxWidth, _maxHeight]; //Creates a grid with a certain height and width (defined in editor)

        for (int x = 0; x < _gridArray.GetLength(0); x++) //for loop to generate all the Gridcells in the grid
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                _gridArray[x, z] = CreateGridObject(this, x, z);
            }
        }


        for (int x = 0; x < _gridArray.GetLength(0); x++) //for loop that draws the grid for the client to see
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                _debugTextArray[x, z] = UtilClass.CreateWorldText(null, GameObject.Find("Values").transform, GetWorldPositon(x, z) + new Vector2(5, 5), 15, Color.red, TextAnchor.LowerLeft);
                HorizontalLine(z);
            }
            VerticalLine(x);
        }
        VerticalLine(_maxWidth);
        HorizontalLine(_maxHeight);

    }

    public GridXZ<TGridObject> GetXZ(Vector3 mouse, out int x, out int z)
    {
        x = (int)Math.Floor(mouse.x / _cellsize);
        z = (int)Math.Floor(mouse.y / _cellsize);
        return null;
    }

    public Vector2 GetWorldPositon(int x, int y) => new Vector2(x, y) * _cellsize; //converts grid space into world positions
    private TGridObject CreateGridObject(GridXZ<TGridObject> gridXZ, int x, int z) => func(gridXZ, x, z);
    public TGridObject GetGridObject(int x, int z) { //Returns the grid object in this case GridCell based on the x, y coords given
        
        try { 
            return _gridArray[x, z]; 
        }
        catch (Exception) { return default; }
        
    }
        
    public TGridObject[,] GetGridArray() => _gridArray;

    public TextMesh[,] GetDebugText() => _debugTextArray;


    void VerticalLine(int x) //Draws the vertical lines for the grid
    {
        GameObject verticalLine = new GameObject("VerticalLine");
        verticalLine.transform.SetParent(_tGrid.GetChild(1));
        LineRenderer verticalLineRenderer = verticalLine.AddComponent<LineRenderer>();
        verticalLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        verticalLineRenderer.positionCount = 2;
        verticalLineRenderer.SetPosition(0, GetWorldPositon(x, 0));
        verticalLineRenderer.SetPosition(1, GetWorldPositon(x, _maxHeight));
        verticalLineRenderer.startColor = Color.white;
        verticalLineRenderer.endColor = Color.white;
        verticalLineRenderer.startWidth = 0.1f;
        verticalLineRenderer.endWidth = 0.1f;
    }

    void HorizontalLine(int y) //Draws the horizontal lines for the grid
    {
        GameObject horizontalLine = new GameObject("HorizontalLine");
        horizontalLine.transform.SetParent(_tGrid.GetChild(1));
        LineRenderer horizontalLineRenderer = horizontalLine.AddComponent<LineRenderer>();
        horizontalLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        horizontalLineRenderer.positionCount = 2;
        horizontalLineRenderer.SetPosition(0, GetWorldPositon(0, y));
        horizontalLineRenderer.SetPosition(1, GetWorldPositon(_maxWidth, y));
        horizontalLineRenderer.startColor = Color.white;
        horizontalLineRenderer.endColor = Color.white;
        horizontalLineRenderer.startWidth = 0.1f;
        horizontalLineRenderer.endWidth = 0.1f;
    }
}
