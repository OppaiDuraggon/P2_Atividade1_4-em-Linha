using Godot;

public partial class ColumnPlacer : Node2D
{
    private PackedScene whiteButtonScene = GD.Load<PackedScene>("res://Scenes/WHITEBUTTON.tscn");
    private PackedScene redChipsScene = GD.Load<PackedScene>("res://Scenes/REDCHIP.tscn");
    private PackedScene yellowChipsScene = GD.Load<PackedScene>("res://Scenes/YELLOWCHIP.tscn");
    private PackedScene swapColorChipsScene = GD.Load<PackedScene>("res://Scenes/SWAPCOLORCHIP.tscn");
    private int[] linesInColumn = { 525, 460, 390, 325, 258, 190, 125 };
    private int currentFreeIndex;
    private Chip[] instantiatedChipArray;
    private SpecialAbilityButton abilityButton;
    private bool isFull = false;

    public bool IsFull { get => isFull; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        instantiatedChipArray = new Chip[linesInColumn.Length];
        abilityButton = GetNode<SpecialAbilityButton>("/root/Game/Control/ColorRect/Main/SpecialAbilityButton");

        Node buttonNode = whiteButtonScene.Instantiate();
        AddChild(buttonNode);
        Button button = buttonNode.GetNode<Button>(buttonNode.GetPath());

        button.Pressed += ButtonPressed;
    }

    private void ButtonPressed()
    {
        if (Game.CurrentGame.CurrentGameState != BoardState.PLAYING ||
        Game.CurrentGame.WhoPlays.GetType() == typeof(PlayerBot)) { return; }

        PlayChip();
    }

    public void PlayChip()
    {
        ChipType typeToPlace = Game.CurrentGame.WhoPlays.Chips;
        float freePosition = GetFreePositionInColumn();
        if (freePosition == -1) { return; }

        switch (typeToPlace)
        {
            case ChipType.RED: InstantiateChip(redChipsScene, freePosition); break;
            case ChipType.YELLOW: InstantiateChip(yellowChipsScene, freePosition); break;
            case ChipType.SWAP_COLOR: InstantiateChip(swapColorChipsScene, freePosition); break;
        }

        Game.CurrentGame.ChangeStateTo(BoardState.ANIMATING);
    }

    public void ReportChipPlaced()
    {
        bool isSpecialChip = false;

        if (instantiatedChipArray[currentFreeIndex].GetType() == typeof(SwapColorChips))
        {
            isSpecialChip = true;

            instantiatedChipArray[currentFreeIndex].SwapSpriteToType(abilityButton.PreviousType);
            Game.CurrentGame.WhoPlays.Chips = abilityButton.PreviousType;

            SwapAllColumnSprites();
        }

        Game.CurrentGame.ChangeStateTo(BoardState.PLAYING);

        linesInColumn[currentFreeIndex] = -1;
        Game.CurrentGame.PlaceChipAtPosition(this, currentFreeIndex, isSpecialChip);
    }

    public void ResetColumn()
    {
        foreach (var chip in instantiatedChipArray)
        {
            if (chip == null) { continue; }

            chip.QueueFree();
        }

        linesInColumn = new int[] { 525, 460, 390, 325, 258, 190, 125 };
        instantiatedChipArray = new Chip[linesInColumn.Length];

        abilityButton.WasAbilityUsed = false;
        isFull = false;
    }

    private void SwapAllColumnSprites()
    {
        foreach (Chip chip in instantiatedChipArray)
        {
            if (chip == null) { continue; }

            if (chip.Type == ChipType.RED)
            {
                chip.SwapSpriteToType(ChipType.YELLOW);
            }
            else if (chip.Type == ChipType.YELLOW)
            {
                chip.SwapSpriteToType(ChipType.RED);
            }
        }
    }

    private int GetFreePositionInColumn()
    {
        for (int i = 0; i < linesInColumn.Length; i++)
        {
            if (linesInColumn[i] == -1) { continue; }
            currentFreeIndex = i;

            return linesInColumn[i];
        }
        isFull = true;
        return -1;
    }

    private void InstantiateChip(PackedScene referenceChipScene, float goalPosition)
    {
        Node chipNode = referenceChipScene.Instantiate();
        AddChild(chipNode);
        Chip newChip = chipNode.GetNode<Chip>(chipNode.GetPath());
        newChip.ColumnSpawned = this;
        newChip.PositionToPlace = goalPosition;

        instantiatedChipArray[currentFreeIndex] = newChip;
    }
}