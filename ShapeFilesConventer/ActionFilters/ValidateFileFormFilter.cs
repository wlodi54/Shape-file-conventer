using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;


namespace ShapeFilesConventer.ActionFilters
{
    public class ValidateFileFormFilter :ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
            
            if (!actionContext.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var request = actionContext.Request;
            var provider = new MultipartMemoryStreamProvider();
            if (actionContext.Request.Content.Headers.ContentLength>0)
            {
                request.Content.ReadAsMultipartAsync(provider);

                foreach (var providerContent in provider.Contents)
                {
                    if (!providerContent.Headers.ContentType.MediaType.Equals("application/x-zip-compressed"))
                    {
                        throw new UnsupportedMediaTypeException(".zip file required", MediaTypeHeaderValue.Parse("application/x-zip-compressed"));
                    }
                }

                



            }

        }
    }
}