using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    //swimming movement
    float prevH;
    float h;
    float v;

    

    //jump vars
    bool isJumping;
    float timeSpacePressed;
    bool grounded;
    int pMask;
    
    //Camera + mouse
    float camX;
    float camY;

    Vector2 moveDir;
    Vector2 mousePos;

    bool shootThisFrame;
    Rigidbody2D rigid;
    SpriteRenderer sRend;


    //Modifiable From inspector

    [Header("PLEASE SET BEFORE USE")]
    [Header("Camera settings")]
    [SerializeField]
    Camera MainCamera;

    [SerializeField]
    [Tooltip("The higher this number is, the closer the camera will be to the player")]
    float camLerpGoal = 4;

    [Header ("Movement settings")]

    [SerializeField]
    bool gravityAffectedMovement = false;

    [SerializeField]
    float swimMoveSpeed = 2f;

    [SerializeField]
    float walkMoveSpeed = 2f;

    [SerializeField]
    float walkAccel = 1f;

    [Header("Jump Settings")]

    [Tooltip("The distance the two ground check rays are fom the center of the player")]
    [SerializeField]
    float rayCenterDist = .5f;

    [SerializeField]
    float raySize = 1f;

    [SerializeField]
    float maxJumpTime = .25f;

    [SerializeField]
    float jumpSpeed = 2f;

    [Header("Weapon settings")]

    [SerializeField]
    List<GameObject> weaponPrefabList = new List<GameObject>();




    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        sRend = GetComponent<SpriteRenderer>();

        if(MainCamera == null)
            Debug.Log("NO CAMERA ATTACHED TO PLAYER!!!!");

        //player mask
        pMask = ~(1 << 6);
        

    }

    
    void Update()
    {

        if (gravityAffectedMovement && rigid.gravityScale == 0) //change later so this doesnt have to run every frame? maye on a button or something
        {
            rigid.gravityScale = 1;
        }
        else if(rigid.gravityScale == 1 && !gravityAffectedMovement)
        {
            rigid.gravityScale = 0;
        }


        CheckGrounded();

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

    void CheckGrounded()
    {
        Vector2 leftRay = new Vector2(transform.position.x + rayCenterDist, transform.position.y);
        Vector2 rightRay = new Vector2(transform.position.x - rayCenterDist, transform.position.y);
        Debug.DrawRay(leftRay, new Vector2(0, raySize * -1), Color.magenta);
        Debug.DrawRay(rightRay, new Vector2(0, raySize * -1), Color.magenta);
        if (Physics2D.Raycast(leftRay, Vector2.down, raySize, pMask) || Physics2D.Raycast(rightRay, Vector2.down, raySize, pMask))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        Debug.Log(grounded);
    }

    void InputReader()
    {
        prevH = h;
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Debug.Log(prevH + " " + h);

        if (h == -1)
            sRend.flipX = true;
        else if (h == 1)
            sRend.flipX = false;


        if (gravityAffectedMovement)
        {
            if (Input.GetKeyDown(KeyCode.Space) && grounded && !isJumping)
            {
                isJumping = true;
                timeSpacePressed = Time.time;
                Debug.Log("Jumping");
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
                Debug.Log("End of key press");
            }
            moveDir = new Vector2(rigid.velocity.x, 0);
        }
        else
        {
            if (moveDir.x != h || moveDir.y != v)
            {
                moveDir = new Vector2(h, v);
                moveDir.Normalize();
                moveDir *= swimMoveSpeed;
            }
        }
        mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
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
        //local vars
        float goalSpeed;
        float speedDiff; 


        if (!gravityAffectedMovement)
        {
            rigid.velocity = moveDir; //maybe change from movedir to some sort of vec that decays to move dir for alternate added forces
            Debug.Log(rigid.velocity);
        }
        if (gravityAffectedMovement)
        {
            //Directional Movement
            goalSpeed = h * walkMoveSpeed;
            speedDiff = goalSpeed - rigid.velocity.x;
            rigid.AddForce(new Vector2(speedDiff * walkAccel, 0));





            //Jumping stuff

            if (isJumping && (Time.time - timeSpacePressed < maxJumpTime ))
            {
                moveDir.y = jumpSpeed;
            }
            else if ( !isJumping || (Time.time - timeSpacePressed >= maxJumpTime))
            {
                moveDir.y = rigid.velocity.y;
            }

            if(rigid.velocity.y < -50) //doesnt work rn
            {
                Debug.Log(rigid.velocity.y + " before edit");
                rigid.velocity = new Vector2(rigid.velocity.x, -50f);
                Debug.Log(rigid.velocity.y + " after edit");
            }
            rigid.velocity = moveDir;
        }
        
    }
}
