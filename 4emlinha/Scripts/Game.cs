using Godot;
using System;
using System.Linq;

public partial class Game : Node2D
{
	public static Game CurrentGame;

	private Label gameInfo;
	private Board board;
	private ColumnPlacer[] gameColumnsArray = new ColumnPlacer[6];
	private AudioStreamPlayer audioWin, audioDraw, audioLose;
	private Player Player1, Player2, whoPlays;

	public Player WhoPlays { get => whoPlays; }
	public BoardState CurrentGameState { get => board.boardState; }

	public override void _Ready()
	{
		if (CurrentGame == null)
		{
			CurrentGame = this;
		}
		this.gameInfo = (Label)GetNode("LblInfo");
		this.board = GetNode<Board>("Board");
		this.gameColumnsArray = GetNode("Control/Colums").GetChildren().OfType<ColumnPlacer>().ToArray(); //Buscar os filhos de "colums" do tipo ColumnPlacer e cria um Array.
		/*foreach (var colum in gameColumnsArray)
		{
			GD.Print(colum.Name); //Verificar ordem de colunas.
		}*/
		this.audioWin = GetNode<AudioStreamPlayer>("AudioWin");
		this.audioDraw = GetNode<AudioStreamPlayer>("AudioDraw");
		this.audioLose = GetNode<AudioStreamPlayer>("AudioLose");
		this.Player1 = new Player("Player 1", ChipType.RED);
		this.Player2 = new PlayerBot("PC", ChipType.YELLOW);
		this.AddChild(this.Player1);
		this.AddChild(this.Player2);

		this.whoPlays = this.Player1;
		this.whoPlays.IsPlaying = true;

		this.gameInfo.Text = this.whoPlays.PlayerName + " Playing...";
	}

	public override void _Process(double delta)
	{
		//GD.Print($"{whoPlays.PlayerName} Is playing = ({whoPlays.IsPlaying})");//Check if who is playing is updating correctly Debug.

		if (this.board.boardState == BoardState.GAME_END || (this.board.boardState == BoardState.PLAYING && whoPlays.HasNotPlayed()))
		{
			return;
		}
		//Announce Winner
		else if (this.board.boardState == BoardState.RED_WINS)
		{
			this.gameInfo.Text = whoPlays.PlayerName + " WINS...";
			if (Player1.Chips == ChipType.RED)
			{
				this.audioWin.Play();
			}
			else
			{
				this.audioLose.Play();
			}

			CurrentGame.ChangeStateTo(BoardState.GAME_END);
		}
		else if (this.board.boardState == BoardState.YELLOW_WINS)
		{
			this.gameInfo.Text = whoPlays.PlayerName + " WINS...";
			if (Player1.Chips == ChipType.YELLOW)
			{
				this.audioWin.Play();
			}
			else
			{
				this.audioLose.Play();
			}

			CurrentGame.ChangeStateTo(BoardState.GAME_END);
		}
		else if (this.board.boardState == BoardState.DRAW)
		{
			this.gameInfo.Text = "THE GAME WAS A DRAW...";
			this.audioDraw.Play();
			CurrentGame.ChangeStateTo(BoardState.GAME_END);
		}
	}

	//A bot playing will use this method to play.
	public void PickRandomColumnToPlay()
	{
		int randomIndexToPick = 0;

		for (int i = 0; i < 1000; i++)
		{
			Random random = new Random();
			randomIndexToPick = random.Next(0, gameColumnsArray.Length);

			if (!gameColumnsArray[randomIndexToPick].IsFull) { break; }
		}
		gameColumnsArray[randomIndexToPick].PlayChip();
	}

	// Ends player turn and place chip on board.
	public void PlaceChipAtPosition(ColumnPlacer chipSpawner, int Y, bool wasSpecialChip)
	{
		//Using C# array class to retrieve the corresponding column
		int columnIndex = Array.IndexOf(gameColumnsArray, chipSpawner);

		if (wasSpecialChip)
		{
			this.board.SwapChipsTypeInColum(columnIndex, Y);
		}

		this.board.PlaceChip(this.whoPlays.Chips, columnIndex, Y);

		if (board.boardState != BoardState.PLAYING) { return; }

		whoPlays.IsPlaying = false;
		if (whoPlays == this.Player1)
		{
			whoPlays = this.Player2;
			whoPlays.IsPlaying = true;
		}
		else
		{
			whoPlays = this.Player1;
			whoPlays.IsPlaying = true;
		}
		this.gameInfo.Text = this.whoPlays.PlayerName + " PLAYING";
	}

	public void ChangeStateTo(BoardState newState)
	{
		this.board.boardState = newState;
	}

	public void ResetGame()
	{
		this.board.ResetBoard();
		foreach (ColumnPlacer column in gameColumnsArray)
		{
			column.ResetColumn();
		}

		this.whoPlays = this.Player1;
		this.Player2.IsPlaying = false;
		this.whoPlays.IsPlaying = true;

		this.gameInfo.Text = this.whoPlays.PlayerName + " PLAYING";

		CurrentGame.ChangeStateTo(BoardState.PLAYING);
	}
}
