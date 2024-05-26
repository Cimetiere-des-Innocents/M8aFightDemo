using System;
using Godot;

public partial class RandomMoveEnemy : AbstractEnemy
{
	private enum State
	{
		RandomMoving,
		MovingToPlayer,
		Attacking
	}

	[Export]
	private float speed = 15;

	[Export]
	private float maxHP = 100;

	[Export]
	private PackedScene deathEffect;

	[Export]
	private CharacterBody3D collider;

	[Export]
	private Material material;

	[Export]
	private AnimationPlayer animationPlayer;

	[Export]
	private float attackRange = 1.5f;

	[Export]
	private float attackDamage = 5;

	[Export]
	private PackedScene attackRing;

	[Export]
	private ProgressBar hpBar;

	[Export]
	private Label hpLabel;

	[Export]
	private Label damageLabel;

	[Export]
	private double maxDamageShowTime = 1;

	public float HP { get; set; }

	public Player Player { get; set; }

	public EnemyGenerator EnemyGenerator { get; set; }

	private State state = State.RandomMoving;

	private Vector3 randomDirection;

	private float randomSpeed;

	private float damageCount = 0;

	private float oldDamageCount = 0;

	private double timeSinceLastDamage = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HP = maxHP;
		randomDirection = MoveUtil.GenerateRandomDirection();
		randomSpeed = (float)GD.RandRange(speed / 5, speed);
		state = (State)(GD.Randi() % 2);

		hpBar.MaxValue = maxHP;
		hpBar.Value = HP;
		hpLabel.Text = "HP: " + HP + "/" + maxHP;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeSinceLastDamage += delta;
		if (oldDamageCount != damageCount)
		{
			oldDamageCount = damageCount;
			timeSinceLastDamage = 0;
		}
		if (timeSinceLastDamage >= maxDamageShowTime)
		{
			damageCount = 0;
			oldDamageCount = 0;
		}

		hpBar.Value = Math.Max(HP, 0);
		hpLabel.Text = "HP: " + Math.Max(HP, 0) + "/" + maxHP;
		damageLabel.Text = damageCount == 0 ? "" : "" + damageCount;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (HP <= 0)
		{
			var deathEffectInstance = deathEffect.Instantiate<DeathEffect>();
			GetParent().AddChild(deathEffectInstance);
			deathEffectInstance.Position = Position;
			deathEffectInstance.SetMaterial(material);
			EnemyGenerator.EnemyCount--;
			EnemyGenerator.Enemies.Remove(this);
			QueueFree();
		}

		if (state != State.Attacking && Position.DistanceTo(Player.Position) <= attackRange)
		{
			animationPlayer.Play("attack");
			state = State.Attacking;
			animationPlayer.AnimationFinished += (animationName) =>
			{
				if (animationName == "attack")
				{
					state = State.RandomMoving;
					randomDirection = MoveUtil.GenerateRandomDirection();
				}
			};
		}


		if (state == State.RandomMoving)
		{
			if (GD.Randf() < 0.01f)
			{
				state = State.MovingToPlayer;
				randomSpeed = (float)GD.RandRange(speed / 5, speed);
			}

			var deltaPos = randomDirection * randomSpeed * (float)delta;
			var collision = collider.MoveAndCollide(deltaPos, true);
			Position += MoveUtil.TryMove(Position, deltaPos, collision, () =>
			{
				randomDirection = MoveUtil.GenerateRandomDirection();
				randomSpeed = (float)GD.RandRange(speed / 5, speed);
			});
		}
		else if (state == State.MovingToPlayer)
		{
			if (GD.Randf() < 0.01f)
			{
				state = State.RandomMoving;
				randomDirection = MoveUtil.GenerateRandomDirection();
				randomSpeed = (float)GD.RandRange(speed / 5, speed);
			}

			var playerPosition = Player.Position;
			var deltaPos = playerPosition - Position;
			var deltaPosNormalized = deltaPos.Normalized();
			var deltaPosNormalizedScaled = deltaPosNormalized * randomSpeed * (float)delta;
			var collision = collider.MoveAndCollide(deltaPosNormalizedScaled, true);
			Position += MoveUtil.TryMove(Position, deltaPosNormalizedScaled, collision, () =>
			{
				state = State.RandomMoving;
				randomDirection = MoveUtil.GenerateRandomDirection();
				randomSpeed = (float)GD.RandRange(speed / 5, speed);
			});
		}
	}

	public void Attack()
	{
		if (HP <= 0)
		{
			return;
		}

		var attackRingInstance = attackRing.Instantiate<EnemyAttackRing>();
		attackRingInstance.SetRange(attackRange);
		attackRingInstance.Player = Player;
		attackRingInstance.Damage = attackDamage;
		AddChild(attackRingInstance);
	}

	public override Vector3 GetVelocity()
	{
		if (state == State.Attacking)
		{
			return Vector3.Zero;
		}
		else if (state == State.RandomMoving)
		{
			return randomDirection * randomSpeed;
		}
		else
		{
			return (Player.Position - Position).Normalized() * randomSpeed;
		}
	}

	public override Vector3 GetHitCenter()
	{
		return Position + new Vector3(0, 1, 0);
	}

	public override void Damage(float damage)
	{
		HP -= damage;
		damageCount += damage;
	}
}
