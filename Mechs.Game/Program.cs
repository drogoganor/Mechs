using Mechs;
using Mechs.SampleBase;

var window = new VeldridStartupWindow("Mechs");
var mainMenu = new MainMenu(window);
var game = new Game(window);

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
