using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Demo.UploadEntityImage
{
    public class Upload : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            OrganizationServiceContext orgContext = new OrganizationServiceContext(service);

            Entity entity = (Entity)context.InputParameters["Target"];
            tracer.Trace(entity.GetAttributeValue<string>("ldv_photourl"));
            var url = entity.GetAttributeValue<string>("ldv_photourl");

            StringBuilder _sb = new StringBuilder();

            Byte[] _byte = this.GetImage(url);

            //_sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));

            entity["entityimage"] = _byte; //File.ReadAllBytes(_sb.ToString());

            service.Update(entity);
        }
        private byte[] GetImage(string url)
        {
            Stream stream = null;
            byte[] buf;

            try
            {
                WebProxy myProxy = new WebProxy();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                stream = response.GetResponseStream();

                using (BinaryReader br = new BinaryReader(stream))
                {
                    int len = (int)(response.ContentLength);
                    buf = br.ReadBytes(len);
                    br.Close();
                }

                stream.Close();
                response.Close();
            }
            catch (Exception exp)
            {
                buf = null;
            }

            return (buf);
        }
    }
}

