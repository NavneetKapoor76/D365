using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BDP.DPAM.Shared.Extension_Methods
{
	public static class IOrganizationServiceExtensions
	{

		/// <summary>
		/// Retrieve all records (more than 5000) using fetchXML.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="fetchXml">The fetchXML you want to execute.</param>
		/// <returns>1 EntityCollection containing all records resulting from the given fetchXML</returns>
		public static EntityCollection RetrieveAll(this IOrganizationService service, string fetchXml)
		{

			//the resulting set of all records
			EntityCollection totalResult = new EntityCollection();

			// Set the number of records per page to retrieve.
			int fetchCount = 5000;
			// Initialize the page number.
			int pageNumber = 1;
			// Specify the current paging cookie. For retrieving the first page, pagingCookie should be null.
			string pagingCookie = null;

			while (true)
			{
				//get the records for the current page

				EntityCollection pageResult = service.RetrievePage(fetchXml, pagingCookie, pageNumber, fetchCount);

				//add the pages records to the total result set
				totalResult.Entities.AddRange(pageResult.Entities);

				// Check for morerecords, if it returns 1.
				if (pageResult.MoreRecords)
				{
					// Increment the page number to retrieve the next page.
					pageNumber++;

					// Set the paging cookie to the paging cookie returned from current results.                            
					pagingCookie = pageResult.PagingCookie;
				}
				else
				{
					// If no more records in the result nodes, exit the loop.
					break;
				}
			}


			return totalResult;
		}

		/// <summary>
		/// Retrieve 1 page for a FetchXml
		/// </summary>
		/// <param name="service"></param>
		/// <param name="fetchXml">The fetchXML without paging information</param>
		/// <param name="cookie">the result paging cookie from the previous page (null for first page)</param>
		/// <param name="page">The page number you want to retrieve (1 based)</param>
		/// <param name="count">The number of records per page</param>
		/// <returns>An EntityCollection containing 1 page of the given FetchXML Result</returns>
		public static EntityCollection RetrievePage(this IOrganizationService service, string fetchXml, string cookie, int page, int count)
		{
			// Build fetchXml string with the placeholders.
			string xml = CreateXml(fetchXml, cookie, page, count);

			//get the records for the current page
			return service.RetrieveMultiple(new FetchExpression(xml));
		}

		/// <summary>
		/// Add paging information to a fetchXML (in string format)
		/// </summary>
		/// <param name="xml">The fetchXML on which you want to apply paging</param>
		/// <param name="cookie">the result paging cookie from the previous page (null for first page)</param>
		/// <param name="page">The page number you want to retrieve (1 based)</param>
		/// <param name="count">The number of records per page</param>
		/// <returns>A fetchXML in string format with paging information.</returns>
		private static string CreateXml(string xml, string cookie, int page, int count)
		{
			StringReader stringReader = new StringReader(xml);
			XmlTextReader reader = new XmlTextReader(stringReader);

			// Load document
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);

			return CreateXml(doc, cookie, page, count);
		}

		/// <summary>
		/// Add paging information to a fetchXML (in XmlDocument format)
		/// </summary>
		/// <param name="doc">The fetchXML on which you want to apply paging</param>
		/// <param name="cookie">the result paging cookie from the previous page (null for first page)</param>
		/// <param name="page">The page number you want to retrieve (1 based)</param>
		/// <param name="count">The number of records per page</param>
		/// <returns>A fetchXML in XmlDocument format with paging information.</returns>
		private static string CreateXml(XmlDocument doc, string cookie, int page, int count)
		{
			XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

			if (cookie != null)
			{
				XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
				pagingAttr.Value = cookie;
				attrs.Append(pagingAttr);
			}

			XmlAttribute pageAttr = doc.CreateAttribute("page");
			pageAttr.Value = System.Convert.ToString(page);
			attrs.Append(pageAttr);

			XmlAttribute countAttr = doc.CreateAttribute("count");
			countAttr.Value = System.Convert.ToString(count);
			attrs.Append(countAttr);

			StringBuilder sb = new StringBuilder(1024);
			StringWriter stringWriter = new StringWriter(sb);

			XmlTextWriter writer = new XmlTextWriter(stringWriter);
			doc.WriteTo(writer);
			writer.Close();

			return sb.ToString();
		}

		/// <summary>
		/// Retrieve all records (more than 5000) using QueryExpression.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="query">QueryExpression you want to execute</param>
		/// <returns>1 EntityCollection containing all records resulting from the given QueryExpression</returns>
		public static EntityCollection RetrieveAll(this IOrganizationService service, QueryExpression query)
		{
			//the resulting set of all records
			EntityCollection totalResult = new EntityCollection();

			// Set the number of records per page to retrieve.
			int queryCount = 5000;
			// Initialize the page number.
			int pageNumber = 1;
			// Specify the current paging cookie. For retrieving the first page, pagingCookie should be null.
			string pagingCookie = null;

			while (true)
			{
				//get the records for the current page
				EntityCollection pageResult = service.RetrievePage(query, pagingCookie, pageNumber, queryCount);

				//add the pages records to the total result set
				totalResult.Entities.AddRange(pageResult.Entities);

				// Check for more records, if it returns true.
				if (pageResult.MoreRecords)
				{
					// Increment the page number to retrieve the next page.
					pageNumber++;

					// Set the paging cookie to the paging cookie returned from current results.
					pagingCookie = pageResult.PagingCookie;
				}
				else
				{
					// If no more records are in the result nodes, exit the loop.
					break;
				}
			}

			return totalResult;
		}

		/// <summary>
		/// Retrieve 1 page for a QueryExpression
		/// </summary>
		/// <param name="service"></param>
		/// <param name="query">The QueryExpression to execute</param>
		/// <param name="cookie">the result paging cookie from the previous page (null for first page)</param>
		/// <param name="page">The page number you want to retrieve (1 based)</param>
		/// <param name="count">The number of records per page</param>
		/// <returns>An EntityCollection containing 1 page of the given QueryExpression Result</returns>
		public static EntityCollection RetrievePage(this IOrganizationService service, QueryExpression query, string cookie, int page, int count)
		{
			if (query.PageInfo == null)
				query.PageInfo = new PagingInfo();

			query.PageInfo.Count = count;
			query.PageInfo.PageNumber = page;
			query.PageInfo.PagingCookie = cookie;

			return service.RetrieveMultiple(query);
		}
	}
}
