using static InlineIL.IL;

namespace Jay.Utilities;

public static class Scary
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T NullRef<T>()
    {
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(ref T source)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Ceq();
        return Return<bool>();
    }
    
    /// <summary>
    /// Synchronously consume a <see cref="Task"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Consume(Task task)
    {
        if (task.IsCompleted)
            return;
        task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Synchronously consume a <see cref="Task{TResult}"/>
    /// </summary>
    public static TResult Consume<TResult>(Task<TResult> task)
    {
        if (task.IsCompleted)
            return task.Result;
        return task.GetAwaiter().GetResult();
    }
    
#if !(NET48 || NETSTANDARD2_0)
    public static void Consume(ValueTask valueTask)
    {
        if (valueTask.IsCompleted)
            return;
        valueTask.AsTask().GetAwaiter().GetResult();
    }

    public static TResult Consume<TResult>(ValueTask<TResult> valueTask)
    {
        if (valueTask.IsCompleted)
            return valueTask.Result;
        return valueTask
            .AsTask()
            .GetAwaiter()
            .GetResult();
    }
#endif
}