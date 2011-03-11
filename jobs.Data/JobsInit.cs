using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Jobs.Data.Action;
using Jobs.Data.Index;
using Jobs.Data.Root;
using Raven.Client;
using vlko.BlogModule.RavenDB.Repository;
using vlko.core.InversionOfControl;
using vlko.core.Repository;

namespace Jobs.Data
{
	public class JobsInit
	{
		/// <summary>
		/// Registers the indexes.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		public static void RegisterIndexes(IDocumentStore documentStore)
		{
			new JobSortIndex().Execute(documentStore);
			new JobSearchIndex().Execute(documentStore);
		}

		/// <summary>
		/// Initializes the repositories.
		/// </summary>
		public static void InitializeRepositories()
		{
			IWindsorContainer container = IoC.Container;

			container.Register(
				Component.For<IRepository<Job>>().ImplementedBy<Repository<Job>>(),
				Component.For<JobAction>().ImplementedBy<JobAction>()
				);
		}

	}
}
