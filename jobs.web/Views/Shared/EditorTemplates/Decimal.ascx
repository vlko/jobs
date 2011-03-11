﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script runat="server">
	private object FormattedValue {
		get {
			if (ViewData.TemplateInfo.FormattedModelValue == ViewData.ModelMetadata.Model) {
				return String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:0.00}", ViewData.ModelMetadata.Model);
			}
			return ViewData.TemplateInfo.FormattedModelValue;
		}
	}
</script>
<div class="editor-label">
	<%: Html.LabelFor(model => model)%>
</div>
<div class="editor-field">
	<%= Html.TextBox("", ViewData.Model, cssClass: "text-box single-line")%>
	<%: Html.ValidationMessageFor(model => model)%>
	<% if (string.IsNullOrWhiteSpace(ViewData.ModelMetadata.Description)) {%>
	<span class="editor-hint">
		<%= ViewData.ModelMetadata.Description%>
	</span>
	<% } %>
</div>