﻿using System;
using Raven.Client;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Repository
{
	/// <summary>
	/// Session implementation for Active record.
	/// </summary>
	public sealed class UnitOfWork : IUnitOfWork
	{
		public IDocumentStore DocumentStoreInstance { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWork"/> class.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		public UnitOfWork(IDocumentStore documentStore)
		{
			DocumentStoreInstance = documentStore;
			SessionFactory.RegisterUnitOfWork(this);
		}

		/// <summary>
		/// Gets or sets the unit of work context.
		/// </summary>
		/// <value>The unit of work context.</value>
		public IUnitOfWorkContext UnitOfWorkContext { get; private set; }

		/// <summary>
		/// Inits the unit of work context.
		/// </summary>
		/// <param name="unitOfWorkContext">The unit of work context.</param>
		public void InitUnitOfWorkContext(IUnitOfWorkContext unitOfWorkContext)
		{
			UnitOfWorkContext = unitOfWorkContext;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="UnitOfWork"/> is reclaimed by garbage collection.
		/// </summary>
		~UnitOfWork()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				SessionFactory.UnregisterUnitOfWork(this);
				if (UnitOfWorkContext != null)
				{
					UnitOfWorkContext.Dispose();
				}
			}   
		}
	}
}


