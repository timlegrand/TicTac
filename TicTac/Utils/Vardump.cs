using System;
using System.Reflection;


namespace TicTac.Utils
{
    static class Vardump
    {
        public static void dump(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            // If native (atomic) type
            if (obj.GetType().GetProperties().Length == 0)
            {
                Console.WriteLine("({0}) \"{1}\"", obj.GetType().ToString(), obj.ToString());
                return;
            }

            // Else if complex / user-defined type
            Console.WriteLine(obj.GetType().ToString());
            Console.WriteLine(@"{");
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo p in props)
            {
                try
                {
                    Console.WriteLine("  [\"{0}\"] => ({1}) \"{2}\"",
                        p.Name,
                        (p.GetValue(obj, null) != null) ? p.GetValue(obj, null).GetType().ToString() : "unknown",
                        p.GetValue(obj, null) ?? "null");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine(@"}");
            Console.WriteLine();
        }
    }
}
