using Izenda.AdHoc;

namespace Duncan.PEMS.Web.Reporting
{
	public class AdditionalFilter : Filter
	{
		public AdditionalFilter() {}

		public AdditionalFilter(Filter filter) {
			AliasTable = filter.AliasTable;
			Description = filter.Description;
			FieldFilter = filter.FieldFilter;
			ForceQuotes = filter.ForceQuotes;
			Hidden = filter.Hidden;
			Not = filter.Not;
			Or = filter.Or;
			OrIsBlank = filter.OrIsBlank;
			Parameter = filter.Parameter;
			ParensAfter = filter.ParensAfter;
			ParensBefore = filter.ParensBefore;
			SqlOverride = filter.SqlOverride;
			Table = filter.Table;
			Value = filter.Value;
			Values = filter.Values;
			Column = filter.Column;
			dbColumn = filter.dbColumn;
			LinkedColumns = filter.LinkedColumns;
		}
	}
}