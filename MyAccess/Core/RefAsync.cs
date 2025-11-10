using System;
namespace MyAccess.Core
{
    /// <summary>
    /// 异步结构类型输出用
    /// </summary>
    public class RefAsync<T> where T : struct
    {
        public static implicit operator RefAsync<T>(T value) => new RefAsync<T>(value);
        public static implicit operator T(RefAsync<T> value) => value._value;
        private RefAsync(T value)
        {
            this._value = value;
        }
        private T _value;
        public T Value
        {
            get => _value;
            set => _value = value;
        }
    }
}
