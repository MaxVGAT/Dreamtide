using UnityEngine;

public class EnemyState : EntityState
{

    protected Entity_Enemy enemy;

    public EnemyState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;

        rb = enemy.rb;
        anim = enemy.anim;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.E))
            stateMachine.ChangeState(enemy.attackState);

        anim.SetFloat("moveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);
    }

}
    
