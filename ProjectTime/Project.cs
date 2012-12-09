using System;


namespace ProjectTime
{
    [Serializable()]
    public class Project
    {
        private string _name;

        public Project(string name)
        {
            _name = name;
        }


        // Accessors
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
