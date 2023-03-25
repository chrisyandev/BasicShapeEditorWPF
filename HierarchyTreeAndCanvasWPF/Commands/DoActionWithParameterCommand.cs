using HierarchyTreeAndCanvasWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HierarchyTreeAndCanvasWPF.Commands
{
    public class DoActionWithParameterCommand : CommandBase
    {
        private readonly Action<object> _action;

        public DoActionWithParameterCommand(Action<object> action)
        {
            _action = action;
        }

        public override void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
