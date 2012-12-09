using System;


namespace ProjectTime
{
    [Serializable()]
    public class Phase
    {
        private string _name;

        public Phase(string name)
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
