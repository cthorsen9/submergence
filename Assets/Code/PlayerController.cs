using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    float h;
    float v;

    float camX;
    float camY;

    Vector2 moveDir;
    Vector2 mousePos;

    bool shootThisFrame;
    Rigidbody2D rigid;


    //Modifiable From inspector

    [Header("Please set before use")]
    [SerializeField]
    Camera MainCamera;

    [SerializeField]
    float MoveSpeed = 2f;

    [SerializeField]
    [Tooltip("The higher this number is, the closer the camera will be to the player")]
    float camLerpGoal = 4;

    
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        if(MainCamera == null)
        {
            Debug.Log("NO CAMERA ATTACHED TO PLAYER!!!!");
        }
    }

    
    void Update()
    {

        //Input is recieved
        InputReader();

        //Camera following player + mouse
        FollowCam();

        if (shootThisFrame)
            Shoot();
    }

    void FixedUpdate()
    {
        //Input converted to movement
        PlayerMove();
    }

    void InputReader()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);

        if(moveDir.x != h || moveDir.y != v)
        {
            moveDir = new Vector2(h, v);
            moveDir.Normalize();
            moveDir *= MoveSpeed;
        }
    }

    void FollowCam()
    {
        //lerps between current pos and a point between the mouse and player. camLerpGoal determines the distance the camera is from the player
        camX = Mathf.Lerp(camX, transform.position.x + ((mousePos.x - transform.position.x) /camLerpGoal), Time.time);
        camY = Mathf.Lerp(camY, transform.position.y + ((mousePos.y - transform.position.y) /camLerpGoal), Time.time);
        MainCamera.transform.position = new Vector3(camX, camY, MainCamera.transform.position.z);
    }

    void Shoot()
    {

    }

    void PlayerMove()
    {
        rigid.velocity = moveDir; //maybe change from movedir to some sort of vec that decays to move dir for alternate added forces
        Debug.Log(rigid.velocity);
    }
}
