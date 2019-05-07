package org.crayonlang.libraries.textencoding;

import java.util.ArrayList;
import java.util.HashMap;
import org.crayonlang.interpreter.PlatformTranslationHelper;
import org.crayonlang.interpreter.structs.*;
import org.crayonlang.interpreter.TranslationHelper;

public final class LibraryWrapper {

  private static java.util.Random PST_random = new java.util.Random();
  private static final String[] PST_emptyArrayString = new String[0];
  @SuppressWarnings("rawtypes")
  private static final ArrayList[] PST_emptyArrayList = new ArrayList[0];
  @SuppressWarnings("rawtypes")
  private static final HashMap[] PST_emptyArrayMap = new HashMap[0];

  private static final int[] PST_intBuffer16 = new int[16];
  private static final double[] PST_floatBuffer16 = new double[16];
  private static final String[] PST_stringBuffer16 = new String[16];

  private static final java.nio.charset.Charset UTF8 = java.nio.charset.Charset.forName("UTF-8");
  private static String PST_base64ToString(String b64Value) {
    int inputLength = b64Value.length();

    if (inputLength == 0) return "";
    while (inputLength > 0 && b64Value.charAt(inputLength - 1) == '=') {
      b64Value = b64Value.substring(0, --inputLength);
    }
    int bitsOfData = inputLength * 6;
    int outputLength = bitsOfData / 8;

    int[] buffer = new int[outputLength];
    char c;
    int charValue;
    for (int i = 0; i < inputLength; ++i) {
      c = b64Value.charAt(i);
      charValue = -1;
      switch (c) {
        case '=': break;
        case '+': charValue = 62;
        case '/': charValue = 63;
        default:
          if (c >= 'A' && c <= 'Z') {
            charValue = c - 'A';
          } else if (c >= 'a' && c <= 'z') {
            charValue = c - 'a' + 26;
          } else if (c >= '0' && c <= '9') {
            charValue = c - '0' + 52;
          }
          break;
      }

      if (charValue != -1) {
        int bitOffset = i * 6;
        int targetIndex = bitOffset / 8;
        int bitWithinByte = bitOffset % 8;
        switch (bitOffset % 8) {
          case 0:
            buffer[targetIndex] |= charValue << 2;
            break;
          case 2:
            buffer[targetIndex] |= charValue;
            break;
          case 4:
            buffer[targetIndex] |= charValue >> 2;
            if (targetIndex + 1 < outputLength)
              buffer[targetIndex + 1] |= charValue << 6;
            break;
          case 6:
            buffer[targetIndex] |= charValue >> 4;
            if (targetIndex + 1 < outputLength)
              buffer[targetIndex + 1] |= charValue << 4;
            break;
        }
      }
    }
    for (int i = 0; i < buffer.length; ++i) buffer[i] &= 255;

    // new String(buffer, UTF8) computes garbage surrogate pair values, so we must do this manually.
    java.util.ArrayList<Integer> codePoints = new java.util.ArrayList<Integer>();
    int b, cp;
    for (int i = 0; i < buffer.length; ++i) {
      b = buffer[i];
      if ((b & 0x80) == 0) {
        cp = b;
      } else if ((b & 0xe0) == 0xc0) {
        cp = ((b & 0x1f) << 6) + (buffer[i + 1] & 0x3f);
        ++i;
      } else if ((b & 0xf0) == 0xe0) {
        cp = ((b & 0x0f) << 12) + ((buffer[i + 1] & 0x3f) << 6) + (buffer[i + 2] & 0x3f);
        i += 2;
      } else {
        cp = ((b & 0x07) << 18) + ((buffer[i + 1] & 0x3f) << 12) + ((buffer[i + 2] & 0x3f) << 6) + (buffer[i + 3] & 0x3f);
        i += 3;
      }
      codePoints.add(cp);
    }
    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < codePoints.size(); ++i) {
      cp = codePoints.get(i);
      if (cp < 0x10000) {
        sb.append((char)cp);
      } else {
        cp -= 0x10000;
        int s1 = 0xd800 | ((cp >> 10) & 0x03ff);
        int s2 = 0xdc00 | (cp & 0x03ff);
        sb.append(new String(new char[] { (char) s1, (char) s2 }));
      }
    }
    return sb.toString();
  }

  private static int[] PST_convertIntegerSetToArray(java.util.Set<Integer> original) {
    int[] output = new int[original.size()];
    int i = 0;
    for (int value : original) {
      output[i++] = value;
    }
    return output;
  }

  private static String[] PST_convertStringSetToArray(java.util.Set<String> original) {
    String[] output = new String[original.size()];
    int i = 0;
    for (String value : original) {
      output[i++] = value;
    }
    return output;
  }

  private static boolean PST_isValidInteger(String value) {
    try {
      Integer.parseInt(value);
    } catch (NumberFormatException nfe) {
      return false;
    }
    return true;
  }

  private static String PST_joinChars(ArrayList<Character> chars) {
    char[] output = new char[chars.size()];
    for (int i = output.length - 1; i >= 0; --i) {
      output[i] = chars.get(i);
    }
    return String.copyValueOf(output);
  }

  private static String PST_joinList(String sep, ArrayList<String> items) {
    int length = items.size();
    if (length < 2) {
      if (length == 0) return "";
      return items.get(0);
    }

    boolean useSeparator = sep.length() > 0;
    StringBuilder sb = new StringBuilder(useSeparator ? (length * 2 - 1) : length);
    sb.append(items.get(0));
    if (useSeparator) {
      for (int i = 1; i < length; ++i) {
        sb.append(sep);
        sb.append(items.get(i));
      }
    } else {
      for (int i = 1; i < length; ++i) {
        sb.append(items.get(i));
      }
    }

    return sb.toString();
  }

  private static <T> T PST_listPop(ArrayList<T> list) {
    return list.remove(list.size() - 1);
  }

  private static String[] PST_literalStringSplit(String original, String sep) {
    ArrayList<String> output = new ArrayList<String>();
    ArrayList<String> currentPiece = new ArrayList<String>();
    int length = original.length();
    int sepLength = sep.length();
    char firstSepChar = sep.charAt(0);
    char c;
    int j;
    boolean match;
    for (int i = 0; i < length; ++i) {
      c = original.charAt(i);
      match = false;
      if (c == firstSepChar) {
        match = true;
        for (j = 1; j < sepLength; ++j) {
          if (i + j < length ) {
            if (sep.charAt(j) != original.charAt(i + j)) {
              match = false;
              break;
            }
          } else {
            match = false;
          }
        }
      }

      if (match) {
        output.add(PST_joinList("", currentPiece));
        currentPiece.clear();
        i += sepLength - 1;
      } else {
        currentPiece.add("" + c);
      }
    }
    output.add(PST_joinList("", currentPiece));
    return output.toArray(new String[output.size()]);
  }

  private static String PST_reverseString(String original) {
    char[] output = original.toCharArray();
    int length = output.length;
    int lengthMinusOne = length - 1;
    int half = length / 2;
    char c;
    for (int i = 0; i < half; ++i) {
      c = output[i];
      output[i] = output[lengthMinusOne - i];
      output[lengthMinusOne - i] = c;
    }
    return String.copyValueOf(output);
  }

  private static boolean PST_checkStringInString(String haystack, int index, String expectedValue) {
    int evLength = expectedValue.length();
    if (evLength + index > haystack.length()) return false;
    if (evLength == 0) return true;
    if (expectedValue.charAt(0) != haystack.charAt(index)) return false;
    if (expectedValue.charAt(evLength - 1) != haystack.charAt(index + evLength - 1)) return false;
    if (evLength <= 2) return true;
    for (int i = evLength - 2; i > 1; --i) {
      if (expectedValue.charAt(i) != haystack.charAt(index + i)) return false;
    }
    return true;
  }

  private static String PST_trimSide(String value, boolean isLeft) {
    int i = isLeft ? 0 : value.length() - 1;
    int end = isLeft ? value.length() : -1;
    int step = isLeft ? 1 : -1;
    char c;
    boolean trimming = true;
    while (trimming && i != end) {
      c = value.charAt(i);
      switch (c) {
        case ' ':
        case '\n':
        case '\t':
        case '\r':
          i += step;
          break;
        default:
          trimming = false;
          break;
      }
    }

    return isLeft ? value.substring(i) : value.substring(0, i + 1);
  }

  private static void PST_parseFloatOrReturnNull(double[] outParam, String rawValue) {
    try {
      outParam[1] = Double.parseDouble(rawValue);
      outParam[0] = 1;
    } catch (NumberFormatException nfe) {
      outParam[0] = -1;
    }
  }

  private static <T> ArrayList<T> PST_multiplyList(ArrayList<T> list, int n) {
    int len = list.size();
    ArrayList<T> output = new ArrayList<T>(len * n);
    if (len > 0) {
      if (len == 1) {
        T t = list.get(0);
        while (n --> 0) {
          output.add(t);
        }
      } else {
        while (n --> 0) {
          output.addAll(list);
        }
      }
    }
    return output;
  }

  private static <T> ArrayList<T> PST_concatLists(ArrayList<T> a, ArrayList<T> b) {
    ArrayList<T> output = new ArrayList(a.size() + b.size());
    output.addAll(a);
    output.addAll(b);
    return output;
  }

  private static <T> void PST_listShuffle(ArrayList<T> list) {
    int len = list.size();
    for (int i = len - 1; i >= 0; --i) {
      int ti = PST_random.nextInt(len);
      if (ti != i) {
        T t = list.get(ti);
        list.set(ti, list.get(i));
        list.set(i, t);
      }
    }
  }

  private static boolean[] PST_listToArrayBool(ArrayList<Boolean> list) {
    int length = list.size();
    boolean[] output = new boolean[length];
    for (int i = 0; i < length; ++i) output[i] = list.get(i);
    return output;
  }

  private static byte[] PST_listToArrayByte(ArrayList<Byte> list) {
    int length = list.size();
    byte[] output = new byte[length];
    for (int i = 0; i < length; ++i) output[i] = list.get(i);
    return output;
  }

  private static int[] PST_listToArrayInt(ArrayList<Integer> list) {
    int length = list.size();
    int[] output = new int[length];
    for (int i = 0; i < length; ++i) output[i] = list.get(i);
    return output;
  }

  private static double[] PST_listToArrayDouble(ArrayList<Double> list) {
    int length = list.size();
    double[] output = new double[length];
    for (int i = 0; i < length; ++i) output[i] = list.get(i);
    return output;
  }

  private static char[] PST_listToArrayChar(ArrayList<Character> list) {
    int length = list.size();
    char[] output = new char[length];
    for (int i = 0; i < length; ++i) output[i] = list.get(i);
    return output;
  }

  private static int[] PST_sortedCopyOfIntArray(int[] nums) {
    int[] output = java.util.Arrays.copyOf(nums, nums.length);
    java.util.Arrays.sort(output);
    return output;
  }

  private static String[] PST_sortedCopyOfStringArray(String[] values) {
    String[] output = java.util.Arrays.copyOf(values, values.length);
    java.util.Arrays.sort(output);
    return output;
  }

  public static Value lib_textencoding_convertBytesToText(VmContext vm, Value[] args) {
    if ((args[0].type != 6)) {
      return org.crayonlang.interpreter.vm.CrayonWrapper.buildInteger(vm.globals, 2);
    }
    ListImpl byteList = ((ListImpl) args[0].internalValue);
    int format = ((int) args[1].internalValue);
    ListImpl output = ((ListImpl) args[2].internalValue);
    String[] strOut = PST_stringBuffer16;
    int length = byteList.size;
    int[] unwrappedBytes = new int[length];
    int i = 0;
    Value value = null;
    int c = 0;
    while ((i < length)) {
      value = byteList.array[i];
      if ((value.type != 3)) {
        return org.crayonlang.interpreter.vm.CrayonWrapper.buildInteger(vm.globals, 3);
      }
      c = ((int) value.internalValue);
      if (((c < 0) || (c > 255))) {
        return org.crayonlang.interpreter.vm.CrayonWrapper.buildInteger(vm.globals, 3);
      }
      unwrappedBytes[i] = c;
      i += 1;
    }
    int sc = crayonlib.textencoding.TextEncodingHelper.bytesToText(unwrappedBytes, format, strOut);
    if ((sc == 0)) {
      org.crayonlang.interpreter.vm.CrayonWrapper.addToList(output, org.crayonlang.interpreter.vm.CrayonWrapper.buildString(vm.globals, strOut[0]));
    }
    return org.crayonlang.interpreter.vm.CrayonWrapper.buildInteger(vm.globals, sc);
  }

  public static Value lib_textencoding_convertTextToBytes(VmContext vm, Value[] args) {
    String value = ((String) args[0].internalValue);
    int format = ((int) args[1].internalValue);
    boolean includeBom = ((boolean) args[2].internalValue);
    ListImpl output = ((ListImpl) args[3].internalValue);
    ArrayList<Value> byteList = new ArrayList<Value>();
    int[] intOut = PST_intBuffer16;
    int sc = crayonlib.textencoding.TextEncodingHelper.textToBytes(value, includeBom, format, byteList, vm.globals.positiveIntegers, intOut);
    int swapWordSize = intOut[0];
    if ((swapWordSize != 0)) {
      int i = 0;
      int j = 0;
      int length = byteList.size();
      Value swap = null;
      int half = (swapWordSize >> 1);
      int k = 0;
      while ((i < length)) {
        k = (i + swapWordSize - 1);
        j = 0;
        while ((j < half)) {
          swap = byteList.get((i + j));
          byteList.set((i + j), byteList.get((k - j)));
          byteList.set((k - j), swap);
          j += 1;
        }
        i += swapWordSize;
      }
    }
    if ((sc == 0)) {
      org.crayonlang.interpreter.vm.CrayonWrapper.addToList(output, org.crayonlang.interpreter.vm.CrayonWrapper.buildList(byteList));
    }
    return org.crayonlang.interpreter.vm.CrayonWrapper.buildInteger(vm.globals, sc);
  }
}
