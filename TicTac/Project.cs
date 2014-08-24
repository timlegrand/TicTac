using System;


namespace TicTac
{
    [Serializable()]
    public class Project
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Project()
        {
            Id = null;
            Name = null;
            Description = null;
        }

        //Operators
        public bool Equals(Project p)
        {
            if (p == null)
            {
                return false;
            }

            var desc = ((p.Description == null && Description == null) ||
                        (p.Description == "" && Description == "") ||
                        (p.Description == null && Description == "") ||
                        (p.Description == "" && Description == null));

            return (p.Name == Name &&
                    desc &&
                    p.Id == Id);
        }

        // Service
        public bool IsValid()
        {
            // Description might be null
            return ((Id != null) && (Name != null));
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
