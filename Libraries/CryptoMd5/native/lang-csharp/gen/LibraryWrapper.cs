using Interpreter.Structs;
using System.Collections.Generic;
using System.Linq;

namespace Interpreter.Libraries.CryptoMd5
{
    public static class LibraryWrapper
    {
        private static readonly int[] PST_IntBuffer16 = new int[16];
        private static readonly double[] PST_FloatBuffer16 = new double[16];
        private static readonly string[] PST_StringBuffer16 = new string[16];
        private static readonly System.Random PST_Random = new System.Random();

        public static bool AlwaysTrue() { return true; }
        public static bool AlwaysFalse() { return false; }

        public static string PST_StringReverse(string value)
        {
            if (value.Length < 2) return value;
            char[] chars = value.ToCharArray();
            return new string(chars.Reverse().ToArray());
        }

        private static readonly string[] PST_SplitSep = new string[1];
        private static string[] PST_StringSplit(string value, string sep)
        {
            if (sep.Length == 1) return value.Split(sep[0]);
            if (sep.Length == 0) return value.ToCharArray().Select<char, string>(c => "" + c).ToArray();
            PST_SplitSep[0] = sep;
            return value.Split(PST_SplitSep, System.StringSplitOptions.None);
        }

        private static string PST_FloatToString(double value)
        {
            string output = value.ToString();
            if (output[0] == '.') output = "0" + output;
            if (!output.Contains('.')) output += ".0";
            return output;
        }

        private static readonly System.DateTime PST_UnixEpoch = new System.DateTime(1970, 1, 1);
        private static double PST_CurrentTime
        {
            get { return System.DateTime.UtcNow.Subtract(PST_UnixEpoch).TotalSeconds; }
        }

        private static string PST_Base64ToString(string b64Value)
        {
            byte[] utf8Bytes = System.Convert.FromBase64String(b64Value);
            string value = System.Text.Encoding.UTF8.GetString(utf8Bytes);
            return value;
        }

        // TODO: use a model like parse float to avoid double parsing.
        public static bool PST_IsValidInteger(string value)
        {
            if (value.Length == 0) return false;
            char c = value[0];
            if (value.Length == 1) return c >= '0' && c <= '9';
            int length = value.Length;
            for (int i = c == '-' ? 1 : 0; i < length; ++i)
            {
                c = value[i];
                if (c < '0' || c > '9') return false;
            }
            return true;
        }

        public static void PST_ParseFloat(string strValue, double[] output)
        {
            double num = 0.0;
            output[0] = double.TryParse(strValue, out num) ? 1 : -1;
            output[1] = num;
        }

        private static List<T> PST_ListConcat<T>(List<T> a, List<T> b)
        {
            List<T> output = new List<T>(a.Count + b.Count);
            output.AddRange(a);
            output.AddRange(b);
            return output;
        }

        private static List<Value> PST_MultiplyList(List<Value> items, int times)
        {
            List<Value> output = new List<Value>(items.Count * times);
            while (times-- > 0) output.AddRange(items);
            return output;
        }

        private static bool PST_SubstringIsEqualTo(string haystack, int index, string needle)
        {
            int needleLength = needle.Length;
            if (index + needleLength > haystack.Length) return false;
            if (needleLength == 0) return true;
            if (haystack[index] != needle[0]) return false;
            if (needleLength == 1) return true;
            for (int i = 1; i < needleLength; ++i)
            {
                if (needle[i] != haystack[index + i]) return false;
            }
            return true;
        }

        private static void PST_ShuffleInPlace<T>(List<T> list)
        {
            if (list.Count < 2) return;
            int length = list.Count;
            int tIndex;
            T tValue;
            for (int i = length - 1; i >= 0; --i)
            {
                tIndex = PST_Random.Next(length);
                tValue = list[tIndex];
                list[tIndex] = list[i];
                list[i] = tValue;
            }
        }

        public static Value lib_md5_addBytes(VmContext vm, Value[] args)
        {
            ObjectInstance obj = (ObjectInstance)args[0].internalValue;
            ListImpl fromByteList = (ListImpl)args[1].internalValue;
            List<int> toByteList = (List<int>)obj.nativeData[0];
            int length = fromByteList.size;
            int i = 0;
            while ((i < length))
            {
                toByteList.Add((int)fromByteList.array[i].internalValue);
                i += 1;
            }
            return vm.globalFalse;
        }

        public static int lib_md5_bitShiftRight(int value, int amount)
        {
            if ((amount == 0))
            {
                return value;
            }
            if ((value > 0))
            {
                return (value >> amount);
            }
            int mask = 2147483647;
            return (((value >> 1)) & ((mask >> ((amount - 1)))));
        }

        public static int lib_md5_bitwiseNot(int x)
        {
            return (-x - 1);
        }

        public static Value lib_md5_digestMd5(VmContext vm, Value[] args)
        {
            ObjectInstance obj = (ObjectInstance)args[0].internalValue;
            ListImpl output = (ListImpl)args[1].internalValue;
            List<int> byteList = (List<int>)obj.nativeData[0];
            int[] resultBytes = lib_md5_digestMd5Impl(byteList);
            int i = 0;
            while ((i < 16))
            {
                int b = resultBytes[i];
                Interpreter.Vm.CrayonWrapper.addToList(output, vm.globals.positiveIntegers[b]);
                i += 1;
            }
            return args[1];
        }

        public static int[] lib_md5_digestMd5Impl(List<int> inputBytes)
        {
            int[] s = new int[64];
            int[] K = new int[64];
            s[0] = 7;
            s[1] = 12;
            s[2] = 17;
            s[3] = 22;
            s[4] = 7;
            s[5] = 12;
            s[6] = 17;
            s[7] = 22;
            s[8] = 7;
            s[9] = 12;
            s[10] = 17;
            s[11] = 22;
            s[12] = 7;
            s[13] = 12;
            s[14] = 17;
            s[15] = 22;
            s[16] = 5;
            s[17] = 9;
            s[18] = 14;
            s[19] = 20;
            s[20] = 5;
            s[21] = 9;
            s[22] = 14;
            s[23] = 20;
            s[24] = 5;
            s[25] = 9;
            s[26] = 14;
            s[27] = 20;
            s[28] = 5;
            s[29] = 9;
            s[30] = 14;
            s[31] = 20;
            s[32] = 4;
            s[33] = 11;
            s[34] = 16;
            s[35] = 23;
            s[36] = 4;
            s[37] = 11;
            s[38] = 16;
            s[39] = 23;
            s[40] = 4;
            s[41] = 11;
            s[42] = 16;
            s[43] = 23;
            s[44] = 4;
            s[45] = 11;
            s[46] = 16;
            s[47] = 23;
            s[48] = 6;
            s[49] = 10;
            s[50] = 15;
            s[51] = 21;
            s[52] = 6;
            s[53] = 10;
            s[54] = 15;
            s[55] = 21;
            s[56] = 6;
            s[57] = 10;
            s[58] = 15;
            s[59] = 21;
            s[60] = 6;
            s[61] = 10;
            s[62] = 15;
            s[63] = 21;
            K[0] = lib_md5_uint32Hack(55146, 42104);
            K[1] = lib_md5_uint32Hack(59591, 46934);
            K[2] = lib_md5_uint32Hack(9248, 28891);
            K[3] = lib_md5_uint32Hack(49597, 52974);
            K[4] = lib_md5_uint32Hack(62844, 4015);
            K[5] = lib_md5_uint32Hack(18311, 50730);
            K[6] = lib_md5_uint32Hack(43056, 17939);
            K[7] = lib_md5_uint32Hack(64838, 38145);
            K[8] = lib_md5_uint32Hack(27008, 39128);
            K[9] = lib_md5_uint32Hack(35652, 63407);
            K[10] = lib_md5_uint32Hack(65535, 23473);
            K[11] = lib_md5_uint32Hack(35164, 55230);
            K[12] = lib_md5_uint32Hack(27536, 4386);
            K[13] = lib_md5_uint32Hack(64920, 29075);
            K[14] = lib_md5_uint32Hack(42617, 17294);
            K[15] = lib_md5_uint32Hack(18868, 2081);
            K[16] = lib_md5_uint32Hack(63006, 9570);
            K[17] = lib_md5_uint32Hack(49216, 45888);
            K[18] = lib_md5_uint32Hack(9822, 23121);
            K[19] = lib_md5_uint32Hack(59830, 51114);
            K[20] = lib_md5_uint32Hack(54831, 4189);
            K[21] = lib_md5_uint32Hack(580, 5203);
            K[22] = lib_md5_uint32Hack(55457, 59009);
            K[23] = lib_md5_uint32Hack(59347, 64456);
            K[24] = lib_md5_uint32Hack(8673, 52710);
            K[25] = lib_md5_uint32Hack(49975, 2006);
            K[26] = lib_md5_uint32Hack(62677, 3463);
            K[27] = lib_md5_uint32Hack(17754, 5357);
            K[28] = lib_md5_uint32Hack(43491, 59653);
            K[29] = lib_md5_uint32Hack(64751, 41976);
            K[30] = lib_md5_uint32Hack(26479, 729);
            K[31] = lib_md5_uint32Hack(36138, 19594);
            K[32] = lib_md5_uint32Hack(65530, 14658);
            K[33] = lib_md5_uint32Hack(34673, 63105);
            K[34] = lib_md5_uint32Hack(28061, 24866);
            K[35] = lib_md5_uint32Hack(64997, 14348);
            K[36] = lib_md5_uint32Hack(42174, 59972);
            K[37] = lib_md5_uint32Hack(19422, 53161);
            K[38] = lib_md5_uint32Hack(63163, 19296);
            K[39] = lib_md5_uint32Hack(48831, 48240);
            K[40] = lib_md5_uint32Hack(10395, 32454);
            K[41] = lib_md5_uint32Hack(60065, 10234);
            K[42] = lib_md5_uint32Hack(54511, 12421);
            K[43] = lib_md5_uint32Hack(1160, 7429);
            K[44] = lib_md5_uint32Hack(55764, 53305);
            K[45] = lib_md5_uint32Hack(59099, 39397);
            K[46] = lib_md5_uint32Hack(8098, 31992);
            K[47] = lib_md5_uint32Hack(50348, 22117);
            K[48] = lib_md5_uint32Hack(62505, 8772);
            K[49] = lib_md5_uint32Hack(17194, 65431);
            K[50] = lib_md5_uint32Hack(43924, 9127);
            K[51] = lib_md5_uint32Hack(64659, 41017);
            K[52] = lib_md5_uint32Hack(25947, 22979);
            K[53] = lib_md5_uint32Hack(36620, 52370);
            K[54] = lib_md5_uint32Hack(65519, 62589);
            K[55] = lib_md5_uint32Hack(34180, 24017);
            K[56] = lib_md5_uint32Hack(28584, 32335);
            K[57] = lib_md5_uint32Hack(65068, 59104);
            K[58] = lib_md5_uint32Hack(41729, 17172);
            K[59] = lib_md5_uint32Hack(19976, 4513);
            K[60] = lib_md5_uint32Hack(63315, 32386);
            K[61] = lib_md5_uint32Hack(48442, 62005);
            K[62] = lib_md5_uint32Hack(10967, 53947);
            K[63] = lib_md5_uint32Hack(60294, 54161);
            int a0 = lib_md5_uint32Hack(26437, 8961);
            int b0 = lib_md5_uint32Hack(61389, 43913);
            int c0 = lib_md5_uint32Hack(39098, 56574);
            int d0 = lib_md5_uint32Hack(4146, 21622);
            int originalLength = inputBytes.Count;
            inputBytes.Add(128);
            while (((inputBytes.Count % 64) != 56))
            {
                inputBytes.Add(0);
            }
            inputBytes.Add(0);
            inputBytes.Add(0);
            inputBytes.Add(0);
            inputBytes.Add(0);
            inputBytes.Add(((originalLength >> 24) & 255));
            inputBytes.Add(((originalLength >> 16) & 255));
            inputBytes.Add(((originalLength >> 8) & 255));
            inputBytes.Add((originalLength & 255));
            int[] M = new int[16];
            int m = 0;
            int mask32 = lib_md5_uint32Hack(65535, 65535);
            int F = 0;
            int g = 0;
            int t = 0;
            int rotAmt = 0;
            int chunkIndex = 0;
            while ((chunkIndex < inputBytes.Count))
            {
                int j = 0;
                while ((j < 16))
                {
                    t = (chunkIndex + (j * 4));
                    m = (inputBytes[t] << 24);
                    m += (inputBytes[(t + 1)] << 16);
                    m += (inputBytes[(t + 2)] << 8);
                    m += inputBytes[(t + 3)];
                    M[j] = m;
                    j += 1;
                }
                int A = a0;
                int B = b0;
                int C = c0;
                int D = d0;
                int i = 0;
                while ((i < 64))
                {
                    if ((i < 16))
                    {
                        F = (((B & C)) | ((lib_md5_bitwiseNot(B) & D)));
                        g = i;
                    }
                    else
                    {
                        if ((i < 32))
                        {
                            F = (((D & B)) | ((lib_md5_bitwiseNot(D) & C)));
                            g = ((((5 * i) + 1)) & 15);
                        }
                        else
                        {
                            if ((i < 48))
                            {
                                F = (B ^ C ^ D);
                                g = ((((3 * i) + 5)) & 15);
                            }
                            else
                            {
                                F = (C ^ ((B | lib_md5_bitwiseNot(D))));
                                g = (((7 * i)) & 15);
                            }
                        }
                    }
                    F = (((F + A + K[i] + M[g])) & mask32);
                    A = D;
                    D = C;
                    C = B;
                    rotAmt = s[i];
                    B = (B + ((((F << rotAmt)) | lib_md5_bitShiftRight(F, (32 - rotAmt)))));
                    i += 1;
                }
                a0 = (((a0 + A)) & mask32);
                b0 = (((b0 + B)) & mask32);
                c0 = (((c0 + C)) & mask32);
                d0 = (((d0 + D)) & mask32);
                chunkIndex += 64;
            }
            int[] output = new int[16];
            output[0] = ((a0 >> 24) & 255);
            output[1] = ((a0 >> 16) & 255);
            output[2] = ((a0 >> 8) & 255);
            output[3] = (a0 & 255);
            output[4] = ((b0 >> 24) & 255);
            output[5] = ((b0 >> 16) & 255);
            output[6] = ((b0 >> 8) & 255);
            output[7] = (b0 & 255);
            output[8] = ((c0 >> 24) & 255);
            output[9] = ((c0 >> 16) & 255);
            output[10] = ((c0 >> 8) & 255);
            output[11] = (c0 & 255);
            output[12] = ((d0 >> 24) & 255);
            output[13] = ((d0 >> 16) & 255);
            output[14] = ((d0 >> 8) & 255);
            output[15] = (d0 & 255);
            return output;
        }

        public static Value lib_md5_initHash(VmContext vm, Value[] args)
        {
            ObjectInstance obj = (ObjectInstance)args[0].internalValue;
            obj.nativeData = new object[1];
            obj.nativeData[0] = new List<int>();
            return vm.globalNull;
        }

        public static int lib_md5_uint32Hack(int left, int right)
        {
            return (((left << 16)) | right);
        }
    }
}
