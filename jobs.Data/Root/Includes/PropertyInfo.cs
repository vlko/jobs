using System.ComponentModel.DataAnnotations;

namespace Jobs.Data.Root.Includes
{
	public class PropertyInfo
	{
		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		[Required(ErrorMessage = "Popis nezadaný.")]
		[StringLength(50, ErrorMessage = "Maximálna dĺžka 50 znakov.")]
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the value of property.
		/// </summary>
		/// <value>The value of property.</value>
		[Required(ErrorMessage = "Hodnota nezadaná.")]
		[StringLength(50, ErrorMessage = "Maximálna dĺžka 50 znakov.")]
		public string Value { get; set; }

	}
}
