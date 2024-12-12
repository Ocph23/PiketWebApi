//using System.Text.Json.Serialization;

//namespace PiketWebApi.Abstractions
//{
//    public enum ErrorType
//    {
//        Failure = 0,
//        NotFound = 1,
//        Validation = 2,
//        Conflict = 3,
//        AccessUnAuthorized = 4,
//        AccessForbidden = 5
//    }


//    public class Error
//    {
//        private Error(
//            string code,
//            string description,
//            ErrorType errorType
//        )
//        {
//            Code = code;
//            Description = description;
//            ErrorType = errorType;
//        }

//        public string Code { get; }

//        public string Description { get; }

//        public ErrorType ErrorType { get; }

//        public static Error Failure(string code, string description) =>
//            new(code, description, ErrorType.Failure);

//        public static Error NotFound(string code, string description) =>
//            new(code, description, ErrorType.NotFound);

//        public static Error Validation(string code, string description) =>
//            new(code, description, ErrorType.Validation);

//        public static Error Conflict(string code, string description) =>
//            new(code, description, ErrorType.Conflict);

//        public static Error AccessUnAuthorized(string code, string description) =>
//            new(code, description, ErrorType.AccessUnAuthorized);

//        public static Error AccessForbidden(string code, string description) =>
//            new(code, description, ErrorType.AccessForbidden);
//    }

//    public class Result
//    {
//        protected Result()
//        {
//            IsSuccess = true;
//            Error = default;
//        }

//        protected Result(Error error)
//        {
//            IsSuccess = false;
//            Error = error;
//        }

//        public bool IsSuccess { get; }

//        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
//        public Error? Error { get; }

//        public static implicit operator Result(Error error) =>
//            new(error);

//        public static Result Success() =>
//            new();

//        public static Result Failure(Error error) =>
//            new(error);
//    }

//    public sealed class ResultT<TValue> : Result
//    {
//        private readonly TValue? _value;

//        private ResultT(
//            TValue value
//        ) : base()
//        {
//            _value = value;
//        }

//        private ResultT(
//            Error error
//        ) : base(error)
//        {
//            _value = default;
//        }

//        public TValue Value =>
//            IsSuccess ? _value! : throw new InvalidOperationException("Value can not be accessed when IsSuccess is false");

//        public static implicit operator ResultT<TValue>(Error error) =>
//            new(error);

//        public static implicit operator ResultT<TValue>(TValue value) =>
//            new(value);

//        public static ResultT<TValue> Success(TValue value) =>
//            new(value);

//        public static new ResultT<TValue> Failure(Error error) =>
//            new(error);
//    }
//    public static class ResultExtensions
//    {
//        public static T Match<T>(
//            this Result result,
//            Func<T> onSuccess,
//            Func<Error, T> onFailure)
//        {
//            return result.IsSuccess ? onSuccess() : onFailure(result.Error!);
//        }

//        public static T Match<T, TValue>(
//            this ResultT<TValue> result,
//            Func<TValue, T> onSuccess,
//            Func<Error, T> onFailure)
//        {
//            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error!);
//        }
//    }
//}
