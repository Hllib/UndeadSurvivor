using Fusion;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : NetworkBehaviour
{
    protected float speed;
    protected Vector3 currentTarget;
    protected int damage;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected NetworkPlayer player;

    protected bool isDead;

    protected float tempSpeed;

    [SerializeField]
    protected Transform face;
    [SerializeField]
    protected Transform back;

    protected float canAttack = 0.0f;
    protected float attackRate;
    protected float attackRadius;
    private GameController _gameController;
    private NetworkRunner _networkRunner;

    [SerializeField]
    protected EnemyScriptableObject enemyScriptableObject;

    protected virtual void Init()
    {
        this.animator = GetComponent<Animator>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _networkRunner = GameObject.FindGameObjectWithTag("NetworkRunner").GetComponent<NetworkRunner>();
        FindPlayers();
        if (player != null)
        {
            this.player.OnPlayerDead += OnPlayerDead;
        }

        SetInitialSettings();
    }

    private void OnPlayerDead(object sender, EventArgs e)
    {
        FindPlayers();
    }

    private void FindPlayers()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Any(player => player.GetComponent<NetworkPlayer>().IsDead == false))
        {
            int index = 0;
            do
            {
                index = UnityEngine.Random.Range(0, players.Length);

            } while (players.ElementAt(index).GetComponent<NetworkPlayer>().IsDead);
            this.player = players.ElementAt(index).GetComponent<NetworkPlayer>();
        }
        else
        {
            _gameController.FinishGame();
        }
    }

    public override void Spawned()
    {
        Init();
    }

    protected void FlipDirection()
    {
        transform.Rotate(0f, 180f, 0f);
    }

    public virtual void CalculateMovement()
    {
        if (player != null)
        {
            if (!player.IsDead)
            {
                currentTarget = player.transform.position;
                transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
            }
        }
    }

    protected abstract void SetInitialSettings();

    protected virtual void CheckAttackZone()
    {
        float distance = Vector3.Distance(this.transform.localPosition, player.transform.localPosition);
        if (distance < attackRadius && Time.time > canAttack)
        {
            Attack();
            canAttack = Time.time + attackRate;
        }
    }

    protected abstract void Attack();

    public virtual void CheckLookDirection()
    {
        float faceToTargetDistance = MathF.Abs(currentTarget.x - face.position.x);
        float backToTargetDistance = MathF.Abs(currentTarget.x - back.position.x);

        if (faceToTargetDistance > backToTargetDistance)
        {
            FlipDirection();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (player != null)
        {
            CalculateMovement();
            CheckAttackZone();
            CheckLookDirection();
        }
    }
}