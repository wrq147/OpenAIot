using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Common.Attr
{
    /// <summary>
    /// 关联Id字段的必填验证
    /// </summary>
    public class RequiredWithAttribute : ValidationAttribute
    {
        /// <summary>
        /// 指定关联Id字段（当前Id字段为null,则当前字段必填）
        /// </summary>
        public string WithNullId { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var prop = validationContext.ObjectType.GetProperty(WithNullId);
            if (prop.GetValue(validationContext.ObjectInstance) == null)
            {
                if (value == null)
                {
                    return new ValidationResult(this.ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
