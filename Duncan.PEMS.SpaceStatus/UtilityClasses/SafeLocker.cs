// This code is based on blog at: http://blog.decarufel.net/2009/06/how-to-implement-lock-with-timeout.html
using System;
using System.Linq;
using System.Threading;

namespace Duncan.PEMS.SpaceStatus.UtilityClasses
{
    public class SafeLocker : IDisposable
    {
        private readonly object[] _padlocks;
        private readonly bool[] _securedFlags;

        private SafeLocker(object padlock, int milliSecondTimeout)
        {
            _padlocks = new[] { padlock };
            _securedFlags = new[] { Monitor.TryEnter(padlock, milliSecondTimeout) };
        }

        private SafeLocker(object[] padlocks, int milliSecondTimeout)
        {
            _padlocks = padlocks;
            _securedFlags = new bool[_padlocks.Length];
            for (int i = 0; i < _padlocks.Length; i++)
                _securedFlags[i] = Monitor.TryEnter(padlocks[i], milliSecondTimeout);
        }

        public bool Secured
        {
            get { return _securedFlags.All(s => s); }
        }

        public static void Lock(object[] padlocks, int millisecondTimeout, Action codeToRun)
        {
            using (var bolt = new SafeLocker(padlocks, millisecondTimeout))
                if (bolt.Secured)
                    codeToRun();
                else
                    throw new TimeoutException(string.Format("SafeLocker.Lock wasn't able to acquire a lock in {0}ms",
                                                             millisecondTimeout));
        }

        public static void Lock(object padlock, int millisecondTimeout, Action codeToRun)
        {
            using (var bolt = new SafeLocker(padlock, millisecondTimeout))
                if (bolt.Secured)
                    codeToRun();
                else
                    throw new TimeoutException(string.Format("SafeLocker.Lock wasn't able to acquire a lock in {0}ms",
                                                             millisecondTimeout));
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            for (int i = 0; i < _securedFlags.Length; i++)
                if (_securedFlags[i])
                {
                    Monitor.Exit(_padlocks[i]);
                    _securedFlags[i] = false;
                }
        }

        #endregion
    }
}
