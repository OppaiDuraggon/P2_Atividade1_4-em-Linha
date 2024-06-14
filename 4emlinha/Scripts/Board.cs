using Godot;

public enum BoardState { PLAYING, ANIMATING, RED_WINS, YELLOW_WINS, DRAW, GAME_END }

public struct GridCell
{
	public ChipType State;
	public int Column;
	public int Line;
}

public partial class Board : Node2D
{
	public BoardState boardState = BoardState.PLAYING;

	private GridCell[,] gameGrid = new GridCell[6, 7];
	private int chipsInLine = 0;
	private int chipsPlacedCounter = 0;

	private string playerPlays = "Player:";
	private string botPlays = "Bot:";
	private string winningSequence = "Winning Line:";

	public bool PlaceChip(ChipType chipToPlace, int X, int Y)
	{
		if (gameGrid[X, Y].State == ChipType.EMPTY)
		{
			gameGrid[X, Y].State = chipToPlace;
			gameGrid[X, Y].Column = X;
			gameGrid[X, Y].Line = Y;
			chipsPlacedCounter++;

			GridCell cellWhereChipWasPlaced = gameGrid[X, Y];
			//GD.Print($"{chipToPlace} Chip was Placed at ({X},{Y})"); //Check coordinates Debug.
			//GD.Print($"{cellWhereChipWasPlaced} CHECK ({cellWhereChipWasPlaced.Line},{cellWhereChipWasPlaced.Column})"); //Check coordinates Debug.

			/*if (Game.CurrentGame.WhoPlays.GetType() == typeof(Player)) { playerPlays += $"->({X},{Y})"; GD.Print(playerPlays)}
			else { botPlays += $"->({X},{Y})"; GD.Print(botPlays) }*/ //Check Placed Pieces by player and Bot

			EvaluateIfHasWon(cellWhereChipWasPlaced);
			return true;
		}
		return false;
	}

	public void SwapChipsTypeInColum(int column, int line)
	{
		int lineToCheck = line;

		while (lineToCheck - 1 >= 0)
		{
			lineToCheck--;

			if (gameGrid[column, lineToCheck].State == ChipType.RED)
			{
				gameGrid[column, lineToCheck].State = ChipType.YELLOW;
			}
			else if (gameGrid[column, lineToCheck].State == ChipType.YELLOW)
			{
				gameGrid[column, lineToCheck].State = ChipType.RED;
			}
			EvaluateIfHasWon(gameGrid[column, lineToCheck]);
		}
	}

	public void ResetBoard()
	{
		gameGrid = new GridCell[6, 7];
		chipsPlacedCounter = 0;
	}

	private void EvaluateIfHasWon(GridCell cellToEvaluate)
	{
		chipsInLine = 1;

		CheckHorizontalWin(cellToEvaluate);

		if (chipsInLine >= 4)
		{
			DeclareWinner();
			return;
		}
		else { winningSequence = "Winning Line:"; chipsInLine = 1; }

		CheckVerticalWin(cellToEvaluate);

		if (chipsInLine >= 4)
		{
			DeclareWinner();
			return;
		}
		else { winningSequence = "Winning Line:"; chipsInLine = 1; }

		// "\" -> Primary Diagonal -- "/" -> SecondaryDiagonal

		CheckPrimaryDiagonal(cellToEvaluate);

		if (chipsInLine >= 4)
		{
			DeclareWinner();
			return;
		}
		else { winningSequence = "Winning Line:"; chipsInLine = 1; }

		CheckSecondaryDiagonal(cellToEvaluate);

		if (chipsInLine >= 4)
		{
			DeclareWinner();
			return;
		}
		else { winningSequence = "Winning Line:"; chipsInLine = 1; }

		if (chipsPlacedCounter >= gameGrid.GetLength(0) * gameGrid.GetLength(1))
		{
			Game.CurrentGame.ChangeStateTo(BoardState.DRAW);
		}
	}

	private void DeclareWinner()
	{
		if (Game.CurrentGame.WhoPlays.Chips == ChipType.RED)
		{
			Game.CurrentGame.ChangeStateTo(BoardState.RED_WINS);
			GD.Print(winningSequence); //Check winning Line Debug.
		}
		else
		{
			Game.CurrentGame.ChangeStateTo(BoardState.YELLOW_WINS);
			GD.Print(winningSequence); //Check winning Line Debug.
		}
	}

	private void CheckHorizontalWin(GridCell cellPosition)
	{
		int columnToCheck = cellPosition.Column;
		winningSequence += $"->({columnToCheck},{cellPosition.Line})";

		//verifica na horizontal para a esquerda
		for (int i = 0; i < 3; i++)
		{
			if (columnToCheck - 1 < 0) { break; }

			columnToCheck--;
			if (gameGrid[columnToCheck, cellPosition.Line].State == cellPosition.State)
			{
				winningSequence += $"->({columnToCheck},{cellPosition.Line})";
				chipsInLine++;
			}
			else { break; }
		}

		columnToCheck = cellPosition.Column;

		//verifica na horizontal para a direita
		for (int i = 0; i < 3; i++)
		{
			if (columnToCheck + 1 >= gameGrid.GetLength(0)) { break; }

			columnToCheck++;
			if (gameGrid[columnToCheck, cellPosition.Line].State == cellPosition.State)
			{
				winningSequence += $"->({columnToCheck},{cellPosition.Line})";
				chipsInLine++;
			}
			else { break; }
		}
	}

	private void CheckVerticalWin(GridCell cellPosition)
	{
		int lineToCheck = cellPosition.Line;
		winningSequence += $"->({cellPosition.Column},{lineToCheck})";

		//verifica na vertical para baixo
		for (int i = 0; i < 3; i++)
		{
			if (lineToCheck - 1 < 0) { break; }

			lineToCheck--;
			if (gameGrid[cellPosition.Column, lineToCheck].State == cellPosition.State)
			{
				winningSequence += $"->({cellPosition.Column},{lineToCheck})";
				chipsInLine++;
			}
			else { break; }
		}

		lineToCheck = cellPosition.Line;

		//verifica na vertical para cima
		for (int i = 0; i < 3; i++)
		{
			if (lineToCheck + 1 >= gameGrid.GetLength(1)) { break; }

			lineToCheck++;
			if (gameGrid[cellPosition.Column, lineToCheck].State == cellPosition.State)
			{
				winningSequence += $"->({cellPosition.Column},{lineToCheck})";
				chipsInLine++;
			}
			else { break; }
		}
	}

	private void CheckPrimaryDiagonal(GridCell cellPosition)
	{
		int columnToCheck = cellPosition.Column;
		int lineToCheck = cellPosition.Line;
		winningSequence += $"->({columnToCheck},{lineToCheck})";

		// Check Primary Diagonal down from position
		for (int i = 0; i < 3; i++)
		{
			if (columnToCheck + 1 >= gameGrid.GetLength(0) || lineToCheck - 1 < 0) { break; }

			columnToCheck++;
			lineToCheck--;
			if (gameGrid[columnToCheck, lineToCheck].State == cellPosition.State)
			{
				winningSequence += $"->({columnToCheck},{lineToCheck})";
				chipsInLine++;
			}
			else { break; }
		}

		columnToCheck = cellPosition.Column;
		lineToCheck = cellPosition.Line;

		// Check Primary Diagonal up from position
		for (int i = 0; i < 3; i++)
		{
			if (columnToCheck - 1 < 0 || lineToCheck + 1 >= gameGrid.GetLength(1)) { break; }

			columnToCheck--;
			lineToCheck++;
			if (gameGrid[columnToCheck, lineToCheck].State == cellPosition.State)
			{
				winningSequence += $"->({columnToCheck},{lineToCheck})";
				chipsInLine++;
			}
			else { break; }
		}
	}

	private void CheckSecondaryDiagonal(GridCell cellPosition)
	{
		int columnToCheck = cellPosition.Column;
		int lineToCheck = cellPosition.Line;
		winningSequence += $"->({columnToCheck},{lineToCheck})";

		// Check Secondary Diagonal down from position
		for (int i = 0; i < 3; i++)
		{
			if (columnToCheck - 1 < 0 || lineToCheck - 1 < 0) { break; }

			columnToCheck--;
			lineToCheck--;
			if (gameGrid[columnToCheck, lineToCheck].State == cellPosition.State)
			{
				winningSequence += $"->({columnToCheck},{lineToCheck})";
				chipsInLine++;
			}
			else { break; }
		}

		columnToCheck = cellPosition.Column;
		lineToCheck = cellPosition.Line;

		// Check Secondary Diagonal up from position
		for (int i = 0; i < 3; i++)
		{
			if (columnToCheck + 1 >= gameGrid.GetLength(0) || lineToCheck + 1 >= gameGrid.GetLength(1)) { break; }

			columnToCheck++;
			lineToCheck++;
			if (gameGrid[columnToCheck, lineToCheck].State == cellPosition.State)
			{
				winningSequence += $"->({columnToCheck},{lineToCheck})";
				chipsInLine++;
			}
			else { break; }
		}
	}
}