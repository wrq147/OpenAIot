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
        if (row.type == "数字") {
            if (!form[row.mapid]) {
                form[row.mapid] = Number(form[row.mapid]);
            }
        } else if (row.type == "时间") {
            form[row.mapid] = dayjs(form[row.mapid]).valueOf();
        } else if (row.type == "复选框") {
            if (form[row.mapid] && form[row.mapid].length > 0) {
                form[row.mapid] = form[row.mapid].join(",");
            } else {
                form[row.mapid] = "";
            }
        } else {
            if (!form[row.mapid]) {
                form[row.mapid] = "";
            }
        }
    }
}

//初始化自定义表单
export function setCustomDefaultValue(filedTableList, form, formRules, initForm) {
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
                formRules["RepBat." + rw.mapid] = rowRules;
            } else {
                let rowRules = [
                    { required: true, trigger: "blur", message: "请输入" + rw.name },
                ];
                formRules["RepBat." + rw.mapid] = rowRules;
            }
        }
    });
}