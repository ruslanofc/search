using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Search.Crawler
{
	class Program
	{
		private static async Task Main(string[] args)
		{
			var listUrls = GetUrls();
			var configuration = Configuration.Default.WithDefaultLoader();
			List<IDocument> documents = new List<IDocument>();

			foreach (var item in listUrls)
			{
				var document = await BrowsingContext
					.New(configuration)
					.OpenAsync(item);
				documents.Add(document);
			}

			SaveHtml(documents);
		}

		private static List<string> GetUrls()
		{
			string textFromFile;
			using (FileStream fstream = File.OpenRead(@"C:\Users\Microsoft\source\repos\Search\urls.txt"))
			{
				byte[] array = new byte[fstream.Length];
				fstream.Read(array, 0, array.Length);
				textFromFile = System.Text.Encoding.Default.GetString(array);
			}

			return textFromFile.Split('\n').ToList();
		}

		private static void SaveHtml(List<IDocument> documents)
		{
			var index = 0;
			foreach (var item in documents)
			{
				var currentFilePath = Path.Combine(@"C:\Users\Microsoft\source\repos\Search\выкачка\", $"{index}.txt");
				var awd = item.All
				.Where(ElementPredicate)
				.Select(element => element.Flatten())
				.Distinct()
				.Where(element => !string.IsNullOrWhiteSpace(element.TextContent))
				.Select(element => Regex.Replace(element.TextContent, @"\n+", "\n"))
				.Select(text => Regex.Replace(text, @"\t+", "\t"))
				.ToList();
				File.AppendAllLines(currentFilePath, awd);
				index++;
			}
		}

		private static bool ElementPredicate(IElement element) 
		{
			=> element is not(IHtmlScriptElement || IHtmlHtmlElement
				   || IHtmlHeadElement
				   || IHtmlBodyElement
				   || IHtmlStyleElement
				   || IHtmlDivElement)
			   && element.GetType().Name != "HtmlSemanticElement"
			   && element.HasZeroOrOneChild();
		}
	}
}
