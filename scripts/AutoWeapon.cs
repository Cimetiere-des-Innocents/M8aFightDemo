using Godot;
using System.Linq;

public partial class AutoWeapon : Node3D
{
	[Export]
	private EnemyGenerator enemyGenerator;

	[Export]
	private double coolDown = 0.1;

	[Export]
	private float damage = 5;

	[Export]
	private Node3D sceneRoot;

	[Export]
	private float projectileSpeed = 50;

	[Export]
	private float range = 20;

	[Export]
	private PackedScene projectile;

	[Export]
	private Player player;

	private double timeSinceLastAttack = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player.Dead)
		{
			return;
		}

		timeSinceLastAttack += delta;
		if (timeSinceLastAttack < coolDown)
		{
			return;
		}

		timeSinceLastAttack -= coolDown;
		var enemies = enemyGenerator.Enemies;
		if (!enemies.Any())
		{
			return;
		}

		var closestEnemy = enemies.MinBy(enemy => enemy.GetHitCenter().DistanceTo(GlobalPosition));
		var enemyPosition = closestEnemy.GetHitCenter();
		var distance = GlobalPosition.DistanceTo(enemyPosition);
		if (distance > range)
		{
			return;
		}


		var projectileInstance = projectile.Instantiate<AutoWeaponProjectile>();
		projectileInstance.Position = GlobalPosition;
		projectileInstance.Velocity = MoveUtil.GetBulletVelocity(GlobalPosition, player.Velocity, enemyPosition, closestEnemy.GetVelocity(), projectileSpeed);
		projectileInstance.Damage = damage;
		projectileInstance.Range = range;
		sceneRoot.AddChild(projectileInstance);
	}
}
