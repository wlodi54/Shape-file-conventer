using System;
using System.IO;
using System.Web;
using System.Web.Http;
using GeoAPI;
using NetTopologySuite;
using ShapeFilesConventer.ActionFilters;
using ShapeFilesConventer.Infrastructure;


namespace ShapeFilesConventer.Controllers
{
    public class ValuesController : ApiController
    {
        [ValidateFileFormFilter]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Map/Show")]
        public string GetFile()
        {
           
            if (!Directory.Exists(Constans.Constans.PathToSaveFile))
            {
                throw new Exception(Constans.Constans.AppDataFolderDosentExists);
            }
            string resultGeoJson="";
            var stream = HttpContext.Current.Request.GetBufferedInputStream();
            StreamReader reader=new StreamReader(stream);
            reader.ReadToEnd();
            
            var postedFile = HttpContext.Current.Request.Files[Constans.Constans.FormObjectName];
            var ignore = HttpContext.Current.Request.InputStream;
            if (postedFile != null)
            {
                ClearDataDirectory(Constans.Constans.PathToSaveFile);
                var pathToZipFile = Path.Combine(Constans.Constans.PathToSaveFile, postedFile.FileName);
                postedFile.SaveAs(pathToZipFile);
               
                string fullPathToShpFile = ZipFileHelper.ExtractZipFile(pathToZipFile);

                IGeometryServices geometryServices = new NtsGeometryServices();

                ShapeFileHelper shapeFileHelper = new ShapeFileHelper(fullPathToShpFile);
                resultGeoJson = shapeFileHelper.ConvertShapeFileToGeoJson();
                ClearDataDirectory(Constans.Constans.PathToSaveFile);
                
            }
            return resultGeoJson;
        }

        private static void ClearDataDirectory(string pathToSaveFile)
        {
            System.GC.Collect();
            GC.WaitForPendingFinalizers();
          foreach (var fileSystemEntry in Directory.GetDirectories(pathToSaveFile))
            {

                Directory.Delete(fileSystemEntry, true);
                
            }
            foreach (var file in Directory.GetFiles(pathToSaveFile))
            {
                File.Delete(file);
            }
        }

      
    }
}


