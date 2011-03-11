using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Security.Application;

namespace vlko.core.ValidationAtribute
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class AntiXssAttribute : ActionFilterAttribute, IAuthorizationFilter
	{
		private static readonly AntiXssHtmlTextAttribute AntiXssHtmlText = new AntiXssHtmlTextAttribute();
		/// <summary>
		/// Processes the property.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="property">The property.</param>
		private static void ProcessProperty(object value, PropertyDescriptor property)
		{
			if (property.Attributes.Contains(AntiXssHtmlText))
			{
				property.SetValue(value, AntiXss.GetSafeHtmlFragment((string) property.GetValue(value)));
			}
		}

		/// <summary>
		/// Called by the MVC framework before the action method executes.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			foreach (KeyValuePair<string, object> actionParameter in filterContext.ActionParameters.ToArray())
			{
				var value = actionParameter.Value;
				if (value is string)
				{
					// do nothing this contains text not affected for xss attack
				}
				else
				{
					SecureProperties(value);
				}
			}
		}

		/// <summary>
		/// Secures the properties.
		/// </summary>
		/// <param name="value">The value.</param>
		private void SecureProperties(object value)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
			foreach (PropertyDescriptor property in properties)
			{
				if (property.PropertyType == typeof(string))
				{
					ProcessProperty(value, property);
				}
				else if (property.PropertyType.GetInterface("IEnumerable") != null)
				{
					var propValue = property.GetValue(value);
					if (propValue != null)
					{
						foreach (object item in (IEnumerable)propValue)
						{
							SecureProperties(item);
						}
					}
				}
			}
		}

		/// <summary>
		/// Called when authorization is required.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
			{
				throw new ArgumentNullException("filterContext");
			}
			// disable default validation request
			filterContext.Controller.ValidateRequest = false;
		}
	}
}
