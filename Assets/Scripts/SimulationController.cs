using UnityEngine;
using System.Collections;
public class SimulationController : MonoBehaviour {

    //Size of Map meters
    public float xOfMap;
    public float zOfMap;
    private Vector3 sizeOfMap;
    private Vector3 locationOfMap = new Vector3(0,0,0);
    private Quaternion OrientationOfMap = new Quaternion();
    public GameObject Car;
    //Size of tree meters
    public float radiusTree;
    public float heightTree;
    private Vector3 sizeOfTree;
    private Vector3[] treeLocalPositions;
    //Number of trees
    private int numberOfTrees = 20;
    //Tree GameObject
    public GameObject Tree;
    private GameObject GO;
    public GameObject Forest;
    private GameObject[] AForest;
    //Plane GameObject
    public GameObject Map;
    // Use this for initialization
    private Vector3[] depthTree = new Vector3[2];
    private Vector3 currentPosition = new Vector3(0, .5f, -18f);
    private Vector3 nextSpot;
    private Vector3 debug;
    private float speed = .1f;
    bool inProcessOfTurning;
    void awake()
    {
        xOfMap = 4f;
        zOfMap = 4f;
        radiusTree = 0.125f;
        heightTree = 2.5f;
        numberOfTrees = 20;

    }
	void Start () {
        inProcessOfTurning = false;
        sizeOfTree = new Vector3(2*radiusTree, heightTree, 2*radiusTree);
        sizeOfMap = new Vector3(xOfMap,1,zOfMap);
        OrientationOfMap = Quaternion.identity;
        Tree.transform.localScale = sizeOfTree;
        Map.transform.localScale = sizeOfMap;
        AForest = new GameObject[numberOfTrees];
        treeLocalPositions = new Vector3[numberOfTrees];
        nextSpot = currentPosition;
        SpawnForest();

	}
	
	// Update is called once per frame
	void Update () {

	}

    //Update is called over fixed interval
    void FixedUpdate () {
        MoveCar();
    }

    //Function to Spawn Forest
    void SpawnForest()
    {
        Instantiate(Map,locationOfMap,OrientationOfMap);
        int i = 0; 
        float k = 0;
        float xscale = 5f;
        float zscale = 2.5f;
        int treesInRow = 4;
        float xshift = -8f;
        float zshift = -10f;
        while (i < numberOfTrees)
        {
            for (float j = 0; i < numberOfTrees && j < treesInRow;j++)
            {
                Vector3 spawnPosition = new Vector3(k*xscale + xshift, 2.5f, j*zscale + zshift);
                Quaternion spawnRotation = new Quaternion();
                spawnRotation = Quaternion.identity;
                GO = Instantiate(Tree, spawnPosition, spawnRotation) as GameObject;
                AForest[i] = GO;
                AForest[i].transform.parent = Forest.transform;
                i++;
            }
            k++;
        }


    }

    //Returns wheather you need to turn or not 
     public bool lidar(Vector3[] treeDirections)
    {
        Vector2 temp = new Vector2(0,0);
        Vector2[] treePolor = new Vector2[numberOfTrees]; 
        bool turn = true;
        Vector3[] treePositions = new Vector3[numberOfTrees];
         for(int i = 0;i < numberOfTrees;i++)
        {   
            treePositions[i] = AForest[i].transform.position;
            treeLocalPositions[i] = treePositions[i] - Car.transform.position;  
            temp.x = (Mathf.Sqrt((treeLocalPositions[i].x*treeLocalPositions[i].x)+(treeLocalPositions[i].z*treeLocalPositions[i].z)));
            // Condition in case z is equal to zero
            if(treeLocalPositions[i].z == 0 && treeLocalPositions[i].x != 0) temp.y=(treeLocalPositions[i].x/Mathf.Abs(treeLocalPositions[i].x))*90f;
            else if (treeLocalPositions[i].z == 0 && treeLocalPositions[i].z == 0) temp.y = 0;
            else temp.y = (Mathf.Atan2(treeLocalPositions[i].z, treeLocalPositions[i].x) * Mathf.Rad2Deg);
            treePolor[i] = temp;
            if (treePolor[i].y > 0) turn = false;
        }

        TreeSort(treePolor,treePositions);
        treeDirections[0] = treePositions[0];
        treeDirections[1] = treePositions[1];
        return turn;
    }
    //Works
    public Vector3 GPS()
    {   Vector3 Ret = new Vector3(0,0,0);
        Ret = Car.transform.position;
        return Ret;
    }

    void TreeSort(Vector2[] polarTrees,Vector3[] cartesianTrees){
    /* polarTrees is an n-element array of  Vector2's*/
    int i, j, min;
    Vector2 tmp;
    Vector3 tmp3;
    /* for i from 0 to n-2...*/
    for (i=0; i<numberOfTrees-1; i++) {
    /* find minimum element from i to numberOfTrees-1 */
    min = i; /* i-th element might be minimum */
    for (j=i+1; j< numberOfTrees; j++) {
    if (polarTrees[j].x < polarTrees[min].x && polarTrees[j].y > 0) min = j;
    }
    /* exchange a[i] and a[min] */
    tmp = polarTrees[i]; tmp3 = cartesianTrees[i];
    polarTrees[i] = polarTrees[min]; cartesianTrees[i] = cartesianTrees[min];
    polarTrees[min] = tmp; cartesianTrees[min] = tmp3;
    }
    return;
}

    void MoveCar()
    {
        float step = speed;
        bool turn;
        if(currentPosition != nextSpot)
        {
            Car.transform.position = Vector3.MoveTowards(Car.transform.position, nextSpot, step);            
        }
        currentPosition = GPS();
        turn = lidar(depthTree);
        //if (turn) inProcessOfTurning = true;
        nextSpot = spotDetermination(depthTree);
    }

    Vector3 spotDetermination(Vector3[] treesPoints)
    {
        Vector3 ret;
        if (treesPoints[0].x == treesPoints[1].x && treesPoints[0].z == treesPoints[1].z)
            ret = Car. transform.position;
        else
        {
            ret.x = (treesPoints[0].x + treesPoints[1].x)/2;
            ret.y = Car.transform.position.y;
            ret.z = (treesPoints[0].z + treesPoints[1].z)/2;
        }
        return ret;
    }
    void turn()
    {
        float step = speed;
        Car.transform.position = Vector3.MoveTowards(Car.transform.position, nextSpot, step);
    }
 
}
