using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Jobs.Data.Action;
using Jobs.Data.Root;
using Jobs.Data.Root.Includes;
using jobs.web.ViewModel.Contact;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Services;
using vlko.core.ValidationAtribute;

namespace jobs.web.Controllers
{
	public class ProjectController : BaseController
	{
		/// <summary>
		/// URL: /Project/Index
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Index(PagedModel<Job> pageModel)
		{
			pageModel.LoadData(RepositoryFactory.Action<JobAction>()
				.GetProjects()
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
			var offer = RepositoryFactory.Action<JobAction>().Load("jobs/" + id);
			return ViewWithAjax(offer);
		}

		/// <summary>
		/// URL: /Project/Confirm
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Confirm(string id)
		{
			bool confirmed = false;
			using (var tran = RepositoryFactory.StartTransaction())
			{
				confirmed = RepositoryFactory.Action<JobAction>().Confirm(id);
				tran.Commit();
			}
			if (confirmed)
			{
				return View("Confirmed");
			}
			return new HttpNotFoundResult();
		}

		/// <summary>
		/// URL: /People/Close
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Close(string id)
		{
			var job = RepositoryFactory.Action<JobAction>().GetJobByCloseToken(id);
			if (job != null)
			{
				return View(job);
			}
			return new HttpNotFoundResult();
		}

		/// <summary>
		/// URL: /People/Close [post]
		/// </summary>
		/// <param name="job">The job.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult Close(Job job)
		{
			bool closed = false;
			using (var tran = RepositoryFactory.StartTransaction())
			{
				closed = RepositoryFactory.Action<JobAction>().CloseJob(job.Id);
				tran.Commit();
			}
			if (closed)
			{
				return View("CloseOk");
			}
			return new HttpNotFoundResult();
		}

		/// <summary>
		/// URL: /Project/Contact
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Contact(string id)
		{
			var job = RepositoryFactory.Action<JobAction>().Load("jobs/" + id);
			if (job != null)
			{
				return ViewWithAjax(new ContactModel { JobId = job.SimpleId });
			}
			return new HttpNotFoundResult();
		}

		/// <summary>
		/// URL: /Project/Contact
		/// </summary>
		/// <param name="contact">The contact.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		[AntiXss]
		public ActionResult Contact(ContactModel contact)
		{
			if (ModelState.IsValid)
			{
				var job = RepositoryFactory.Action<JobAction>().Load("jobs/" + contact.JobId);
				if (job == null)
				{
					return new HttpNotFoundResult();
				}
				if (SendContactEmail(contact, job))
				{
					return ViewWithAjax("ContactOk");
				}
				return ViewWithAjax("ContactFail");
			}
			return ViewWithAjax(contact);
		}

		/// <summary>
		/// URL: /Project/Create
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Create()
		{
			var job = new Job
						{
							Properties = new PropertyInfo[] {}
						};
			return ViewWithAjax(job);
		}

		/// <summary>
		/// URL: /Project/Create
		/// </summary>
		/// <param name="job">The job.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		[AntiXss]
		public ActionResult Create(Job job)
		{
			if (job.Properties == null)
			{
				job.Properties = new PropertyInfo[] {};
			}
			if (Request.Form["AddProperty"] != null)
			{
				var propertyList = job.Properties.ToList();
				propertyList.Add(new PropertyInfo());
				job.Properties = propertyList.ToArray();
			}
			else if (GetRemoveItemIndex() >= 0)
			{
				var removeItemIndex = GetRemoveItemIndex();
				var propertyList = job.Properties.ToList();

				RemoveFromModelState(removeItemIndex, propertyList.Count);

				propertyList.RemoveAt(removeItemIndex);
				job.Properties = propertyList.ToArray();
			}
			else if (ModelState.IsValid)
			{
				using (var tran = RepositoryFactory.StartTransaction())
				{
					job.JobType = JobTypeEnum.Project;
					RepositoryFactory.Action<JobAction>().Create(job);
					tran.Commit();
				}
				if (SendNotificationEmail(job))
				{
					return ViewWithAjax("SendOk");
				}
				return ViewWithAjax("SendFail");
			}
			return ViewWithAjax(job);
		}

		/// <summary>
		/// Sends the contact email.
		/// </summary>
		/// <param name="contact">The contact.</param>
		/// <param name="job">The job.</param>
		private bool SendContactEmail(ContactModel contact, Job job)
		{
			string mailTemplate = RenderPartialViewToString("ContactMail", contact);
			var match = Regex.Match(mailTemplate, "<email>(.*)</email>", RegexOptions.Singleline);
			var mailText = match.Groups[1].Value;
			// send email to creator
			return SendEmail(job.Email, "[jobs.preweb.sk] Odpoved od " + contact.Email, mailText);
		}

		/// <summary>
		/// Sends the notification email.
		/// </summary>
		/// <param name="job">The job.</param>
		private bool SendNotificationEmail(Job job)
		{
			string mailTemplate = RenderPartialViewToString("SendMail", job);
			var match = Regex.Match(mailTemplate, "<email>(.*)</email>", RegexOptions.Singleline);
			var mailText = match.Groups[1].Value;
			// send email to creator
			return SendEmail(job.Email, "[jobs.preweb.sk] Potvrdenie registracie", mailText);
		}

		/// <summary>Sends the email.</summary>
		/// <param name="recipient">The recipient.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="mailText">The mail text.</param>
		/// <returns>True if success; otherwise false.</returns>
		private bool SendEmail(string recipient, string subject, string mailText)
		{
			var mailService = IoC.Resolve<IEmailService>();
			return mailService.Send(recipient, subject, mailText, true);
		}

		/// <summary>
		/// Renders the partial view to string.
		/// </summary>
		/// <param name="viewName">Name of the view.</param>
		/// <param name="model">The model.</param>
		/// <returns>Partial view as text.</returns>
		protected string RenderPartialViewToString(string viewName, object model)
		{
			if (string.IsNullOrEmpty(viewName))
				viewName = ControllerContext.RouteData.GetRequiredString("action");

			ViewData.Model = model;

			using (StringWriter sw = new StringWriter())
			{
				ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
				ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
				viewResult.View.Render(viewContext, sw);

				return sw.GetStringBuilder().ToString();
			}
		}


		/// <summary>
		/// Removes the ModelState.
		/// </summary>
		/// <param name="removeItemIndex">Index of the remove item.</param>
		/// <param name="count">The count.</param>
		private void RemoveFromModelState(int removeItemIndex, int count)
		{
			for (int i = removeItemIndex; i < count; i++)
			{
				ModelState["Properties[" + i + "].Key"] = ModelState["Properties[" + (i + 1) + "].Key"];
				ModelState["Properties[" + i + "].Value"] = ModelState["Properties[" + (i + 1) + "].Value"];
			}
			ModelState.Remove("Properties[" + (count - 1) + "].Key");
			ModelState.Remove("Properties[" + (count - 1) + "].Value");
		}

		/// <summary>
		/// Gets the index of the remove item.
		/// </summary>
		/// <returns>Index of remove item or -1 if not found.</returns>
		private int GetRemoveItemIndex()
		{
			foreach (var formKey in Request.Form.AllKeys)
			{
				if (formKey.StartsWith("RemoveProperty_"))
				{
					return int.Parse(formKey.Replace("RemoveProperty_", string.Empty));
				}
			}
			return -1;
		}
	}
}
