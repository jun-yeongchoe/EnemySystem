using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IState
{
    Enemy enemy;
    [SerializeField] float patrolRadius = 15f;
    Vector3 patrolDestination;
    private Vector3 currentDestination;
    private bool goingToStart;

    public PatrolState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("패트롤 상태");
        enemy.agent.speed = 3.5f;
        goingToStart = true;
        currentDestination = enemy.startPos;
    }

    public void Update()
    {
        if (enemy.target != null)
        {
            float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);
            if (distance < enemy.checkRadius) enemy.stateMachine.ChangeState(new DetectedState(enemy));
        }

        if(!enemy.agent.pathPending && enemy.agent.remainingDistance <= 0.7f)
        {
            if (goingToStart)
            {
                currentDestination = SetPatrolPoint();
                goingToStart = false;
                enemy.agent.SetDestination(currentDestination);
            }
            else
            {
                currentDestination = enemy.startPos;
                goingToStart = true;
                enemy.agent.SetDestination(currentDestination);
            }
        }
    }

    public void Exit()
    {

    }

    Vector3 SetPatrolPoint()
    {
        //if (enemy.isSetDestination) return patrolDestination;
        Vector2 rand = Random.insideUnitCircle * patrolRadius;
        Vector3 patrolPoint = enemy.patrolCenterPos.position + new Vector3(rand.x, 0, rand.y);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(patrolPoint, out hit, 1f, NavMesh.AllAreas))
        {
            patrolDestination = hit.position;
        }
        else
        {
            patrolDestination = patrolPoint;
        }

        enemy.isSetDestination = true;
        Debug.Log($"처음 순찰 위치 : {patrolDestination}");
        return patrolDestination;
    }
}
