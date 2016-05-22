using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RESTService.Lib
{
    [ServiceContract(Name = "RESTPongServices")]
    public interface IRESTPongServices
    {
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetEstadisticasRoute, BodyStyle = WebMessageBodyStyle.Bare)]
        string GetEstadisticas();


        [OperationContract]
        [WebGet(UriTemplate = Routing.GetInitiate, BodyStyle = WebMessageBodyStyle.Bare)]
        void Iniciar();
    }
}
