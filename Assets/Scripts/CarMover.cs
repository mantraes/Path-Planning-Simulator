using UnityEngine;
using System.Collections;


public class CarMover : MonoBehaviour {

        public SimulationController simulationController;
        private Vector3[] depthTree = new Vector3[2];
        private Vector3 currentPosition = new Vector3(0,.5f,-18f);
        private Vector3 nextSpot;
        private float speed = .0025f;
	// Use this for initialization
	void Start () {
        nextSpot = currentPosition;
	}
	
	// Update is called once per frame
	void Update () {
    
	}

    void FixedUpdate(){
    }

    Vector3 spotDetermination(Vector3[] treesPoints){
        Vector3 ret;
        if (treesPoints[0].x == treesPoints[1].x && treesPoints[0].z == treesPoints[1].z)
            ret = transform.position;
        else
        {
            ret.x = treesPoints[0].x + treesPoints[1].x / 2;
            ret.y = transform.position.y;
            ret.z = treesPoints[0].z + treesPoints[1].z / 2;
        }
        return ret;

    }

    public void MoveCar() {
        float step = speed;
        bool stop;
        while (currentPosition != nextSpot)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextSpot, step);
            currentPosition = simulationController.GPS();
        }
        stop = simulationController.lidar(depthTree);
        while (stop) ;
        nextSpot = spotDetermination(depthTree);
    }

}
