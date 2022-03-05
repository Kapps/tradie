﻿using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Transactions;

namespace Tradie.Common {
	/// <summary>
	/// Extension methods revolving around managing DbContext instances.
	/// </summary>
	public static class DbContextExtensions {
		/// <summary>
		/// Returns a connection being used by this DbContext.
		/// If the connection is not currently open, it will be opened before being returned.
		/// </summary>
		public static async Task<T> GetOpenedConnection<T>(this DbContext context, CancellationToken cancellationToken = default) where T : DbConnection {
			var conn = (T)context.Database.GetDbConnection();
			if(conn.State == ConnectionState.Closed) {
				Console.WriteLine("Opening Async");
				await conn.OpenAsync(cancellationToken);
				conn.EnlistTransaction(Transaction.Current);
			}

			return conn;
		} 
	}
}