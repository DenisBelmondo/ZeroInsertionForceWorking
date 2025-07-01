using System.Numerics;

namespace Belmondo.ZeroInsertionForce;

public class World
{
    public struct Spawned<T>(T value)
    {
        public T Value = value;
        public bool IsFlaggedForDeletion;

        public static implicit operator T(Spawned<T> spawned) => spawned.Value;
        public static implicit operator Spawned<T>(T value) => new(value);
    }

    private InputManager _inputManager;

    public required InputManager InputManager
    {
        get => _inputManager;

        set
        {
            _inputManager = value;
            _inputManager.InputActionPressed += OnInputManagerActionPressed;
        }
    }

    public required IAudioPlayer AudioPlayer;

    public Player Player = new()
    {
        Transform = new()
        {
            Direction = Vector2.UnitX,
        },
    };

    public readonly SparseSet<Spawned<Bullet>> Bullets = [];

    private void UpdatePlayer(double deltaSeconds)
    {
        Vector2 moveVector = InputManager.MoveAxes;

        moveVector = moveVector.SafeNormalize();
        moveVector /= 1 + Convert.ToSingle(InputManager.IsActionPressed(InputAction.SpeedChange));
        Player.Velocity += moveVector;
        Player.Transform.Direction = (InputManager.MouseWorldPosition - Player.Transform.Position).SafeNormalize();
        Player.Transform.Position += Player.Velocity * (float)deltaSeconds;
        Player.Velocity = Player.Velocity.Lerp(Vector2.Zero, 4 * (float)deltaSeconds);

        Player.BulletCooldown = System.Math.Max(Player.BulletCooldown - deltaSeconds, 0);
    }

    private void UpdateBullets(double deltaSeconds)
    {
        foreach (ref var bullet in Bullets.Span)
        {
            bullet.Value.Value.Transform.Position += bullet.Value.Value.Velocity * (float)deltaSeconds;

            if (bullet.Value.Value.Transform.Position.LengthSquared() > 100)
            {
                bullet.Value.IsFlaggedForDeletion = true;
            }
        }

        Bullets.RemoveAll(bullet => bullet.Value.IsFlaggedForDeletion);
    }

    public void SpawnBullet(Bullet bullet)
    {
        Bullets.Add(bullet);
        AudioPlayer.PlaySound(SoundID.PlayerShoot);
    }

    public void Update(double deltaSeconds)
    {
        UpdatePlayer(deltaSeconds);
        UpdateBullets(deltaSeconds);
    }

    private void OnInputManagerActionPressed(InputAction inputAction)
    {
        if (inputAction == InputAction.AttackPrimary && Player.BulletCooldown <= 0)
        {
            SpawnBullet(new Bullet
            {
                Transform = Player.Transform with
                {
                    Position = Player.Transform.Position + Player.Transform.Direction,
                },
                Velocity = Player.Transform.Direction * 20,
            });

            Player.BulletCooldown = 1.0 / 8.0;
        }
    }
}
