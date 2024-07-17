using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using Unity.VisualScripting.ReorderableList;

public class AimStateManager : MonoBehaviour
{
    [HideInInspector] public AimBaseState curState;
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();

    [SerializeField] float mouseSens = 1;
    float xAxis, yAxis;
    [SerializeField] Transform camFollowPos;

    [HideInInspector] public Animator _a;
    [HideInInspector] public InputReader _ir;
    [HideInInspector] public CinemachineVirtualCamera _cam;
    public float adsFov = 40;
    [HideInInspector] public float hipFov;
    [HideInInspector] public float curFov;
    public float fovSmootheSpeed;

    [SerializeField] Transform aimPos;
    [SerializeField] float aimSmoothSpeed = 20;
    [SerializeField] LayerMask aimMask;

    float xFollowPos;
    float yFollowPos, ogYPos;
    [SerializeField] float shoulderSwapSpeed = 10;
    MovementStateManager movement;
    public bool inCover;

    private void Start()
    {
        movement = GetComponent<MovementStateManager>();
        xFollowPos = camFollowPos.localPosition.x;
        ogYPos = camFollowPos.localPosition.y;
        yFollowPos = ogYPos;
        _a = GetComponent<Animator>();
        _ir = GetComponent<InputReader>();

        _cam = GetComponentInChildren<CinemachineVirtualCamera>();
        hipFov = _cam.m_Lens.FieldOfView;

        SwitchState(Hip);
    }

    private void Update()
    {
        xAxis += Input.GetAxisRaw("Mouse X") * mouseSens;
        yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSens;
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        _cam.m_Lens.FieldOfView = Mathf.Lerp(_cam.m_Lens.FieldOfView, curFov, fovSmootheSpeed * Time.deltaTime);

        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask)) 
            aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);

        MoveCamera();

        curState.UpdateState(this);
    }

    private void LateUpdate()
    {
        camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
    }

    public void SwitchState(AimBaseState state)
    {
        curState = state;
        curState.EnterState(this);
    }

    void MoveCamera()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) xFollowPos = -xFollowPos;
        yFollowPos = ogYPos;

        Vector3 newFollwPos = new Vector3(xFollowPos, yFollowPos, camFollowPos.localPosition.z);
        camFollowPos.localPosition = Vector3.Lerp(camFollowPos.localPosition, newFollwPos, shoulderSwapSpeed * Time.deltaTime);
    }
}
