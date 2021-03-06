using Interpreter.Structs;
using System.Collections.Generic;
using System.Linq;

namespace Interpreter.Libraries.Dispatcher
{
    public static class LibraryWrapper
    {
        public static Value lib_dispatcher_flushNativeQueue(VmContext vm, Value[] args)
        {
            object[] nd = ((ObjectInstance)args[0].internalValue).nativeData;
            List<Value> output = new List<Value>();
            DispatcherHelper.FlushNativeQueue(nd, output);
            if ((output.Count == 0))
            {
                return vm.globalNull;
            }
            return Interpreter.Vm.CrayonWrapper.buildList(output);
        }

        public static Value lib_dispatcher_initNativeQueue(VmContext vm, Value[] args)
        {
            ObjectInstance obj = (ObjectInstance)args[0].internalValue;
            object[] nd = new object[2];
            nd[0] = new object();
            nd[1] = new List<Value>();
            obj.nativeData = nd;
            return vm.globalNull;
        }
    }
}
