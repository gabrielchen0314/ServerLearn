using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> s_Dic_Projectiles = new Dictionary<int, Projectile>();
    private static int m_NextProjectile = 1;

    public int m_ID;
    public Rigidbody m_Rigidbody;
    public int m_ThrownByPlayer;
    public Vector3 m_InitialForce;
    public float m_ExplosionRadius = 1.5f;
    public float m_ExplosionDamage = 75.0f;

    private void Start()
    {
        m_ID = m_NextProjectile;
        m_NextProjectile++;
        s_Dic_Projectiles.Add(m_ID,this);

        ServerSend.SpawnProjectile(this, m_ThrownByPlayer);

        m_Rigidbody.AddForce(m_InitialForce);
        StartCoroutine(ExplodeAfterTime());
    }

    private void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Exploade();
    }

    public void Initialize(Vector3 iInitialMovementDirection, float iInitialForceStrength, int iThrownByPlayer)
    {
        m_InitialForce = iInitialMovementDirection * iInitialForceStrength;
        m_ThrownByPlayer = iThrownByPlayer;
    }

    private void Exploade()
    {
        ServerSend.ProjectileExploded(this);

        Collider[] _colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius);
        foreach (Collider _collider in _colliders)
        {
            if (_collider.CompareTag("Player"))
            {
                _collider.GetComponent<Player>().TakeDamage(m_ExplosionDamage);
            }
            else if (_collider.CompareTag("Enemy"))
            {
                _collider.GetComponent<Enemy>().TakeDamage(m_ExplosionDamage);
            }
        }

        s_Dic_Projectiles.Remove(m_ID);
        Destroy(gameObject);
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(10f);

        Exploade();
    }
}
