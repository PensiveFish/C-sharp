<img src="https://user-images.githubusercontent.com/40713378/42179681-86eb153c-7e3d-11e8-8d47-bd9d4211835d.png" width="250" height="250" /><img src="https://user-images.githubusercontent.com/40713378/42179683-88cf251e-7e3d-11e8-9325-2d96d9139815.png" width="250" height="250" /><img src="https://user-images.githubusercontent.com/40713378/42179686-8a063206-7e3d-11e8-9387-a7cfc0d8dca0.png" width="250" height="250" />

This maze generator is using hunt and kill algorithm.

Function InitializeMaze() inside MazeLoader.cs was made for wall and floor prefab with size of Vector3(2, 0.1, 2) where 0.1 can be adjusted, but 2 is related with scope variable size.
Therefore you should adjust the size in order of good work with different wall and floor prefabs.

Put MazeLoader.cs on any object inside your scene.
