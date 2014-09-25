using System;


namespace TicTac
{
    [Serializable()]
    public class Phase
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Phase()
        {
            Id = null;
            Name = null;
            Description = null;
        }

        //Operators
        public bool Equals(Phase p)
        {
            if (p == null)
            {
                return false;
            }

            return (p.Name == Name &&
                    p.Description == Description &&
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

        internal bool IsValidWithoutId()
        {
            return ((Name != null));
        }

        internal void CopyIn(Phase p)
        {
            Id = p.Id ?? null;
            Name = p.Name;
            Description = p.Description;
        }
    }
}
