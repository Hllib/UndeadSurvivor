using Fusion;
using System;
using System.Linq;
using UnityEngine;

public abstract class Enemy : NetworkBehaviour
{
    protected float Speed;
    protected Vector3 CurrentTarget;
    protected int DamageDone;
    protected Animator Animator;
    protected SpriteRenderer SpriteRenderer;
    protected NetworkPlayer Player;

    protected bool IsDead;

    protected float TempSpeed;

    [SerializeField]
    protected Transform Face;
    [SerializeField]
    protected Transform Back;

    protected float CanAttack = 0.0f;
    protected float AttackRate;
    protected float AttackRadius;
    private GameController _gameController;

    [SerializeField]
    protected EnemyScriptableObject EnemyScriptableObject;

    protected virtual void Init()
    {
        this.Animator = GetComponent<Animator>();
        this.SpriteRenderer = GetComponent<SpriteRenderer>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        FindPlayers();
        if (Player != null)
        {
            this.Player.OnPlayerDead += OnPlayerDead;
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
            this.Player = players.ElementAt(index).GetComponent<NetworkPlayer>();
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
        if (Player != null)
        {
            if (!Player.IsDead)
            {
                CurrentTarget = Player.transform.position;
                transform.position = Vector3.MoveTowards(transform.position, CurrentTarget, Speed * Time.deltaTime);
            }
        }
    }

    protected abstract void SetInitialSettings();

    protected virtual void CheckAttackZone()
    {
        float distance = Vector3.Distance(this.transform.localPosition, Player.transform.localPosition);
        if (distance < AttackRadius && Time.time > CanAttack)
        {
            Attack();
            CanAttack = Time.time + AttackRate;
        }
    }

    protected abstract void Attack();

    public virtual void CheckLookDirection()
    {
        float faceToTargetDistance = MathF.Abs(CurrentTarget.x - Face.position.x);
        float backToTargetDistance = MathF.Abs(CurrentTarget.x - Back.position.x);

        if (faceToTargetDistance > backToTargetDistance)
        {
            FlipDirection();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Player != null)
        {
            CalculateMovement();
            CheckAttackZone();
            CheckLookDirection();
        }
    }
}