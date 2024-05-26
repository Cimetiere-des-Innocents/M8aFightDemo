using Godot;
using System;

public partial class ManualWeapon : Node3D
{
	[Export]
	private double coolDown = 3;

	[Export]
	private float damage = 50;

	[Export]
	private Node3D sceneRoot;

	[Export]
	private float projectileSpeed = 20;

	[Export]
	private Player player;

	[Export]
	private float explosionRadius = 5;

	[Export]
	private double attackInputOffset = 0.2;

	[Export]
	private PackedScene projectile;

	[Export]
	private ProgressBar cdBar;

	private double timeSinceLastAttack = 0;

	private bool attackQueued = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timeSinceLastAttack = coolDown;
		cdBar.MaxValue = coolDown;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("attack") && coolDown - timeSinceLastAttack < attackInputOffset)
		{
			attackQueued = true;
		}
	}

	public override void _Process(double delta)
	{
		var mousePos = GetViewport().GetMousePosition();
		cdBar.Position = new Vector2(mousePos.X - cdBar.Size.X / 2, mousePos.Y + cdBar.Size.Y / 2);
		cdBar.Visible = timeSinceLastAttack < coolDown;
		cdBar.Value = timeSinceLastAttack;
	}

	public override void _PhysicsProcess(double delta)
	{
		bool canHit;
		var projectileVelocity = MoveUtil.GetBombVelocity(GlobalPosition, player.AimPosition, projectileSpeed, out canHit);

		if (attackQueued && timeSinceLastAttack >= coolDown)
		{
			attackQueued = false;
			timeSinceLastAttack = 0;
			var projectileInstance = projectile.Instantiate<ManualWeaponProjectile>();
			projectileInstance.Position = GlobalPosition;
			projectileInstance.Velocity = projectileVelocity;
			projectileInstance.Damage = damage;
			projectileInstance.ExplosionRadius = explosionRadius;
			var aimDir = player.AimPosition - GlobalPosition;
			var aimRot = -(float)Math.PI / 2.0f - (float)Math.Atan2(aimDir.Z, aimDir.X);
			projectileInstance.SetVisualRotation(new Vector3(0, aimRot, 0));
			sceneRoot.AddChild(projectileInstance);
		}

		timeSinceLastAttack += delta;
	}
}
