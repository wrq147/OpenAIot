let Condition = () =>
    import ('./ConditionNodeConfig.vue')
let Concurrent = () =>
    import ('./ConcurrentNodeConfig.vue')
let Delay = () =>
    import ('./DelayNodeConfig.vue')
let Trigger = () =>
    import ('./TriggerNodeConfig.vue')
let Conversion = () =>
    import ('./ConversionNodeConfig.vue')
let Datawrite = () =>
    import ('./DataWriteNodeConfig.vue')
let Metronome = () =>
    import ('./MetronomeNodeConfig.vue')
let Warn = () =>
    import ('./WarnMessageNodeConfig.vue')
let Func = () =>
    import ('./FuncNodeConfig.vue')
let FormAuthorityConfig = () =>
    import ('./FormAuthorityConfig.vue')
let Except = () =>
    import ('./ExceptNodeConfig.vue')
let Redirect = () =>
    import ('./RedirectNodeConfig.vue')
let Cleardelta = () =>
    import ('./ClearDeltaNodeConfig.vue')
let Timescheduler = () =>
    import ('./TimeSchedulerNodeConfig.vue')
let Pid = () =>
    import ('./PIDNodeConfig.vue')
let Notice = () =>
    import ('./NoticeNodeConfig.vue')
let Tag = () =>
    import ('./TagNodeConfig.vue')
let Setprop = () =>
    import ('./SetPropNodeConfig.vue')
export default {
    Concurrent,
    Condition,
    Delay,
    Trigger,
    Conversion,
    Datawrite,
    Metronome,
    Warn,
    Func,
    Except,
    Redirect,
    Cleardelta,
    Timescheduler,
    Pid,
    Notice,
    Tag,
    Setprop,
    FormAuthorityConfig
}