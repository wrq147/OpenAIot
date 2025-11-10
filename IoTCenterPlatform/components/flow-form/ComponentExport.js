let TextInput = () =>
    import ('./form-components/text-input.vue')
let NumberInput = () =>
    import ('./form-components/number-input.vue')
let AmountInput = () =>
    import ('./form-components/amount-input.vue')
let TextareaInput = () =>
    import ('./form-components/textarea-input.vue')
let SelectInput = () =>
    import ('./form-components/select-input.vue')
let MultipleSelect = () =>
    import ('./form-components/multiple-select.vue')
let DateTime = () =>
    import ('./form-components/DateTime.vue')
let DateTimeRange = () =>
    import ('./form-components/DateTimeRange.vue')

let Description = () =>
    import ('./form-components/Description.vue')
let ImageUpload = () =>
    import ('./form-components/ImageUpload.vue')
let FileUpload = () =>
    import ('./form-components/FileUpload.vue')
let Location = () =>
    import ('./form-components/Location.vue')
let DeptPicker = () =>
    import ('./form-components/DeptPicker.vue')
let UserPicker = () =>
    import ('./form-components/UserPicker.vue')
let DevicPicker = () =>
    import ('./form-components/DevicPicker.vue')
let SignPanel = () =>
    import ('./form-components/SignPannel.vue')

let SpanLayout = () =>
    import ('./form-components/SpanLayout.vue')
let TableList = () =>
    import ('./form-components/TableList.vue')

let ParamInput = () =>
    import ('./form-components/ParamInput.vue')
export default {
    //基础组件
    TextInput,
    NumberInput,
    AmountInput,
    TextareaInput,
    SelectInput,
    MultipleSelect,
    DateTime,
    DateTimeRange,
    UserPicker,
    DeptPicker,
    DevicPicker,
    //高级组件
    Description,
    FileUpload,
    ImageUpload,
    Location,
    SignPanel,
    SpanLayout,
    TableList,
    ParamInput
}