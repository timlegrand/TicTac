using System;


namespace ProjectTime
{
    [Serializable()]
    public class Architect
    {
        private string _firstName;
        private string _lastName;
        private string _company;

        public Architect(string firstName, string lastName, string company)
        {
            _firstName = firstName;
            _lastName = lastName;
            _company = company;
        }

        // Accessors
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
    }
}
