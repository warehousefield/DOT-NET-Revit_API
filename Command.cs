using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;
using Autodesk.Revit.DB.Structure;


namespace Task2
{
    [Transaction(TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class GeomObjects : IExternalCommand
	{		
		public Result Execute(ExternalCommandData commandData,
			ref string message, ElementSet elements)

		{
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.View view = doc.ActiveView;
           
            VectorClass.Vector(doc,uidoc, view, PointClass.p(uidoc), PointClass.p(uidoc));
   
            Autodesk.Revit.UI.UIApplication revit = commandData.Application;
            m_revit = revit;

            Reference r5 = revit.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            ElementId id5 = r5.ElementId;
            idelem = id5;

            GeomObjectsObjectsForm displayForm = new GeomObjectsObjectsForm(this);
            displayForm.StartPosition = FormStartPosition.CenterParent;
            ElementSet selection = new ElementSet();          
            selection.Insert(revit.ActiveUIDocument.Document.GetElement(id5));
          
            bool isSingle = true;                  
            bool isAllFamilyInstance = true; 

            if (selection.IsEmpty)
            {
                // nothing selected
                message = "Please select some beams, braces or columns.";
                return Autodesk.Revit.UI.Result.Failed;
            }
            else if (1 != selection.Size)
            {
                isSingle = false;
                try
                {
                    if (DialogResult.OK != displayForm.ShowDialog())
                    {
                        return Autodesk.Revit.UI.Result.Cancelled;
                    }
                }
                catch (Exception)
                {
                    return Autodesk.Revit.UI.Result.Failed;
                }  
            }
           
            foreach (Autodesk.Revit.DB.Element e in selection)
            {
                FamilyInstance familyComponent = e as FamilyInstance;
                if (familyComponent != null)
                {
                    if (StructuralType.Beam == familyComponent.StructuralType
                 || StructuralType.Brace == familyComponent.StructuralType)
                    {
                        string returnValue = PointClass.FindParameter(AngleDefinitionName, familyComponent, AngleDefinitionName);
                        displayForm.rotationTextBox.Text = returnValue.ToString();
                    }
                    else if (StructuralType.Column == familyComponent.StructuralType)
                    {
                        Location columnLocation = familyComponent.Location;
                        LocationPoint pointLocation = columnLocation as LocationPoint;
                        double temp = pointLocation.Rotation;
                        string output = (Math.Round(temp * 180 / (Math.PI), 3)).ToString();
                        displayForm.rotationTextBox.Text = output;
                    }
                    else
                    {
                        message = "choose a point";
                        elements.Insert(familyComponent);

                        return Autodesk.Revit.UI.Result.Failed;
                    }
                }
                else
                {
                    if (isSingle)
                    {
                        message = "choose a point";
                        elements.Insert(e);

                        return Autodesk.Revit.UI.Result.Failed;
                    }
                    // there is some objects is not familyInstance
                    message = "choose a point";
                    elements.Insert(e);
                    isAllFamilyInstance = false;
                }
            }

            if (isSingle)
            {
                try
                {
                    if (DialogResult.OK != displayForm.ShowDialog())
                    {
                        return Autodesk.Revit.UI.Result.Cancelled;
                    }
                }
                catch (Exception)
                {
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }

            if (isAllFamilyInstance)
            {
                return Autodesk.Revit.UI.Result.Succeeded;
            }
            else
            {
                //output error information
                return Autodesk.Revit.UI.Result.Failed;
            }
        }     
        public void RotateElement()
        {
            //Reference r5 = m_revit.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            //ElementId id5 = r5.ElementId;

            Transaction transaction = new Transaction(m_revit.ActiveUIDocument.Document, "RotatePoint");
            transaction.Start();
            try
            {
                

                ElementSet selection = new ElementSet();  
                selection.Insert(m_revit.ActiveUIDocument.Document.GetElement(idelem));
               
                foreach (Autodesk.Revit.DB.Element e in selection)
                {
                    FamilyInstance familyComponent = e as FamilyInstance;
                    if (familyComponent == null)
                    {                        
                        continue;
                    }

                    if (StructuralType.Beam == familyComponent.StructuralType
                       || StructuralType.Brace == familyComponent.StructuralType)
                    {
                        ParameterSetIterator paraIterator = familyComponent.Parameters.ForwardIterator();
                        paraIterator.Reset();

                        while (paraIterator.MoveNext())
                        {
                            object para = paraIterator.Current;
                            Parameter objectAttribute = para as Parameter;
                            //set generic property named "Cross-Section Rotation"                           
                            if (objectAttribute.Definition.Name.Equals(AngleDefinitionName))
                            {
                                Double originDegree = objectAttribute.AsDouble();
                                double rotateDegree = m_receiveRotationTextBox * Math.PI / 180;
                                if (!m_isAbsoluteChecked)
                                {
                                    // absolute rotation
                                    rotateDegree += originDegree;
                                }
                                objectAttribute.Set(rotateDegree);
                                // relative rotation
                            }
                        }
                    }
                    else if (StructuralType.Column == familyComponent.StructuralType)
                    {
                        // rotate a point
                        Autodesk.Revit.DB.Location columnLocation = familyComponent.Location;
                        // get the location object
                        LocationPoint pointLocation = columnLocation as LocationPoint;
                        Autodesk.Revit.DB.XYZ insertPoint = pointLocation.Point;
                        // get the location point
                        double temp = pointLocation.Rotation;
                        //existing rotation
                        Autodesk.Revit.DB.XYZ directionPoint = new Autodesk.Revit.DB.XYZ(0, 0, 1);
                        // define the vector of axis
                        Line rotateAxis = Line.CreateUnbound(insertPoint, directionPoint);
                        double rotateDegree = m_receiveRotationTextBox * Math.PI / 180;
                        // rotate point by rotate method
                        if (m_isAbsoluteChecked)
                        {
                            rotateDegree -= temp;
                        }
                        bool rotateResult = pointLocation.Rotate(rotateAxis, rotateDegree);
                        if (rotateResult == false)
                        {
                            TaskDialog.Show("Revit", "Rotate Failed.");
                        }
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", "Rotate failed! " + ex.Message);
                transaction.RollBack();
            }
        }

        Autodesk.Revit.UI.UIApplication m_revit = null;
        double m_receiveRotationTextBox;
        bool m_isAbsoluteChecked;
        const string AngleDefinitionName = "Cross-Section Rotation";

        public ElementId idelem { get; set; }

        public double ReceiveRotationTextBox
        {
            get
            {
                return m_receiveRotationTextBox;
            }
            set
            {
                m_receiveRotationTextBox = value;
            }
        }
        public bool IsAbsoluteChecked
        {
            get
            {
                return m_isAbsoluteChecked;
            }
            set
            {
                m_isAbsoluteChecked = value;
            }
        }
        public GeomObjects()
        { }


    }
}
