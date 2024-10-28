using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public partial class Component : Area2D
{
	/*
	The building blocks of a vessel (or wreckage). All ship components should ultimately inherit from this.
	*/

	//Stores references for connection points and their attachments.
	public Dictionary<ConnectionPoint, ConnectionPoint> ConDict = new();
	public AnimatedSprite2D anim;
	public CollisionShape2D col;
	public CompCollection RootCollection;
	
	public Main mainParent;

	//Probably vestigial. To be removed.
	bool flick = false;

	//Set by contained collection. Designates it as a root component.
	public bool IsRootComponent = false;

	//Designates whether this component should be draggable.
	public bool IsDraggable = true;

	//Designates whether this component is currently being dragged around.
	public bool IsBeingDragged = false;

	//Designated whether this component is currently snapped to an attachment.
    public bool IsSnapped = false;

	//Is the mouse over this component?
	private bool _mouseIsOver = false;

	//Used to track the mouse when being dragged.
    private Vector2 _dragOffset = Vector2.Zero;

	//Distance from another module before snapping to it in build mode.
    private float _snapThreshold = 20f;

	//How overlapped this component can tolerate (% of area)
	public float OverlapThreshold = 20;

	public bool IsCockpit;
	
	//Tracking and calculating overlaps. Currently only works for a single other item and may need changing.
	public (bool, Area2D) IsOverlapping = (false, null);

	//Largely for test purposes, will probably become vestigial.
	private Random random= new();

	public ConnectionPoint CurrentlySelected;


	/// <summary>
	/// Identify, categorise and store connection points in dict.
	/// </summary>
	public void InstantiatePoints()
	{
		GD.Print("Checking for connection points");
		foreach (Node node in GetChildren())
		{
			GD.Print(node.Name);
			if (node is ConnectionPoint conPoint)
			{
				conPoint.Parent = this;
				float rot = 0;
				string n = conPoint.Name;
				if (n.Contains('U'))
				{
					rot = 0;
				}
				else if (n.Contains('D'))
				{
					rot = 180;
				}
				else if (n.Contains('L'))
				{
					rot = 270;
				}
				else if (n.Contains('R'))
				{
					rot = 90;
				}
				conPoint.RotationDegrees = rot;
				ConDict.Add(conPoint, null);
			}
		}
		GD.PrintRich(ConDict.Count + " connection points detected.");
	}

	public override void _Ready()
	{
		GD.Print("Module instantiated.");

		foreach (Node node in GetChildren())
		{
			if (node is AnimatedSprite2D a)
			{
				anim = a;
			}
			else if (node is CollisionShape2D c)
			{
				col = c;
			}
		}
		MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
		AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;

		InstantiatePoints();
	}

	public override void _Process(double delta)
	{
		// if (!flick)
		// {
		// 	InstantiateComponent();
		// 	flick = true;
		// }
	}



#region input shit
	public override void _Input(InputEvent @event)
    {
		//If a mouse button has done a thing:
        if (@event is InputEventMouseButton mouseEvent)
        {
			//If the left button has been pressed and mouse is over this component:
            if (mouseEvent.ButtonIndex == MouseButton.Left && _mouseIsOver)
            {
				//create instance in the holder collection.
				//Should probably make sure this doesn't clash with points.
            }

			//If the right button has been pressed and mouse is over this component:
			else if (mouseEvent.ButtonIndex == MouseButton.Right && _mouseIsOver)
			{
				//to be determined.
			}
		}
    }

    private void OnMouseEntered()
    {
        _mouseIsOver = true;
    }

    private void OnMouseExited()
    {
        _mouseIsOver = false;
    }

#endregion

	private void OnAreaEntered(Area2D area)
	{
		if (area is Component compo)
		{
			IsOverlapping.Item1 = true;
			IsOverlapping.Item2 = compo;
		}
	}

	private void OnAreaExited(Area2D area)
	{
		if (area is Component compo)
		{
			IsOverlapping.Item1 = false;
			IsOverlapping.Item2 = null;
		}
	}

	public List<ConnectionPoint> GetConnectionPoints()
	{
		List<ConnectionPoint> output = new();
		foreach (Node n in GetChildren())
		{
			if (n is ConnectionPoint cp)
			{
				output.Add(cp);
			}
		}
		return output;
	}

	/// <summary>
	/// Check if this module is overlapping with another too much and if so, reposition it.
	/// </summary>
	public void CheckOverlap()
	{
		if (IsOverlapping.Item1 == true && IsOverlapping.Item2 != null)
		{
			if (IsOverlapping.Item2 is Component comparer)
			{
				Rectangle thisRect = GetSpriteRect();
				int thisArea = thisRect.Width * thisRect.Height;
				Rectangle compRect = comparer.GetSpriteRect();
				int compArea = compRect.Width * compRect.Height;
				Rectangle intersect = Rectangle.Intersect(compRect, thisRect);
				int intArea = intersect.Width * intersect.Height;

				if (intArea >= thisArea / (100 / OverlapThreshold) || intArea >= compArea / (100 / comparer.OverlapThreshold))
				{
					//Scatter();
				}
			}
		}
	}

	public bool CheckOverlap(ConnectionPoint thisPoint, ConnectionPoint subjectPoint, Component subject)
	{
		Rectangle thisRect = GetSpriteRect(thisPoint);
		int thisArea = thisRect.Width * thisRect.Height;
		Rectangle subRect = subject.GetSpriteRect(subjectPoint);
		int subArea = thisRect.Width * thisRect.Height;
		Rectangle intersect = Rectangle.Intersect(subRect, thisRect);
		int intArea = intersect.Width * intersect.Height;


		if (intArea >= thisArea / (100 / OverlapThreshold) || intArea >= subArea / (100 / subject.OverlapThreshold))
		{
			return false;
		}
		return true;
	}

	public bool CheckOverlap(Rectangle r1, Rectangle r2)
	{
		int r1Area = r1.Width * r1.Height;
		int r2Area = r2.Width * r2.Height;
		Rectangle intersect = Rectangle.Intersect(r1, r2);
		int intArea = intersect.Width * intersect.Height;


		if (intArea >= r1Area / (100 / 20) || intArea >= r2Area / (100 / 20))
		{
			return false;
		}
		return true;
	}

	public Vector2 GetOffsetPosition(ConnectionPoint subject)
	{
		return Position - subject.Position;
	}

	public Rectangle GetSpriteRect(ConnectionPoint subject)
	{
		Vector2 offsetPosition = GetOffsetPosition(subject);
		Vector2 dims = GetSpriteDims();
		return new Rectangle(new Point((int)offsetPosition.X, (int)offsetPosition.Y), new Size((int)dims.X, (int)dims.Y));
	}

	public Rectangle GetSpriteRect()
	{
		Vector2 dims = GetSpriteDims();
		return new Rectangle(new Point((int)Position.X, (int)Position.Y), new Size((int)dims.X, (int)dims.Y));
	}

	public Rectangle GetSpriteRect(Vector2 testPoint)
	{
		Vector2 dims = GetSpriteDims();
		return new Rectangle(new Point((int)testPoint.X, (int)testPoint.Y), new Size((int)dims.X, (int)dims.Y));
	}

	public Vector2 GetSpriteDims()
	{
		foreach (Node node in GetChildren())
		{
			if (node is AnimatedSprite2D anim)
			{
				return anim.SpriteFrames.GetFrameTexture("default", 0).GetSize();
				
			}
		}
		return Vector2.Zero;
	}

	public void Scatter()
	{
		Position = new Vector2(random.Next(30, 800), random.Next(20, 200));
		CheckOverlap();
	}

	/// <summary>
	/// Does a connection point have an attachment?
	/// </summary>
	public bool HasAttachment(ConnectionPoint point)
	{
		if (ConDict[point] != null)
		{
			return true;
		}
		else return false;
	}

	/// <summary>
	/// Set the attachment for a connection point.
	/// </summary>
	public void SetAttachment(ConnectionPoint point, ConnectionPoint target)
	{
		ConDict[point] = target;
	}

	/// <summary>
	/// Make this recognise itself as the root component in a collection.
	/// </summary>
	public void SetAsRootComponent()
	{
		IsRootComponent = true;
		anim.Frame = 1;
	}

	/// <summary>
	/// Make this component dethrone itself.
	/// </summary>
	public void RemoveAsRootComponent()
	{
		IsRootComponent = false;
		anim.Frame = 0;
	}
}
