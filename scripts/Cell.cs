using Godot;
using System;

public partial class Cell : Button
{
	[Export]
	private Texture2D OTexture;
	[Export]
	private Texture2D XTexture;

	private GridContainer TTTR;
	private TurnManager TurnMGR;
	private TextureRect TurnIND;

	private void advanceTurn()
	{
		this.Icon = TurnMGR.current ? XTexture : OTexture;
		TurnIND.Texture = !TurnMGR.current ? XTexture : OTexture;
		TurnMGR.current = !TurnMGR.current;
		this.Disabled = true;
	}

	public override void _Pressed()
	{

		advanceTurn();
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
