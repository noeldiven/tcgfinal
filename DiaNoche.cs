using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiaNoche : MonoBehaviour
{
    private float min, grados;
    public float timeSpeed = 1;
    public Scrollbar tiempo;
 
    // Update is called once per frame
    void Update()
    {
        min = tiempo.value * 1440.0f;
   
        grados = min / 4;
        this.transform.localEulerAngles = new Vector3(grados, -90f, 0f);
        if (grados >= 180)
        {
            this.GetComponent<Light>().enabled = false;
        }
        else
        {
            this.GetComponent<Light>().enabled = true;
       
        }
    }
}





