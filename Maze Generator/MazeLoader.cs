using UnityEngine;

public class MazeLoader : MonoBehaviour {

    public int Rows, Columns; //number of row and columns of our maze
    public GameObject wall; //prefab for walls
    public GameObject floor; //prefab for floor
    public float size = 2f;
    public Transform startTransform; //start position for our maze

    MazeCell[,] Cells;
    Vector3 startPos;

	void Start ()
    	{
        if(startTransform == null || wall == null || floor == null)
        {
            Debug.LogError("Empty variable startTransform/wall/floor on " + gameObject.name.ToUpper());
            return;
        }

        startPos = startTransform.position; //take position of our start transform

        InitializeMaze(); //create general field with cells

        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(Cells); //initialize
        ma.CreateMaze(); //create our path

	}

    //Function to create general field of maze without any path inside it
    void InitializeMaze()
    {
        Cells = new MazeCell[Rows, Columns];

        for(int r = 0; r < Rows; r++) //cycle for rows
        {
            for(int c = 0; c < Columns; c++) //in each row we make cycle for all columns
            {
                Cells[r, c] = new MazeCell();

                Cells[r, c].floor = Instantiate(floor, startPos + new Vector3(r * size, -(size / 2f), c * size), Quaternion.identity) as GameObject; //instantiate floor
                Cells[r, c].floor.name = "Floor " + r + "," + c; //change name for inspector

                if(c == 0)
                {
                    Cells[r, c].westWall = Instantiate(wall, startPos + new Vector3(r * size, 0, (c * size) - (size / 2)), Quaternion.Euler(0, -90, 90)) as GameObject; //instantiate west wall with proper angle
                    Cells[r, c].westWall.name = "West Wall " + r + "," + c; //change name for inspector
                }

                Cells[r, c].eastWall = Instantiate(wall, startPos + new Vector3(r * size, 0, (c * size) + (size / 2f)), Quaternion.Euler(0, -90, 90)) as GameObject; //instantiate east wall with proper angle
                Cells[r, c].eastWall.name = "East Wall " + r + "," + c; //change name for inspector

                if (r == 0)
                {
                    Cells[r, c].northWall = Instantiate(wall, startPos + new Vector3((r * size) - (size / 2f), 0, c * size), Quaternion.Euler(0, 0, 90)) as GameObject; //instantiate north wall with proper angle
                    Cells[r, c].northWall.name = "North Wall " + r + "," + c; //change name for inspector
                }

                Cells[r, c].southWall = Instantiate(wall, startPos + new Vector3((r * size) + (size / 2f), 0, c * size), Quaternion.Euler(-90, 0, 90)) as GameObject; //instantiate south wall with proper angle
                Cells[r, c].southWall.name = "South Wall " + r + "," + c; //change name for inspector
            }
        }
    }
}
