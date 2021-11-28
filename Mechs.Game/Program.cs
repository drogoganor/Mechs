using SampleBase;

namespace Mechs.Game
{
    class Program
    {
        public static void Main(string[] args)
        {
            var window = new VeldridStartupWindow("Mechs");
            var mainMenu = new MainMenu(window);
            var game = new TexturedCube(window);

            mainMenu.Show();
            mainMenu.OnNewGame += () =>
            {
                mainMenu.Hide();
                game.Show();
                game.OnEndGame += () =>
                {
                    game.Hide();
                    mainMenu.Show();
                };
            };

            window.Run();
        }
    }
}
