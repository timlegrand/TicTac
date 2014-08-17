using System;


namespace TicTac
{
    [Serializable()]
    public class Architect
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Company { get; set; }

        public Architect()
        {
            Id = null;
            FirstName = null;
            LastName = null;
            Company = -1;
        }

        //Operators
        public bool Equals(Architect a)
        {
            if (a == null)
            {
                return false;
            }

            return (a.LastName == LastName &&
                    a.FirstName == FirstName &&
                    a.Company == Company &&
                    a.Id == Id);
        }

        // Service
        public bool IsValid()
        {
            return ((Id != null) && (FirstName != null) && (LastName != null) && (Company != -1));
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
