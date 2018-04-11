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
        private const string MutexGuid = "f043c687-6714-45b8-b293-9939066dcd73";

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
