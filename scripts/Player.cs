using Godot;
using System;

public partial class Player : Node3D
{
	[Export]
	private float speed = 10;

	[Export]
	private Camera3D camera;

	[Export]
	private Node3D aim;

	[Export]
	private Node3D visualComponent;

	[Export]
	private CharacterBody3D collider;

	[Export]
	private float maxHP = 100;

	[Export]
	private PackedScene deathEffect;

	[Export]
	private ProgressBar hpBar;

	[Export]
	private Label hpLabel;

	[Export]
	private Material material;

	private Vector2 mousePosition;

	public float HP { get; set; }

	public bool Dead { get; set; } = false;

	public Vector3 Velocity { get; set; }

	public Vector3 AimPosition { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HP = maxHP;
		hpBar.MaxValue = maxHP;
		hpBar.Value = HP;
		hpLabel.Text = "HP: " + HP + "/" + maxHP;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		hpBar.Value = Math.Max(HP, 0);
		hpLabel.Text = "HP: " + Math.Max(HP, 0) + "/" + maxHP;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			mousePosition = mouseMotion.Position;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Dead)
		{
			return;
		}

		if (HP <= 0)
		{
			Dead = true;
			var deathEffectInstance = deathEffect.Instantiate<DeathEffect>();
			deathEffectInstance.SetMaterial(material);
			AddChild(deathEffectInstance);
			GetNode<Node3D>("VisualComponent").QueueFree();
		}

		var deltaPos = new Vector3(0, 0, 0);
		if (Input.IsActionPressed("move_front"))
		{
			deltaPos += Vector3.Forward;
		}
		if (Input.IsActionPressed("move_back"))
		{
			deltaPos += Vector3.Back;
		}
		if (Input.IsActionPressed("move_left"))
		{
			deltaPos += Vector3.Left;
		}
		if (Input.IsActionPressed("move_right"))
		{
			deltaPos += Vector3.Right;
		}

		Velocity = deltaPos.LengthSquared() == 0 ? Vector3.Zero : deltaPos.Normalized() * speed;
		deltaPos = Velocity * (float)delta;

		var collision = collider.MoveAndCollide(deltaPos, true);
		Position += MoveUtil.TryMove(Position, deltaPos, collision);

		var rayFrom = camera.ProjectRayOrigin(mousePosition);
		var rayTo = rayFrom + camera.ProjectRayNormal(mousePosition) * 1000;
		var rayQuery = PhysicsRayQueryParameters3D.Create(rayFrom, rayTo, 0b0001);
		var rayResult = GetWorld3D().DirectSpaceState.IntersectRay(rayQuery);

		if (rayResult.ContainsKey("collider"))
		{
			AimPosition = (Vector3)rayResult["position"];
			aim.Position = AimPosition;

			var aimDir = AimPosition - Position;
			var aimRot = -(float)Math.PI / 2.0f - (float)Math.Atan2(aimDir.Z, aimDir.X);
			visualComponent.Rotation = new Vector3(0, aimRot, 0);
		}
	}
}
