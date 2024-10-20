using Godot;
using System;
using System.Linq;

public partial class ConnectionPoint : Area2D
{
	/*
	There will be one or more or these as children of every component. Their job is to facilitate the connection and disconnection of the parent with other components.
	*/

	//Reference to current attachment.
	public ConnectionPoint Attachment = null;
	public Vector2 LocationOffset;
	public float RotationOffset;

	private AnimatedSprite2D anim;

	//Reference to parent component.
	public Component RootNode;

	public void InstantiatePoint(Component parent)
	{

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RootNode = GetOwner<Component>();
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;

		foreach (Node node in GetChildren())
		{
			if (node is AnimatedSprite2D a)
			{
				anim = a;
				
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnAreaEntered(Area2D area)
    {
        // Only apply any logic if this module is actively being dragged
        // and only if we're colliding with another snap point
        if (RootNode.IsBeingDragged && area is ConnectionPoint target)
        {
            //If colliding snap point is not currently occupied:
            if (!RootNode.HasAttachment(this))
            {
                Vector2 snapPosition = target.GlobalPosition - Position;
            
                RootNode.Position = snapPosition;

                // Set both this and its counterpart to snapped.
                Snap(target);
            }
        }
    }

    /// <summary>
    /// If this snap point detects itself leaving another snap point it unsnaps.
    /// </summary>
    private void OnAreaExited(Area2D area)
    {
        if (RootNode.IsBeingDragged && area is ConnectionPoint target)
        {
			//Tell both parties of the attachment to unsnap.
			if (RootNode.HasAttachment(this) && target.RootNode.HasAttachment(target))
			{
				//GD.Print(this);
				UnSnap(target);
			}
        }
    }

	/// <summary>
	/// Attach this point with the target.
	/// </summary>
	public void Snap(ConnectionPoint target, bool initiator = true)
    {
		if (initiator)
		{
			target.Snap(this, false);
			RootNode.NowSnapped(this, target);
		}
		else
		{
			RootNode.NowSnapped(this, target, false);
		}
		Attachment = target;
		anim.Frame = 1;
    }

	/// <summary>
	/// Unsnap this point from the target.
	/// </summary>
    public void UnSnap(ConnectionPoint target, bool initiator = true)
    {
		if (initiator)
		{
			target.UnSnap(this, false);
			RootNode.NowUnSnapped(this, true);
		}
		else
		{
			RootNode.NowUnSnapped(target, false);
		}
		Attachment = null;
        anim.Frame = 0;
    }
}
