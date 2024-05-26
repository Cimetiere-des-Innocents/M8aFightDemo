using Godot;

public partial class DeathEffect : Node3D
{
	[Export]
	private AnimationPlayer animationPlayer;

	[Export]
	private MeshInstance3D body;

	public void SetMaterial(Material material)
	{
		body.MaterialOverride = material;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var rotation = new Vector3(0, (float)GD.RandRange(0, 360), 0);
		RotationDegrees = rotation;

		animationPlayer.Play("death");

		animationPlayer.AnimationFinished += onAnimationFinished;
	}

	private void onAnimationFinished(StringName animName)
	{
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
