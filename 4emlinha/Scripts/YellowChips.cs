using Godot;
using System;

public partial class YellowChips : Chip
{
	public YellowChips() : base(ChipType.YELLOW)
	{
		this.Texture = (Texture2D)ResourceLoader.Load("res://Assets/Circle_YELLOW.png");
	}
}
