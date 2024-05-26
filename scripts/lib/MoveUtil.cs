using System;
using Godot;

public class MoveUtil
{
    public static float GravityValue = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
    public static Vector3 Gravity = new Vector3(0, -GravityValue, 0);
    public static Vector3 TryMove(Vector3 pos, Vector3 delta, KinematicCollision3D collision, Action onCollision = null)
    {
        if (collision == null)
        {
            return delta;
        }
        else
        {
            var collisionCount = collision.GetCollisionCount();
            var canMove = true;
            for (var i = 0; i < collisionCount; i++)
            {
                var collisionBody = collision.GetCollider(i);
                if (collisionBody is Node3D node3D)
                {
                    var bodyPos = node3D.GlobalPosition;
                    var bodyDir = bodyPos - pos;
                    var angle = bodyDir.AngleTo(delta);
                    if (angle < (float)Math.PI / 6.0f)
                    {
                        canMove = false;
                    }
                }
            }
            if (canMove)
            {
                return delta;
            }
            else
            {
                if (onCollision != null)
                {
                    onCollision();
                }
                return Vector3.Zero;
            }
        }
    }
    public static Vector3 GenerateRandomDirection()
    {
        var randomAngle = GD.RandRange(0, Math.PI * 2.0f);
        return new Vector3((float)Math.Cos(randomAngle), 0, (float)Math.Sin(randomAngle));
    }

    public static Vector3 GetBulletVelocity(Vector3 playerPos, Vector3 playerVelocity, Vector3 enemyPos, Vector3 enemyVelocity, float bulletSpeed)
    {
        var enemyVToPlayer = enemyVelocity - playerVelocity;
        var distance = enemyPos - playerPos;
        var d2 = distance.LengthSquared();
        var dv = distance.Dot(enemyVToPlayer);
        var v2 = enemyVelocity.LengthSquared();
        var delta = dv * dv - 4 * d2 * (v2 - bulletSpeed * bulletSpeed);
        if (delta < 0)
        {
            return (enemyPos - playerPos).Normalized() * bulletSpeed + playerVelocity;
        }

        var u = (-dv + (float)Math.Sqrt(delta)) / (2 * d2);
        var bulletVToPlayer = u * distance + enemyVToPlayer;
        return bulletVToPlayer + playerVelocity;
    }

    public static Vector3 GetBombVelocity(Vector3 playerPos, Vector3 targetPos, float maxBombSpeed, out bool canHit)
    {
        var maxAxisSpeed = maxBombSpeed / (float)Math.Sqrt(2);
        var distance = targetPos.DistanceTo(playerPos);
        var yDistance = targetPos.Y - playerPos.Y;
        var horizontalDistance = Math.Sqrt(distance * distance - yDistance * yDistance);
        float axisSpeed;
        canHit = true;
        if (yDistance > horizontalDistance)
        {
            canHit = false;
            axisSpeed = maxAxisSpeed;
        }
        else
        {
            axisSpeed = (float)(horizontalDistance / Math.Sqrt(2 * (horizontalDistance - yDistance) / GravityValue));
        }
        var xDistance = targetPos.X - playerPos.X;
        var zDistance = targetPos.Z - playerPos.Z;
        var xSpeed = axisSpeed / horizontalDistance * xDistance;
        var zSpeed = axisSpeed / horizontalDistance * zDistance;
        return new Vector3((float)xSpeed, axisSpeed, (float)zSpeed);
    }
}