using Godot;

public abstract partial class AbstractEnemy : Node3D
{
    public uint id { get; set; }
    public abstract Vector3 GetVelocity();
    public abstract Vector3 GetHitCenter();
    public abstract void Damage(float damage);
};