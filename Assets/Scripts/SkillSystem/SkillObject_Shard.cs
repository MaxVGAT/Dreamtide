using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{

    [SerializeField] private GameObject vfxPrefab;

    public void SetupShard(float detonationTime)
    {
        Invoke(nameof(Explode), detonationTime);
    }

    private void Explode()
    {
        DamageEnemiesInRadius(transform, checkRadius);
        Instantiate(vfxPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Entity_Enemy>() == null)
            return;

        Explode();
    }

}
