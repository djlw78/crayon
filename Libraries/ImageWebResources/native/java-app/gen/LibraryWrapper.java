package org.crayonlang.libraries.imagewebresources;

import java.util.ArrayList;
import java.util.HashMap;
import org.crayonlang.interpreter.PlatformTranslationHelper;
import org.crayonlang.interpreter.structs.*;
import org.crayonlang.interpreter.TranslationHelper;

public final class LibraryWrapper {

  public static Value lib_imagewebresources_bytesToImage(VmContext vm, Value[] args) {
    ObjectInstance objInstance1 = ((ObjectInstance) args[0].internalValue);
    Object object1 = objInstance1.nativeData[0];
    ListImpl list1 = ((ListImpl) args[1].internalValue);
    Value value = org.crayonlang.interpreter.vm.CrayonWrapper.getItemFromList(list1, 0);
    Object[] objArray1 = new Object[3];
    objInstance1 = ((ObjectInstance) value.internalValue);
    objInstance1.nativeData = objArray1;
    if (ImageDownloader.bytesToImage(object1, objArray1)) {
      Value width = org.crayonlang.interpreter.vm.CrayonWrapper.buildInteger(vm.globals, ((int) objArray1[1]));
      Value height = org.crayonlang.interpreter.vm.CrayonWrapper.buildInteger(vm.globals, ((int) objArray1[2]));
      list1.array[1] = width;
      list1.array[2] = height;
      return vm.globalTrue;
    }
    return vm.globalFalse;
  }

  public static Value lib_imagewebresources_jsDownload(VmContext vm, Value[] args) {
    return vm.globalNull;
  }

  public static Value lib_imagewebresources_jsGetImage(VmContext vm, Value[] args) {
    return vm.globalFalse;
  }

  public static Value lib_imagewebresources_jsPoll(VmContext vm, Value[] args) {
    return vm.globalFalse;
  }
}
