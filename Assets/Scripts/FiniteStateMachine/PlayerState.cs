public abstract class PlayerState : EntityState
{

    protected Entity_Player player;
    protected PlayerInputSet input;
    protected Player_SkillManager skillManager;

    public PlayerState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
        stats = player.stats;
        skillManager = player.skillManager;
    }

    public override void Update()
    {
        base.Update();

        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            skillManager.dash.SetSkillOnCooldown();
            stateMachine.ChangeState(player.dashState);
        }

        if(input.Player.UltimateSkill.WasPressedThisFrame() && skillManager.domain.CanUseSkill())
        {
            if (skillManager.domain.InstantDomain())
            {
                skillManager.domain.CreateDomain();
            }
            else
                stateMachine.ChangeState(player.domainState);

            skillManager.domain.SetSkillOnCooldown();
        }

    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();

        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    private bool CanDash()
    {

        if (skillManager.dash.CanUseSkill() == false)
            return false;

        if (player.isWallDetected)
            return false;

        if (stateMachine.currentState == player.dashState)
            return false;

        return true;
    }

}
