using UnityEngine;
using UnityEngine.InputSystem;

public class PLAYER : MonoBehaviour
{
    public float WalkSpeed;
    public float RunSpeed;
    public float JumpForce;
    public Vector2 LookSens;

    [Header("Train Settings")]
    public float TrainShake;
    [HideInInspector]
    public GameObject CamParent;
    GameObject Cam;
    CharacterController cc;
    Vector2 MoveDir;
    PlayerInput IA;
    float Ycam;
    float Ymove;
    float Speed;

    [HideInInspector]
    public bool CanMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        Cam = GameObject.FindWithTag("MainCamera");
        CamParent = Cam.transform.parent.gameObject;
        IA = GetComponent<PlayerInput>();
        CanMove = true;
    }

    public void OnMove(InputValue Dir)
    {
        if(CanMove)
        {
            MoveDir = Dir.Get<Vector2>();
        }
        else
        {
            MoveDir = new Vector2(0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            HandleMovement();
            HandleInteract();
        }
    }

    void HandleMovement()
    {
        Speed = IA.actions["Run"].ReadValue<float>() > 0 ? RunSpeed : WalkSpeed;

        //HandleCamera
        Ycam = Mathf.Clamp(Ycam - IA.actions["Mouse"].ReadValue<Vector2>().y * LookSens.y/10, -89, 89);
        CamParent.transform.localRotation = Quaternion.Euler(Ycam, 0, 0);
        transform.Rotate(0, IA.actions["Mouse"].ReadValue<Vector2>().x * LookSens.x/10, 0);

        //Handle gravity
        if(cc.isGrounded)
        {
            Ymove = IA.actions["Jump"].ReadValue<float>() > 0 ? JumpForce : 0;
        }
        else
        {
            Ymove -= Time.deltaTime * 10;
        }
        

        if(TrainShake > 0)
        {
            TrainShake -= Time.deltaTime * 5;
        }

        Vector2 ShakeOffset = new Vector2(Random.Range(-TrainShake, TrainShake), Random.Range(-TrainShake, TrainShake));
        //Snazz
        float CamStep = MoveDir.magnitude > 0 ? Mathf.Cos(Time.time * (MoveDir.magnitude * Speed))/10 : Mathf.Cos(Time.time) /30;
        Vector3 CamRotOffset = new Vector3(ShakeOffset.y, ShakeOffset.x, ((IA.actions["Mouse"].ReadValue<Vector2>().x/ 6) - MoveDir.x * 2) + (MoveDir.magnitude > 0 ? CamStep * 5 : 0));
        Cam.transform.localRotation = Quaternion.Lerp(Cam.transform.localRotation, Quaternion.Euler(CamRotOffset), Time.deltaTime * 5);

        Vector3 CamPosOffset = new Vector3(CamStep, -Mathf.Abs(CamStep), 0);
        Cam.transform.localPosition = Vector3.Lerp(Cam.transform.localPosition, CamPosOffset, Time.deltaTime * 5);

        Vector3 TempVec = transform.forward * MoveDir.y + transform.right * MoveDir.x;
        cc.Move(((TempVec * Speed) + transform.up * Ymove) * Time.deltaTime);
    }

    void HandleInteract()
    {
        if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out RaycastHit hir, 3f))
        {
            if (IA.actions["Interact"].ReadValue<float>() > 0)
            {
                if(hir.transform.gameObject.tag == "Interactable")
                {
                    hir.transform.GetComponent<InteractableBase>().OnInteracted();
                    CanMove = false;
                    print("Interactinmg");
                }
            }
        }
    }

    public void ResetCam()
    {
        Cam.transform.parent = CamParent.transform;
        CanMove = true;
    }

    public void OnShake(float Amount)
    {
        TrainShake = Amount;
    }

}
