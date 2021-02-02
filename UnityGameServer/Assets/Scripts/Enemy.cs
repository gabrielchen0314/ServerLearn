using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static int m_MaxEnemies = 10;
    public static Dictionary<int, Enemy> s_Dic_enemies = new Dictionary<int, Enemy>();
    private static int m_NextEnemyID = 1;

    public int m_ID;
    public EnemyState m_State;
    public Player m_Target;
    public CharacterController m_Controller;
    public Transform m_ShootOrigin;
    public float m_Gravity = -9.81f;
    public float m_PatrolSpeed = 2f;
    public float m_ChaseSpeed = 8f;
    public float m_Health;
    public float m_MaxHealth = 100f;
    public float m_DetectionRange = 30f;
    public float m_ShootRange = 15f;
    public float m_ShootAccuarcy = 0.1f;
    public float m_PatrolDuration = 3f;
    public float m_IdleDuration = 1f;

    private bool m_IsPatrolRoutineRunning;
    private float m_yVelocity = 0;

    private void Start()
    {
        m_ID = m_NextEnemyID;
        m_NextEnemyID++;
        s_Dic_enemies.Add(m_ID,this);

        ServerSend.SpaenEnemy(this);

        m_State = EnemyState.patrol;
        m_Gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        m_PatrolSpeed *= Time.fixedDeltaTime;
        m_ChaseSpeed *= Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        switch (m_State)
        {
            case EnemyState.idle:
                LookForPlayer();
                break;
            case EnemyState.patrol:
                if (!LookForPlayer())
                {
                    Patrol();
                }
                break;
            case EnemyState.chase:
                Chase();
                break;
            case EnemyState.attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private bool LookForPlayer()
    {
        foreach(Client _client in Server.s_Dic_Clients.Values)
        {
            if (_client.m_Player != null)
            {
            	Vector3 _enemyToPlayer = _client.m_Player.transform.position - transform.position;
            	if(_enemyToPlayer.magnitude <= m_DetectionRange)
            	{
                	if(Physics.Raycast(m_ShootOrigin.position,_enemyToPlayer,out RaycastHit _hit, m_DetectionRange))
                	{
                    	if (_hit.collider.CompareTag("Player"))
                    	{
                        	m_Target = _hit.collider.GetComponent<Player>();
                        	if (m_IsPatrolRoutineRunning)
                        	{
                            	m_IsPatrolRoutineRunning = false;
                            	StopCoroutine(StartPatrol());
                        	}

                        	m_State = EnemyState.chase;
                        	return true;
						}
                    }
                }
            }
        }

        return false;
    }

    private void Patrol()
    {
        if (!m_IsPatrolRoutineRunning)
        {
            StartCoroutine(StartPatrol());
        }

        Move(transform.forward, m_PatrolSpeed);
    }

    private IEnumerator StartPatrol()
    {
        m_IsPatrolRoutineRunning = true;
        Vector2 _RandomPatrolDirection = Random.insideUnitCircle.normalized;
        transform.forward = new Vector3(_RandomPatrolDirection.x, 0f, _RandomPatrolDirection.y);

        yield return new WaitForSeconds(m_PatrolDuration);

        m_State = EnemyState.idle;

        yield return new WaitForSeconds(m_IdleDuration);

        m_State = EnemyState.patrol;
        m_IsPatrolRoutineRunning = false;
    }

    private void Chase()
    {
        if (CanSeeTarget())
        {
            Vector3 _EnemyToPlayer = m_Target.transform.position - transform.position;

            if(_EnemyToPlayer.magnitude <= m_ShootRange)
            {
                m_State = EnemyState.attack;
            }
            else
            {
                Move(_EnemyToPlayer, m_ChaseSpeed);
            }
        }
        else
        {
            m_Target = null;
            m_State = EnemyState.patrol;
        }
    }

    private void Attack()
    {
        if (CanSeeTarget())
        {
            Vector3 _EnemyToPlayer = m_Target.transform.position - transform.position;
            transform.forward = new Vector3(_EnemyToPlayer.x, 0f, _EnemyToPlayer.z);

            if (_EnemyToPlayer.magnitude <= m_ShootRange)
            {
                Shoot(_EnemyToPlayer);
            }
            else
            {
                Move(_EnemyToPlayer, m_ChaseSpeed);
            }
        }
        else
        {
            m_Target = null;
            m_State = EnemyState.patrol;
        }
    }

    private void Move(Vector3 iDirection, float iSpeed)
    {
        iDirection.y = 0;
		transform.forward = iDirection;
        Vector3 _Movement = transform.forward * iSpeed;

        if (m_Controller.isGrounded)
        {
            m_yVelocity = 0f;
        }
        m_yVelocity += m_Gravity;

        _Movement.y = m_yVelocity;
        m_Controller.Move(_Movement);

        ServerSend.EnemyPsoition(this);
    }

    private void Shoot(Vector3 iShootDirection)
    {
        if (Physics.Raycast(m_ShootOrigin.position, iShootDirection, out RaycastHit _hit, m_ShootRange))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                if(Random.value <= m_ShootAccuarcy)
                {
                    _hit.collider.GetComponent<Player>().TakeDamage(50);
                }
            }
        }
    }

    public void TakeDamage(float iDamage)
    {
        m_Health -= iDamage;
        if (m_Health <= 0)
        {
            m_Health = 0;

            s_Dic_enemies.Remove(m_ID);
            Destroy(gameObject);
        }

        ServerSend.EnemyHealth(this);
    }

    private bool CanSeeTarget()
    {
        if(m_Target = null)
        {
            return false;
        }

        if (Physics.Raycast(m_ShootOrigin.position, m_Target.transform.position - transform.position, out RaycastHit _hit, m_DetectionRange))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
}

public enum EnemyState
{
    idle,
    patrol,
    chase,
    attack
}
