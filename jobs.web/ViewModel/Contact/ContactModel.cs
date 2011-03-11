using System.ComponentModel.DataAnnotations;
using Microsoft.Web.Mvc;

namespace jobs.web.ViewModel.Contact
{
	public class ContactModel
	{
		/// <summary>
		/// Gets or sets the job id.
		/// </summary>
		/// <value>The job id.</value>
		[Key]
		public string JobId { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		[Display(Name = "Kontaktný email")]
		[Required(ErrorMessage = "Nie je zadané")]
		[EmailAddress(ErrorMessage = "Zadajte platný email")]
		[StringLength(50, ErrorMessage = "Maximálna dĺžka je 80 znakov")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		[Display(Name = "Text správy")] 
		[Required(ErrorMessage = "Nie je zadané")]
		[DataType(DataType.MultilineText)]
		public string Text { get; set; }
	}
}