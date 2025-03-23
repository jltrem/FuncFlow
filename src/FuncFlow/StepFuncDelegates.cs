namespace FuncFlow;


// StepFunc delegates (for 0, 1, 2, and 3 dependencies)

public delegate TContext StepFunc<TContext>(TContext context);
public delegate TContext StepFunc<TContext, in T1>(TContext context, T1 dep1);
public delegate TContext StepFunc<TContext, in T1, in T2>(TContext context, T1 dep1, T2 dep2);
public delegate TContext StepFunc<TContext, in T1, in T2, in T3>(TContext context, T1 dep1, T2 dep2, T3 dep3);
public delegate Task<TContext> StepFuncAsync<TContext>(TContext context);
public delegate Task<TContext> StepFuncAsync<TContext, in T1>(TContext context, T1 dep1);
public delegate Task<TContext> StepFuncAsync<TContext, in T1, in T2>(TContext context, T1 dep1, T2 dep2);
public delegate Task<TContext> StepFuncAsync<TContext, in T1, in T2, in T3>(TContext context, T1 dep1, T2 dep2, T3 dep3);

