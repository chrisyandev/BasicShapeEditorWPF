using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HierarchyTreeWPF.Models
{
    public class TreeItem : TreeViewItem
    {
        public TreeItem(int id, string header)
        {
            Id = id;
            Header = header;
        }

        public int Id { get; set; }
    }
}
