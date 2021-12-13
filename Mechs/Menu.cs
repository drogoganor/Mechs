using ImGuiNET;
using Mechs.SampleBase;
using Veldrid;

namespace Mechs
{
    public abstract class Menu : IGameScreen
    {
        public IApplicationWindow Window { get; }
        public GraphicsDevice? GraphicsDevice { get; private set; }
        public ResourceFactory? ResourceFactory { get; private set; }
        public Swapchain? MainSwapchain { get; private set; }

        private ImGuiRenderer? imGuiRenderer;
        private CommandList? commandList;
        private bool isShown;
        private InputSnapshot? inputSnapshot;

        protected virtual string GetTitle() => GetType().Name;
        protected ImFontPtr? font;

        public Menu(IApplicationWindow window)
        {
            Window = window;
            Window.Resized += HandleWindowResize;
            Window.GraphicsDeviceCreated += OnGraphicsDeviceCreated;
            Window.GraphicsDeviceDestroyed += OnDeviceDestroyed;
        }

        public void OnGraphicsDeviceCreated(GraphicsDevice gd, ResourceFactory factory, Swapchain sc)
        {
            GraphicsDevice = gd;
            ResourceFactory = factory;
            MainSwapchain = sc;

            commandList = gd.ResourceFactory.CreateCommandList();
            imGuiRenderer = new ImGuiRenderer(
                gd,
                gd.MainSwapchain.Framebuffer.OutputDescription,
                (int)Window.Width,
                (int)Window.Height);

            font = ImGui.GetIO().Fonts.AddFontFromFileTTF(@"C:\Windows\Fonts\ARIAL.TTF", 26);
            imGuiRenderer.RecreateFontDeviceTexture();
        }

        public void Show()
        {
            isShown = true;
            Window.Rendering += PreDraw;
            Window.Rendering += Draw;
        }

        public void Hide()
        {
            isShown = false;
            Window.Rendering -= PreDraw;
            Window.Rendering -= Draw;
        }

        protected virtual void OnDeviceDestroyed()
        {
            Hide();
            Window.Resized -= HandleWindowResize;
            Window.GraphicsDeviceCreated -= OnGraphicsDeviceCreated;
            Window.GraphicsDeviceDestroyed -= OnDeviceDestroyed;

            GraphicsDevice = null;
            ResourceFactory = null;
            MainSwapchain = null;

            imGuiRenderer = null;
            commandList = null;
            font = null;
        }

        private void PreDraw(float deltaSeconds)
        {
            if (imGuiRenderer == null) return;

            inputSnapshot = InputTracker.FrameSnapshot;
            imGuiRenderer.Update(1f / 60f, inputSnapshot);
        }

        protected virtual void Draw(float deltaSeconds)
        {
            if (imGuiRenderer == null) return;
            if (commandList == null) return;
            if (GraphicsDevice == null) return;

            if (isShown)
            {
                commandList.Begin();
                commandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
                commandList.ClearColorTarget(0, new RgbaFloat(0, 0, 0.2f, 1f));
                imGuiRenderer.Render(GraphicsDevice, commandList);
                commandList.End();
                GraphicsDevice.SubmitCommands(commandList);
                GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
            }
        }

        protected virtual void HandleWindowResize()
        {
            if (imGuiRenderer == null) return;

            imGuiRenderer.WindowResized((int)Window.Width, (int)Window.Height);
        }

        protected static void HorizontallyCenteredText(string text, float width)
        {
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((width - textWidth) * 0.5f);
            ImGui.Text(text);
        }
    }
}
