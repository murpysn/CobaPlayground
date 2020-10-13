using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviour
{
    public float moveSpeed;
    public float rotSpeed;
    public float pushDist;

    private Animator anim;
    private Rigidbody rb;
    private Vector3 target;

    private int moveDir;

    private bool isMove = false;
    public bool pushOrPullMode;
    public bool isDoPushOrPull;

  //  public MovableObjects movableObjects;
    public LayerMask clickLayer;

    private int fingerID = -1;

    void Awake()
    {
      #if !UNITY_EDITOR
        fingerID = 0;
      #endif
    }

    void Start() {
      anim = GetComponent<Animator>();
      rb = GetComponent<Rigidbody>();
      target = transform.position;
    }

    void Update(){
      if (EventSystem.current.IsPointerOverGameObject(fingerID))    // is the touch on the GUI
      {
        // GUI Action
        return;
      }

      if(!GlobalConfiguration.gamePaused){

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit mouseHit;
        if (Physics.Raycast(ray, out mouseHit, clickLayer) && Input.GetMouseButtonDown(0)) {
            target = mouseHit.point;

            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", true);
            isMove = true;
        }

        RaycastHit pushHit;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(Vector3.forward), out pushHit, pushDist) && pushHit.transform.tag == "Movable") {
            pushOrPullMode = true;
        }

       /* if(Input.GetMouseButtonDown(0) && pushOrPullMode){
            if(movableObjects != null) {
              movableObjects = null;
            }
            pushOrPullMode = false;
        }*/
      }
    }

    void FixedUpdate() {
      if(!GlobalConfiguration.gamePaused){
        if(isMove)
          MoveToTarget();

        if(isDoPushOrPull)
          PushOrPull();
      }
    }

    private void MoveToTarget() {
      target = new Vector3(target.x, transform.position.y, target.z);

      Quaternion rotTarget = Quaternion.LookRotation(target - transform.position);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, rotTarget, rotSpeed);

      transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.fixedDeltaTime);

      float dist = Vector3.Distance(transform.position, target);
      if(dist < 0.1f) {
        StopMoving();
      }

      RaycastHit pushHit;
      if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(Vector3.forward), out pushHit, pushDist)) {
        StopMoving();

        if(pushHit.transform.tag == "Movable"){
          //movableObjects = pushHit.transform.GetComponent<MovableObjects>();
        }
      }
    }

    void StopMoving(){
      rb.velocity = Vector3.zero;
      anim.SetBool("isIdle", true);
      anim.SetBool("isWalking", false);
      anim.SetBool("isPushing", false);
      isMove = false;
    }

    public void PushOrPull(){
        float movement = moveDir * moveSpeed * Time.fixedDeltaTime;
        transform.Translate(Vector3.forward * movement);
    }

    public void StopPushOrPull(){
      StopMoving();
    //  if(movableObjects != null) {
      //  movableObjects.DisconnectJoint();
     // }
      isDoPushOrPull = false;
    }

    public void DoPushOrPull(int direction){
      if(pushOrPullMode){
        //movableObjects.ConnectJoint(rb);

        if(direction > 0) {
          anim.SetBool("isIdle", false);
          anim.SetBool("isWalking", false);
          anim.SetBool("isPushing", true);
        } else {
          anim.SetBool("isIdle", false);
          anim.SetBool("isWalking", false);
          anim.SetBool("isPushing", false);
        }

        moveDir = direction;
        isDoPushOrPull = true;
      }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + new Vector3(0, 0.5f, 0), transform.position + new Vector3(0, 0.5f, 0) + transform.forward * pushDist);
    }
}
