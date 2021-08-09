using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void VoidFuncVoid();
public class mineplayer : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    Dictionary<Vector3, Quaternion> rotations;

    public GameObject selection;
    public GameObject prefab;

    Vector3 diff;

    Vector3 mouseDelta = Vector3.zero;
    Vector3 amount = new Vector3(-137, -38, 50);


    Vector3 cameraDir = Vector3.zero;
    Quaternion cameraRot = Quaternion.identity;
    Vector3 cameraPos = Vector3.zero;

    Vector3 focusPos = Vector3.zero;

    VoidFuncVoid cameraAction;

    public float sensitivity = 5.0f;

    public float zoomNear = 20;
    public float zoomFar = 300;

    Vector3 repeatLastPos = Vector3.zero;
    Vector3 lastDiff = Vector3.zero;


    enum UserEvents
    {
        NONE = 0
        //Raycast HIT
        , HIT = 0b_0000_0001
        //Mouse Buttons
        , LMB = 0b_0000_0010
        , RMB = 0b_0000_0100
        , MMB = 0b_0000_1000
        , LMB_D = 0b_0001_0000
        , RMB_D = 0b_0010_0000
        //Keys
        , ALT = 0b_0100_0000
        , F = 0b_1000_0000
        , SP = 0b_1_0000_0000
    };

    Dictionary<UserEvents, VoidFuncVoid> userActions;
    UserEvents userEvents;

    void NOP() { }  //No OPeration



    // Start is called before the first frame update
    void Start()
    {
        rotations = new Dictionary<Vector3, Quaternion>() {
             {Vector3.back, Quaternion.identity}
            ,{Vector3.up, Quaternion.Euler(90,0,0)}
            ,{Vector3.left, Quaternion.Euler(0,90,0)}
            ,{Vector3.forward, Quaternion.Euler(0,180,0)}
            ,{Vector3.down, Quaternion.Euler(270,0,0)}
            ,{Vector3.right, Quaternion.Euler(0,270,0)}
        };


        cameraAction = NOP;

        CameraRotateView();

        userActions = new Dictionary<UserEvents, VoidFuncVoid>() {
            {UserEvents.NONE, ()=>{}}

            //Cuando el raton esta sobre un objeto
            ,{UserEvents.HIT, ()=>{
                Debug.DrawLine(ray.origin, hit.point);
                diff = hit.point - hit.transform.position;

                diff *= 2;
                float f = 0.95f;
                diff.x = Mathf.Abs(diff.x) < f? 0 : diff.x;
                diff.y = Mathf.Abs(diff.y) < f? 0 : diff.y;
                diff.z = Mathf.Abs(diff.z) < f? 0 : diff.z;

                if (rotations.ContainsKey(diff))
                {
                    selection.SetActive(true);
                    selection.transform.position = hit.transform.position;
                    selection.transform.rotation = rotations[diff];
                }
            } }

            //Clic
            ,{UserEvents.HIT | UserEvents.LMB | UserEvents.LMB_D, ()=>{
                repeatLastPos = diff + hit.transform.position;
                Instantiate(prefab, repeatLastPos, Quaternion.identity);

                lastDiff = diff;
                repeatLastPos += lastDiff;
            } }

            //Clic derecho
            ,{UserEvents.HIT | UserEvents.RMB | UserEvents.RMB_D, ()=>{
                if (hit.transform.position != Vector3.zero)
                {
                    //reset repeat last
                    repeatLastPos = Vector3.zero;
                    Destroy(hit.transform.gameObject, 0.1f);
                }
            } }

            //Repeat last...
            ,{ UserEvents.SP, ()=>{
                if(repeatLastPos != Vector3.zero)
                {
                    Instantiate(prefab, repeatLastPos, Quaternion.identity);
                    repeatLastPos += lastDiff;
                }
            } }
            ,{ UserEvents.HIT | UserEvents.SP, ()=>{
                if(repeatLastPos != Vector3.zero)
                {
                    Instantiate(prefab, repeatLastPos, Quaternion.identity);
                    repeatLastPos += lastDiff;
                }
            } }


            //Rotate View
            ,{UserEvents.ALT | UserEvents.LMB, ()=>{
                cameraAction = CameraRotateView;
            } }

             //Rotate View
            ,{ UserEvents.HIT | UserEvents.ALT | UserEvents.LMB, ()=>{
                 cameraAction = CameraRotateView;
            } }

             //Focus View
            ,{ UserEvents.HIT | UserEvents.F , ()=>{
                focusPos = hit.transform.position;
            } }

            ,{ UserEvents.F , ()=>{
                focusPos = Vector3.zero;
            } }

             //Pan View
            ,{ UserEvents.MMB , ()=>{
                cameraAction = CameraPanView;
            } }
            ,{ UserEvents.HIT | UserEvents.MMB , ()=>{
                cameraAction = CameraPanView;
            } }

            ,{ UserEvents.HIT | UserEvents.ALT | UserEvents.MMB , ()=>{
                cameraAction = CameraPanView;
            } }

            ,{  UserEvents.ALT | UserEvents.MMB , ()=>{
                cameraAction = CameraPanView;
            } }

             //Right clic Zoom
            ,{ UserEvents.HIT | UserEvents.ALT | UserEvents.RMB, ()=>{
                cameraAction = CameraRBMZoom;
            } }
            ,{ UserEvents.ALT | UserEvents.RMB, ()=>{
                cameraAction = CameraRBMZoom;
            } }

        };

    }


    void CameraPanView() {
        mouseDelta.Set(
            -Input.GetAxisRaw("Mouse X"),
            -Input.GetAxisRaw("Mouse Y"),
            -Input.GetAxisRaw("Mouse ScrollWheel"));
        focusPos += (Camera.main.transform.right * mouseDelta.x
                     + Camera.main.transform.up * mouseDelta.y)
                     * sensitivity * 0.1f;
        CameraReposition();
    }
    void CameraRBMZoom() {
        mouseDelta.Set(
           -Input.GetAxisRaw("Mouse X"),
           -Input.GetAxisRaw("Mouse Y"),
           -Input.GetAxisRaw("Mouse ScrollWheel"));
        amount.z += (mouseDelta.x + mouseDelta.y + mouseDelta.z)
                    * sensitivity;
        amount.z = Mathf.Clamp(amount.z, zoomNear, zoomFar);
        CameraReposition();
    }
    void CameraJustWheelZoom() {
        mouseDelta.Set(0, 0, -Input.GetAxisRaw("Mouse ScrollWheel") );
        amount += mouseDelta * sensitivity;
        amount.z = Mathf.Clamp(amount.z, zoomNear, zoomFar);
        CameraReposition();
    }
    void CameraRotateView() {
        Debug.Log("camerarotateview()");
        mouseDelta.Set(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y"),
            -Input.GetAxisRaw("Mouse ScrollWheel"));
        amount += mouseDelta * sensitivity;
        amount.z = Mathf.Clamp(amount.z, zoomNear, zoomFar);
        amount.y = Mathf.Clamp(amount.y, -89, 89);

        cameraRot = Quaternion.AngleAxis(amount.x, Vector3.up)
                    * Quaternion.AngleAxis(amount.y, Vector3.right);
        CameraReposition();
    }
    void CameraReposition() {
        cameraPos = cameraRot * Vector3.forward;
        cameraPos *= amount.z * 0.1f;
        cameraPos += focusPos;
        Camera.main.transform.position = cameraPos;
        Camera.main.transform.LookAt(focusPos);
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        userEvents = UserEvents.NONE;
        cameraAction = CameraJustWheelZoom;

        //Le pegamos a algo?
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100.0f)) { userEvents |= UserEvents.HIT; }

        //Tecla alt
        if (Input.GetKey(KeyCode.LeftAlt)) { userEvents |= UserEvents.ALT; }
        if (Input.GetKey(KeyCode.RightAlt)) { userEvents |= UserEvents.ALT; }

        //Tecla F
        if (Input.GetKeyDown(KeyCode.F)) { userEvents |= UserEvents.F; }

        //Espacio
        if (Input.GetKeyDown(KeyCode.Space)) { userEvents |= UserEvents.SP; }


        //Botones del raton
        if (Input.GetMouseButton(0)) { userEvents |= UserEvents.LMB; }
        if (Input.GetMouseButton(1)) { userEvents |= UserEvents.RMB; }
        if (Input.GetMouseButton(2)) { userEvents |= UserEvents.MMB; }

        if (Input.GetMouseButtonDown(0)) { userEvents |= UserEvents.LMB_D; }
        if (Input.GetMouseButtonDown(1)) { userEvents |= UserEvents.RMB_D; }


        selection.SetActive(false);

        if (userActions.ContainsKey(userEvents)) { userActions[userEvents](); }

    }


    void LateUpdate()
    {
        cameraAction();

    }

}