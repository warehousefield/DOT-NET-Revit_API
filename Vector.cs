using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;
using Autodesk.Revit.DB.Structure;
using View = Autodesk.Revit.DB.View;

namespace Task2
{
    class VectorClass
    {
        public static void Vector(Document doc,UIDocument uidoc, View view, XYZ point1, XYZ point2)
        {
            Transaction t = new Transaction(doc, "Line1");

            t.Start();
            Line line1 = Line.CreateBound(point1, point2);
            doc.Create.NewDetailCurve(view, line1);
            t.Commit();

            TaskDialog.Show("Line1", "Vector1 Done");

            XYZ direction = line1.Direction;
            XYZ n = direction.Normalize();
            //XYZ p3 = new XYZ();

            int data2;
            bool ret2 = FormWindow.CollectDataInput("Please input an distance:", out data2);
            MessageBox.Show(string.Format("Successful: {0}\nData: {1}", ret2, data2));

            XYZ res = point1.Add(n.Multiply(data2));
            XYZ p4 = uidoc.Selection.PickPoint("4 point");

            Transaction t2 = new Transaction(doc, "Line2");

            t2.Start();
            Line line2 = Line.CreateBound(p4, res);
            doc.Create.NewDetailCurve(view, line2);
            t2.Commit();

            TaskDialog.Show("Line2", "Vector2 Done");

            int data3;
            bool ret3 = FormWindow.CollectDataInput("Please input an distance:", out data3);
            MessageBox.Show(string.Format("Successful: {0}\nData: {1}", ret3, data3));

            XYZ offset = point1.Add(n.Multiply(data3));

            Reference r = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            ElementId id = r.ElementId;

            Transaction t3 = new Transaction(doc, "Offset");

            t3.Start();
            Line line3 = Line.CreateBound(p4, offset);
            doc.Create.NewDetailCurve(view, line3);
            doc.Delete(id);

            t3.Commit();

            // Determine angle
            XYZ vectorangle = p4 - offset;

            XYZ vectorangle2 = point2-point1;
            double angle = vectorangle.AngleTo(vectorangle2);

            angle = angle * (180 / Math.PI); // degrees

            TaskDialog.Show("angle", angle.ToString());

            if (angle == 90)
            {
                TaskDialog.Show("Прямой угол", "Прямой угол");
            }
        }
    }
}
