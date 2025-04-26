using Godot;
using System;

public partial class RestartButton : Button
{
	public override void _Pressed()
	{
		GetTree().ReloadCurrentScene();
	}
}
