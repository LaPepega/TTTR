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
	private static PackedScene tileScene;
	private static GridContainer TTTR;
	private static TurnManager TurnMGR;
	private static TextureRect WinRect;

	private static string[] checks = { "012", "345", "678", // Horizontal
									   "036", "147", "258", // Vertical
									   "048", "642", };     // Diagonal

	public override void _Ready()
	{
		TurnMGR = GetNode<TurnManager>("/root/Root/TurnManager");
		TTTR = GetNode<GridContainer>("/root/Root/TTTR");
		tileScene = ResourceLoader.Load<PackedScene>("res://scenes/WinTile.tscn");
		WinRect = GetNode<TextureRect>("/root/Root/WinRect");
	}

	private static void SwitchGrid(GridContainer grid, bool state)
	{
		grid.PropagateCall("set_mouse_filter", new() {
			state ? (int)Control.MouseFilterEnum.Pass
				  : (int)Control.MouseFilterEnum.Ignore }
		);
	}

	private static void SwitchAllGrids(bool state)
	{
		foreach (Node child in TTTR.GetChildren())
		{
			var cast = child as GridContainer;
			if (cast != null)
				SwitchGrid(cast, state);
		}
	}

	//FIXME: Can totally be done better
	private static bool RunChecks(GridContainer grid)
	{
		foreach (var check in checks)
		{
			if (grid.GetChild(check[0] - '0') is TextureRect zeroB && zeroB.Texture != null
				&& grid.GetChild(check[1] - '0') is TextureRect oneB && oneB.Texture != null
				&& grid.GetChild(check[2] - '0') is TextureRect twoB && twoB.Texture != null
				&& zeroB.Texture == oneB.Texture && oneB.Texture == twoB.Texture)
				return true;

			if (grid.GetChild(check[0] - '0') is Cell zeroS && zeroS.Icon != null
				&& grid.GetChild(check[1] - '0') is Cell oneS && oneS.Icon != null
				&& grid.GetChild(check[2] - '0') is Cell twoS && twoS.Icon != null
				&& zeroS.Icon == oneS.Icon && oneS.Icon == twoS.Icon)
				return true;
		}
		return false;
	}

	private static void WinGrid(GridContainer grid)
	{
		TextureRect tile = tileScene.Instantiate<TextureRect>();
		tile.Texture = TurnMGR.current ? TurnMGR.OTexture : TurnMGR.XTexture;

		var i = grid.GetIndex();
		grid.QueueFree();

		grid.GetParent().AddChild(tile);
		grid.GetParent().MoveChild(tile, i);
		grid.GetParent().MoveChild(grid, 10);

		tile.Name = i.ToString();
	}

	private void CheckWins()
	{
		if (!RunChecks(GetParent<GridContainer>()))
			return;

		WinGrid(GetParent<GridContainer>());

		var winner = TurnMGR.current ? "O" : "X";
		GD.Print($"{winner} WIN! at {GetParent().Name}");

		if (RunChecks(TTTR))
			Win(TurnMGR.current);
	}

	private static bool GridAvailable(GridContainer grid)
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

	private void Win(bool who)
	{
		TTTR.Visible = false;
		TurnMGR.TurnIND.Visible = false;
		TurnMGR.TurnCNT.Visible = false;
		WinRect.Texture = who ? TurnMGR.OTexture : TurnMGR.XTexture;
	}
	public override void _Pressed()
	{
		TurnMGR.AdvanceTurn(this);
		CheckWins();

		GridContainer nextMoveGrid = TTTR.GetChild<Node>(this.GetIndex()) as GridContainer;


		if (GridAvailable(nextMoveGrid))
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
