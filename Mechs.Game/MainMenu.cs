using ImGuiNET;
using SampleBase;
using System.Numerics;

namespace Mechs.Game
{
    public class MainMenu : Menu
    {
        public event Action OnNewGame;

        public MainMenu(IApplicationWindow window) : base(window)
        {
        }

        private void NewGame()
        {
            Hide();
            OnNewGame?.Invoke();
        }

        private void ExitGame()
        {
            Hide();
            Window.Close();
        }

        protected override void Draw(float deltaSeconds)
        {
            var windowSize = new Vector2(Window.Width, Window.Height);
            var menuSize = new Vector2(400, 600);
            var menuPadding = 40f;
            var buttonSize = new Vector2(menuSize.X - menuPadding, 32);
            ImGui.SetNextWindowSize(menuSize);

            var menuPos = (windowSize - menuSize) / 2;
            ImGui.SetNextWindowPos(menuPos);
            ImGui.PushFont(font.Value);

            if (ImGui.Begin("Main Menu",
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoBackground |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoResize))
            {
                HorizontallyCenteredText("Mechs", menuSize.X);

                ImGui.SetCursorPosX(menuPadding / 2f);
                if (ImGui.Button("New Game", buttonSize))
                {
                    NewGame();
                }

                ImGui.SetCursorPosX(menuPadding / 2f);
                if (ImGui.Button("Quit", buttonSize))
                {
                    ExitGame();
                }
            }

            base.Draw(deltaSeconds);
        }
    }
}
