using System;

namespace TemplateAction.Label.Expression
{
    public interface IOp
    {
        /// <summary>
        /// 设置下一个单元
        /// </summary>
        IOp NextOp { set; }
        /// <summary>
        /// 除
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Div(TAVar a, TAVar b);
        /// <summary>
        /// 乘
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Mul(TAVar a, TAVar b);
        /// <summary>
        /// 加
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Add(TAVar a, TAVar b);
        /// <summary>
        /// 减
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Sub(TAVar a, TAVar b);
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Gt(TAVar a, TAVar b);
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Lt(TAVar a, TAVar b);
        /// <summary>
        /// 大于等于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Ge(TAVar a, TAVar b);
        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Le(TAVar a, TAVar b);
        /// <summary>
        /// 不等于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Ne(TAVar a, TAVar b);
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        TAVar Eq(TAVar a, TAVar b);
        TAVar Or(TAVar a, TAVar b);
        TAVar And(TAVar a, TAVar b);
        /// <summary>
        /// 负号
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        TAVar Neg(TAVar a);
        /// <summary>
        /// 取反
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        TAVar Reverse(TAVar a);
        /// <summary>
        /// 求余
        /// </summary>
        TAVar Mod(TAVar a, TAVar b);
    }
}
