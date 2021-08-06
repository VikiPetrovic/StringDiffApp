using System;
using System.Collections.Generic;

namespace DiffApp
{
    public static class Helpers
    {
        public static int[] DoXor(byte[] array1, byte[] array2)
        {           
            // executes XOR on byte arrays to detect different bytes
            if (array1 == null || array2 == null)
            {
                return null;
            }

            if (array1.Length != array2.Length)
            {
                return null;
            }

            int[] xored_diff = new int[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                xored_diff[i] = Convert.ToInt32(array1[i]) ^ Convert.ToInt32(array2[i]);
            }
            return xored_diff;
        }

        public static List<ResultDifference> FindDifferences(int[] xored_diff)
        {
            // find differences - offsets and lengths
            List<ResultDifference> myDiffs = new List<ResultDifference>();
            int offset, length = 0;
            for (int i = 0; i < xored_diff.Length; i++)
            {
                int currVal = xored_diff[i];
                if (currVal == 1)
                {
                    offset = i;
                    length = 1;
                    int j;
                    for (j = i + 1; j < xored_diff.Length; j++)
                    {
                        if (xored_diff[j] == 0)
                        {
                            // end of difference
                            i = j;
                            break;
                        }
                        else
                        {
                            length++;                            
                        }
                    }
                    // end of difference
                    myDiffs.Add(new ResultDifference() { offset = offset, length = length });
                    offset = 0;
                    length = 0;

                    if (j == xored_diff.Length)
                    {
                        // entire array checked
                        break;
                    }
                }
            }
            return myDiffs;
        }
    }

    public static class GlobalVars
    {
        // global variables
        public static Dictionary<int, DiffEntry> all_entries = new Dictionary<int, DiffEntry>();
    }
}
