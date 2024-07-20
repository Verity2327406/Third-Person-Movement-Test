using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using Unity.VisualScripting.ReorderableList;
using DG.Tweening;
using System.Xml.Serialization;

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

    #region Cover
    [Header("Cover")]
    [Tooltip("Player offset from the cover")]
    public float shootFromCoverOffset;

    Vector3 shootFromHighCover;
    Vector3 highCornerReturnPos;

    bool aimingFromHighCover = false;
    bool poppedOut = false;
    bool isBack = true;
    [HideInInspector] public float oldAxis;
    #endregion

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
        #region Player Cam
        xAxis += Input.GetAxisRaw("Mouse X") * mouseSens;
        yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSens;
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        _cam.m_Lens.FieldOfView = Mathf.Lerp(_cam.m_Lens.FieldOfView, curFov, fovSmootheSpeed * Time.deltaTime);

        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask)) 
            aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);

        MoveCamera();
        #endregion

        #region Cover
        PopOutAndShoot();
        BackInCover();
        ExitAndWalk();
        #endregion

        curState.UpdateState(this);
    }

    private void LateUpdate()
    {
        if(movement.cover)
            camFollowPos.localEulerAngles = new Vector3(yAxis, xAxis, camFollowPos.localEulerAngles.z);
        else
        {
            camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
        }
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

    public void ShootFromHighCoverSetup(Vector3 direction, bool inHighCover)
    {
        shootFromHighCover = transform.position + (direction.normalized * shootFromCoverOffset);
        aimingFromHighCover = inHighCover;
    }

    private void ExitAndWalk()
    {
        if(poppedOut && _ir.MovementValue.y > 0)
        {
            movement.ExitCover();
            poppedOut = false;
            xAxis = oldAxis;
        }
    }

    private void PopOutAndShoot()
    {
        if (aimingFromHighCover)
        {
            poppedOut = true;
            isBack = false;
            highCornerReturnPos = transform.position;
            transform.DOMove(shootFromHighCover, 0.25f);
        }
    }

    private void BackInCover()
    {
        if (!movement.cover) return;
        if (_ir.ADSValue) return;

        if(poppedOut && !isBack)
        {
            transform.DOMove(highCornerReturnPos, .25f);
            poppedOut = false;
            aimingFromHighCover = false;
            isBack = true;
        }
    }
}
