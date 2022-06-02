using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartToolLibrary
{
    public class Project
    {
        public static readonly string DefaultName = "Default";
        public static readonly string DefaultPath = string.Empty;
        public static readonly string DefaultDescription = "No description...";

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
