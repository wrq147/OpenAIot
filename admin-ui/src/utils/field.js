import {
    parseTime
} from './common'
var dayjs = require("@/utils/day.js");
//显示自定义字段
export function getFieldShow(model, field) {
    let modelval = model[field.mapid];
    switch (field.type) {
        case "时间":
            return parseTime(modelval);
        case "图片":
            return '<el-image fit="cover" style="width: 54px; height: 54px" :src="' + modelval + '?wh=500x500"><div slot="error" class="image-slot"><i class="el-icon-picture-outline"></i></div></el-image>';
        case "关联对象":
            {
                if (modelval && modelval.indexOf(",") > -1) {
                    let arr = modelval.split(",");
                    return arr[1];
                } else {
                    return "";
                }
            }
        default:
            return modelval;

    }
}

export function checkBeforeSave(filedTableList, form) {
    for (let i = 0; i < filedTableList.length; i++) {
        let row = filedTableList[i];
        if (row.type == "时间") {
            form[row.mapid] = dayjs(form[row.mapid]).valueOf();
        } else if (row.type == "复选框") {
            if (form[row.mapid] && form[row.mapid].length > 0) {
                form[row.mapid] = form[row.mapid].join(",");
            }
        }
    }
}

//初始化自定义表单
export function setCustomDefaultValue(filedTableList, form, formRules, initForm, pre) {
    //设置自定义的变量初始化
    filedTableList.map((rw) => {
        if (initForm) {
            if (rw.type == "时间") {
                let newStr = rw.format.replace(/y/g, "Y");
                newStr = newStr.replace(/d/g, "D");
                form[rw.mapid] = dayjs(initForm[rw.mapid]).format(newStr);
            } else {
                if (rw.type == "数字") {
                    form[rw.mapid] = Number(initForm[rw.mapid]);
                } else if (rw.type == "复选框") {
                    form[rw.mapid] = initForm[rw.mapid].split(",");
                } else {
                    form[rw.mapid] = initForm[rw.mapid];
                }
            }
        } else {
            if (rw.defval != "" && rw.defval != undefined && rw.defval != null) {
                if (rw.type == "时间") {
                    let newStr = rw.format.replace(/y/g, "Y");
                    newStr = newStr.replace(/d/g, "D");
                    form[rw.mapid] = dayjs(rw.defval).format(newStr);
                } else {
                    if (rw.type == "数字") {
                        form[rw.mapid] = Number(rw.defval);
                    } else {
                        form[rw.mapid] = rw.defval;
                    }
                }
            } else {
                if (rw.type == "复选框") {
                    form[rw.mapid] = [];
                } else if (rw.type == "时间") {
                    form[rw.mapid] = '';
                }
            }
        }
        if (rw.is_required) {
            if (rw.type == "单选框" || rw.type == "复选框" || rw.type == "时间") {
                let rowRules = [
                    {
                        required: true,
                        trigger: "change",
                        message: "请选择" + rw.name,
                    },
                ];
                formRules[pre + rw.mapid] = rowRules;
            } else {
                let rowRules = [
                    { required: true, trigger: "blur", message: "请输入" + rw.name },
                ];
                formRules[pre + rw.mapid] = rowRules;
            }
        }
    });
}


export function setFormItemHide(item, form) {
    //判断字段是否隐藏
    if (item.conditions && item.conditions.length > 0) {
        let result = false;
        let conditionsResArr = [];
        for (let i = 0; i < item.conditions.length; i++) {
            let row = item.conditions[i];
            conditionsResArr[i] = returnCompareResult(
                row.field,
                row.compare,
                row.val,
                row.valtype,
                form
            );
        }
        for (let i = 0; i < conditionsResArr.length; i++) {
            if (i == 0) {
                result = conditionsResArr[i];
            } else {
                if (item.groups && item.groups[i - 1]) {
                    if (item.groups[i - 1] == "and") {
                        result = result && conditionsResArr[i];
                    } else if (item.groups[i - 1] == "or") {
                        result = result || conditionsResArr[i];
                    }
                } else {
                    result = result || conditionsResArr[i];
                }
            }
        }
        return result;
    } else {
        return false;
    }
}
function returnCompareResult(field, compare, val, type, form) {
    //隐藏规则设置方法
    let result = true;
    switch (compare) {
        case "=":
            result = form[field] == val;
            break;
        case "!=":
            result = form[field] != val;
            break;
        case "IN":
            result = form[field] && form[field].indexOf(val) > -1;
            break;
        case "NOTIN":
            result =
                !form[field] ||
                (form[field] && form[field].indexOf(val) == -1);
            break;
        case "ISNULL":
            result = form[field] == "" || form[field] == null;
            break;
        case "NOTNULL":
            result = form[field] != "" && form[field] != null;
            break;
        case ">":
            if (type && type == "时间") {
                result = val.timeValue && dayjs(form[field]).valueOf() > dayjs(val.timeValue).valueOf();
            } else if (type && type == "数字") {
                result = form[field] > val;
            }
            break;
        case "<":
            if (type && type == "时间") {
                result =
                    val.timeValue &&
                    dayjs(form[field]).valueOf() <
                    dayjs(val.timeValue).valueOf();
            } else if (type && type == "数字") {
                result = form[field] < val;
            }
            break;
        case "==":
            if (type && type == "时间") {
                result =
                    val.timeValue &&
                    dayjs(form[field]).valueOf() ==
                    dayjs(val.timeValue).valueOf();
            } else if (type && type == "数字") {
                result = form[field] == val;
            }
            break;
        case "><":
            if (type && type == "时间") {
                result =
                    val.timeValue &&
                    dayjs(form[field]).valueOf() !=
                    dayjs(val.timeValue).valueOf();
            } else if (type && type == "数字") {
                result = form[field] != val;
            }
            break;
        case ">=":
            if (type && type == "时间") {
                result =
                    val.timeValue &&
                    dayjs(form[field]).valueOf() >=
                    dayjs(val.timeValue).valueOf();
            } else if (type && type == "数字") {
                result = form[field] >= val;
            }
            break;
        case "<=":
            if (type && type == "时间") {
                result =
                    val.timeValue &&
                    dayjs(form[field]).valueOf() <=
                    dayjs(val.timeValue).valueOf();
            } else if (type && type == "数字") {
                result = form[field] <= val;
            }
            break;
        case "INRANGE":
            if (type && type == "数字") {

                if (val.min && val.max) {
                    if (form[field] >= val.min && form[field] <= val.max) {
                        result = true;
                    } else {
                        result = false;
                    }
                }
            }

            break;
        case "NOTINRANGE":
            if (type && type == "数字") {
                if (val.min && val.max) {
                    if (form[field] < val.min && form[field] > val.max) {
                        result = true;
                    } else {
                        result = false;
                    }
                }
            }
            break;
        case "SELECTRANGE":
            if (type && type == "时间") {
                if (val[0] && val[1]) {
                    let max = Math.max(...val);
                    let min = Math.min(...val);
                    if (form[field] >= min && form[field] <= max) {
                        result = true;
                    } else {
                        result = false;
                    }
                }
            }
            break;
        case "DYNAMICS":
            if (type && type == "时间") {
                if (val[0] && val[1]) {
                    let max = Math.max(...val);
                    let min = Math.min(...val);
                    if (form[field] >= min && form[field] <= max) {
                        result = true;
                    } else {
                        result = false;
                    }
                }
            }
            break;
    }
    return result;
}