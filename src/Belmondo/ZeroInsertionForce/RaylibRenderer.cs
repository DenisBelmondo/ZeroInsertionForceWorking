using System.ComponentModel;
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
    private const string VERTEX_SHADER_GL33_SRC =
    """
        #version 330 core

        in vec3 vertexPosition;
        in vec2 vertexTexCoord;
        in vec4 vertexColor;

        uniform mat4 mvp;
        uniform float zIndex;

        out vec2 fragTexCoord;
        out vec4 fragColor;
        out float fragZIndex;

        void main() {
            fragTexCoord = vertexTexCoord;
            fragColor = vertexColor;
            fragZIndex = zIndex;
            gl_Position = mvp * vec4(vertexPosition, 1.0);
        }
    """;

    private const string FRAGMENT_SHADER_GL33_SRC =
    """
        #version 330 core

        in vec2 fragTexCoord;
        in vec4 fragColor;

        uniform sampler2D texture0;
        uniform vec4 colDiffuse;
        uniform sampler2D depthTexture;

        out vec4 finalColor;

        void main() {
            vec4 texelColor = texture(texture0, fragTexCoord);
            finalColor = texelColor * colDiffuse * fragColor;
        }
    """;

    private const string DEPTH_PASS_FRAGMENT_SHADER_GL33_SRC =
    """
        #version 330 core

        in vec2 fragTexCoord;
        in vec4 fragColor;
        in float fragZIndex;

        uniform sampler2D texture0;

        out vec4 finalColor;

        void main() {
            vec4 texelColor = texture(texture0, fragTexCoord);

            finalColor.rgb = vec3(float(fragZIndex));
            finalColor.a = mix(0.0, 1.0, texelColor.a > 0.001);
        }
    """;

    private static Shader _depthPassShader;
    private static Shader _shader;
    private static Texture2D _corinneTexture;
    private static Texture2D _bulletTexture;

    private static void LoadResources()
    {
        _depthPassShader = LoadShaderFromMemory(VERTEX_SHADER_GL33_SRC, DEPTH_PASS_FRAGMENT_SHADER_GL33_SRC);
        _shader = LoadShaderFromMemory(VERTEX_SHADER_GL33_SRC, FRAGMENT_SHADER_GL33_SRC);
        _corinneTexture = LoadTexture("static/textures/corinne.png");
        _bulletTexture = LoadTexture("static/textures/bullets.png");
    }

    private static void UnloadResources()
    {
        UnloadShader(_depthPassShader);
        UnloadShader(_shader);
        UnloadTexture(_corinneTexture);
        UnloadTexture(_bulletTexture);
    }
}

partial class RaylibRenderer
{
    private const int SCREEN_WIDTH = 640;
    private const int SCREEN_HEIGHT = 480;

    private RenderTexture2D _depthBuffer;
    private RenderTexture2D _backBufferTexture;
    private Camera2D _camera = new(Vector2.Zero, Vector2.Zero, 0, 64);

    public Vector2 GetMouseWorldPosition() => GetScreenToWorld2D(GetMousePosition(), _camera);

    public void Initialize()
    {
        InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Zero Insertion Force");

        (int screenWidth, int screenHeight) = (GetScreenWidth(), GetScreenHeight());
        (int halfScreenWidth, int halfScreenHeight) = (screenWidth / 2, screenHeight / 2);

        _depthBuffer = LoadRenderTexture(screenWidth, screenHeight);
        _backBufferTexture = LoadRenderTexture(screenWidth, screenHeight);
        LoadResources();
        _camera.Offset = new(halfScreenWidth, halfScreenHeight);
    }

    public void Uninitialize()
    {
        UnloadRenderTexture(_depthBuffer);
        UnloadRenderTexture(_backBufferTexture);
        UnloadResources();
        CloseWindow();
    }
}

partial class RaylibRenderer : IRenderer<Game>
{
    private float _currentPlayerFrame;

    private void RenderDepthPass(in World world, double deltaSeconds)
    {
        BeginTextureMode(_depthBuffer);
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

        SetShaderValue(_depthPassShader, GetShaderLocation(_depthPassShader, "zIndex"), 0.5F, ShaderUniformDataType.Float);

        BeginShaderMode(_depthPassShader);
        DrawTexturePro(
            _corinneTexture,
            new((int)_currentPlayerFrame * 32,
            angleFrame * 32, 32 * MathF.Sign(directionRadians), 32),
            new(player.Transform.Position - Vector2.One / (32 / 16.0F), 1, 1),
            Vector2.Zero,
            0,
            Color.White);
        EndShaderMode();

        SetShaderValue(_depthPassShader, GetShaderLocation(_depthPassShader, "zIndex"), 0.4F, ShaderUniformDataType.Float);

        BeginShaderMode(_depthPassShader);

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

        EndShaderMode();
        EndMode2D();
        EndTextureMode();
    }

    private void RenderNormalPass(in World world, double deltaSeconds)
    {
        BeginTextureMode(_backBufferTexture);
        ClearBackground(Color.Black);
        BeginMode2D(_camera);
        SetShaderValue(_shader, GetShaderLocation(_shader, "depthTexture"), _depthBuffer.Texture, ShaderUniformDataType.Sampler2D);
        BeginShaderMode(_shader);

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
            RenderDepthPass(game.World, deltaSeconds);
            // RenderNormalPass(game.World, deltaSeconds);
        }

        BeginDrawing();
        DrawTextureRec(
            _depthBuffer.Texture,
            new Rectangle(0, 0, _depthBuffer.Texture.Width, -_depthBuffer.Texture.Height),
            Vector2.Zero,
            Color.White);
        EndDrawing();
    }
}

partial class RaylibRenderer : IDisposable
{
    private bool _hasBeenDisposed;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool _)
    {
        if (!_hasBeenDisposed)
        { /*
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            } */

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            Uninitialize();
            _hasBeenDisposed = true;
        }
    }

    ~RaylibRenderer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(false);
    }
}
