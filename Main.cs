using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	CompCollection ship;
	CompCollection holder;
	public override void _Ready()
	{
		Utilities.CurrentScene = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("AddThing"))
		{
			if (ship == null)
			{
				PackedScene collectionScene = GD.Load<PackedScene>("res://Components/CompCollection.tscn");
				PackedScene componentScene = GD.Load<PackedScene>("res://Components/Component.tscn");
				CompCollection cc = collectionScene.Instantiate<CompCollection>();
				cc.Position = new Vector2(400, 300);
				ship = cc;
				AddChild(ship);
				Component c = componentScene.Instantiate<Component>();
				cc.AddChild(c);
				c.SetAsRootComponent();
			}
			if (holder == null)
			{
				PackedScene collectionScene = GD.Load<PackedScene>("res://Components/CompCollection.tscn");
				PackedScene componentScene = GD.Load<PackedScene>("res://Components/Component.tscn");
				CompCollection cc = collectionScene.Instantiate<CompCollection>();
				cc.Position = new Vector2(200, 300);
				holder = cc;
				AddChild(holder);
				Component c = componentScene.Instantiate<Component>();
				cc.AddChild(c);
			}
		}
	}
}