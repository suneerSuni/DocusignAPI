using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DocuSignService.Util
{
    public class Impersonator : IDisposable
    {
        /// <summary>
        /// Used for FormdataSource windows Impersonation
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="domainName"></param>
        /// <param name="password"></param>
        public Impersonator(
            string userName,
            string domainName,
            string password)
        {
            ImpersonateValidUser(userName, domainName, password);
        }
        /// <summary>
        /// Used for SQL server app pool impersonation
        /// when connnection string has windows Auth
        /// </summary>
        public Impersonator()
        {
            //if (Facade.CoreConnectionStringManager.AppPoolUsr)
            //{
            //    impersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
            //}
        }
        /// <summary>
        /// Used for FormDataSource which has Windows auth without Impersonation.
        /// If SQL conn string has Windows Auth, impersonation will be set in 
        /// abve function, in that case no need to execute below 
        /// func(thats why negation in condition)
        /// </summary>
        /// <param name="formDS"></param>
        public Impersonator(bool formDS)
        {
            //if (!Facade.CoreConnectionStringManager.AppPoolUsr)
            //{
            //    impersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
            //}
        }
        public void Dispose()
        {
            UndoImpersonation();
        }
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int LogonUser(
            string lpszUserName,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(
            IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(
            IntPtr handle);

        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private void ImpersonateValidUser(
            string userName,
            string domain,
            string password)
        {
            WindowsIdentity tempWindowsIdentity = null;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            try
            {
                if (RevertToSelf())
                {
                    if (LogonUser(
                        userName,
                        domain,
                        password,
                        LOGON32_LOGON_INTERACTIVE,
                        LOGON32_PROVIDER_DEFAULT,
                        ref token) != 0)
                    {
                        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            impersonationContext = tempWindowsIdentity.Impersonate();
                        }
                        else
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }
                    else
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    CloseHandle(token);
                }
                if (tokenDuplicate != IntPtr.Zero)
                {
                    CloseHandle(tokenDuplicate);
                }
            }
        }
        private void ImpersonateAppPoolUser()
        {
            WindowsIdentity tempWindowsIdentity = null;
            IntPtr token = IntPtr.Zero;

            try
            {
                if (RevertToSelf())
                {
                    tempWindowsIdentity = new WindowsIdentity(token);
                    impersonationContext = tempWindowsIdentity.Impersonate();
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    CloseHandle(token);
                }
            }
        }
        private void UndoImpersonation()
        {
            if (impersonationContext != null)
            {
                impersonationContext.Undo();
            }
        }
        private WindowsImpersonationContext impersonationContext = null;
    }
}
