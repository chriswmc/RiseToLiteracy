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


            AnalysisHintNode hintNode = inkAnalyzer.CreateAnalysisHint();
            hintNode.Location.MakeInfinite();
            hintNode.WordMode = true;
            hintNode.AllowPartialDictionaryTerms = true;
            string letterWithCaps = "(a|A|b|B|c|C|d|D|e|E|f|F|g|G|h|H|i|I|j|J|k|K|l|L| " + " m|M|n|N|o|O|p|P|q|Q|r|R|s|S|t|T|u|U|v|V|w|W|x|X|y|Y|z|Z)";
            string letter = "(a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z)";
            hintNode.Factoid = letterWithCaps + letter + letter + letter + letter + letter + letter + letter + letter + letter + letter + letter + letter +
                letter + letter + letter + letter + letter + letter + letter + letter + letter + letter + letter + letter;
            hintNode.CoerceToFactoid = true;
            hintNode.Name = "Allow Partial Dictionary Terms & Enable WordMode"; 

            var analysisStatus = inkAnalyzer.Analyze();

            if (analysisStatus.Successful)
            {
                ContextNodeCollection nodes = inkAnalyzer.FindNodesOfType(ContextNodeType.InkWord);

                if (nodes.Count == 0)
                    return "Could not recognize any words.";

                string recognizedString = "";
                foreach (InkWordNode node in nodes)
                {
                    recognizedString += node.GetRecognizedString();
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
