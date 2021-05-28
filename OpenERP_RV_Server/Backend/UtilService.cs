using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OpenERP_RV_Server.Models.PagedModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenERP_RV_Server.Backend
{
    public class UtilService
    {
        public static string GetAppSettingsConfiguration(string section, string property)
        {
            //https://stackoverflow.com/questions/31453495/how-to-read-appsettings-values-from-json-file-in-asp-net-core
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection(section)[property];
            return configuration;

        }

        /// <summary>
        /// Static method for get generic paged list model from T, where T is the final View Model for response
        /// </summary>
        /// <typeparam name="T">T is the View Model for response</typeparam>
        /// <typeparam name="V">V is the Entity Database Model (DbSet)</typeparam>
        /// <param name="itemsPerPage">itemsPerPage in Query</param>
        /// <param name="entityQueryData">Raw query data from table or DbSet</param>
        /// <param name="entityModelItems">Materialized list items for the pagedListModel</param>
        /// <returns></returns>
        public static PagedListModel<T> GetPagedEntityModel<T, V>(int itemsPerPage, IQueryable<V> entityQueryData, List<T> entityModelItems)
        {
            PagedListModel<T> response = new PagedListModel<T>();
            response.Items = entityModelItems;
            response.TotalItems = entityQueryData.Count();
            response.TotalPages = (int)Math.Ceiling(((double)response.TotalItems / (double)itemsPerPage));

            return response;
        }


        public static string ReadFormFileAsync(IFormFile file)
        {
            var xmlString = "";
            if (file == null || file.Length == 0)
            {
                return null;
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                xmlString = reader.ReadToEnd();
            }

            return xmlString;
        }

        public static T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }


    }
}
