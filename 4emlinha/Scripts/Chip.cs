using Godot;

public enum ChipType { EMPTY, RED, YELLOW, SWAP_COLOR }

public partial class Chip : Sprite2D
{
	public ChipType Type;
	public float PositionToPlace;
	public ColumnPlacer ColumnSpawned;

	private bool isPlaced = false;

	protected Chip(ChipType type)
	{
		this.Type = type;
	}

	protected Chip(ChipType type, float positionToPlace)
	{
		this.Type = type;
		this.PositionToPlace = positionToPlace;
	}

	public override void _Process(double delta)
	{
		if (isPlaced == true)
		{
			return;
		}

		GlobalPosition += new Vector2(0, 1000f * (float)delta);

		if (GlobalPosition.Y >= PositionToPlace)
		{
			GlobalPosition = new Vector2(GlobalPosition.X, PositionToPlace);
			isPlaced = true;

			ColumnSpawned.ReportChipPlaced();
		}
	}

	public void SwapSpriteToType(ChipType chipType)
	{
		if (chipType == ChipType.RED)
		{
			this.Texture = (Texture2D)ResourceLoader.Load("res://Assets/Circle_RED.png");
		}
		else
		{
			this.Texture = (Texture2D)ResourceLoader.Load("res://Assets/Circle_YELLOW.png");
		}
	}
}