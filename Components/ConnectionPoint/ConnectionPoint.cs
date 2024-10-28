using Godot;
using System;
using System.Linq;

public partial class ConnectionPoint : Area2D
{
	/*
	There will be one or more or these as children of every component. Their job is to facilitate the connection and disconnection of the parent with other components.
	*/

	public Vector2 LocationOffset;
	public float RotationOffset;
	public Vector2 LocationPositionOffset;
	public bool IsSnapped;
	public bool IsSelected;
	public bool CanBeSnapped;
	private bool _mouseIsOver = false;
	public bool OverlappingAnotherConnectionPoint;
	public ConnectionPoint OverlappedConnectionpoint;

	private BuildMode BM;

	private AnimatedSprite2D anim;

	//Reference to parent component.
	public Component Parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

		foreach (Node node in GetChildren())
		{
			if (node is AnimatedSprite2D a)
			{
				anim = a;
			}
		}

		BM = (BuildMode)GetParent().GetParent().GetParent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
    {
		//If a mouse button has done a thing:
        if (@event is InputEventMouseButton mouseEvent)
        {
			//If the left button has been pressed and mouse is over this component:
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed && _mouseIsOver)
            {
				BM.LeftClick(this);
            }

			//If the right button has been pressed and mouse is over this component:
			else if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed && IsSelected)
			{
				SetAsUnSelected();
				BM.UpdateSelectionStatus(null);
			}
		}
    }

	private void OnMouseEntered()
	{
		if (!_mouseIsOver)
		{
			GD.Print(Position);
		}
		//If not already snapped:
		if (!IsSnapped)
		{
			Scale = new Vector2(2, 2);
			_mouseIsOver = true;
		}
	}

	private void OnMouseExited()
	{
		if (!IsSelected)
		{
			Scale = new Vector2(1, 1);
		}
		_mouseIsOver = false;
	}

	public void SetAsAttached()
	{
		anim.Frame = 2;
		IsSnapped = true;
	}

	public void SetAsUnAttached()
	{
		anim.Frame = 0;
		IsSnapped = false;
	}

	public void SetAsSelected()
	{
		anim.Frame = 1;
		IsSelected = true;
	}

	public void SetAsUnSelected()
	{
		anim.Frame = 0;
		IsSelected = false;
		Scale = new Vector2(1, 1);
	}
}