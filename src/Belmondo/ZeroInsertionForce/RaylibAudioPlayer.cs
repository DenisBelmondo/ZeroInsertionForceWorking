using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Belmondo.ZeroInsertionForce;

public sealed partial class RaylibAudioPlayer : IAudioPlayer
{
    private static readonly Sound[] _sounds = new Sound[Enum.GetValues<SoundID>().Length];

    private static void UnloadResources()
    {
        foreach (var sound in _sounds)
        {
            UnloadSound(sound);
        }
    }

    private static void LoadResources()
    {
        _sounds[(int)SoundID.PlayerShoot] = LoadSound("static/audio/sounds/player_shoot.wav");
    }

    public static void Initialize()
    {
        InitAudioDevice();
        LoadResources();
    }

    public void PlaySound(SoundID soundID)
    {
        Raylib.PlaySound(_sounds[(int)soundID]);
    }
}

partial class RaylibAudioPlayer : IDisposable
{
    private bool _wasDisposed;

    private void Dispose(bool _)
    {
        if (!_wasDisposed)
        { /*
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            } */

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null?

            UnloadResources();
            CloseAudioDevice();
            _wasDisposed = true;
        }
    }

    // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~RaylibAudioPlayer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
