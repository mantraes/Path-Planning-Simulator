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
    public int numberOfTrees = 20;
    //Tree GameObject
    public GameObject Tree;
    private GameObject GO;
    public GameObject Forest;
    private GameObject[] AForest;
    //Plane GameObject
    public GameObject Map;
    //Restart Option
    private bool restart = false;
	// Use this for initialization

    void awake()
    {
        xOfMap = 4f;
        zOfMap = 4f;
        radiusTree = 0.125f;
        heightTree = 2.5f;
        numberOfTrees = 20;
    }
	void Start () {
        sizeOfTree = new Vector3(2*radiusTree, heightTree, 2*radiusTree);
        sizeOfMap = new Vector3(xOfMap,1,zOfMap);
        OrientationOfMap = Quaternion.identity;
        Tree.transform.localScale = sizeOfTree;
        Map.transform.localScale = sizeOfMap;
        AForest = new GameObject[numberOfTrees];
        treeLocalPositions = new Vector3[numberOfTrees];
        SpawnForest();
        object[] depthTree = new object[4];
        lidar(depthTree);
        restart = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (restart == true && Input.GetKeyDown(KeyCode.R))
        { 
            Application.LoadLevel(Application.loadedLevel);
        }
	}

    //Update is called over fixed interval
    void FixedUpdate () {
        GPS();
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
                AForest[i].transform.parent = Car.transform;
                i++;
            }
            k++;
        }


    }

    //Returns wheather you need to turn or not 
    bool lidar(object[] treeDirections)
    {
        Vector2 temp = new Vector2(0,0);
        Vector2[] treePolor = new Vector2[numberOfTrees]; 
        bool turn = true;
        for(int i = 0;i < numberOfTrees;i++)
        {
            treeLocalPositions[i] = AForest[i].transform.localPosition;
            temp.x = (Mathf.Sqrt((treeLocalPositions[i].x * treeLocalPositions[i].x) + (treeLocalPositions[i].z * treeLocalPositions[i].z)));
            // Condition in case z is equal to zero
            if(treeLocalPositions[i].z == 0 && treeLocalPositions[i].x != 0) temp.y = (treeLocalPositions[i].x/ Mathf.Abs(treeLocalPositions[i].x))* 90f;
            else if (treeLocalPositions[i].z == 0 && treeLocalPositions[i].z == 0) temp.y = 0;
            else temp.y = (Mathf.Atan2(treeLocalPositions[i].x, treeLocalPositions[i].z) * Mathf.Rad2Deg);
            if (treeLocalPositions[i].x < 0 && treeLocalPositions[i].z > 0) temp.y = temp.y + 180f;
            else if (treeLocalPositions[i].x < 0 && treeLocalPositions[i].z < 0) temp.y = temp.y - 180f; 
            treePolor[i] = temp;
            if (treePolor[i].y > 0) turn = false;
        }

        TreeSort(treePolor);
        treeDirections[0] = treePolor[0].x;
        treeDirections[1] = treePolor[0].y;
        treeDirections[2] = treePolor[1].x;
        treeDirections[3] = treePolor[1].y;
        return turn;
    }
    //Works
    Vector3 GPS()
    {   Vector3 Ret = new Vector3(0,0,0);
        Ret = Car.transform.position;
        return Ret;
    }

    void TreeSort(Vector2[] polarTrees){
    /* polarTrees is an n-element array of  Vector2's*/
    int i, j, min;
    Vector2 tmp;
    /* for i from 0 to n-2...*/
    for (i=0; i<numberOfTrees-1; i++) {
    /* find minimum element from i to numberOfTrees-1 */
    min = i; /* i-th element might be minimum */
    for (j=i+1; j< numberOfTrees; j++) {
    if (polarTrees[j].x < polarTrees[min].x && polarTrees[j].y > 0) min = j;
    }
    /* exchange a[i] and a[min] */
    tmp = polarTrees[i]; 
    polarTrees[i] = polarTrees[min];
    polarTrees[min]= tmp;
    }
    return;
}

}
