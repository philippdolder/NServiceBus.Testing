﻿#pragma warning disable 1591

namespace NServiceBus.Testing
{
    using System;

    public partial class Saga<T> where T : Saga
    {
        [ObsoleteEx(
            Message = "Use 'ExpectSagaCompleted' or 'ExpectSagaNotCompleted' instead",
            RemoveInVersion = "8",
            TreatAsErrorFromVersion = "7")]
        public Saga<T> AssertSagaCompletionIs(bool complete)
        {
            throw new NotImplementedException();
        }

        [ObsoleteEx(
            Message = "HandleCurrentMessageLater has been deprecated.",
            RemoveInVersion = "8",
            TreatAsErrorFromVersion = "7")]
        public Saga<T> ExpectHandleCurrentMessageLater()
        {
            throw new NotSupportedException();
        }
    }

    public partial class Handler<T>
    {
        [ObsoleteEx(
            Message = "HandleCurrentMessageLater has been deprecated.",
            RemoveInVersion = "8",
            TreatAsErrorFromVersion = "7")]
        public Handler<T> ExpectHandleCurrentMessageLater()
        {
            throw new NotSupportedException();
        }
    }
}

#pragma warning restore 1591