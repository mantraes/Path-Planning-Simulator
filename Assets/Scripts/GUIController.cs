using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIController : MonoBehaviour {

	// Use this for initialization

    public Slider NOFTslider;
    public Text NOFTvalue;
    public Slider NOFRslider;
    public Text NOFRvalue;
    public Slider Seedslider;
    public Text Seedvalue;
    public GameObject sharedVariablesPre;
    private GameObject sharedVariables;
    private SharedVariables sharedVariablesScript;
    void Awake()
    {
    }
    void Start() {
        sharedVariables = GameObject.FindGameObjectWithTag("Variables");
        if (sharedVariables == null)
        {
            Instantiate(sharedVariablesPre, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            sharedVariables = GameObject.FindGameObjectWithTag("Variables");
            sharedVariablesScript = sharedVariables.GetComponent<SharedVariables>();
            NOFTvalue.text = NOFTslider.value.ToString();
            NOFRvalue.text = NOFRslider.value.ToString();
            if (Seedslider.value == 1) Seedvalue.text = "None";
            else Seedvalue.text = (Seedslider.value - 1f).ToString();
            sharedVariablesScript.numOfRows = (int)NOFRslider.value;
            sharedVariablesScript.numOfTrees = (int)NOFTslider.value;
            sharedVariablesScript.seed = (int)Seedslider.value - 1;
        }
        else {
            sharedVariablesScript = sharedVariables.GetComponent<SharedVariables>();
            NOFTvalue.text = sharedVariablesScript.numOfTrees.ToString();
            NOFRvalue.text = sharedVariablesScript.numOfRows.ToString();
            if (sharedVariablesScript.seed == 0) Seedvalue.text = "None";
            else Seedvalue.text = (sharedVariablesScript.seed).ToString();
            NOFRslider.value = sharedVariablesScript.numOfRows;
            NOFTslider.value = sharedVariablesScript.numOfTrees;
            Seedslider.value = sharedVariablesScript.seed + 1;
        }
    }

    public void StartScene()
    {

        sharedVariablesScript.numOfRows = (int)NOFRslider.value;
        sharedVariablesScript.numOfTrees = (int)NOFTslider.value;
        sharedVariablesScript.seed = (int)Seedslider.value - 1;
        DontDestroyOnLoad(sharedVariables.transform.gameObject);
        Application.LoadLevel("Standard_Situation");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void changeValue() { 
    NOFTvalue.text = NOFTslider.value.ToString();
    NOFRvalue.text = NOFRslider.value.ToString();
    if (Seedslider.value == 1) Seedvalue.text = "None";
    else Seedvalue.text = (Seedslider.value - 1f).ToString();
    }

    

}
