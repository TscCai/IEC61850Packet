using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Utils
{
    public static class ArrayEx
    {
        public static bool IsSame(this Array a, Array b)
        {
            bool result = false;
            if (a.Length == b.Length)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (!a.GetValue(i).Equals(b.GetValue(i)))
                    {
                        result = false;
                        break;
                    }
                    result = true;
                }
            }
            return result;
        }

    }
}
