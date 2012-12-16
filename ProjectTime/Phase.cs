using System;


namespace ProjectTime
{
    [Serializable()]
    public class Phase
    {
        private int? _id;
        private string _name;
        
        public Phase(string name)
        {
            _id = null;
            _name = name;
        }

        public Phase(int id, string name)
        {
            _id = id;
            _name = name;
        }


        // Accessors
        public int? Id
        {
            get { return _id; }
            set { _id = value; }
        }
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        // Service
        public bool IsPushable()
        {
            return ((_id == null) && (_name != null));
        }
        
        public override string ToString()
        {
            return _name;
        }
    }
}
