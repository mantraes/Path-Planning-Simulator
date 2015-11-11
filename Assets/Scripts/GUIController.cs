using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIController : MonoBehaviour {

	// Use this for initialization

    public Slider NOFTslider;
    public Text NOFTvalue;
    public Slider NOFRslider;
    public Text NOFRvalue;
    public Text IFmapSize;
    public GameObject sharedVariables;
    private SharedVariables sharedVariablesScript;
    void Awake()
    {
        DontDestroyOnLoad(sharedVariables.transform.gameObject);
    }
    void Start() {
        NOFTvalue.text = NOFTslider.value.ToString();
        NOFRvalue.text = NOFRslider.value.ToString();
        sharedVariables = GameObject.Find("SharedVariables");
        sharedVariablesScript = sharedVariables.GetComponent<SharedVariables>();
        sharedVariablesScript.numOfRows = (int)NOFRslider.value;
        sharedVariablesScript.numOfTrees = (int)NOFTslider.value;
        ;

    }

    public void StartScene()
    {
        sharedVariablesScript.numOfRows = (int)NOFRslider.value;
        sharedVariablesScript.numOfTrees = (int)NOFTslider.value;
        sharedVariablesScript.sizeOfMap = int.Parse(IFmapSize.text);
        Application.LoadLevel("Standard_Situation");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void changeValue() { 
    NOFTvalue.text = NOFTslider.value.ToString();
    NOFRvalue.text = NOFRslider.value.ToString();
    }

}
