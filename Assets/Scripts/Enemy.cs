using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Engage")]
    [SerializeField] public float checkRadius = 10f;
    [SerializeField] public float atkRadius = 2f;

    [SerializeField] public Transform target;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] public PlayerController pc;
    public NavMeshAgent agent;
    [SerializeField] public Transform patrolCenterPos;

    public bool isSetDestination;
    public int hp;
    [SerializeField] Transform weapon;
    private int atkDmg = 5;
    private float atkRange = 1f;

    public StateMachine stateMachine { get; private set; }

    public Vector3 startPos;
    [SerializeField] Animator anim;

    //Blend, Attack
    const string AnimBlend = "Blend";
    const string AnimAttack = "Attack";

    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine();
        startPos=transform.position;
        isSetDestination = false;
        hp = 100;
    }

    private void Start()
    {
        stateMachine.ChangeState(new PatrolState(this));
    }

    void Update()
    {
        stateMachine.Update();
        UpdateBlendValue();
    }

    void UpdateBlendValue()
    {
        if (anim == null) return;
        Vector3 velocity = agent.velocity;
        Vector2 vec = new Vector2(velocity.x, velocity.z);
        float blend = vec.magnitude / agent.speed;

        if (blend < 0) blend = 0;
        if (blend > 1) blend = 1;

        anim.SetFloat(AnimBlend, blend);
    }

    public void HandleAtk()
    {
        if (anim == null) return;
        StartCoroutine(PulseBool(AnimAttack));
        
    }

    public void TakeDmg(int dmg)
    {
        hp -= dmg;
        Debug.Log("적 남은체력 : " + hp);
        if(hp <= 0) gameObject.SetActive(false);
        Debug.Log("적이 죽음");
    }

    private void Atk()
    {
        if (weapon == null) return;
        Collider[] hits = Physics.OverlapSphere(weapon.transform.position, atkRange, targetLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDmg(atkDmg);
            }
        }
    }

    IEnumerator PulseBool(string name)
    {
        anim.SetBool(name, true);
        yield return null;
        anim.SetBool(name, false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, atkRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
