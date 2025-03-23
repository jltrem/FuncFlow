namespace FuncFlow;

public class PipelineError(string message) : Exception(message);

public class PipelineStepException(string message, Exception innerException)
    : Exception(message, innerException);
    