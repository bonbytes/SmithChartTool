﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics;

namespace SmithChartToolLibrary
{
    public class Project
    {
        public static readonly string DefaultName = "Default";
        public static readonly string DefaultPath = string.Empty;
        public static readonly string DefaultDescription = "No description...";

        private const char HeaderMarker = '#';
        private const char DataMarker = '!';

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                    _name = value;
            }
        }
        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                    _path = value;
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                    _description = value;
            }
        }

        public static void SaveToFile(string path, string projectName, string description, double frequency, Complex32 refImpedance, bool isNormalized, IList<SchematicElement> elements)
        {
            char[] charsToTrim = { '(', ')' };
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
                sw.WriteLine(DataMarker + "refImpedance " + refImpedance.ToString().Trim(charsToTrim));
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

        public static ObservableSchematicList ReadFromFile(string path, out string projectName, out string projectDescription, out double frequency, out Complex32 refImpedance, out bool isNormalized)
        {
            ObservableSchematicList list = new ObservableSchematicList();
            projectName = "Default";
            frequency = 1e9;
            refImpedance = new Complex32(50, 0);
            isNormalized = false;
            int numElements = 2;

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
                            case ("!projectName"):
                                projectName = argument;
                                break;

                            case "!frequency":
                                if (!(double.TryParse(argument, out frequency)))
                                    throw new ArgumentException("Invalid double value representation in project file", "Frequency");
                                break;

                            case "!refImpedance":
                                if (!(Complex32.TryParse(argument, out refImpedance)))
                                    throw new ArgumentException("Invalid complex value representation in project file", "ReferenceImpedance");
                                break;

                            case "!isNormalized":
                                if (!(bool.TryParse(argument, out isNormalized)))
                                    throw new ArgumentException("Invalid boolean value representation in project file", "IsNormalized");
                                break;

                            case "!numElements":
                                if (!(int.TryParse(argument, out numElements)))
                                    throw new ArgumentException("Invalid integer value representation in project file", "numElements");
                                break;
                        }
                    }
                    else
                    {
                        list.Add(ElementFromLine(ref data));
                        //numElements++;
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


        public Project()
        {
            Init();
        }

        public void Init()
        {
            Description = DefaultDescription;
            Name = DefaultName;
            Path = DefaultPath;
        }
    }
}
