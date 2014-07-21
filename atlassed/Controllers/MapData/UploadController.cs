using Atlassed.Models.MapData;
using Atlassed.Repositories;
using Atlassed.Repositories.MapData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    [RoutePrefix("api/upload")]
    public class UploadController : SinglePageAppApiController
    {
        private IExistenceRepository<int> _mapEntityClassRepository;
        private IExistenceRepository<int> _mapRepository;

        public UploadController(SqlConnectionFactory f)
        {
            _mapEntityClassRepository = new MapEntityClassRepository(f, new MapEntityClassValidator(new MetaClassValidator()));
            _mapRepository = new MapRepository(f);
        }

        [Route("images/map/{mapId}/exists")]
        public bool GetMapImageExists(int mapId)
        {
            return File.Exists(HttpContext.Current.Server.MapPath(string.Format(Map.MapImagePath, mapId)));
        }

        [Route("images/map/{mapId}")]
        public HttpResponseMessage PostMap(int mapId)
        {
            if (!_mapRepository.RecordExists(mapId))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return UploadFile(
                string.Format(Map.MapImagePath, mapId),
                Map.MaxMapImageFileSize,
                Map.AllowedMapImageFileTypes
            );
        }

        [Route("images/entityClassIcon/{classId}/exists")]
        public bool GetEntityIconImageExists(int classId)
        {
            return File.Exists(HttpContext.Current.Server.MapPath(string.Format(MapEntityClass.EntityIconPath, classId)));
        }

        [Route("images/entityClassIcon/{classId}")]
        public HttpResponseMessage PostEntityIcon(int classId)
        {
            if (!_mapEntityClassRepository.RecordExists(classId))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return UploadFile(
                string.Format(MapEntityClass.EntityIconPath, classId),
                MapEntityClass.MaxEntityIconFileSize,
                MapEntityClass.AllowedEntityIconFileTypes
            );
        }

        private HttpResponseMessage UploadFile(string relativePath, int maxFileSize, MediaTypeHeaderValue accept)
        {
            if (Request.Content.Headers.ContentLength > maxFileSize)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "File too large");

            if (!Request.Content.Headers.ContentType.Equals(accept))
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var req = HttpContext.Current.Request;

            var filePath = HttpContext.Current.Server.MapPath(relativePath);

            if (!CopyFile(req.InputStream, filePath, maxFileSize))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "File too large");

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        private bool CopyFile(Stream s, string outputPath, int maxFileSize)
        {
            int fileSize = 0;
            using (var outputFile = new FileStream(outputPath, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[1024];
                int c;
                do
                {
                    c = s.Read(buffer, 0, buffer.Length);
                    fileSize += c;
                    if (fileSize > maxFileSize) return false;
                    outputFile.Write(buffer, 0, c);
                }
                while (c == buffer.Length);
            }

            return true;
        }
    }
}