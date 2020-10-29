using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatResults.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlatResults.AspNetCore
{
    public class FlatResultsFormatter : TextOutputFormatter
    {
        const char ListSeparator = ',';

        private readonly string _fieldsQueryStringParameter;

        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings
            = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public FlatResultsFormatter(string fieldsQueryStringParameter)
        {
            _fieldsQueryStringParameter = fieldsQueryStringParameter;
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x.flatresults+json"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            return DocumentMapperConfig.IsValidType(type);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (selectedEncoding == null)
            {
                throw new ArgumentNullException(nameof(selectedEncoding));
            }

            Document document;
            if (context.Object is Document)
                document = context.Object as Document;
            else
            {
                document = DocumentExtensions.ToDocument(context.Object as dynamic, fields: GetFields(context.HttpContext));
            }

            var response = context.HttpContext.Response;
            await response.WriteAsync(JsonConvert.SerializeObject(document, DefaultJsonSerializerSettings));
        }

        private IEnumerable<string> GetFields(HttpContext context)
        {
            if (!context.Request.Query.ContainsKey(_fieldsQueryStringParameter))
                return null;

            return context.Request.Query[_fieldsQueryStringParameter].ToString().Split(ListSeparator).Select(s => s.Trim());
        }
    }
}
