using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchyTreeAndCanvasWPF.Utilities
{
    public static class UIDGenerator
    {
        private static HashSet<string> generatedIds = new HashSet<string>();

        public static string GenerateUniqueId()
        {
            string id = Guid.NewGuid().ToString();

            // Check if the generated ID already exists
            while (generatedIds.Contains(id))
            {
                id = Guid.NewGuid().ToString();
            }

            // Add the generated ID to the set
            generatedIds.Add(id);

            return id;
        }
    }
}
