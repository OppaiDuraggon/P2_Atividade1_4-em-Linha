using Godot;

public partial class RedChips : Chip
{
	public RedChips() : base(ChipType.RED)
	{
		this.Texture = (Texture2D)ResourceLoader.Load("res://Assets/Circle_RED.png");
	}

}
