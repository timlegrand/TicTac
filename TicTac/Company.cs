using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTac
{
    class Company
    {
        private int? _id;
        private string _name;

        public Company()
        {
            _id = null;
            _name = null;
        }

        public Company(string name)
        {
            _id = null;
            _name = name;
        }

        public Company(int id, string name)
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
        public bool IsValid()
        {
            return ((_id == null) && (_name != null));
        }
        
        public override string ToString()
        {
            return _name;
        }
    }
}
