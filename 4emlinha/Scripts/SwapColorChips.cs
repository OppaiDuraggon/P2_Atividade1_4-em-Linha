using Godot;

public partial class SwapColorChips : Chip
{
	public SwapColorChips() : base(ChipType.SWAP_COLOR)
	{
		this.Texture = (Texture2D)ResourceLoader.Load("res://Assets/Circle_White.png");
	}

}
