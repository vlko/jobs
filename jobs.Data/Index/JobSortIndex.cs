using System.Linq;
using Jobs.Data.Root;
using Raven.Client.Indexes;

namespace Jobs.Data.Index
{
	public class JobSortIndex : AbstractIndexCreationTask<Job>
	{
		public JobSortIndex()
		{
			Map = items => from item in items
						   select new
									{
										item.CreateDate,
										item.Title,
										item.Description,
										item.Place,
										item.Prerequirements,
										item.JobType,
										item.ActivationToken,
										item.CloseToken
									};
		}
	}
}
