using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jobs.Data.Root;

namespace jobs.web.ViewModel.Home
{
	public class HomeViewModel
	{
		/// <summary>
		/// Gets or sets the people.
		/// </summary>
		/// <value>The people.</value>
		public Job[] People { get; set; }
		/// <summary>
		/// Gets or sets the projects.
		/// </summary>
		/// <value>The projects.</value>
		public Job[] Projects { get; set; }
	}
}