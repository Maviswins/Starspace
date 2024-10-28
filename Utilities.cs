using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public partial class Utilities : Node
{
    public static Node CurrentScene;
    public static Vector2 AngleRotate (Vector2 input, float rotation)
	{
		Vector2 output = input;
		input = Vector2.FromAngle(rotation);
		return output;
	}

    #region node management

    /// <summary>
    /// Safely add a component to a compcollection.
    /// </summary>
    public static void AddChildDeferred(Component subject, CompCollection target)
    {
        if (subject != null && target != null)
        {
            if (subject.GetParent() != null)
            {
                RemoveChildDeferred(subject);
            }
            Callable c = new(target, "AddChild");
            c.CallDeferred(subject);
        }
    }

    /// <summary>
    /// Safely add a collection to a parent node.
    /// </summary>
    public static void AddChildDeferred(CompCollection subject, Node target)
    {
        if (subject != null && target != null)
        {
            if (subject.GetParent() != null)
            {
                RemoveChildDeferred(subject);
            }
            Callable c = new(target, "AddChild");
            c.CallDeferred(subject);
        }
    }

    /// <summary>
    /// Safely remove a child component from parent.
    /// </summary>
    public static void RemoveChildDeferred(Component subject)
    {
        if (subject != null)
        {
            if (subject.GetParent() != null)
            {
                Callable c = new(subject.GetParent(), "RemoveChild");
                c.CallDeferred(subject);
            }
        }
    }

     /// <summary>
    /// Safely remove a child collection from parent.
    /// </summary>
    private static void RemoveChildDeferred(CompCollection subject)
    {
        if (subject != null)
        {
            if (subject.GetParent() != null)
            {
                Callable c = new(subject.GetParent(), "RemoveChild");
                c.CallDeferred(subject);
            }
        }
    }
    #endregion

    #region Collection Management

    /// <summary>
    /// Check if a component is a child node and internally registered to a collection.
    /// </summary>
    private static bool ComponentIsInCollection(Component subject, CompCollection target)
    {
        if (subject != null && target != null)
        {
            if (subject.GetParent() == target)
            {
                if (target.Components.Contains(subject) || target.RootComponent == subject)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Try and add a component to the target collection.
    /// </summary>
    private static void AddComponentToCollection(Component subject, CompCollection target)
    {
        if (subject is not null && target is not null)
        {
            //Make sure it's not in the collection already.
            if (!ComponentIsInCollection(subject, target))
            {
                //If it's not already a child node:
                if (subject.GetParent() != target)
                {
                    //If it's in another collection, sanitise before removal.
                    if (subject.GetParent() != target && subject.GetParent() is CompCollection origin)
                    {
                        RemoveComponentFromCollection(subject, origin);
                    }
                    else if (subject.GetParent() is not null)
                    {
                        RemoveChildDeferred(subject);
                    }
                    //Add it.
                    AddChildDeferred(target, subject);
                }
                //target.InitialiseComponent(subject);
            }
        }
    }

    /// <summary>
    /// Try and remove a component from the target collection.
    /// </summary>
    private static void RemoveComponentFromCollection(Component subject, CompCollection target)
    {
        if (subject != null && target != null && ComponentIsInCollection(subject, target))
        {
            if (subject.GetParent() == target)
            {
                RemoveChildDeferred(subject);
                //target.SanitiseComponent(subject);
            }
        }
    }

    private static List<Component> RemoveAllComponentsFromCollection(CompCollection subject)
    {
        List<Component> output = new();

        if (subject != null)
        {
            if (subject.Components.Any())
            {
                while (subject.Components.Any())
                {
                    Component item = subject.Components.First();
                    RemoveComponentFromCollection(item, subject);
                    output.Add(item);
                }
                subject.Components.Clear();
            }
            if (subject.RootComponent != null)
            {
                Component item = subject.RootComponent;
                RemoveComponentFromCollection(item, subject);
                output.Add(item);
            }
        }
        return output;
    }

    public static void AddMultipleComponentsToCollection(List<Component> intake, CompCollection target)
    {
        if (intake.Any())
        {
            foreach (Component item in intake)
            {
                AddComponentToCollection(item, target);
            }
        }
    }

    /// <summary>
    /// Self explanatory.
    /// </summary>
    public static void TransferComponentBetweenCollections(Component subject, CompCollection origin, CompCollection target)
    {
        RemoveComponentFromCollection(subject, origin);
        AddComponentToCollection(subject, target);
    }

    /// <summary>
	/// Merge this collection into the target collection.
	/// </summary>
	// public static void MergeIntoCollection(CompCollection subject, CompCollection target)
	// {
	// 	//Basic check to ensure the cockpit isn't being merged into another collection. Swap roles if so.
	// 	if (subject.HasCockpit)
	// 	{
	// 		MergeIntoCollection(target, subject);
	// 	}
	// 	else
	// 	{
    //         List<Component> outflow = RemoveAllComponentsFromCollection(subject);
    //         AddMultipleComponentsToCollection(outflow, target);
	// 	}
	// }

    /// <summary>
    /// Try to split a collection into two or more collections using the subject component as a breakpoint.
    /// </summary>
    public static void SplitCollection(CompCollection parent, Component subject, bool killComponent = false)
    {
        List<Component> floodFrom = new();
        List<List<Component>> splits = new();

        //If subject component is the proper origin of the flood:
        if (!killComponent)
        {
            floodFrom.Add(subject);
        }

        //If the subject is to be ignored, begin flood from its attachments.
        else if (killComponent)
        {
            floodFrom = GetAttachedComponents(parent, subject);
        }
        
        //Iteratively add split lists to the main container.
        foreach (Component origin in floodFrom)
        {
            splits.Add(RunFloodLoop(parent, origin, subject, killComponent));
        }

        //Weed out any blank splits or duplicates.
        ValidateSplits(splits, subject, killComponent);

        //Actually do the split.
        FinaliseSplitCollection(splits, floodFrom);
    }

    /// <summary>
    /// Flood from a given component, iteratively adding to output unless certain criteria are hit.
    /// </summary>
    private static List<Component> RunFloodLoop(CompCollection parent, Component origin, Component killedComp, bool killComponent)
    {
        //Add flood origin as first item.
        Queue<Component> splitCandidates = new();
        splitCandidates.Enqueue(origin);

        //Loop control.
        int QueueCount = 0;
        int killCounter = 0;
		bool killLoop = false;

		while (!killLoop)
        {
            //Queue will be cleared if burndown occurs.
            if (splitCandidates.Any())
            {
                Component currentSubject = splitCandidates.Dequeue();

                //Find attachments for dequeued component.
                List<Component> attachments = GetAttachedComponents(parent, currentSubject);
                foreach (Component attachment in attachments)
                {
                    //Check if each attachment warrants a burndown.
                    if (!CheckForBurndown(parent, attachment))
                    {
                        //Check if each attachment is a valid addition to the queue.
                        if (CheckForSplitQueue(attachment, killedComp, splitCandidates, killComponent))
                        {
                            //If all good enqueue the attachment.
                            splitCandidates.Enqueue(attachment);
                        }
                    }

                    //Burn down the whole queue and kill the loop if warranted.
                    else
                    {
                        splitCandidates.Clear();
                        killLoop = true;
                        break;
                    }
                }

                //If queue hasn't been burned down, re-add the subject of the last loop.
                if (splitCandidates.Any())
                {
                    if (splitCandidates.Contains(currentSubject))
                    {
                        splitCandidates.Enqueue(currentSubject);
                    }
                }

                //If queue has been burned down, ensure the origin remains as the sole output.
                else if (!splitCandidates.Any())
                {
                    if (splitCandidates.Contains(origin))
                    {
                        splitCandidates.Enqueue(origin);
                    }
                }

                //If the queue hasn't changed in size this loop, up the kill counter.
                if (splitCandidates.Count == QueueCount)
                {
                    killCounter++;
                }

                //Update the count checker if queue size differs.
                else
                {
                    QueueCount = splitCandidates.Count;
                }

                if (killCounter > 20)
                {
                    killLoop = true;
                }
            }
            else
            {
                killLoop = true;
            }
        }

        //Output as list because fuck queues.
        List<Component> output = new();
        if (splitCandidates.Any())
        {
            output = splitCandidates.ToList();
        }
        return output;
    }

    /// <summary>
    /// Instantiate a number of new collections based off the final results from the split logic.
    /// </summary>
    private static void FinaliseSplitCollection(List<List<Component>> splits, List<Component> origins)
    {
        foreach (Component origin in origins)
        {
            foreach (List<Component> split in splits)
            {
                //Basic check to match split lists and root components.
                if (split.Contains(origin))
                {
                    AddMultipleComponentsToCollection(split, InstantiateNewCollection(origin));
                }
            }
        }
    }

    /// <summary>
    /// Make sure there's no bumd shit in the split algo output (hopefully)
    /// </summary>
    private static List<List<Component>> ValidateSplits(List<List<Component>> splits, Component killedComp, bool killComponent)
    {
        List<List<Component>> filter = new();
        foreach (List<Component> list in splits)
        {
            //Check split list has contents.:
            if (list.Any())
            {
                //Check if the list is a duplicate of one in the filter:
                if (filter.Any())
                {
                    var checker = new HashSet<Component>(list);
                    foreach (List<Component> filterList in filter)
                    {
                        if (!checker.SetEquals(filterList))
                        {
                            filter.Add(list);
                        }
                    }
                }
            }
        }
        return filter;
    }

    /// <summary>
    /// Check if a component should trigger a burndown
    /// </summary>
    private static bool CheckForBurndown(CompCollection parent, Component subject)
    {
        //If it's not in the collection for some reason, null, or is the root of the parent collection:
        if (!CollectionHasComponent(parent, subject) || subject is null || subject == parent.RootComponent)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if a component is a valid addition for a split:
    /// </summary>
    private static bool CheckForSplitQueue(Component subject, Component killedComp, Queue<Component> splitCandidates, bool killComponent)
    {
        //If not already in the split:
        if (!splitCandidates.Contains(subject))
        {
            //If it's not supposed to be a killed component:
            if (killComponent && subject != killedComp)
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Iteratively get all components attached to a subject component that are in the same collection.
    /// </summary>
    private static List<Component> GetAttachedComponents(CompCollection parent, Component subject)
    {
        List<Component> output = new();
        foreach (KeyValuePair<ConnectionPoint, ConnectionPoint> kvp in subject.ConDict)
        {
            if (kvp.Value != null && CollectionHasComponent(parent, (Component)kvp.Value.GetParent()))
            {
                output.Add((Component)kvp.Value.GetParent());
            }
        }
        return output;
    }

    /// <summary>
    /// Check if a collection has a certain component registered.
    /// </summary>
    private static bool CollectionHasComponent(CompCollection parent, Component subject)
    {
        if (parent.RootComponent == subject || parent.Components.Contains(subject))
        {
            return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// Create a new component and a container collection.
    /// </summary>
    public static Component InstantiateNewComponent(CompCollection parent)
    {
        PackedScene componentPointScene = GD.Load<PackedScene>("res://Components/Component.tscn");
		Component c = componentPointScene.Instantiate<Component>();
        AddChildDeferred(c, parent);
        c.Position = parent.Position;
        return c;
    }

    /// <summary>
    /// Create a new collection using origin as root.
    /// </summary>
    public static CompCollection InstantiateNewCollection(Component origin)
    {
        if (origin != null)
        {
            PackedScene collectionScene = GD.Load<PackedScene>("res://Components/CompCollection.tscn");
            CompCollection col = collectionScene.Instantiate<CompCollection>();
            AddComponentToCollection(origin, col);
            AddChildDeferred(col, CurrentScene);
            return col;
        }
        return null;
    }
}
