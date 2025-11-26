import {
    parseTime
} from './common'

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
