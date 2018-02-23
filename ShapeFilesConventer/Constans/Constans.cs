
using System.Web;

namespace ShapeFilesConventer.Constans
{
    public class Constans
    {
       public static readonly string PathToSaveFile = HttpContext.Current.Server.MapPath(@"~\App_Data");
       public static readonly string Error = "Error. geoJson variable is null";
       public static readonly string FormObjectName = "UploadedFile";
       public static readonly string AppDataFolderDosentExists = "No App_Data folder.";
    }
}