using Godot;

public partial class AutoWeaponProjectile : Node3D
{
	[Export]
	private CharacterBody3D collider;

	[Export]
	private PackedScene hitEffect;

	public Vector3 Velocity { get; set; }

	public float Damage { get; set; }

	public float Range { get; set; }

	private float distanceTraveled = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		var collision = collider.MoveAndCollide(Velocity * (float)delta, true);
		if (collision != null)
		{
			var collisionNode = (CollisionObject3D)collision.GetCollider();
			var enemy = collisionNode.GetParent();
			var enemyVelocity = Vector3.Zero;
			if (enemy is AbstractEnemy abstractEnemy)
			{
				abstractEnemy.Damage(Damage);
				enemyVelocity = abstractEnemy.GetVelocity();
			}
			var effectInstance = hitEffect.Instantiate<AutoWeaponProjectileHit>();
			effectInstance.Velocity = enemyVelocity;
			GetParent().AddChild(effectInstance);
			effectInstance.Position = collision.GetPosition();
			QueueFree();
		}
		else
		{
			distanceTraveled += Velocity.Length() * (float)delta;
			if (distanceTraveled > Range)
			{
				QueueFree();
			}
			else
			{
				Position += Velocity * (float)delta;
			}
		}
	}
}
