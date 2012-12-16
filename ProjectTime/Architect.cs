using System;


namespace ProjectTime
{
    [Serializable()]
    public class Architect
    {
        private int? _id;
        private string _firstName;
        private string _lastName;
        private string _company;

        public Architect()
        {
            _id = null;
            _firstName = null;
            _lastName = null;
            _company = null;
        }

        public Architect(string firstName, string lastName, string company)
        {
            _id = null;
            _firstName = firstName;
            _lastName = lastName;
            _company = company;
        }

        public Architect(int id, string firstName, string lastName, string company)
        {
            _id = id;
            _firstName = firstName;
            _lastName = lastName;
            _company = company;
        }

        // Accessors
        public int? Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }

        public bool IsPushable()
        {
            return ((_id == null) && (_firstName != null) && (_lastName != null) && (_company != null));
        }
    }
}
