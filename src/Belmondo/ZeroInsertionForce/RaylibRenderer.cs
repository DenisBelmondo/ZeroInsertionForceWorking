using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Belmondo.ZeroInsertionForce;

public partial class RaylibRenderer
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

public partial class RaylibRenderer
{
    private const string FULL_SCREEN_VERTEX_SHADER_GL33_SRC =
    """
        #version 330 core

        layout (location = 0) in vec3 vertexPosition;
        layout (location = 1) in vec2 vertexTexCoord;

        out vec2 fragTexCoord;

        void main() {
            gl_Position = vec4(vertexPosition, 1.0);
            fragTexCoord = vertexTexCoord;
        }
    """;

    private const string FRAGMENT_SHADER_GL33_SRC =
    """
        #version 330 core

        uniform sampler2D depthTexture;
        uniform sampler2D albedoTexture;

        out vec4 finalColor;

        void main() {
            finalColor = vec4(1.0);
        }
    """;

    private Camera2D _camera = new(Vector2.Zero, Vector2.Zero, 0, 64);

    public Vector2 GetMouseWorldPosition() => GetScreenToWorld2D(GetMousePosition(), _camera);

    public void Initialize()
    {
        InitWindow(640, 480, "Zero Insertion Force");
        LoadResources();

        (int halfScreenWidth, int halfScreenHeight) = (GetScreenWidth() / 2, GetScreenHeight() / 2);

        _camera.Offset = new(halfScreenWidth, halfScreenHeight);
    }

    public static void Uninitialize()
    {
        UnloadResources();
        CloseWindow();
    }
}

public partial class RaylibRenderer : IRenderer<Game>
{
    private struct SuperBuffer
    {
        public uint FrameBuffer;
        public uint DepthRenderBuffer;
    }

    private float _currentPlayerFrame;

    private void RenderWorld(in World world, double deltaSeconds)
    {
        var player = world.Player;
        var directionRadians = player.Transform.Direction.AngleTo(Vector2.UnitY);
        var angleFrame = directionRadians * 180F / MathF.PI;

        angleFrame = Math.Stepify(angleFrame + 45, 90);
        angleFrame /= 90;
        angleFrame = System.Math.Abs(angleFrame);

        _currentPlayerFrame += (float)deltaSeconds * 10;
        _currentPlayerFrame %= 3;

        DrawTexturePro(_corinneTexture, new((int)_currentPlayerFrame * 32, angleFrame * 32, 32 * MathF.Sign(directionRadians), 32), new(player.Transform.Position - Vector2.One / (32 / 16.0F), 1, 1), Vector2.Zero, 0, Color.White);
        DrawLineV(player.Transform.Position, player.Transform.Position + player.Transform.Direction, Color.Red);

        // TODO: maybe instance? raylib takes care of batching automatically
        // so this isn't so bad.
        foreach (ref var bullet in world.Bullets.Span)
        {
            DrawTexturePro(_bulletTexture, new(0, 0, 8, 8), new(bullet.Value.Value.Transform.Position - Vector2.One / (32 / 8.0F), 0.25F, 0.25F), Vector2.Zero, 0, Color.White);
        }
    }

    public void Render(in Game game, double deltaSeconds)
    {
        BeginDrawing();
        ClearBackground(Color.Black);
        BeginMode2D(_camera);

        if (game.World is not null)
        {
            RenderWorld(game.World, deltaSeconds);
        }

        EndMode2D();
        EndDrawing();
    }
}

public partial class RaylibRenderer : IDisposable
{
    private bool _hasBeenDisposed;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_hasBeenDisposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            Uninitialize();
            _hasBeenDisposed = true;
        }
    }

    ~RaylibRenderer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }
}
