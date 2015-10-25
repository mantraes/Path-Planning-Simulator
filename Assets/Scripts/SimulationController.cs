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
        object[] depthTree = new object[4];
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

    void lidar(object[] treeDirections)
    {

        ArrayList magDistancesFrom = new ArrayList();
        ArrayList angFrom = new ArrayList();
        //float[] mag = new float[numberOfTrees];
        //float[] ang = new float[numberOfTrees];
        for(int i = 0;i < numberOfTrees;i++)
        {
            treeLocalPositions[i] = AForest[i].transform.localPosition;
            //mag[i] = Mathf.Sqrt((treeLocalPositions[i].x * treeLocalPositions[i].x) + (treeLocalPositions[i].z * treeLocalPositions[i].z));
            //ang[i] = Mathf.Atan2(treeLocalPositions[i].x, treeLocalPositions[i].z) * Mathf.Rad2Deg;
            magDistancesFrom.Add(Mathf.Sqrt((treeLocalPositions[i].x * treeLocalPositions[i].x) + (treeLocalPositions[i].z * treeLocalPositions[i].z)));
            angFrom.Add(Mathf.Atan2(treeLocalPositions[i].x, treeLocalPositions[i].z) * Mathf.Rad2Deg);
        }
        magDistancesFrom.Sort();
        angFrom.Sort();
        treeDirections[0] = magDistancesFrom[0];
        treeDirections[1] = angFrom[0];
        treeDirections[2] = magDistancesFrom[1];
        treeDirections[3] = angFrom[1];
        return;
    }
    //Works
    Vector3 GPS()
    {   Vector3 Ret = new Vector3(0,0,0);
        Ret = Car.transform.position;
        return Ret;
    }
   
  

}
