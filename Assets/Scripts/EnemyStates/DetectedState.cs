using UnityEngine;
using UnityEngine.AI;

public class DetectedState : IState
{
    private Enemy enemy;

    public DetectedState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("적발견 상태");
        enemy.agent.isStopped = false;
    }

    public void Update()
    {
        if (enemy.target == null)
        {
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);

        enemy.agent.speed = 7;
        enemy.agent.SetDestination(enemy.target.position);

        if (distance <= enemy.atkRadius)
        {
            enemy.stateMachine.ChangeState(new EngageState(enemy));
        }
        else if (distance > enemy.checkRadius)
        {
            enemy.agent.speed = 3.5f;
            enemy.agent.isStopped = false;
            enemy.agent.SetDestination(enemy.startPos);
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
        }
        
    }

    public void Exit()
    {

    }
}
