using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace HidWizards.UCR.Utilities
{
    internal class SingleGlobalInstance : IDisposable
    {
        public bool HasHandle { get; }
        Mutex _mutex;
        private const string MutexGuid = "edfb1606-ecfd-4bc4-9a41-56d3ba492d62";

        private void InitMutex()
        {
            var mutexId = $"Global\\{{{MutexGuid}}}";
            _mutex = new Mutex(false, mutexId);

            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            _mutex.SetAccessControl(securitySettings);
        }

        public SingleGlobalInstance()
        {
            HasHandle = false;
            InitMutex();
            try
            {
                HasHandle = _mutex.WaitOne(0, false);
            }
            catch (AbandonedMutexException)
            {
                HasHandle = true;
            }
        }


        public void Dispose()
        {
            if (_mutex == null) return;
            if (HasHandle)
            {
                _mutex.ReleaseMutex();
                _mutex.Dispose();
            }
        }
    }
}
