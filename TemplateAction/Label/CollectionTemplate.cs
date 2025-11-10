namespace TemplateAction.Label
{
    /// <summary>
    /// 集合模板，所有有子项的模板继承自此
    /// </summary>
    public abstract class CollectionTemplate : Template
    {
        public delegate void InitAction();
        public delegate bool LoopAction(Template child);
        public void LoopChild(ITemplateContext context, LoopAction loop)
        {
            LoopChild(context, loop, null);
        }
        public void LoopChild(ITemplateContext context, LoopAction loop, InitAction init)
        {
            int popcount = context.StackCount();
            init?.Invoke();
            foreach (Template child in mChilds)
            {
                if (!loop(child) || context.BreakCount > 0)
                {
                    break;
                }
            }
            popcount = context.StackCount() - popcount;
            while (popcount > 0)
            {
                context.PopGlobal();
                --popcount;
            }
        }
    }
}
