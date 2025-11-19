using System.Collections;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("GroundCheck")]
    [SerializeField] Transform groundPoint;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] Animator anim;
    CharacterController characterController;

    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform wand;

    private Vector3 velocity;

    [SerializeField] private LayerMask enemyLayer;
    private int atkDmg = 10;
    private float atkRange = 1f;
    private int hp = 100;

    //Blend, Attack, Jump
    const string AnimBlend = "Blend";
    const string AnimAttack = "Attack";
    const string AnimJump = "Jump";

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        //여기서 메서드 전부 호출하고
        //움직임을 입력받고
        Vector2 input = GetMovementInput();

        //카메라가 바라보는 방향계산하고
        Quaternion camYaw = GetCameraYaw();

        //입력값, 카메라 방향을 이용해 실제 월드 이동 방향벡터 계산
        Vector3 moveDir = GetMoveDir(input, camYaw);

        //바닥에 닿았냐 확인
        bool isGround = CheckGrounded();

        //중력적용, 
        HandleGravity(isGround);

        //점프입력처리
        HandleJump(isGround);

        //최종 이동
        MoveCharacter(moveDir);

        //회전 처리
        RotateCharacter(moveDir);

        //공격
        HandleAtk();

        //블랜드값 업데이트
        UpdateBlendValue();

    }

    

    // 1. 입력처리 메서드(플레이어가 지금 어느 방향키를 누르고 있는지 확인하는 메서드)
    Vector2 GetMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        return new Vector2(h, v);
    }

    // 2. 카메라 바라보는 방향 사용 - Y축만
    Quaternion GetCameraYaw()
    {
        if (cameraTransform == null) return Quaternion.identity;

        return Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        // X, Z 축 회전은 무시하고 Y축 회전만 가져오겠다.
    }

    // 3. 이동방향 계산
    Vector3 GetMoveDir(Vector2 input, Quaternion cameraYaw)
    {
        Vector3 dir = new Vector3(input.x, 0, input.y);

        return (cameraYaw * dir).normalized;
    }

    // 4. 바닥체크
    bool CheckGrounded()
    {
        return Physics.CheckSphere(groundPoint.position, groundCheckRadius, groundLayer);
    }

    // 5. 중력처리
    void HandleGravity(bool grounded)
    {
        if (grounded && velocity.y < 0)
        {
            velocity.y = -1;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    // 6. 점프처리
    void HandleJump(bool grounded)
    {
        if (!grounded) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpForce;

            //애니메이션 처리
            if (anim != null) StartCoroutine(PulseBool(AnimJump));
        }
    }

    // 7. 실제 이동 처리 
    void MoveCharacter(Vector3 moveDir)
    {
        Vector3 move = moveDir * moveSpeed;
        move.y = velocity.y;

        characterController.Move(move * Time.deltaTime);
    }

    // 8. 회전을 처리하는 메서드
    void RotateCharacter(Vector3 moveDir)
    {
        if (moveDir == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
            );
    }

    // 9. 공격처리하는 메서드
    void HandleAtk()
    {
        if (anim == null) return;
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(PulseBool(AnimAttack));
        }
    }
    private void Atk()
    {
        if (wand == null) return;
        Collider[] hits = Physics.OverlapSphere(wand.transform.position, atkRange, enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDmg(atkDmg);
            }
        }
    }

    // 10. 애니메이션 블랜드 값을 업데이트 하는 메서드
    void UpdateBlendValue()
    {
        if (anim == null) return;
        Vector2 vec = new Vector2(
            characterController.velocity.x, // 좌우 속도
            characterController.velocity.z  // 앞뒤 속도
            );
        float blend = vec.magnitude / moveSpeed;

        if (blend < 0) blend = 0;
        if (blend > 1) blend = 1;

        anim.SetFloat(AnimBlend, blend);
    }

    // 11. 파라미터 갱신 코루틴
    IEnumerator PulseBool(string name)
    {
        anim.SetBool(name, true);
        yield return null;
        anim.SetBool(name, false);
    }

    public void TakeDmg(int dmg)
    {
        hp -= dmg;
        Debug.Log("플레이어 남은체력 : " + hp);
        if (hp <= 0)
        {
            gameObject.SetActive(false);
            Debug.Log("플레이어가 죽음");
        }
    }

}
