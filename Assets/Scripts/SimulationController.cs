using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class SimulationController : MonoBehaviour {

    //Size of Map variables x and y
    private float xOfMap;
    private float zOfMap;
    private Vector3 sizeOfMap;
    private Vector3 locationOfMap = new Vector3(0,0,0);
    private Quaternion OrientationOfMap = new Quaternion();
    //Conection to Game Object J5
    public GameObject Car;
    //Size of tree 
    public float radiusTree;
    public float heightTree;
    private Vector3 sizeOfTree;
    //Holds the location of trees local position to the bot
    private Vector3[] treeLocalPositions;
    //Number of trees
    private int numberOfTrees;
    //Tree GameObject
    public GameObject Tree;
    //Connection to parent gameobject of all the trees 
    public GameObject Forest;
    //An array that holds all the tree game objects
    private GameObject[] AForest;
    //Connection to Plane GameObject that acts as Field
    public GameObject Map;
    // Holds the polor locations of the trees with respect to the bot
    private Vector2[] treePolor; 
    // Holds the location of the two closest trees in absolute coordinates
    private Vector3[] depthTree = new Vector3[2];
    // Hold location of bot's current position
    private Vector3 currentPosition = new Vector3(0, .5f, -18f);
    // Holds location of the next spot the bot needs to head towards
    private Vector3 nextSpot;
    //not used right now
    private Vector3 nextRotation;
    //Vector3 used for debuging
    private Vector3 debug;
    //not used right now
    private Vector3 targetAngles;
    //Variable that holds the speed for the bot
    private float speed = .05f;
    //bool that is true if bot is turning
    private bool inProcessOfTurning;
    //Holds wich direction bot is going (if positive bot is heading in positive Z direction) 
    private float direction;
    //bool that is true if the bot is stopped
    private bool stopped;
    //bool that is true if it is time to rotate
    private bool rotating;
    //Holds the current rotation of bot
    private Quaternion rotation;
    //use this for initiation of variables before the start
    private GameObject sharedVariables;
    private SharedVariables sharedVariablesScript;
    private float safeDistance = 1.7f;
    //UI variables
    private Text Firsttreeposition;
    private Text Secondtreeposition;
    private Text GPSposition;
    private Text IMUrotation;
    private int GPSUpdate;
    private int LidarUpdate;
    private int refresh = 20;
    void awake()
    {
        xOfMap = 4f;
        zOfMap = 4f;
        radiusTree = 0.125f;
        heightTree = 2.5f;
        numberOfTrees = 20;
    }
	void Start () {
        sharedVariables = GameObject.Find("SharedVariables");
        Firsttreeposition = GameObject.Find("First Closest Tree").GetComponent<Text>();
        Secondtreeposition = GameObject.Find("Second Closest Tree").GetComponent<Text>();
        GPSposition = GameObject.Find("Position Data").GetComponent<Text>();
        IMUrotation = GameObject.Find("Rotation Data").GetComponent<Text>();
        Firsttreeposition.text = "0";
        Secondtreeposition.text = "0";
        GPSposition.text = "0";
        IMUrotation.text = "0";
        if (sharedVariables == null)
        {
            xOfMap = 4f;
            zOfMap = 4f;
            numberOfTrees = 14;
        }
        else
        {
            sharedVariablesScript = sharedVariables.GetComponent<SharedVariables>();
            xOfMap = sharedVariablesScript.sizeOfMap;
            zOfMap = sharedVariablesScript.sizeOfMap;
            numberOfTrees = sharedVariablesScript.numOfTrees;
        }

        inProcessOfTurning = false;
        stopped = false;
        rotating = false;
        direction = 1;
        GPSUpdate = 0;
        LidarUpdate = 0;
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
        if (stopped) 
            ;
        else if (inProcessOfTurning) turn();
        else MoveCar();
    }

    //Function to Spawn Forest
    void SpawnForest()
    {
        Instantiate(Map,locationOfMap,OrientationOfMap);
        int i = 0; 
        int k = 0;
        float xscale = 5f; //used to scale seperation of trees in x direction
        float zscale = 2.5f; //used to scale seperation of tree in z direction
        int numOfRows;
        if (sharedVariables == null) numOfRows = 4;
        else numOfRows = sharedVariablesScript.numOfRows; 
        float xshift = -8f; // Gives the shift of the bunch of trees in the x direction
        float zshift = -10f; // Gives the shift of the bunch of trees in the z direction
        GameObject temp; //Temp gameobject to store created trees 
        //Loop instatiates the trees in the parent forest
        while (i < numberOfTrees)
        {
            for (float j = 0; i < numberOfTrees && j < numOfRows;j++)
            {
                Vector3 spawnPosition = new Vector3(k*xscale + xshift, 2.5f, j*zscale + zshift);
                Quaternion spawnRotation = new Quaternion();
                spawnRotation = Quaternion.identity;
                temp = Instantiate(Tree, spawnPosition, spawnRotation) as GameObject;
                AForest[i] = temp;
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
         //initilize memory space for array to size of number of trees
        treePolor = new Vector2[numberOfTrees]; 
        Vector3[] treePositions = new Vector3[numberOfTrees];
        bool turn = true;
         //loop stores the polor coordinates of every tree into treePolor[]
         for(int i = 0;i < numberOfTrees;i++)
        {   
            treePositions[i] = AForest[i].transform.position;
            treeLocalPositions[i] = treePositions[i] - Car.transform.position;  
            temp.x = (Mathf.Sqrt((treeLocalPositions[i].x*treeLocalPositions[i].x)+(treeLocalPositions[i].z*treeLocalPositions[i].z)));
            // Condition in case z is equal to zero
            if(treeLocalPositions[i].z != 0 && treeLocalPositions[i].x == 0) temp.y=(treeLocalPositions[i].x/Mathf.Abs(treeLocalPositions[i].x))*90f*direction;
            else if (treeLocalPositions[i].z == 0 && treeLocalPositions[i].x == 0) temp.y = 0;
            else temp.y = (Mathf.Atan2(treeLocalPositions[i].z, treeLocalPositions[i].x) * Mathf.Rad2Deg)*direction;
            treePolor[i] = temp;
            if (Mathf.Round(treePolor[i].y) < 178 && treePolor[i].y > 2) turn = false;
            if (direction == -1)
            {
                if (treePolor[i].y < 90 && Mathf.Round(treePolor[i].y) > 0) treePolor[i].y = treePolor[i].y + 90f;
                else if(treePolor[i].y > 90 && Mathf.Round(treePolor[i].y) < 180)treePolor[i].y = treePolor[i].y - 90f;
            }
        }

        TreeSort(treePolor,treePositions);
        if (LidarUpdate == refresh)
        {
            Firsttreeposition.text = "Mag: " + treePolor[0].x.ToString("F2") + "     " + treePolor[0].y.ToString("F2") + " degrees";
            Secondtreeposition.text = "Mag: " + treePolor[1].x.ToString("F2") + "     " + treePolor[1].y.ToString("F2") + " degrees";
            LidarUpdate = 0;
        }
        LidarUpdate++;
        //Stores the 2 closest trees absolute position into treeDirections[]
        treeDirections[0] = treePositions[0];
        treeDirections[1] = treePositions[1];
        //Returns wether to turn or not
        return turn;
    }
    //Function returns the cars curent position
    public Vector3 GPS()
    {   Vector3 Ret = new Vector3(0,0,0);
        Ret = Car.transform.position;
        if (GPSUpdate == refresh)
        {
            GPSposition.text = "x = " + Ret.x.ToString("F2") + " z = " + Ret.z.ToString("F2");
            IMUrotation.text = Car.transform.rotation.eulerAngles.y.ToString("F2") + " degrees";
            GPSUpdate = 0;
        }
        GPSUpdate++;
        return Ret;
    }
    //Sorts by the closet positive Polar Coordinates and applys that result to sort the cartesion absolute position  
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
        if (Mathf.Round(polarTrees[min].y) > 178 || (Mathf.Round(polarTrees[min].y) < 2))
            min = j;
        else if (polarTrees[min].y < 90 && polarTrees[j].y < polarTrees[min].y && polarTrees[min].x < polarTrees[j].x);
        else if (polarTrees[min].y > 90 && polarTrees[j].y > polarTrees[min].y && polarTrees[min].x < polarTrees[j].x);
        else if (polarTrees[j].x < polarTrees[min].x && polarTrees[j].y > 2 && Mathf.Round(polarTrees[j].y) < 178)
                min = j;
        }
    /* exchange a[i] and a[min] */
    tmp = polarTrees[i]; tmp3 = cartesianTrees[i];
    polarTrees[i] = polarTrees[min]; cartesianTrees[i] = cartesianTrees[min];
    polarTrees[min] = tmp; cartesianTrees[min] = tmp3;
    }
    for (i = 1; i < numberOfTrees - 1; i++)
    {
        /* find minimum element from i to numberOfTrees-1 */
        min = i; /* i-th element might be minimum */
        for (j = i + 1; j < numberOfTrees; j++)
        {
            if(polarTrees[0].y < 90 && polarTrees[min].y < 90  && polarTrees[j].y > 2 && Mathf.Round(polarTrees[j].y) < 178) min =j; 
            else if(polarTrees[0].y > 90 && polarTrees[min].y > 90  && polarTrees[j].y > 2 && Mathf.Round(polarTrees[j].y) < 178) min =j;
            else if (polarTrees[0].y < 90 && polarTrees[j].y > 90 && polarTrees[j].y > 2 && Mathf.Round(polarTrees[j].y) < 178)
            {
                if (polarTrees[j].x < polarTrees[min].x) min = j; 
            }
            else if (polarTrees[0].y > 90 && polarTrees[j].y < 90 && polarTrees[j].y > 2 && Mathf.Round(polarTrees[j].y) < 178)
            {
                if (polarTrees[j].x < polarTrees[min].x) min = j;
            }
        }
        /* exchange a[i] and a[min] */
        tmp = polarTrees[i]; tmp3 = cartesianTrees[i];
        polarTrees[i] = polarTrees[min]; cartesianTrees[i] = cartesianTrees[min];
        polarTrees[min] = tmp; cartesianTrees[min] = tmp3;
    }
    return;
}
    //Moves car incrementilly closer to next spot
    void MoveCar()
    {
        float step = speed;
        bool turn;
        //Move towards next spot if you are not at it
        if(currentPosition != nextSpot)
        {
            Car.transform.position = Vector3.MoveTowards(Car.transform.position, nextSpot, step);            
        }
        currentPosition = GPS();
        turn = lidar(depthTree);
        //If turn is true set manual next spot and change the direction
        if (turn)
        {
            inProcessOfTurning = true;
            nextSpot = currentPosition+new Vector3(5f, 0, direction*7f);
            nextRotation.y = Car.transform.rotation.eulerAngles.y + direction*180f;
            direction = -1*direction;
        }
        else nextSpot = spotDetermination(depthTree);
        stopped = checkStop(treePolor);
    }
    //Determines next spot based on closest trees
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
    //Moves Car incremently to next spot (Rotating bot work in progress)
    void turn()
    {
        float step = speed;
        float rStep = speed * 10;
        Car.transform.position = Vector3.MoveTowards(Car.transform.position, nextSpot, step);
        Car.transform.Rotate(Vector3.up,-direction*rStep);
        GPS();
        if (Car.transform.position == nextSpot && Mathf.Round(Car.transform.rotation.eulerAngles.y) == Mathf.Round(nextRotation.y))
        {
            inProcessOfTurning = false;
            rotating = true;
            lidar(depthTree);
            stopped = checkStopt(treePolor);
        }
        return;
    }
    //Checks if the bot needs to stop (That there are no more trees)
    bool checkStopt(Vector2[] treePoints)
    {
        bool ret = true;
        if ((treePoints[0].y < 2 && treePoints[1].y > 178) || (treePoints[1].y < 2 && treePoints[0].y > 178)) ret = false;
        else if ((treePoints[0].y >= 90 && treePoints[1].y <= 90) || (treePoints[0].y <= 90 && treePoints[1].y >= 90)) ret = false; 
        return ret;
    }
    //

    bool checkStop(Vector2[] treePoints)
    {
        bool ret = true;
        if((Mathf.Round(treePoints[0].y) <= 90) || (Mathf.Round(treePoints[1].y) <= 90)) ret = false;
        return ret;
    }

    void rotate()
    {

        rotating = false;
        return;
    }




}
