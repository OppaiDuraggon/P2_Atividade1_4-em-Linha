using Godot;

public partial class RestartButton : Button
{
	public override void _Ready()
	{
		this.Pressed += RestartButtonPressed;
	}

	private void RestartButtonPressed()
	{
		Game.CurrentGame.ResetGame();
	}
}
