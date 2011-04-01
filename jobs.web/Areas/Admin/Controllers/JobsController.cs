using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jobs.Data.Action;
using Jobs.Data.Root;
using vlko.core;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.Repository;
using vlko.core.ValidationAtribute;

namespace vlko.web.Areas.Admin.Controllers
{
	[AuthorizeRoles(Settings.AdminRole)]
	[AreaCheck("Admin")]
	public class JobsController : BaseController
	{
		/// <summary>
		/// URL: /Project/Index
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Index(PagedModel<Job> pageModel)
		{
			pageModel.LoadData(RepositoryFactory.Action<JobAction>()
				.GetAll()
				.OrderByDescending(item => item.CreateDate));
			return ViewWithAjax(pageModel);
		}

		/// <summary>
		/// URL: /Project/Details
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Details(string id)
		{
			var job = RepositoryFactory.Action<JobAction>().Load("jobs/" + id);
			return ViewWithAjax(job);
		}

		/// <summary>
		/// URL: /Jobs/Activate
		/// </summary>
		/// <param name="activationToken">The activation token.</param>
		/// <returns>Action result.</returns>
		public ActionResult Activate(string activationToken)
		{
			bool activated = false;
			using (var tran = RepositoryFactory.StartTransaction())
			{
				activated = RepositoryFactory.Action<JobAction>().Confirm(activationToken);
				tran.Commit();
			}
			if (activated)
			{
				return RedirectToAction("Index");
			}
			return new HttpNotFoundResult();
		}

		/// <summary>
		/// URL: /Jobs/Deactivate
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Deactivate(string id)
		{
			bool deactivated = false;
			using (var tran = RepositoryFactory.StartTransaction())
			{
				deactivated = RepositoryFactory.Action<JobAction>().Deactivate("jobs/" + id);
				tran.Commit();
			}
			if (deactivated)
			{
				return RedirectToAction("Index");
			}
			return new HttpNotFoundResult();
		}

		/// <summary>
		/// URL: /Jobs/GenerateCloseToken
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult GenerateCloseToken(string id)
		{
			using (var tran = RepositoryFactory.StartTransaction())
			{
				RepositoryFactory.Action<JobAction>().GenerateCloseToken("jobs/" + id);
				tran.Commit();
			}
			return RedirectToAction("Index");
		}

		/// <summary>
		/// URL: /Jobs/OpenJob
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult OpenJob(string id)
		{
			using (var tran = RepositoryFactory.StartTransaction())
			{
				RepositoryFactory.Action<JobAction>().OpenJob("jobs/" + id);
				tran.Commit();
			}
			return RedirectToAction("Index");
		}
	}
}