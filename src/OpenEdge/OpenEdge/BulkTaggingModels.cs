using System;
using System.Collections.Generic;

namespace OpenEdge;

public enum BulkTagOperationMode
{
	Merge,
	RemoveSpecific,
	KeepOnlySelected,
	RemoveAll
}

public sealed class BulkTagCategoryDefinition
{
	public string Title { get; init; } = "";

	public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
}
