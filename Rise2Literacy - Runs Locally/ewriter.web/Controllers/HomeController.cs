using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net.Http;
using System.Web.Http;
using System.Windows.Ink;
using System.Windows.Input;
using Newtonsoft.Json;

namespace eWriter.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public String Recognize(String strokes)
        {
            var strokePointsData = JsonConvert.DeserializeObject<dynamic>(strokes);

            var strokeCollection = GetStrokeCollectionFromPoints(strokePointsData);

            var inkAnalyzer = new InkAnalyzer();

            /*
            inkAnalyzer.AddStrokes(strokeCollection);
            inkAnalyzer.SetStrokesType(strokeCollection, StrokeType.Writing);
            inkAnalyzer.SetStrokesLanguageId(strokeCollection, 0x09);

            var analysisStatus = inkAnalyzer.Analyze();

            if (analysisStatus.Successful)
            {
                var recognizedString = inkAnalyzer.GetRecognizedString();
                return recognizedString;
            }
            else
            {
               return "Data not recognized";
            }
            */


            inkAnalyzer.AddStrokes(strokeCollection);
            inkAnalyzer.SetStrokesType(strokeCollection, StrokeType.Writing);
            inkAnalyzer.SetStrokesLanguageId(strokeCollection, 0x09);

            var analysisStatus = inkAnalyzer.Analyze();

            if (analysisStatus.Successful)
            {
                ContextNodeCollection nodes = inkAnalyzer.FindNodesOfType(ContextNodeType.InkWord);

                if (nodes.Count == 0)
                    return "Could not recognize any words.";

                string recognizedString = "";
                foreach (InkWordNode node in nodes)
                {
                    recognizedString += node.GetRecognizedString() + ' ';
                }
                return recognizedString;
            }

            return "Data not recognized";
        }

        private StrokeCollection GetStrokeCollectionFromPoints(dynamic strokePoints)
        {
            var strokeCollection = new StrokeCollection();

            foreach (var stroke in strokePoints.Strokes)
            {
                var points = new StylusPointCollection();

                foreach (var point in stroke.Points)
                {
                    var x = (float)point.X;
                    var y = (float)point.Y;

                    points.Add(new StylusPoint(x, y));
                }

                strokeCollection.Add(new Stroke(points));
            }

            return strokeCollection;
        }

    }
}
