using Godot;

public partial class EnemyAttackRing : Node3D
{
	[Export]
	private Node3D rangeNode;

	[Export]
	private AnimationPlayer animationPlayer;

	[Export]
	private Area3D collider;

	public Player Player { get; set; }

	public float Damage { get; set; }

	private bool hasHit = false;

	public void SetRange(float range)
	{
		rangeNode.Scale = new Vector3(range, range, range);
	}

	public override void _Ready()
	{
		animationPlayer.AnimationFinished += (name) =>
		{
			QueueFree();
		};

		collider.BodyEntered += (body) =>
		{
			if (!hasHit)
			{
				Player.HP -= Damage;
			}
			hasHit = true;
		};
	}
}
