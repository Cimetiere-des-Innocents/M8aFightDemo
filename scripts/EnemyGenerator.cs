using Godot;
using System.Collections.Generic;

public partial class EnemyGenerator : Node
{
	[Export]
	private Player player;

	[Export]
	private PackedScene enemy;

	[Export]
	private int maxEnemies = 20;

	[Export]
	private float spawnDelay = 5;

	public int EnemyCount { get; set; }
	public HashSet<RandomMoveEnemy> Enemies { get; } = new HashSet<RandomMoveEnemy>();

	private double timeSinceLastSpawn = 0;

	private uint lastId = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeSinceLastSpawn += delta;
		if (timeSinceLastSpawn > spawnDelay)
		{
			timeSinceLastSpawn = 0;
			if (EnemyCount < maxEnemies)
			{
				var enemyInstance = enemy.Instantiate<RandomMoveEnemy>();
				GetParent().AddChild(enemyInstance);
				var randomDirection = MoveUtil.GenerateRandomDirection();
				var randomDistance = (float)GD.RandRange(10, 20);
				enemyInstance.Position = player.Position + randomDirection * randomDistance;
				enemyInstance.Player = player;
				enemyInstance.EnemyGenerator = this;
				enemyInstance.id = lastId++;
				Enemies.Add(enemyInstance);
				EnemyCount++;
			}
		}
	}
}
