using System;
using System.ComponentModel.DataAnnotations;
using Jobs.Data.Root.Includes;
using Microsoft.Web.Mvc;

namespace Jobs.Data.Root
{
	public class Job
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[Key]
		public string Id { get; set; }

		/// <summary>
		/// Gets the simple id.
		/// </summary>
		[Editable(false)]
		public string SimpleId
		{
			get
			{
				if (!string.IsNullOrEmpty(Id))
				{
					var split = Id.Split('/');
					if (split.Length == 2)
					{
						return split[1];
					}
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the type of the job.
		/// </summary>
		/// <value>The type of the job.</value>
		[Editable(false)]
		public JobTypeEnum JobType { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		[Display(Name = "Nadpis")]
		[Required(ErrorMessage = "Nutné zadať")]
		[StringLength(80, ErrorMessage = "Maximálna dĺžka je 80 znakov")]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		[Display(Name = "Email")]
		[Required(ErrorMessage = "Nutné zadať")]
		[EmailAddress(ErrorMessage = "Zadajte platný email")]
		[StringLength(80, ErrorMessage = "Maximálna dĺžka je 80 znakov")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the place.
		/// </summary>
		/// <value>The place.</value>
		[Display(Name = "Miesto")]
		[Required(ErrorMessage = "Nutné zadať")]
		[StringLength(80, ErrorMessage = "Maximálna dĺžka je 80 znakov")]
		public string Place { get; set; }

		/// <summary>
		/// Gets or sets the pre-requirements.
		/// </summary>
		/// <value>The pre-requirements.</value>
		[Display(Name = "Požiadavky")]
		[Required(ErrorMessage = "Nutné zadať")]
		[StringLength(80, ErrorMessage = "Maximálna dĺžka je 80 znakov")]
		public string Prerequirements { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[Display(Name = "Popis")]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the properties of job.
		/// </summary>
		/// <value>The properties of person.</value>
		public PropertyInfo[] Properties { get; set; }

		/// <summary>
		/// Gets or sets the create date.
		/// </summary>
		/// <value>The create date.</value>
		public DateTime CreateDate { get; set; }

		/// <summary>
		/// Gets or sets the activation token.
		/// </summary>
		/// <value>The activation token.</value>
		public string ActivationToken { get; set; }

	}
}
