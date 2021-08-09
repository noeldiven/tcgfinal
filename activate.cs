using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class activate : MonoBehaviour
{
    public firtpersoncontroller mycontroller;
    public mineplayer draw;
    //public GameObject disen;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void diseno()
    {
        //disen.SetActive(true);
        mycontroller.enabled = false;
        draw.enabled = true;
    }

    public void test()
    {
        mycontroller.enabled = true;
        draw.enabled = false;
    }

    void Update()
    {
     
    }
}
