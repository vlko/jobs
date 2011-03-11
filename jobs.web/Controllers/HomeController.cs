using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jobs.Data.Action;
using jobs.web.ViewModel.Home;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.Repository;
using vlko.BlogModule.Action;

namespace vlko.web.Controllers
{
	[HandleError]
	public class HomeController : BaseController
	{
		/// <summary>
		/// URL: Home/Index
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Index()
		{
			return View(new HomeViewModel
			                    	{
										People = RepositoryFactory.Action<JobAction>().GetPeople().ToPage(0, 20),
										Projects = RepositoryFactory.Action<JobAction>().GetProjects().ToPage(0, 20),
			                    	});
		}
	}
}
