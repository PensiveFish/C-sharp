using UnityEngine;

public class HuntAndKillMazeAlgorithm : MazeAlgorithm
{

    //variables to trace algorithm path
    int currentRow;
    int currentColumn;

    //to see if we've been inside each cell
    bool courseComplete;

    public HuntAndKillMazeAlgorithm(MazeCell[,] mazeCells) : base(mazeCells) { }

    public override void CreateMaze()
    {
        HuntAndKill();
    }

    //Main function to generate path while all cells isn't visited
    void HuntAndKill()
    {
        mazeCells[currentRow, currentColumn].visited = true;

        while (!courseComplete) //while there exists cell which is unvisited
        {
            Kill();
            Hunt();
        }
    }

    //Function to make path by destroying walls
    void Kill()
    {
        while (RouteStillAvailable(currentRow, currentColumn))
        {
            int direction = Random.Range(1, 5); //get random direction (north,south,east or west)

            if (direction == 1 && CellIsAvailable(currentRow - 1, currentColumn))
            {
                DestroyWall(mazeCells[currentRow, currentColumn].northWall);
                DestroyWall(mazeCells[currentRow - 1, currentColumn].southWall);
                currentRow--;
            }
            else if (direction == 2 && CellIsAvailable(currentRow + 1, currentColumn))
            {
                DestroyWall(mazeCells[currentRow, currentColumn].southWall);
                DestroyWall(mazeCells[currentRow + 1, currentColumn].northWall);
                currentRow++;
            }
            else if (direction == 3 && CellIsAvailable(currentRow, currentColumn + 1))
            {
                DestroyWall(mazeCells[currentRow, currentColumn].eastWall);
                DestroyWall(mazeCells[currentRow, currentColumn + 1].westWall);
                currentColumn++;
            }
            else if (direction == 4 && CellIsAvailable(currentRow, currentColumn - 1))
            {
                DestroyWall(mazeCells[currentRow, currentColumn].westWall);
                DestroyWall(mazeCells[currentRow, currentColumn - 1].eastWall);
                currentColumn--;
            }

            mazeCells[currentRow, currentColumn].visited = true;
        }
    }

    //Function which called after Kill function to see if we still have unvisited cells
    void Hunt()
    {
        courseComplete = true; // change bool to completed

        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                if (!mazeCells[r, c].visited && CellHasAnAdjacentVisitedCell(r, c))
                {
                    courseComplete = false; // change back to false if we found unvisited cell
                    currentRow = r;
                    currentColumn = c;
                    DestroyAdjacentWall(currentRow, currentColumn);
                    mazeCells[currentRow, currentColumn].visited = true;
                    return;
                }
            }
        }
    }


    //Function to see if we can go in at least 1 of 4 direction and return true if it is so
    bool RouteStillAvailable(int row, int column)
    {
        bool ThereIsAvailableRoute = false;

        //here we are simply watch in all 4 direction to see if there is cell which is unvisited
        //when we found 1 cell then we no longer watching for other directions
        if (row > 0 && !mazeCells[row - 1, column].visited)
            ThereIsAvailableRoute = true;
        else if (row < mazeRows - 1 && !mazeCells[row + 1, column].visited)
            ThereIsAvailableRoute = true;
        else if (column > 0 && !mazeCells[row, column - 1].visited)
            ThereIsAvailableRoute = true;
        else if (column < mazeColumns - 1 && !mazeCells[row, column + 1].visited)
            ThereIsAvailableRoute = true;

        return ThereIsAvailableRoute;
    }

    //Function to see if cell on our next path step is exist and we are not going outside of our maze or into already visited cell
    bool CellIsAvailable(int row, int column)
    {
        if (row >= 0 && row < mazeRows && column >= 0 && column < mazeColumns && !mazeCells[row, column].visited)
            return true;
        else
            return false;
    }

    //Function for destroying wall if it exists
    void DestroyWall(GameObject wall)
    {
        if (wall != null)
            Object.Destroy(wall);
    }

    //Function to see if near cell there are any djacent visited cell
    bool CellHasAnAdjacentVisitedCell(int row, int column)
    {

        bool ThereIsVisitedCell = false;

        if (row > 0 && mazeCells[row - 1, column].visited) // watch 1 row up if we're on row 1 or greater
            ThereIsVisitedCell = true;
        else if (row < (mazeRows - 2) && mazeCells[row + 1, column].visited) // watch one row down if we're the second-to-last row (or less)
            ThereIsVisitedCell = true;
        else if (column > 0 && mazeCells[row, column - 1].visited) // watch one row left if we're column 1 or greater
            ThereIsVisitedCell = true;
        else if (column < (mazeColumns - 2) && mazeCells[row, column + 1].visited) // watch one row right (east) if we're the second-to-last column (or less)
            ThereIsVisitedCell = true;

        return ThereIsVisitedCell;
    }

    //Function to destroy adjacent wall
    void DestroyAdjacentWall(int row, int column)
    {
        bool wallDestroyed = false;

        while (!wallDestroyed)
        {
            int direction = Random.Range(1, 5);

            if (direction == 1 && row > 0 && mazeCells[row - 1, column].visited)
            {
                DestroyWall(mazeCells[row, column].northWall);
                DestroyWall(mazeCells[row - 1, column].southWall);
                wallDestroyed = true;
            }
            else if (direction == 2 && row < (mazeRows - 2) && mazeCells[row + 1, column].visited)
            {
                DestroyWall(mazeCells[row, column].southWall);
                DestroyWall(mazeCells[row + 1, column].northWall);
                wallDestroyed = true;
            }
            else if (direction == 3 && column > 0 && mazeCells[row, column - 1].visited)
            {
                DestroyWall(mazeCells[row, column].westWall);
                DestroyWall(mazeCells[row, column - 1].eastWall);
                wallDestroyed = true;
            }
            else if (direction == 4 && column < (mazeColumns - 2) && mazeCells[row, column + 1].visited)
            {
                DestroyWall(mazeCells[row, column].eastWall);
                DestroyWall(mazeCells[row, column + 1].westWall);
                wallDestroyed = true;
            }
        }

    }

}