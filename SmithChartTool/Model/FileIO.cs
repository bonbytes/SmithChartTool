using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmithChartTool.View;
using SmithChartTool.Utility;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;
using MathNet.Numerics;

namespace SmithChartTool.Model
{
    public static class FileIO
    { 
        private const char HeaderMarker = '#';
        private const char DataMarker = '!';

        public static void SaveProjectToFile(string path, string projectName, string description, double frequency, Complex32 refImpedance, bool isNormalized, Collection<SchematicElement> elements)
        {
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine(HeaderMarker + DateTime.Today.ToString(" MMMM dd, yyyy") + " " + DateTime.Now.ToLongTimeString());
                sw.WriteLine(HeaderMarker + path);
                sw.WriteLine(HeaderMarker + "" + HeaderMarker + " Description");

                if (description != null && description.Length > 0)
                {
                    string[] descriptionStringArray = description.Split('\n');

                    if (descriptionStringArray != null && descriptionStringArray.Length > 0)
                        foreach (string str in descriptionStringArray)
                            if (str != null && str.Length > 1)
                            {
                                while (str.IndexOf('\n') != -1 && str.Length > 0)
                                    str.Remove(str.IndexOf('\n'));

                                sw.Write(HeaderMarker + "" + HeaderMarker + str);
                            }
                }
                sw.WriteLine();
                sw.WriteLine(HeaderMarker + " Settings");
                sw.WriteLine(DataMarker + "projectName " + projectName);
                sw.WriteLine(DataMarker + "frequency " + frequency);
                sw.WriteLine(DataMarker + "refImpedance " + refImpedance);
                sw.WriteLine(DataMarker + "isNormalized " + isNormalized);
                sw.WriteLine(DataMarker + "numElements " + elements.Count());


                if (elements != null && elements.Count > 0)
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        SchematicElement el = elements.ElementAt(i);
                        sw.WriteLine(el.ToStringSimple());
                    }
            }
        }

        public static Collection<SchematicElement> ReadProjectFromFile(string path, out string projectName, out string projectDescription, out double frequency, out Complex32 refImpedance, out bool isNormalized)
        {
            Collection<SchematicElement> list = new Collection<SchematicElement>();
            projectName = "";
            frequency = 0.0;
            refImpedance = new Complex32();
            isNormalized = false;
            int numElements = 0;

            using (StreamReader sr = File.OpenText(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if ((line.FirstOrDefault() == HeaderMarker) || line == "")
                        continue;

                    string[] data = line.Split(' '); // split line at every whitespace to generate multiple string entries

                    if (line.First() == DataMarker)
                    {
                        string argument = data[1];
                        switch (data[0])
                        {
                            case ("!projectName"): projectName = argument; break;

                            case "!frequency": frequency = double.Parse(argument); break;

                            case "!refImpedance": refImpedance = Complex32.Parse(argument); break;

                            case "!isNormalized": isNormalized = bool.Parse(argument); break;

                            case "!numElements": numElements = int.Parse(argument); break;
                        }
                    }
                    else
                    {
                        list.Add(ElementFromLine(ref data));
                        numElements++;
                    }
                }
                projectDescription = ReadDescriptionFromFile(path);

            }
            return list;
        }

        public static string ReadDescriptionFromFile(string path)
        {
            string ret = string.Empty;

            if (!File.Exists(path))
                return ret;

            using (StreamReader sr = File.OpenText(path))
            {
                bool addLines = false;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.Length < 2)
                        continue;

                    if (addLines && line.Substring(0, 2) == HeaderMarker.ToString())
                        ret += line.Remove(0, 2) + "\n";

                    // found description
                    if (line.Substring(0, 2) == (HeaderMarker.ToString() + HeaderMarker.ToString()))
                    {
                        if (addLines == true)
                            break;
                        addLines = true;
                    }
                }
            }

            return ret;
        }

        public static SchematicElement ElementFromLine(ref string[] data)
        {
            return new SchematicElement() { Type = SchematicElementType.Port };
        }

    }
}
