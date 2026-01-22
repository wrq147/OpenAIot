using MyAccess.Aop;

namespace Common.Share
{
    /// <summary>
    /// 业务逻辑返回处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusResponse<T> : ITransReturn
    {
        private int mCode;
        private string mMessage;
        private T mData;
        public BusResponse() { }
        public BusResponse(int code, string message, T data)
        {
            mCode = code;
            mMessage = message;
            mData = data;
        }
        public int Code
        {
            get { return mCode; }
            set { mCode = value; }
        }
        public string Message
        {
            get { return mMessage; }
            set { mMessage = value; }
        }
        public T Data
        {
            get { return mData; }
            set
            {
                mData = value;
            }   
        }
        public DefaultAjaxResult<T> ToAjaxResult()
        {
            return new DefaultAjaxResult<T>(mCode, mMessage, mData);
        }
        public bool IsSuccess()
        {
            return mCode == Constants.SUCCESS_CODE;
        }

        public static BusResponse<T> Success(T value = default(T), string message = "")
        {
            return new BusResponse<T>(Constants.SUCCESS_CODE, message, value);
        }
        public static BusResponse<T> Error(int code, string message)
        {
            return new BusResponse<T>(code, message, default(T));
        }
        public static BusResponse<T> ErrorBusy()
        {
            return new BusResponse<T>(Constants.ERROR_BUSY, "业务繁忙,请稍候重试", default(T));
        }
    }
}
