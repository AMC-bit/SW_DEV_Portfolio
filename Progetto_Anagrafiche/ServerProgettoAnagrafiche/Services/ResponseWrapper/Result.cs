namespace BlazorServerProgettoAnagrafiche.Services.ResponseWrapper
{
    public class Result
        {
        // refactored, can return just success/failure with errors now
        public List<string> Errors { get; set; } = new List<string>();
        public bool Success => Errors.Count == 0;
        }

    public class Return<T> : Result
        {
        // refactored, optionally I can also return data
        public T? Data { get; set; }
        }
    }

