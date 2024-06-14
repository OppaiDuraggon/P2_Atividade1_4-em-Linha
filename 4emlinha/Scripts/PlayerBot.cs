using Godot;
using System;

public partial class PlayerBot : Player
{
	public PlayerBot(string namePlayer, ChipType chipsType) : base(namePlayer, chipsType)
	{

	}

	public override bool HasNotPlayed()
	{
		Game.CurrentGame.PickRandomColumnToPlay();
		return IsPlaying;
	}

}
