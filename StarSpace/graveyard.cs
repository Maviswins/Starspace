//The home of code no longer to be used unless it needs to be used again:

/*

    private static void BeginSplit(CompCollection subject, Component origin)
    {
        //Double check we're not splitting the entire collection.
        Queue<Component> splitList = new();
        Queue<Component> discardList = new();

        //Iterate through connected components for a valid subject.
        foreach (KeyValuePair<ConnectionPoint, ConnectionPoint> kvp in origin.ConDict)
        {
            if (kvp.Value != null)
            {
                if (!discardList.Contains(kvp.Key.RootNode) && !splitList.Contains(kvp.Key.RootNode) && kvp.Key.RootNode != subject.RootComponent)
                {
                    //If a valid subject is found, add it to the split list and begin the flood-fill subloop.
                    splitList.Enqueue(kvp.Key.RootNode);
                    SplitBufferSubloop(splitList, discardList, origin);
                }
            }
        }
        //Split off any modules that need splitting.
        if (splitList.Any())
        {
            SplitCollection(splitList, origin);
        }
    }

    /// <summary>
	/// Flood from subject until either the target is reached again, or it hits a cause for burndown.
	/// </summary>
	private static void SplitBufferSubloop(Queue<Component> splitList, Queue<Component> discardList, Component origin)
	{
		int killCounter = 0;
		bool killLoop = false;
		while (!killLoop)
		{
			if (splitList.Any())
			{
				Component subject = splitList.Dequeue();

				foreach (KeyValuePair<ConnectionPoint, ConnectionPoint> kvp in subject.ConDict)
				{
					if (kvp.Value != null)
					{
						//Double check we're not being silly and enqueuing the target.
						if (kvp.Value.RootNode != origin)
						{
							//Check for burndown criteria. Fire subloop if met and cancel current loop.
							if (discardList.Contains(kvp.Value.RootNode) || kvp.Value.RootNode.IsRootComponent)
							{
								discardList.Enqueue(subject);
								SplitBurndownSubloop(splitList, discardList, subject, origin);
								break;
							}
							else if (!splitList.Contains(kvp.Value.RootNode))
							{
								splitList.Enqueue(kvp.Value.RootNode);
							}
						}
					}
				}
			}
			//Try and prevent forever loops.
			killCounter++;
			if (killCounter > 20 || !splitList.Any())
			{
				killLoop = true;
			}
		}
	}

	/// <summary>
	/// Reverse-flood fill to nullify invalid subjects for a split until target is reached.
	/// </summary>
	private static void SplitBurndownSubloop(Queue<Component> splitList, Queue<Component> discardList, Component subject, Component target)
	{
		foreach (KeyValuePair<ConnectionPoint, ConnectionPoint> kvp in subject.ConDict)
		{
			if (kvp.Value != null)
			{
				if (kvp.Value.RootNode != target && !kvp.Value.RootNode.IsRootComponent)
				{
					//Disregard items not already in splitList.
					if (splitList.Contains(kvp.Value.RootNode))
					{
						splitList = RemoveQueueItem(splitList, kvp.Value.RootNode);

						if (!discardList.Contains(kvp.Value.RootNode))
						{
							discardList.Enqueue(kvp.Value.RootNode);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Turn a queue into a list, remove an item, then turn it into a queue again.
	/// </summary>
	private static Queue<Component> RemoveQueueItem(Queue<Component> queue, Component removal)
	{
		List<Component> tempList = queue.ToList();
		tempList.Remove(removal);
		Queue<Component> output = new();
		foreach (Component item in tempList)
		{
			output.Enqueue(item);
		}
		return output;
	}

	/// <summary>
	/// Split off a new collection from this one using a previously generated splitList.
	/// </summary>
	public static void SplitCollection(Queue<Component>splitList, Component origin)
	{
		//Instantiate new collection.
		CompCollection newParent = InstantiateNewCollection(origin);

		//Transfer custody.
        AddMultipleComponentsToCollection(splitList.ToList(), newParent);
	}

		public override void _Input(InputEvent @event)
    {
		//If left mouse is down:
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
			//If the mouse is over this component:
            if (_isMouseOver)
            {
				//If it's not already registered as being dragged:
                if (!IsBeingDragged && mouseEvent.Pressed)
                {
                    IsBeingDragged = true;

					//If this isn't the root component for a collection, start tracking for an unsnap.
					if (!IsRootComponent)
					{
						_dragOffset = Position - GetGlobalMousePosition();
					}
                }
            }

            if (IsBeingDragged && !mouseEvent.Pressed)
            {
                IsBeingDragged = false;
				
				if (IsSnapped)
				{
					//RootCollection.CheckForMerge(this);
				}

				//Check for overlap on release.
				if (IsOverlapping.Item1 == true)
				{
					CheckOverlap();
				}
            }
        }

		//Snap logic
        else if (@event is InputEventMouseMotion && IsBeingDragged)
        {
            //Will only freely track cursor position if it is not currently snapped to another
            //module or exceeds the threshold to unsnap.
            if (!IsSnapped)
            {
                Position = GetGlobalMousePosition() + _dragOffset;
            }
            else
            {
                if (Position.DistanceTo(GetGlobalMousePosition()) > _snapThreshold)
                {
                    IsSnapped = false;
                }
            }
        }
    }
    */