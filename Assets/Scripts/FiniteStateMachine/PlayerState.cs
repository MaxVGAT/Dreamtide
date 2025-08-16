public abstract class PlayerState : EntityState
{

    protected Entity_Player player;
    protected PlayerInputSet input;
    protected Player_SkillManager skills;

    public PlayerState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
        stats = player.stats;
        skills = player.skillManager;
    }

    public override void Update()
    {
        base.Update();

        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            skills.dash.SetSkillOnCooldown();
            stateMachine.ChangeState(player.dashState);
        }

    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();

        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    private bool CanDash()
    {

        if (skills.dash.CanUseSkill() == false)
            return false;

        if (player.isWallDetected)
            return false;

        if (stateMachine.currentState == player.dashState)
            return false;

        return true;
    }

}
