using Godot;
using System;

public partial class TurnManager : Timer
{
	public bool current = true;

	[Export]
	public Texture2D OTexture;
	[Export]
	public Texture2D XTexture;
	private TextureRect TurnIND;
	private Label TurnCNT;

	public void AdvanceTurn(Cell callerCell)
	{
		callerCell.Icon = this.current ? XTexture : OTexture;
		TurnIND.Texture = !this.current ? XTexture : OTexture;
		this.current = !this.current;

		TurnCNT.Text = (Int32.Parse(TurnCNT.Text) + 1).ToString();

		callerCell.Disabled = true;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TurnIND = GetNode<TextureRect>("/root/Root/TurnIndicator");
		TurnCNT = GetNode<Label>("/root/Root/TurnCount");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
