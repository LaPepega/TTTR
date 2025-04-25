using Godot;
using System;
using System.Linq;

public partial class Cell : Button
{
	[Export]
	private Texture2D OTexture;
	[Export]
	private Texture2D XTexture;

	private GridContainer TTTR;
	private TurnManager TurnMGR;
	private TextureRect TurnIND;

	private static string[] checks = { "012", "345", "678", // Horizontal
									   "036", "147", "258", // Vertical
									   "048", "642", };     // Diagonal

	private void advanceTurn()
	{
		this.Icon = TurnMGR.current ? XTexture : OTexture;
		TurnIND.Texture = !TurnMGR.current ? XTexture : OTexture;

		TurnMGR.current = !TurnMGR.current;
		TurnMGR.counter += 1;

		this.Disabled = true;
	}

	private void switchGrid(GridContainer grid, bool state)
	{
		grid.PropagateCall("set_mouse_filter", new() {
			state ? (int)Control.MouseFilterEnum.Pass
				  : (int)Control.MouseFilterEnum.Ignore }
		);
	}

	private bool checkSmallWin()
	{

		var p = GetParent();
		foreach (var check in checks)
		{
			var zero = p.GetNode<Cell>(check.Substr(0, 1)).Icon;
			var one = p.GetNode<Cell>(check.Substr(1, 1)).Icon;
			var two = p.GetNode<Cell>(check.Substr(2, 1)).Icon;

			if (zero != null && one != null && two != null
				&& zero == one && one == two)
				return true;
		}
		return false;
	}

	public override void _Pressed()
	{
		advanceTurn();
		GridContainer nextMoveGrid = TTTR.GetNode<GridContainer>((string)this.Name);

		foreach (GridContainer child in TTTR.GetChildren().Cast<GridContainer>())
		{
			switchGrid(child, false);
		}

		switchGrid(nextMoveGrid, true);

		if (checkSmallWin())
			GD.Print($"WIN! at {this.Name}");


	}

	public override void _Ready()
	{
		TurnMGR = GetNode<TurnManager>("/root/Root/TurnManager");
		TurnIND = GetNode<TextureRect>("/root/Root/TurnIndicator");
		TTTR = GetNode<GridContainer>("/root/Root/TTTR");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
