using Godot;

public partial class AutoWeaponProjectileHit : Node3D
{
	[Export]
	private AnimationPlayer animationPlayer;

	public Vector3 Velocity { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer.AnimationFinished += (name) =>
		{
			QueueFree();
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += Velocity * (float)delta;
	}
}
