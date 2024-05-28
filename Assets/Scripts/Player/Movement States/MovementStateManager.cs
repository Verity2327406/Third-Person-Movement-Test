using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    #region Movement
    public float curMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;

    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float xInput, yInput;
    [HideInInspector] public bool isSprint {  get; private set; }
    [HideInInspector] public Animator _a;
    #endregion

    #region GroundCheck
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;
    #endregion

    #region Gravity
    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;
    #endregion

    #region Input
    CharacterController controller;
    [HideInInspector] public InputReader ir;
    #endregion

    [HideInInspector] public MovementBaseState curState;

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public RunState Run = new RunState();
    public CoverState Cover = new CoverState();

    public bool cover;

    private void Start()
    {
        _a = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        ir = GetComponent<InputReader>();

        SwitchState(Idle);
    }

    private void Update()
    {
        xInput = ir.MovementValue.x;
        yInput = ir.MovementValue.y;

        GetDirectionAndMove();
        Gravity();

        _a.SetFloat("hInput", xInput);
        _a.SetFloat("vInput", yInput);

        isSprint = ir.SprintValue;

        curState.UpdateState(this);
    }

    public void SwitchState(MovementBaseState state)
    {
        curState = state;
        curState.EnterState(this);
    }

    private void GetDirectionAndMove()
    {
        if(cover)
            dir = transform.forward * 0 + transform.right * ir.MovementValue.x;
        else
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
    //}
}
