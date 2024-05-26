using Godot;
using System.Collections.Generic;

public partial class ManualWeaponProjectileHit : Node3D
{
	[Export]
	private Area3D collider;

	[Export]
	private Node3D rangeNode;

	[Export]
	private AnimationPlayer animationPlayer;

	public Vector3 Velocity { get; set; }

	public float Damage { get; set; }

	public float ExplosionRadius { get; set; }
	// Called when the node enters the scene tree for the first time.

	private HashSet<uint> damagedEnemies = new HashSet<uint>();
	public override void _Ready()
	{
		animationPlayer.AnimationFinished += (name) =>
		{
			QueueFree();
		};

		rangeNode.Scale = new Vector3(ExplosionRadius, ExplosionRadius, ExplosionRadius);

		collider.BodyEntered += (body) =>
		{
			var enemy = body.GetParent();
			if (enemy is AbstractEnemy abstractEnemy)
			{
				if (!damagedEnemies.Contains(abstractEnemy.id))
				{
					abstractEnemy.Damage(Damage);
					damagedEnemies.Add(abstractEnemy.id);
				}
			}
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += Velocity * (float)delta;
	}
}
