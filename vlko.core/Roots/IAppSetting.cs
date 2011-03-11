﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.Roots
{
	public interface IAppSetting
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		string Id { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		string Value { get; set; }
	}
}
