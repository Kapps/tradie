﻿using System;
namespace Tradie.Common.RawModels {
    /// <summary>
	/// League is the raw information for a timed or permanent league.
	/// </summary>
	/// <param name="Name">The unique name for the league.</param>
    public readonly record struct League(string Name);
}