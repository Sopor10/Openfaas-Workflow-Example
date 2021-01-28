using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Function
{
	public class TypedHandler
	{
		public async Task<Output> Handle(Input input) 
		{
			input = input ?? throw new ArgumentNullException();

			var document = new HtmlDocument();
			
			document.LoadHtml(input.Html);
			var tables = ScrapHtmlTable(document);



			return new Output() {Table = tables.Skip(input.Number).FirstOrDefault()};
		}

		public IEnumerable<Table> ScrapHtmlTable(HtmlDocument document)
		{

			var parsedTbl =
				document.DocumentNode.SelectNodes("//table")
					.Select(Table.FromHtml);

			return parsedTbl;
		}
	}

	public record Table
	{
		public ImmutableList<string> Header { get; init; }
		
		public ImmutableList<ImmutableList<string>> Data { get; init; }

		public static Table FromHtml(HtmlNode x)
		{
			var data = x
				.Descendants("tr")
				.Skip(1) //To Skip Table Header Row
				.Where(tr => tr.Elements("td").Count() > 1)
				.Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToImmutableList())
				.ToList();
			var header = x
				.Descendants("tr")
				.Where(tr => tr.Elements("td").Count() > 1)
				.Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToImmutableList())
				.First();
			return new Table() {Header = header, Data = data.ToImmutableList() };
		}
	}
}