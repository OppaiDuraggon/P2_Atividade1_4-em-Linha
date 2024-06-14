using Godot;

public partial class SpecialAbilityButton : Button
{
	public bool WasAbilityUsed = false;
	private ChipType previousType;

	public ChipType PreviousType { get => previousType; }

	public override void _Ready()
	{
		this.Pressed += SpecialAbilityButtonPressed;
	}

	private void SpecialAbilityButtonPressed()
	{
		if (WasAbilityUsed == true || Game.CurrentGame.WhoPlays.GetType() == typeof(PlayerBot)) { return; }

		WasAbilityUsed = true;
		previousType = Game.CurrentGame.WhoPlays.Chips;
		Game.CurrentGame.WhoPlays.Chips = ChipType.SWAP_COLOR;
	}
}