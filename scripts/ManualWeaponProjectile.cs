using Godot;

public partial class ManualWeaponProjectile : Node3D
{
	[Export]
	private Node3D visualComponent;

	[Export]
	private CharacterBody3D collider;

	[Export]
	private PackedScene hitEffect;

	public Vector3 Velocity { get; set; }

	public float Damage { get; set; }

	public float ExplosionRadius { get; set; }

	public void SetVisualRotation(Vector3 rotation)
	{
		visualComponent.Rotation = rotation;
	}

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
				enemyVelocity = abstractEnemy.GetVelocity();
			}
			var effectInstance = hitEffect.Instantiate<ManualWeaponProjectileHit>();
			effectInstance.Velocity = enemyVelocity;
			effectInstance.Damage = Damage;
			effectInstance.ExplosionRadius = ExplosionRadius;
			GetParent().AddChild(effectInstance);
			effectInstance.Position = collision.GetPosition();
			QueueFree();
		}
		else
		{
			Position += Velocity * (float)delta;
			Velocity += MoveUtil.Gravity * (float)delta;
		}
	}
}
