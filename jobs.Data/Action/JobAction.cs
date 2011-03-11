using System;
using System.Linq;
using Jobs.Data.Index;
using Jobs.Data.Root;
using vlko.BlogModule.RavenDB.Repository;
using vlko.core.Repository;

namespace Jobs.Data.Action
{
	public class JobAction : BaseAction<Job>
	{
		/// <summary>
		/// Gets the people.
		/// </summary>
		/// <returns>Query result.</returns>
		public IQueryResult<Job> GetPeople()
		{
			return new ProjectionAsQueryResult<Job, Job>(
				SessionFactory<Job>.IndexQuery<JobSortIndex>().Where(job => job.JobType == JobTypeEnum.People && job.ActivationToken == null))
				.AddSortMapping(job => job.CreateDate, job => job.CreateDate)
				.AddSortMapping(job => job.Title, job => job.Title)
				.AddSortMapping(job => job.Place, job => job.Place);
		}

		/// <summary>
		/// Gets the projects.
		/// </summary>
		/// <returns>Query result.</returns>
		public IQueryResult<Job> GetProjects()
		{
			return new ProjectionAsQueryResult<Job, Job>(
				SessionFactory<Job>.IndexQuery<JobSortIndex>().Where(job => job.JobType == JobTypeEnum.Project && job.ActivationToken == null))
				.AddSortMapping(job => job.CreateDate, job => job.CreateDate)
				.AddSortMapping(job => job.Title, job => job.Title)
				.AddSortMapping(job => job.Place, job => job.Place);
		}

		/// <summary>
		/// Loads the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public Job Load(string id)
		{
			return SessionFactory<Job>.Load(id);
		}

		/// <summary>
		/// Creates the specified job.
		/// </summary>
		/// <param name="job">The job.</param>
		public void Create(Job job)
		{
			job.Id = SessionFactory<Job>.GenerateId(job);
			job.CreateDate = DateTime.Now;
			job.ActivationToken = Guid.NewGuid().ToString();
			SessionFactory<Job>.Store(job);
		}

		/// <summary>
		/// Confirms the specified token.
		/// </summary>
		/// <param name="token">The token.</param>
		/// <returns></returns>
		public bool Confirm(string token)
		{
			var jobToConfirm = SessionFactory<Job>.IndexQuery<JobSortIndex>().Where(job => job.ActivationToken == token).FirstOrDefault();
			if (jobToConfirm != null)
			{
				jobToConfirm.ActivationToken = null;
				SessionFactory<Job>.Store(jobToConfirm);
				return true;
			}
			return false;
		}
	}
}
