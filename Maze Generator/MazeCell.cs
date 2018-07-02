using UnityEngine;

public class MazeCell
{
    public bool visited; //false for hunt and kill alghoritm
    public GameObject northWall, eastWall, southWall, westWall, floor; //each cell have it's 4 walls and floor, you can add here ceiling as well
}
