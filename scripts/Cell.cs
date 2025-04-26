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
	private Texture2D OTexture;
	[Export]
	private Texture2D XTexture;
	[Export]
	private PackedScene tileScene;

	private GridContainer TTTR;
	private TurnManager TurnMGR;
	private TextureRect TurnIND;
	private Label TurnCNT;

	private static string[] checks = { "012", "345", "678", // Horizontal
									   "036", "147", "258", // Vertical
									   "048", "642", };     // Diagonal

	public override void _Ready()
	{
		TurnMGR = GetNode<TurnManager>("/root/Root/TurnManager");
		TurnIND = GetNode<TextureRect>("/root/Root/TurnIndicator");
		TTTR = GetNode<GridContainer>("/root/Root/TTTR");
		TurnCNT = GetNode<Label>("/root/Root/TurnCount");

	}

	private void advanceTurn()
	{
		this.Icon = TurnMGR.current ? XTexture : OTexture;
		TurnIND.Texture = !TurnMGR.current ? XTexture : OTexture;
		TurnMGR.current = !TurnMGR.current;

		TurnCNT.Text = (Int32.Parse(TurnCNT.Text) + 1).ToString();

		this.Disabled = true;
	}

	private void switchGrid(GridContainer grid, bool state)
	{
		grid.PropagateCall("set_mouse_filter", new() {
			state ? (int)Control.MouseFilterEnum.Pass
				  : (int)Control.MouseFilterEnum.Ignore }
		);
	}

	private void switchAllGrids(bool state)
	{
		foreach (Node child in TTTR.GetChildren())
		{
			var cast = child as GridContainer;
			if (cast != null)
				switchGrid(cast, state);
		}
	}

	private bool checkSmallWin()
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

	private bool checkBigWin()
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

	private void winThisGrid()
	{
		TextureRect tile = tileScene.Instantiate<TextureRect>();
		tile.Texture = TurnMGR.current ? OTexture : XTexture;

		var i = GetParent().GetIndex();
		GetParent().QueueFree();

		TTTR.AddChild(tile);
		TTTR.MoveChild(tile, i);
		TTTR.MoveChild(GetParent(), 10);

		tile.Name = i.ToString();
	}

	private void checkWins()
	{
		if (checkSmallWin())
		{
			winThisGrid();

			var winner = TurnMGR.current ? "O" : "X";
			GD.Print($"{winner} WIN! at {GetParent().Name}");

			if (checkBigWin())
				GD.Print("YAAAAAAY");
		}
	}

	private bool isGridAvailable(GridContainer grid)
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
		advanceTurn();
		checkWins();

		GridContainer nextMoveGrid = TTTR.GetChild<Node>(this.GetIndex()) as GridContainer;


		if (isGridAvailable(nextMoveGrid))
		{
			switchAllGrids(false);
			switchGrid(nextMoveGrid, true);
		}
		else
		{
			switchAllGrids(true);
		}
	}
}
