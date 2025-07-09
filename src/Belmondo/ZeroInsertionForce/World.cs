using System.Numerics;

namespace Belmondo.ZeroInsertionForce;

public sealed partial class World : IResettable
{
    public struct Spawned<T>(T value)
    {
        public T Value = value;
        public bool IsFlaggedForDeletion;

        public static implicit operator T(Spawned<T> spawned) => spawned.Value;
        public static implicit operator Spawned<T>(T value) => new(value);
    }

    private const float MAX_X = 6;
    private const float MAX_Y = 4;
    private const float MAX_RADIUS = 100;
    private const float MAX_RADIUS_SQUARED = MAX_RADIUS * MAX_RADIUS;
    private const double PLAYER_COOLDOWN = 1.0 / 8.0;
    private const int PLAYER_MOVE_SPEED = 20;

    private InputManager? _inputManager;

    public required InputManager InputManager
    {
        get => _inputManager!;

        set
        {
            _inputManager = value;
            _inputManager.InputActionPressed += OnInputManagerActionPressed;
        }
    }

    public required IAudioPlayer AudioPlayer;

    public Player Player;
    public SparseSet<Spawned<Bullet>> Bullets = [];

    public void Reset()
    {
        Player = new();
        Bullets = [];
    }
}

partial class World
{
    private void UpdatePlayer(double deltaSeconds)
    {
        Vector2 moveVector = InputManager.MoveAxes;

        moveVector = moveVector.SafeNormalize();
        moveVector /= 1 + Convert.ToSingle(InputManager.IsActionPressed(InputActionType.SpeedChange));
        Player.Velocity += moveVector;
        Player.Transform.Direction = (InputManager.MouseWorldPosition - Player.Transform.Position).SafeNormalize();
        Player.Transform.Position += Player.Velocity * (float)deltaSeconds;

        if (Player.Transform.Position.X < -MAX_X)
        {
            Player.Transform.Position.X = MAX_X;
        }

        if (Player.Transform.Position.X > MAX_X)
        {
            Player.Transform.Position.X = -MAX_X;
        }

        if (Player.Transform.Position.Y < -MAX_Y)
        {
            Player.Transform.Position.Y = MAX_Y;
        }

        if (Player.Transform.Position.Y > MAX_Y)
        {
            Player.Transform.Position.Y = -MAX_Y;
        }

        Player.Velocity = Player.Velocity.Lerp(Vector2.Zero, 4 * (float)deltaSeconds);
        Player.BulletCooldown = System.Math.Max(Player.BulletCooldown - deltaSeconds, 0);
    }

    private void UpdateBullets(double deltaSeconds)
    {
        foreach (ref var bullet in Bullets.Span)
        {
            bullet.Value.Value.Transform.Position += bullet.Value.Value.Velocity * (float)deltaSeconds;

            if (bullet.Value.Value.Transform.Position.LengthSquared() > MAX_RADIUS_SQUARED)
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

    private void OnInputManagerActionPressed(InputActionType inputAction)
    {
        if (inputAction == InputActionType.AttackPrimary && Player.BulletCooldown <= 0)
        {
            SpawnBullet(new Bullet
            {
                Transform = Player.Transform with
                {
                    Position = Player.Transform.Position + Player.Transform.Direction,
                },
                Velocity = Player.Transform.Direction * PLAYER_MOVE_SPEED,
            });

            Player.BulletCooldown = PLAYER_COOLDOWN;
        }
    }
}
