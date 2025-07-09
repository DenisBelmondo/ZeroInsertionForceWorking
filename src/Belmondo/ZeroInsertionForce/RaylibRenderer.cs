using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Belmondo.ZeroInsertionForce;

public sealed partial class RaylibRenderer;

partial class RaylibRenderer
{
    public class InitializationException : Exception
    {
        public InitializationException()
        {
        }

        public InitializationException(string? message) : base(message)
        {
        }

        public InitializationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}

partial class RaylibRenderer
{
    private static Texture2D _corinneTexture;
    private static Texture2D _bulletTexture;

    private static void LoadResources()
    {
        _corinneTexture = LoadTexture("static/textures/corinne.png");
        _bulletTexture = LoadTexture("static/textures/bullets.png");
    }

    private static void UnloadResources()
    {
        UnloadTexture(_corinneTexture);
        UnloadTexture(_bulletTexture);
    }
}

partial class RaylibRenderer
{
    private const int DEFAULT_SCREEN_WIDTH = 640;
    private const int DEFAULT_SCREEN_HEIGHT = 480;

    private RenderTexture2D _backBufferTexture;
    private int _currentScreenWidth;
    private int _currentScreenHeight;
    private int _halfScreenWidth;
    private int _halfScreenHeight;
    private Camera2D _camera = new(Vector2.Zero, Vector2.Zero, 0, 64);

    public Vector2 GetMouseWorldPosition() => GetScreenToWorld2D(GetMousePosition(), _camera);

    public void UpdateScreenSize()
    {
        (_currentScreenWidth, _currentScreenHeight) = (GetScreenWidth(), GetScreenHeight());
        (_halfScreenWidth, _halfScreenHeight) = (_currentScreenWidth / 2, _currentScreenHeight / 2);
    }

    public void Initialize()
    {
        SetConfigFlags(ConfigFlags.ResizableWindow);
        InitWindow(DEFAULT_SCREEN_WIDTH, DEFAULT_SCREEN_HEIGHT, "Zero Insertion Force");

        UpdateScreenSize();
        _backBufferTexture = LoadRenderTexture(_currentScreenWidth, _currentScreenHeight);
        _camera.Offset = new(_halfScreenWidth, _halfScreenHeight);
        LoadResources();
    }

    public void Uninitialize()
    {
        UnloadRenderTexture(_backBufferTexture);
        UnloadResources();
        CloseWindow();
    }
}

partial class RaylibRenderer : IRenderer<Game>
{
    private float _currentPlayerFrame;

    private void RenderNormalPass(in World world, double deltaSeconds)
    {
        UpdateScreenSize();
        BeginTextureMode(_backBufferTexture);
        ClearBackground(Color.Black);
        BeginMode2D(_camera);

        var player = world.Player;
        var directionRadians = player.Transform.Direction.AngleTo(Vector2.UnitY);
        var angleFrame = directionRadians * 180F / MathF.PI;

        angleFrame = Math.Stepify(angleFrame + 45, 90);
        angleFrame /= 90;
        angleFrame = System.Math.Abs(angleFrame);

        _currentPlayerFrame += (float)deltaSeconds * 10;
        _currentPlayerFrame %= 3;

        DrawTexturePro(
            _corinneTexture,
            new((int)_currentPlayerFrame * 32,
            angleFrame * 32, 32 * MathF.Sign(directionRadians), 32),
            new(player.Transform.Position - Vector2.One / (32 / 16.0F), 1, 1),
            Vector2.Zero,
            0,
            Color.White);

        DrawLineV(player.Transform.Position, player.Transform.Position + player.Transform.Direction, Color.Red);

        // TODO: maybe instance? raylib takes care of batching automatically
        // so this isn't so bad.
        foreach (ref var bullet in world.Bullets.Span)
        {
            DrawTexturePro(
                _bulletTexture,
                new(0, 0, 8, 8),
                new(bullet.Value.Value.Transform.Position - Vector2.One / (32 / 8.0F), 0.25F, 0.25F),
                Vector2.Zero,
                0,
                Color.White);
        }

        // EndShaderMode();
        EndMode2D();
        EndTextureMode();
    }

    public void Render(in Game game, double deltaSeconds)
    {
        if (game.World is not null)
        {
            RenderNormalPass(game.World, deltaSeconds);
        }

        BeginDrawing();
        DrawTexturePro(
            _backBufferTexture.Texture,
            new Rectangle(0, 0, _backBufferTexture.Texture.Width, -_backBufferTexture.Texture.Height),
            new Rectangle(0, 0, GetScreenWidth(), GetScreenHeight()),
            Vector2.Zero,
            0,
            Color.White);
        EndDrawing();
    }
}

partial class RaylibRenderer : IDisposable
{
    private bool _wasDisposed;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool _)
    {
        if (!_wasDisposed)
        { /*
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            } */

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            Uninitialize();
            _wasDisposed = true;
        }
    }

    ~RaylibRenderer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(false);
    }
}
