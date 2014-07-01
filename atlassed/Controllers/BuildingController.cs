using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Atlassed.Controllers
{
    [RoutePrefix("api/buildings")]
    public class BuildingsController : ApiController
    {
        [Route("{buildingId:int}/floors")]
        public List<FloorMap> GetFloors(int buildingId)
        {
            return FloorMap.GetAllFloors(buildingId);
        }

        public Building Get(int id)
        {
            return Building.GetBuilding(id);
        }

        public Building Post(NewBuilding building)
        {
            return Building.Create(building, building.EntityPoints);
        }

        public Building Put([FromBody]Building building)
        {
            return Building.Update(building);
        }

        public bool Delete(int id)
        {
            return Building.GetBuilding(id).Delete();
        }
    }

    public class NewBuilding : Building
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EntityPoints { get; set; }
    }

    public class MetaObjectModelBinder : IModelBinder
    {
        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var modelType = bindingContext.ModelType;
            if (!modelType.IsSubclassOf(typeof(MetaObject)))
            {
                return false;
            }

            var bodyString = actionContext.Request.Content.ReadAsFormDataAsync().Result;
            var result = Activator.CreateInstance(bindingContext.ModelType);
            foreach (KeyValuePair<string, string> _pair in bodyString)
            {
                modelType.GetProperty(_pair.Key).SetValue(result, _pair.Value);
            }

            return true;
        }
    }
}