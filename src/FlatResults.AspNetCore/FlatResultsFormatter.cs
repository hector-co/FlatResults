using System;
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
        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings
            = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public FlatResultsFormatter()
        {
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
                document = DocumentExtensions.ToDocument(context.Object as dynamic);

            var response = context.HttpContext.Response;
            await response.WriteAsync((string)JsonConvert.SerializeObject(document, DefaultJsonSerializerSettings));
        }
    }
}
