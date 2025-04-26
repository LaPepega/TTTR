using Godot;
using System;
using System.Linq;

/* TODO:
	- [X] Draws
	- [ ] UI
	- [ ] Active grid highlight
	- [ ] Restart, menu, etc.
	- [X] Move to occupied grid
	- [X] Grid winner
*/


public partial class Cell : Button
{

	[Export]
	private PackedScene tileScene;

	private GridContainer TTTR;

	private TurnManager TurnMGR;


	private static string[] checks = { "012", "345", "678", // Horizontal
									   "036", "147", "258", // Vertical
									   "048", "642", };     // Diagonal

	public override void _Ready()
	{
		TurnMGR = GetNode<TurnManager>("/root/Root/TurnManager");
		TTTR = GetNode<GridContainer>("/root/Root/TTTR");


	}

	private static void SwitchGrid(GridContainer grid, bool state)
	{
		grid.PropagateCall("set_mouse_filter", new() {
			state ? (int)Control.MouseFilterEnum.Pass
				  : (int)Control.MouseFilterEnum.Ignore }
		);
	}

	private void SwitchAllGrids(bool state)
	{
		foreach (Node child in TTTR.GetChildren())
		{
			var cast = child as GridContainer;
			if (cast != null)
				SwitchGrid(cast, state);
		}
	}

	//FIXME: Repetition
	private bool CheckSmallWin()
	{
		var p = GetParent();
		foreach (var check in checks)
		{
			var zero = p.GetChild<Cell>(check[0] - '0').Icon;
			var one = p.GetChild<Cell>(check[1] - '0').Icon;
			var two = p.GetChild<Cell>(check[2] - '0').Icon;

			if (zero != null && one != null && two != null
				&& zero == one && one == two)
				return true;
		}
		return false;
	}

	private bool CheckBigWin()
	{
		GD.Print("Checking Big");
		foreach (var check in checks)
		{
			var zero = TTTR.GetChild<Node>(check[0] - '0') as TextureRect;
			var one = TTTR.GetChild<Node>(check[1] - '0') as TextureRect;
			var two = TTTR.GetChild<Node>(check[2] - '0') as TextureRect;

			if (zero != null && one != null && two != null
					&& zero.Texture == one.Texture && one.Texture == two.Texture)
				return true;
		}
		return false;
	}

	private void WinThisGrid()
	{
		TextureRect tile = tileScene.Instantiate<TextureRect>();
		tile.Texture = TurnMGR.current ? TurnMGR.OTexture : TurnMGR.XTexture;

		var i = GetParent().GetIndex();
		GetParent().QueueFree();

		TTTR.AddChild(tile);
		TTTR.MoveChild(tile, i);
		TTTR.MoveChild(GetParent(), 10);

		tile.Name = i.ToString();
	}

	private void CheckWins()
	{
		if (!CheckSmallWin())
			return;

		WinThisGrid();

		var winner = TurnMGR.current ? "O" : "X";
		GD.Print($"{winner} WIN! at {GetParent().Name}");

		if (CheckBigWin())
			GD.Print("YAAAAAAY");
	}

	private static bool IsGridAvailable(GridContainer grid)
	{
		if (grid == null)
			return false;

		foreach (Cell c in grid.GetChildren().Cast<Cell>())
		{
			if (c.Icon == null)
				return true;
		}

		return false;
	}

	public override void _Pressed()
	{
		TurnMGR.AdvanceTurn(this);
		CheckWins();

		GridContainer nextMoveGrid = TTTR.GetChild<Node>(this.GetIndex()) as GridContainer;


		if (IsGridAvailable(nextMoveGrid))
		{
			SwitchAllGrids(false);
			SwitchGrid(nextMoveGrid, true);
		}
		else
		{
			SwitchAllGrids(true);
		}
	}
}
