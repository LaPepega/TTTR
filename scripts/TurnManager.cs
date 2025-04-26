using Godot;
using System;

public partial class TurnManager : Timer
{
	public bool current = true;

	[Export]
	public Texture2D OTexture;
	[Export]
	public Texture2D XTexture;
	public TextureRect TurnIND;
	public Label TurnCNT;

	public void AdvanceTurn(Cell callerCell)
	{
		callerCell.Icon = this.current ? XTexture : OTexture;
		TurnIND.Texture = !this.current ? XTexture : OTexture;
		this.current = !this.current;

		TurnCNT.Text = (Int32.Parse(TurnCNT.Text) + 1).ToString();

		callerCell.Disabled = true;
	}


	public override void _Ready()
	{
		TurnIND = GetNode<TextureRect>("/root/Root/TurnIndicator");
		TurnCNT = GetNode<Label>("/root/Root/TurnCount");
	}
}
