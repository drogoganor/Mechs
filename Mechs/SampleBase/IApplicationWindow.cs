using Veldrid;

namespace Mechs.SampleBase
{
    public interface IApplicationWindow
    {
        SamplePlatformType PlatformType { get; }

        event Action<float> Rendering;
        event Action<GraphicsDevice, ResourceFactory, Swapchain> GraphicsDeviceCreated;
        event Action GraphicsDeviceDestroyed;
        event Action Resized;
        event Action<KeyEvent> KeyPressed;

        uint Width { get; }
        uint Height { get; }

        void Run();
        void Close();
    }
}
