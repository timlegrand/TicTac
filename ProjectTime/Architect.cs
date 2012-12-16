﻿using System;


namespace ProjectTime
{
    [Serializable()]
    public class Architect
    {
        private int? _id;
        private string _firstName;
        private string _lastName;
        private int _company;

        public Architect()
        {
            _id = null;
            _firstName = null;
            _lastName = null;
            _company = -1;
        }

        public Architect(string firstName, string lastName, int company)
        {
            _id = null;
            _firstName = firstName;
            _lastName = lastName;
            _company = company;
        }

        public Architect(int id, string firstName, string lastName, int company)
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

        public int Company
        {
            get { return _company; }
            set { _company = value; }
        }

        // Service
        public bool IsPushable()
        {
            return ((_id == null) && (_firstName != null) && (_lastName != null) && (_company != null));
        }

        public override string ToString()
        {
            return _firstName + " " + _lastName;
        }
    }
}
