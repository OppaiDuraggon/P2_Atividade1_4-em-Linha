using Godot;


public partial class Player : Node2D
{
	public readonly string PlayerName;
	public ChipType Chips;

	private bool isPlaying;

	public bool IsPlaying { get => isPlaying; set => isPlaying = value; }

	public Player(string playerName, ChipType chipType)
	{
		this.PlayerName = playerName;
		this.Chips = chipType;
	}

	public virtual bool HasNotPlayed() { return isPlaying; }

}
