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

   
        // Service
        public bool IsValid()
        {
            return ((Id == null) && (Name != null));
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
