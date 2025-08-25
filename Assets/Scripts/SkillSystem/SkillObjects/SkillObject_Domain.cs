using Unity.VisualScripting;
using UnityEngine;

public class SkillObject_Domain : SkillObject_Base
{
    private Skill_Domain domainManager;
    private float expansionSpeed;
    private float slowDownPercent;
    private float duration;

    private Vector3 targetScale;
    private bool isShrinking;

    public void SetupDomain(Skill_Domain domainManager)
    {
        this.domainManager = domainManager;

        duration = domainManager.GetDomainDuration();
        float maxSize = domainManager.maxDomainSize;
        slowDownPercent = domainManager.GetSlowPercentage();
        expansionSpeed = domainManager.expandSpeed;

        targetScale = Vector3.one * maxSize;
        Invoke(nameof(ShrinkDomain), duration);
    }

    private void Update()
    {
        HandleScaling();
    }

    private void HandleScaling()
    {
        float sizeDifference = Mathf.Abs(transform.localScale.x - targetScale.x);
        bool shouldChangeScale = sizeDifference > 0.1f; // Check if domain is big or small enough, relative to target scale

        if (shouldChangeScale)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, expansionSpeed * Time.deltaTime);

        if (isShrinking && sizeDifference < 0.1f)
        {
            TerminateDomain();
        }
    }

    private void TerminateDomain()
    {
        domainManager.ClearTargets();
        Destroy(gameObject);
    }

    private void ShrinkDomain()
    {
        targetScale = Vector3.zero;
        isShrinking = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity_Enemy enemy = collision.GetComponent<Entity_Enemy>();

        if (enemy == null)
            return;

        domainManager.AddTarget(enemy);
        enemy.SlowDownEntityBy(duration, slowDownPercent, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Entity_Enemy enemy = collision.GetComponent<Entity_Enemy>();

        if (enemy == null)
            return;

        enemy.StopSlowDown();
    }
}
