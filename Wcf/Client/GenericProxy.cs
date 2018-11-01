/*
ZoneFox Business Layer event processor based on GNU NEsper
Copyright (C) 2018 ZoneFox

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using System.ServiceModel;

namespace ZF.BL.Nesper.Wcf.Client
{
    public class GenericProxy<T> : ClientBase<T>, IDisposable where T : class
    {
        private volatile bool _isDisposed;

        public GenericProxy()
        {
        }

        public GenericProxy(string endpointCfgName) : base(endpointCfgName)
        {
        }

        public TResult InvokeRemoteAction<TResult>(Func<TResult> function)
        {
            return function.Invoke();
        }

        public void InvokeRemoteAction(Action action)
        {
            action.Invoke();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    CloseOrAbort();

                _isDisposed = true;
            }
        }

        private void CloseOrAbort()
        {
            bool success = false;
            try
            {
                if (State != CommunicationState.Faulted)
                {
                    Close();
                    success = true;
                }
            }
            finally
            {
                if (!success)
                    Abort();
            }
        }
    }
}