using UnityEngine;
using System.Collections;
public class SimulationController : MonoBehaviour {

    //Size of Map meters
    public float xOfMap = 20f;
    public float zOfMap = 20f;
    private Vector3 sizeOfMap;
    private Vector3 locationOfMap = new Vector3(0,0,0);
    private Quaternion OrientationOfMap = new Quaternion();
    //Size of tree meters
    public float radiusTree = 0.125f;
    public float heightTree = 2.5f;
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
	void Start () {
        sizeOfTree = new Vector3(2*radiusTree, heightTree, 2*radiusTree);
        sizeOfMap = new Vector3(xOfMap,0,zOfMap);
        OrientationOfMap = Quaternion.identity;
        Tree.transform.localScale = sizeOfTree;
        Map.transform.localScale = sizeOfMap;
        AForest = new GameObject[numberOfTrees];
        treeLocalPositions = new Vector3[numberOfTrees]; 
        SpawnForest();
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
    
    
    
    }

    //Function to Spawn Forest
    void SpawnForest()
    {
        Instantiate(Map,locationOfMap,OrientationOfMap);
        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-sizeOfMap.x, sizeOfMap.x), sizeOfMap.y, Random.Range(-sizeOfMap.z, sizeOfMap.z));
            Quaternion spawnRotation = new Quaternion();
            spawnRotation = Quaternion.identity;
            GO = Instantiate(Tree, spawnPosition, spawnRotation) as GameObject;
            AForest[i] = GO;
            AForest[i].transform.parent = Forest.transform; 
        }


    }

    void lidar(int[] treeDirections,int num)
    {

        ArrayList magDistancesFrom = new ArrayList(numberOfTrees);
        ArrayList angFrom = new ArrayList(numberOfTrees);
        for(int i = 0;i < numberOfTrees;i++)
        {
            treeLocalPositions[i] = AForest[i].transform.localPosition;
            magDistancesFrom[i] = Mathf.Sqrt(treeLocalPositions[i].x * treeLocalPositions[i].x + treeLocalPositions[i].z * treeLocalPositions[i].z);
            angFrom[i] = Mathf.Atan2(treeLocalPositions[i].x, treeLocalPositions[i].z) * Mathf.Rad2Deg;
        }

        magDistancesFrom.Sort();
        angFrom.Sort();
    }
    void GPS()
    {
        int x;

    }
   
    





}
