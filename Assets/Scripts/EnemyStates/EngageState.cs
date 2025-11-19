using UnityEngine;

public class EngageState : IState
{
    private Enemy enemy;
    private float attackCooldown = 1.2f;
    private float lastAttackTime = -999f;

    public EngageState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("교전 상태");
        enemy.agent.isStopped = true;
    }

    public void Update()
    {
        if(enemy.target == null)
        {
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);

        if (distance > enemy.atkRadius) 
        {
            enemy.stateMachine.ChangeState(new DetectedState(enemy));
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown) 
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    public void Exit()
    {

    }

    private void Attack()
    {
        enemy.transform.LookAt(enemy.target.position);
        enemy.HandleAtk();
    }
}
