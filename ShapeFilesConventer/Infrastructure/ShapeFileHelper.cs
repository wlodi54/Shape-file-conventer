using System;
using System.IO;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;

namespace ShapeFilesConventer.Infrastructure
{
    public class ShapeFileHelper 
    {
        public VectorLayer VectorLayerObject { get; set; }
        public ShapeFileHelper(VectorLayer vectorLayer)
        {
            VectorLayerObject = vectorLayer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToFile">Path to directory where are shape files.</param>
        public ShapeFileHelper(string pathToFile)
        {
            if (File.Exists(pathToFile))
            {
                VectorLayerObject = new VectorLayer("NewLayer") {DataSource = new ShapeFile(pathToFile)};
            }
        }
        

       /// <summary>
       /// Convert VectorLayer property to GeoJson 
       /// </summary>
       /// <returns>string GeoJson format</returns>
      public string ConvertShapeFileToGeoJson()
        {
            FeatureDataSet featureDataSet = new FeatureDataSet();
            
            Envelope envelope = new Envelope(VectorLayerObject.Envelope);
            VectorLayerObject.DataSource.Open();
            VectorLayerObject.DataSource.ExecuteIntersectionQuery(envelope, featureDataSet);
            featureDataSet.Merge(AddFeaturesToFeatureDataTable());
            VectorLayerObject.DataSource.Close();
            
            return ConvertFeatureDataSetToGeoJson(featureDataSet);

        }
            /// <summary>
            /// Method Convert passed FeatureDataSet to string GeoJson
            /// </summary>
            /// <param name="featureDataSet"></param>
            /// <returns>string GeoJson</returns>
        private static string ConvertFeatureDataSetToGeoJson(FeatureDataSet featureDataSet)
        {
            var geoJson = SharpMap.Converters.GeoJSON.GeoJSONHelper.GetData(featureDataSet);
            if (geoJson!=null)
            {
                TextWriter textWriter = new StringWriter();

                SharpMap.Converters.GeoJSON.GeoJSONWriter.Write(geoJson, textWriter);

                return textWriter.ToString();
            }
            throw new Exception("Error. geoJson variable is null");
            
        }

        private FeatureDataTable AddFeaturesToFeatureDataTable()
        {
            FeatureDataTable featureDataTable = new FeatureDataTable();
            
            for (uint i = 0; i < VectorLayerObject.DataSource.GetFeatureCount(); i++)
            {
                FeatureDataRow feature = VectorLayerObject.DataSource.GetFeature(i);

                if (feature.ItemArray.Length > featureDataTable.Columns.Count)
                {
                    int numberOfColumns = feature.ItemArray.Length - featureDataTable.Columns.Count;
                    for (int j = 0; j < numberOfColumns; j++)
                    {
                        featureDataTable.Columns.Add();
                    }
                }
                var newRow = featureDataTable.NewRow();
                newRow.Geometry = feature.Geometry;
                newRow.ItemArray = feature.ItemArray;
                newRow.RowError = feature.RowError;
                featureDataTable.AddRow(newRow);
                
            }

            return featureDataTable;
        }
    }
}