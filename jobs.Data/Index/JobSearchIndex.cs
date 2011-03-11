using System.Linq;
using Jobs.Data.Root;
using Raven.Client.Indexes;
using Raven.Database.Indexing;

namespace Jobs.Data.Index
{
	public class JobSearchIndex : AbstractIndexCreationTask<Job>
	{
		public JobSearchIndex()
		{
			Map = items => from item in items
						   where item.ActivationToken == null
			               select new
			                      	{
			                      		item.Title,
			                      		item.Description,
			                      		item.Place,
			                      		item.Prerequirements,
			                      	};
			Indexes.Add(job => job.Title, FieldIndexing.Analyzed);
			Indexes.Add(job => job.Description, FieldIndexing.Analyzed);
			Indexes.Add(job => job.Prerequirements, FieldIndexing.Analyzed);
		}
	}
}