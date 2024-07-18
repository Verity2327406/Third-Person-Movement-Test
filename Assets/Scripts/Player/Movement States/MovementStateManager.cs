using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour, ICharacterMover
{
    #region Movement
    [Header("Movement")]
    public float curMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float coverSpeed = 1.5f;

    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float xInput, yInput;
    [HideInInspector] public bool isSprint {  get; private set; }
    public Vector3 inCoverMoveDirection { get; set; }
    public Vector3 inCoverProhibitedDirection { get; set; }

    [HideInInspector] public Animator _a;
    #endregion

    #region GroundCheck
    [Header("Ground Check")]
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;
    #endregion

    #region Gravity
    [Header("Gravity")]
    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;
    #endregion

    #region Input
    [Header("Input")]
    CharacterController controller;
    [HideInInspector] public InputReader ir;
    #endregion

    #region Cover
    [Header("Cover")]
    public bool cover;
    [SerializeField] float maxCoverDistance = 3;
    public LayerMask coverLayerMask;

    [SerializeField] Transform highCoverDetection;
    public Transform rightCoverDetector;
    public Transform leftCoverDetector;
    public float horizontalCoverDetectorLength = .75f;
    public bool inHighCover;

    Vector3 coverSurfaceDirection;

    Vector3 coverHitPoint;

    bool autoMoverActive;
    Vector3 autoMoverTarget;
    float autoMoverStoppingDist;

    public WeaponManager weaponManager;
    #endregion

    #region Statemachine
    [HideInInspector] public MovementBaseState curState;

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public RunState Run = new RunState();
    public CoverState Cover = new CoverState();
    #endregion

    private void Start()
    {
        _a = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        ir = GetComponent<InputReader>();

        //Allows change of radius, if needed for specific mission or player, without breaking autoMover
        autoMoverStoppingDist = controller.radius + .01f;

        SwitchState(Idle);
    }

    private void Update()
    {
        xInput = ir.MovementValue.x;
        yInput = ir.MovementValue.y;

        #region Movement
        GetDirectionAndMove();
        Gravity();

        _a.SetFloat("hInput", xInput);
        _a.SetFloat("vInput", yInput);

        isSprint = ir.SprintValue;
        #endregion

        #region Cover
        if (ir.isCover && IsNearCover())
        {
            SetCoverType();
            MoveCharacterToCover();
        }

        if (cover && autoMoverActive)
        {
            MoveToCover();
        }

        if (cover && !autoMoverActive)
        {
            CoverMove();
        }

        if (cover)
        {
            SetCoverType();
            InCoverMovementRestrictor();
        }

        Debug.DrawRay(transform.position, transform.forward * maxCoverDistance, Color.red);
        Debug.DrawRay(highCoverDetection.position, highCoverDetection.forward * maxCoverDistance, Color.red);
        #endregion

        curState.UpdateState(this);
    }

    public void SwitchState(MovementBaseState state)
    {
        curState = state;
        curState.EnterState(this);
    }

    #region Movement
    private void GetDirectionAndMove()
    {
        if (cover) return;

        dir = transform.forward * ir.MovementValue.y + transform.right * ir.MovementValue.x;
        controller.Move(dir.normalized * curMoveSpeed * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);

        if (Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask)) return true;
        else return false;
    }

    private void Gravity()
    {
        if (!IsGrounded()) { velocity.y += gravity * Time.deltaTime; }
        else if (velocity.y < 0) velocity.y = -2;

        controller.Move(velocity * Time.deltaTime);
    }
    #endregion

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
    //}

    #region Cover
    public bool IsNearCover()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxCoverDistance, coverLayerMask))
        {
            coverHitPoint = hitInfo.point;
            coverSurfaceDirection = GetCoverSurfaceDirection(hitInfo.normal);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetCoverType()
    {
        if (Physics.Raycast(highCoverDetection.position, highCoverDetection.forward, maxCoverDistance, coverLayerMask))
        {
            // High cover, so cant always shoot
            inHighCover = true;
            weaponManager.canShoot = false;
            Debug.Log("High");
        }
        else
        {
            // Low cover, so can always shoot
            inHighCover = false;
            weaponManager.canShoot = true;
            Debug.Log("Low");
        }
    }

    private void MoveCharacterToCover()
    {
        cover = true;
        BeginMoveToCover(coverHitPoint);
    }

    public void BeginMoveToCover(Vector3 targetPos)
    {
        autoMoverActive = true;
        autoMoverTarget = targetPos;
        ir.DisableControls();
    }

    private void MoveToCover()
    {
        Vector3 moveDirection = (autoMoverTarget - transform.position).normalized;

        if (Vector3.Distance(transform.position, autoMoverTarget) > autoMoverStoppingDist)
        {
            _a.SetBool("Running", true);
            _a.SetFloat("vInput", 1);
            _a.SetFloat("hInput", 0);
            controller.Move(moveDirection * runSpeed * Time.deltaTime);
        }
        else
        {
            autoMoverActive = false;
            autoMoverTarget = Vector3.zero;
            ir.EnableControls();
        }
    }

    private void InCoverMovementRestrictor()
    {
        bool didRightCoverDetectHit = Physics.Raycast(rightCoverDetector.position, rightCoverDetector.forward, horizontalCoverDetectorLength, coverLayerMask);
        bool didLeftCoverDetectHit = Physics.Raycast(leftCoverDetector.position, leftCoverDetector.forward, horizontalCoverDetectorLength, coverLayerMask);

        Debug.Log("Right: " + didRightCoverDetectHit + ", Left: " + didLeftCoverDetectHit);

        Debug.DrawRay(rightCoverDetector.position, rightCoverDetector.forward * horizontalCoverDetectorLength, Color.white);
        Debug.DrawRay(leftCoverDetector.position, leftCoverDetector.forward * horizontalCoverDetectorLength, Color.white);


        if (!didLeftCoverDetectHit || !didRightCoverDetectHit)
        {
            // At end of Cover
            if (inHighCover)
            {
                //Only shoot if at the end of cover if its a wall
                weaponManager.canShoot = true;
            }

            if (!didLeftCoverDetectHit)
            {
                SetCoverDirection(coverSurfaceDirection, -coverSurfaceDirection);
            } else
            {
                SetCoverDirection(coverSurfaceDirection, coverSurfaceDirection);
            }
        }
        else
        {
            if (inHighCover)
            {
                //Can't shoot since not at end of cover
                weaponManager.canShoot = false;
            }

            SetCoverDirection(coverSurfaceDirection, Vector3.zero);
        }

    }

    private Vector3 GetCoverSurfaceDirection(Vector3 hitNormal)
    {
        return Vector3.Cross(hitNormal, Vector3.up).normalized;
    }

    private void CoverMove()
    {
        Vector3 perpDirection = Vector3.Cross(inCoverMoveDirection, Vector3.up);
        transform.forward = perpDirection;

        Vector3 moveDirection = inCoverMoveDirection.normalized * ir.MovementValue.x;

        if(moveDirection != inCoverProhibitedDirection.normalized)
        {
            controller.Move(moveDirection * (coverSpeed * Time.deltaTime));
        }
    }

    private void SetCoverDirection(Vector3 moveDirection, Vector3 directionToProhibit)
    {
        inCoverMoveDirection = moveDirection;
        inCoverProhibitedDirection = directionToProhibit;
    }
    #endregion
}