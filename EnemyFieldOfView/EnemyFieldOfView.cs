using UnityEngine;
using System.Collections.Generic;

//This script do not have enemy behaviour and it's only searching for 1 collider
//To change it - read commented fields and adjust/delete/rewrite some pieces of code
public class EnemyFieldOfView : MonoBehaviour
{
    [Header("FieldOfView variables")]
    public float viewRadius = 15f; //how far field of view is
    [Range(0, 360)] //clamp from 0 to 360 since it can't be more than circle
    public float viewAngle = 100f; //angle of view
    public LayerMask detectMask; //layer for objects on which we would like to react
    public LayerMask obstacleMask; //layer for obstacle which will block our view
    public Collider[] targetNear = new Collider[1]; //array for near colliders with size of 1

    [Space(10)]
    [Header("FoV visualisation")]
    public float meshResolution = 1f; //how much rays from 1 viewAngle
    public int edgeResolveIterations = 2; //how many times we are smoothing our visualisation
    public float edgeDistanceThreshold = 2f; //the minimum distance to begin iteration of smoothing if both near placed rays are hitting something
    public MeshFilter viewMeshFilter;

    Mesh viewMesh;
    Color32 basicColor; //save basic color of FoV material
    MeshRenderer meshRend; //for optimization to not access in future through GetComponent

    private void Start()
    {
        //start initialization
        viewMesh = new Mesh();
        viewMesh.name = "FoV Mesh";
        viewMeshFilter.mesh = viewMesh;

        meshRend = viewMeshFilter.GetComponent<MeshRenderer>(); //optimization
        basicColor = meshRend.material.color; //save color
    }

    private void Update()
    {
        //OverlapSphereNonAlloc return int therefore we are checking if it's more than 0
        //you can use OverlapSphere, but OverlapSphereNonAlloc is more perfomance, but your array (targetNear) should have some certain size
        if ((Physics.OverlapSphereNonAlloc(transform.position, viewRadius, targetNear, detectMask)) > 0)
        {
            //I'm only looking for player layer therefore I can say that if OverlapSphereNonAlloc return not 0
            //then it certainly will be player, so I can assign it directly
            //if you want to check for multiple object with different priority then you can give array (targetNear) bigger size
            //and then iterate through each element and compare their tag or distance, or whatever you want and after select behaviour
            Transform player = targetNear[0].transform;
            Vector3 dirToTarget = (player.position - transform.position).normalized; //make pointer vector
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) //check if player in sight of view
            {
                float dstToTarget = Vector3.Distance(transform.position, player.position); //calculate distance to player

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) //check if in our way to player we have an obstacle
                {
                    //we see player DO SOMETHING

                    //just for test purpose we change color of our mesh to red if it isn't already red
                    if (meshRend.material.color != Color.red)
                        meshRend.material.color = Color.red;
                }
                else
                {
                    //we don't see player DO SOMETHING

                    //just for test purpose we change color of our mesh back to it basic color if it isn't already have it
                    if (meshRend.material.color != basicColor)
                        meshRend.material.color = basicColor;
                }
            }
            else
            {
                //player isn't in our view angle so we can't see him DO SOMETHING

                //just for test purpose we change color of our mesh back to it basic color if it isn't already have it
                if (meshRend.material.color != basicColor)
                    meshRend.material.color = basicColor;
            }
        }
        else
        {
            //player isn't in our view radius so we can't see him DO SOMETHING

            //just for test purpose we change color of our mesh back to it basic color if it isn't already have it
            if (meshRend.material.color != basicColor)
                meshRend.material.color = basicColor;
        }
    }

    //We are doing it in LateUpdate, because we want to update visualisation FoV only after our movements
    //which you'll probably do in Update
    private void LateUpdate()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution); //count number of rays
        float stepAngleSize = viewAngle / stepCount; //count angle degree for 1 step

        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i; //calculate current angle
            ViewCastInfo newViewCast = ViewCast(angle); //and use it to cast ray and find future vert position

            if (i > 0)
            {
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDistanceThreshold)) //here we begin to smooth our FoV visualisation
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                        viewPoints.Add(edge.pointA);
                    if (edge.pointB != Vector3.zero)
                        viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]); //transform to local space

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    //Function to transform angle value into Vector3 pointer
    public Vector3 DirFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    //Function to calculate future vert position and see if it have obstacle on its way
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle); //transform angle into Vector3
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask)) //if we hit obstacle
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle); //we return hitted position
        else //in another situation
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle); //we return position of direction multiplied by viewRadius
    }

    //Function for smoothing our visualisation of FoV
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (newViewCast.hit == minViewCast.hit && !(Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDistanceThreshold))
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        //make constructor
        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        //make constructor
        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}
