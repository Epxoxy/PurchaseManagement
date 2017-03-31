using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Extension
{
    public class BinAuth
    {
        /// <summary>
        /// Verify a long value (Non negative positive integer) is the power of 2
        /// </summary>
        /// <remarks></remarks>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsBinPower(long n)
        {
            /* 判断是2的幂, 1个数乘以2就是将该数左移1位, 而2的0次幂为1, 所以2的n次幂(就是2的0次幂n次乘以2)就是将1左移n位,
             * 这样我们知道如果一个数n是2的幂, 则其只有首位为1, 其后若干个0, 必然有n & (n - 1)为0.
             * (在求1个数的二进制表示中1的个数的时候说过, n&(n-1)去掉n的最后一个1).
             * 因此, 判断一个数n是否为2的幂, 只需要判断n&(n-1)是否为0即可.
             */
            return (n & (n - 1)) == 0;
        }
        /// <summary>
        /// Get two to the power of x
        /// </summary>
        /// <param name="x">x value</param>
        /// <returns></returns>
        public static long GetBinPower(int x)
        {
            return (long)System.Math.Pow(2, x);
        }
        /// <summary>
        /// Convert a long value to binary number
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GetBin(long n)
        {
            return Convert.ToString(n, 2);
        }

        /// <summary>
        /// GenAuthCode
        /// </summary>
        /// <param name="arr">auth array</param>
        /// <remarks>code = code | n</remarks>
        /// <returns></returns>
        public static long GenAuthCode(params long[] arr)
        {
            if (arr == null)
                throw new Exception("Array can't be null, Grass.BinAuth.GenAuthCode()");
            long code = 0;
            arr.ToList().ForEach(x =>
            {
                if (!IsBinPower(x))
                    throw new Exception(string.Format("Illegal value of {0}, it must be the power of 2", x));
                if (x < 0 || x > 4611686018427387904)
                    throw new Exception(string.Format("Auth value of {0} must bigger than 0 and less than 4611686018427387904", x));
                code = code | x;
            });
            return code;
        }
        /// <summary>
        /// AddAuth
        /// </summary>
        /// <param name="authCode">authCode</param>
        /// <param name="auth">auth, the power of 2</param>
        /// <remarks>code = authCode | auth</remarks>
        /// <returns></returns>
        public static long AddAuth(long authCode, long auth)
        {
            if (!IsBinPower(auth))
                throw new Exception(string.Format("Illegal value of {0}, it must be the power of 2", auth));

            if (auth < 0 || auth > 4611686018427387904)
                throw new Exception(string.Format("Auth value of {0} must bigger than 0 and less than 4611686018427387904", auth));

            long code = authCode | auth;
            return code;
        }
        /// <summary>
        /// RemoveAuth
        /// </summary>
        /// <param name="authCode">authCode</param>
        /// <param name="auth">auth, the power of 2</param>
        /// <remarks>code = authCode & (~auth)</remarks>
        /// <returns></returns>
        public static long RemoveAuth(long authCode, long auth)
        {
            if (!IsBinPower(auth))
                throw new Exception(string.Format("Illegal value of {0}, it must be the power of 2", auth));

            if (auth < 0 || auth > 4611686018427387904)
                throw new Exception(string.Format("Auth value of {0} must bigger than 0 and less than 4611686018427387904", auth));

            long code = authCode & (~auth);
            return code;
        }

        /// <summary>
        /// Verify has auth
        /// </summary>
        /// <param name="authCode">authCode</param>
        /// <param name="auth">auth, the power of 2</param>
        /// <remarks>auth == (authCode & auth)</remarks>
        /// <returns></returns>
        public static bool IsHasAuth(long authCode, long auth)
        {
            if (!IsBinPower(auth))
                throw new Exception(string.Format("Illegal value of {0}, it must be the power of 2", auth));

            if (authCode <= 0 || auth <= 0)
                return false;

            return auth == (authCode & auth);
        }
    }
}
