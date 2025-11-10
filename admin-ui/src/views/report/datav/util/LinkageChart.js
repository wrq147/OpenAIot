import { parseTime } from '@/utils/common'

// 获取可绑定的组件
export function getLinkChart(drawingList) {
    //条件类组件
    let exclusion = ["timeFrame", "input", "inputParam", "select", "cascade", "tab", "textCheckBox", "timeline", "group"];

    let renderCharts = drawingList.filter(item => {
        return exclusion.indexOf(item.chartType) != -1
    })
    return renderCharts;
}

// 判断参数是否关联组件
export function isLinkParam(str) {
    if (typeof str === "string" && str.indexOf('@') == 0) {
        return true;
    } else {
        return false;
    }
}

export function getLinkChartVal(drawingList, name, option, tableType) {
    if (!isLinkParam(name)) {
        return name;
    }
    let newname = name.substring(1);
    let prename = "";
    let lastat = newname.indexOf("@");
    let lastat2 = newname.indexOf("#"); //处理带序号的时间范围选择
    let paramsInx = -1
    if (lastat > -1) {
        prename = newname.substring(lastat + 1);
        newname = newname.substring(0, lastat);
    }
    if (lastat2 > 0) {
        paramsInx = Number(newname.substring(lastat2 + 1))
        newname = newname.substring(0, lastat2);
    }
    if (tableType && tableType == 'spreadSheet') {
        let searchTableData = JSON.parse(localStorage.getItem("searchTableData"))
        if (searchTableData === null) return
        let valObj = searchTableData.find(rw => rw.fieldName == newname)
        return valObj.formatDefault
    } else {
        let newChartList = drawingList.filter(x => x.layerName == newname);
        if (newChartList.length > 0) {
            if (newChartList[0].chartOption.getVal != null) {
                if (newChartList[0].chartOption.preprocess != null && prename != "") {
                    if (prename in option.preprocess) {
                        let code = option.preprocess[prename];
                        let val = newChartList[0].chartOption.getVal();
                        return (eval(code)).call({ parseTime }, val);
                    }
                } else if (paramsInx > -1) { //处理时间范围选择的值
                    if ((newChartList[0].chartOption.getVal())) {
                        return (newChartList[0].chartOption.getVal())[paramsInx];
                    } else {
                        return newChartList[0].chartOption.getVal();
                    }

                }
                return newChartList[0].chartOption.getVal();
            } else if (newChartList[0].chartOption.getVal == undefined) {
                return ''
            }
        }
    }

    return name;
}

function loopReplace(drawingList, target, parent, key, option, tableType) {
    if (typeof target === "string") {
        parent[key] = getLinkChartVal(drawingList, target, option, tableType);
    } else {
        if (target !== null) {
            if (target instanceof Array) {
                target.forEach((item, index) => {
                    loopReplace(drawingList, item, target, index, option, tableType);
                })
            } else if (typeof target === "object") {
                for (let key in target) {
                    loopReplace(drawingList, target[key], target, key, option, tableType);
                }
            }
        }

    }

}

export function replaceLinkParam(drawingList, option, tableType) {
    let newOption = JSON.parse(JSON.stringify(option));
    if (newOption.requestParamType == "PARAM" || newOption.requestParamType == "FORM") {
        if (newOption.requestParameters) {
            newOption.requestParameters.forEach(element => {
                element.value = getLinkChartVal(drawingList, element.value, newOption, tableType);
            });
        }
    } else if (newOption.requestParamType == "JSON") {
        for (let key in newOption.requestParameters) {
            loopReplace(drawingList, newOption.requestParameters[key], newOption.requestParameters, key, newOption, tableType);
        }
    }
    return newOption;
}