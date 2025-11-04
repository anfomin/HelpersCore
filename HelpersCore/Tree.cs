using System.Collections.Immutable;

namespace HelpersCore;

/// <summary>
/// Provides tree-related helper methods.
/// </summary>
public static class Tree
{
	/// <summary>
	/// Creates tree.
	/// </summary>
	/// <param name="childrenSelector">Functions that returns children for specified parent.</param>
	public static Tree<T> Create<T>(Func<T?, IEnumerable<T>> childrenSelector)
		where T : class
	{
		Tree<T> tree = new();
		PopulateChildren(tree, null, childrenSelector);
		return tree;
	}

	static void PopulateChildren<T>(Tree<T> tree, T? parent, Func<T?, IEnumerable<T>> childrenSelector)
		where T : class
	{
		foreach (var item in childrenSelector(parent))
		{
			var node = new Tree<T>.Node(item);
			tree.Children.Add(node);
			PopulateChildren(node, item, childrenSelector);
		}
	}

	/// <summary>
	/// Creates linearized tree from random-sorted list.
	/// </summary>
	/// <param name="childrenSelector">Functions that returns children for specified parent.</param>
	public static List<Node<T>> List<T>(Func<T?, IEnumerable<T>> childrenSelector)
		where T : class
	{
		List<Node<T>> list = [];
		Stack<T> parentsStack = [];
		Add(default);
		return list;

		void Add(T? parent)
		{
			foreach (var item in childrenSelector(parent))
			{
				list.Add(new (item, parent, parentsStack.ToImmutableArray()));
				parentsStack.Push(item);
				Add(item);
				parentsStack.Pop();
			}
		}
	}

	/// <summary>
	/// Enumerates all parents while parent selector return non-null value.
	/// </summary>
	/// <param name="parentSelector">Function that returns parent for specified child.</param>
	public static IEnumerable<T> EnumerateParents<T>(Func<T?, T?> parentSelector)
		where T : class
	{
		T? curr = null;
		while (parentSelector(curr) is { } parent)
		{
			yield return parent;
			curr = parent;
		}
	}

	/// <summary>
	/// Represents tree node with item of <typeparamref name="T"/> and its parents.
	/// </summary>
	/// <param name="Item">Node item.</param>
	/// <param name="Parent">Node parent.</param>
	/// <param name="Parents">Node parents array from top to bottom.</param>
	public record struct Node<T>(T Item, T? Parent, ImmutableArray<T> Parents);
}

/// <summary>
/// Represents tree structure of <typeparamref name="T"/>.
/// </summary>
public class Tree<T>
	where T : class
{
	/// <summary>
	/// Gets tree children.
	/// </summary>
	public List<Node> Children { get; } = [];

	/// <summary>
	/// Recursively finds tree item.
	/// </summary>
	/// <param name="predicate">Predicate to match tree item.</param>
	public Node? Find(Func<T, bool> predicate)
	{
		foreach (var child in Children)
		{
			if (predicate(child.Item))
				return child;
			if (child.Find(predicate) is { } sub)
				return sub;
		}
		return null;
	}

	/// <summary>
	/// Recursively enumerates subtree.
	/// </summary>
	public IEnumerable<(Node Node, T? Parent, T[] Parents)> Subtree()
	{
		foreach (var child in Children)
		{
			T? parent = this is Node p ? p.Item : null;
			yield return (child, parent, parent == null ? [] : [parent]);
			foreach (var (childNode, childParent, childParents) in child.Subtree())
			{
				var childParents2 = parent == null ? childParents : [..childParents, parent];
				yield return (childNode, childParent, childParents2);
			}
		}
	}

	/// <summary>
	/// Represents tree node containing item of <typeparamref name="T"/>.
	/// </summary>
	/// <param name="item">Tree item.</param>
	public class Node(T item) : Tree<T>
	{
		/// <summary>
		/// Gets or sets tree item.
		/// </summary>
		public T Item { get; set; } = item;
	}
}