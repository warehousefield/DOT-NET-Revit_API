using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace Task2
{
    class PointClass
    {
        public static XYZ p (UIDocument uidoc)
        {
            XYZ p = uidoc.Selection.PickPoint("point"); ;

            return p;
        }      
        public static string FindParameter(string parameterName, FamilyInstance familyInstanceName, string AngleDefinitionName)
        {
            ParameterSetIterator i = familyInstanceName.Parameters.ForwardIterator();
            i.Reset();
            string valueOfParameter = null;
            bool iMoreAttribute = i.MoveNext();
            while (iMoreAttribute)
            {
                bool isFound = false;
                object o = i.Current;
                Parameter familyAttribute = o as Parameter;
                if (familyAttribute.Definition.Name == parameterName)
                {
                    //find the parameter whose name is same to the given parameter name 
                    Autodesk.Revit.DB.StorageType st = familyAttribute.StorageType;
                    switch (st)
                    {
                        //get the storage type
                        case StorageType.Double:
                            if (parameterName.Equals(AngleDefinitionName))
                            {
                                //make conversion between degrees and radians
                                Double temp = familyAttribute.AsDouble();
                                valueOfParameter = Math.Round(temp * 180 / (Math.PI), 3).ToString();
                            }
                            else
                            {
                                valueOfParameter = familyAttribute.AsDouble().ToString();
                            }
                            break;
                        case StorageType.ElementId:
                            //get Autodesk.Revit.DB.ElementId as string 
                            valueOfParameter = familyAttribute.AsElementId().IntegerValue.ToString();
                            break;
                        case StorageType.Integer:
                            //get Integer as string
                            valueOfParameter = familyAttribute.AsInteger().ToString();
                            break;
                        case StorageType.String:
                            //get string 
                            valueOfParameter = familyAttribute.AsString();
                            break;
                        case StorageType.None:
                            valueOfParameter = familyAttribute.AsValueString();
                            break;
                        default:
                            break;
                    }
                    isFound = true;
                }
                if (isFound)
                {
                    break;
                }
                iMoreAttribute = i.MoveNext();
            }

            return valueOfParameter;
        }
    }
}
