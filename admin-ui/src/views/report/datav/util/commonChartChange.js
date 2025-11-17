export function stringDataHandle(result, chartOption) { //展示类型为字符串的处理
    if (chartOption && chartOption.dataSourceType == "gobal" && chartOption.globalData && chartOption.globalProcessor) {
        let rawData = result.rawData ? JSON.parse(result.rawData) : []
        if (rawData && rawData.length > 0) {
            let sameProcessor = rawData.find(row => row.title == chartOption.globalProcessor)
            if (sameProcessor) {
                if (sameProcessor.content && sameProcessor.content.length > 0) {
                    if (chartOption.tableSelectLine && chartOption.tableSelectLine.length > 0 && sameProcessor.content.length > 0) {
                        return sameProcessor.content[0][chartOption.tableSelectLine[0]['filed']]
                    } else {
                        return ''
                    }
                } else {
                    return ''
                }
            }
        }
    }
}
export function stringArrayDataHandle(result, chartOption) { //展示数据为单个字符串数组的处理
    if (chartOption && chartOption.dataSourceType == "gobal" && chartOption.globalData && chartOption.globalProcessor) {
        let rawData = result.rawData ? JSON.parse(result.rawData) : []
        if (rawData && rawData.length > 0) {
            let sameProcessor = rawData.find(row => row.title == chartOption.globalProcessor)
            if (sameProcessor) {
                if (sameProcessor.content && sameProcessor.content.length > 0) {
                    if (chartOption.tableSelectLine && chartOption.tableSelectLine.length > 0 && sameProcessor.content.length > 0) {
                        let arr = sameProcessor.content.map(row => row[chartOption.tableSelectLine[0]])
                        return arr
                    } else {
                        return []
                    }
                } else {
                    return []
                }
            }
        }
    }
}
export function objectPaigangBlockDataHandle(result, chartOption) { //展示数据为数组类型的处理
    if (chartOption && chartOption.dataSourceType == "gobal" && chartOption.globalData && chartOption.globalProcessor) {
        let rawData = result.rawData ? JSON.parse(result.rawData) : []
        if (rawData && rawData.length > 0) {
            let sameProcessor = rawData.find(row => row.title == chartOption.globalProcessor)
            if (sameProcessor) {
                if (sameProcessor.content && sameProcessor.content.length > 0) {
                    if (chartOption.tableSelectLine && chartOption.tableSelectLine.length > 0 && sameProcessor.content.length > 0) {
                        let resArr = []
                        try {
                            for (let i = 0; i < sameProcessor.content.length; i++) {
                                let rowContent = sameProcessor.content[i]
                                let obj = {}
                                for (let j = 0; j < chartOption.tableSelectLine.length; j++) {
                                    let rowLine = chartOption.tableSelectLine[j]

                                    if (rowLine.type !== 'array' && rowLine.key != 'jinduType') {
                                        if (rowLine.filed) {
                                            obj[rowLine.key] = rowContent[rowLine.filed]
                                        } else {
                                            obj[rowLine.key] = null
                                        }

                                    } else if (rowLine.type !== 'array' && rowLine.key == "jinduType") {
                                        obj[rowLine.key] = rowLine.filed

                                    } else {
                                        let list = rowLine.filed.map(rn => {
                                            return rowContent[rn]
                                        })
                                        obj[rowLine.key] = list
                                    }

                                }
                                resArr.push(obj)
                            }
                        } catch (error) {
                            console.log("errorerrorerror", error);
                        }
                        return resArr
                    } else {
                        return []
                    }
                } else {
                    return []
                }
            }
        }
    }
}
export function objectArrayDataHandle(result, chartOption) { //展示数据为数组类型的处理
    if (chartOption && chartOption.dataSourceType == "gobal" && chartOption.globalData && chartOption.globalProcessor) {
        let rawData = result.rawData ? JSON.parse(result.rawData) : []
        if (rawData && rawData.length > 0) {
            let sameProcessor = rawData.find(row => row.title == chartOption.globalProcessor)
            if (sameProcessor) {
                if (sameProcessor.content && sameProcessor.content.length > 0) {
                    if (chartOption.tableSelectLine && chartOption.tableSelectLine.length > 0 && sameProcessor.content.length > 0) {
                        let resArr = []
                        for (let i = 0; i < sameProcessor.content.length; i++) {
                            let rowContent = sameProcessor.content[i]
                            let obj = {}
                            for (let j = 0; j < chartOption.tableSelectLine.length; j++) {
                                let rowLine = chartOption.tableSelectLine[j]
                                obj[rowLine.key] = rowContent[rowLine.filed]
                            }
                            resArr.push(obj)
                        }
                        return resArr
                    } else {
                        return []
                    }
                } else {
                    return []
                }
            }
        }
    }
}
export function cardDataHandle(result, chartOption) { //卡片组件的数据处理
    if (chartOption && chartOption.dataSourceType == "gobal" && chartOption.globalData && chartOption.globalProcessor) {
        if (result && result.length > 0 && Array.isArray(result)) {
            let keysArr = Object.keys(chartOption.tableSelectLine)
            let tableObj = {}
            let mainSelecArr = chartOption.tableSelectLine.main
            let mainRawData = JSON.parse((result.find(row => row.name == chartOption.globalData)).rawData)
            let mainProcessor = mainRawData.find(row => row.title == chartOption.globalProcessor)
            if (!(mainProcessor.content && mainProcessor.content.length > 0)) { return [] }
            let mainTableData = mainProcessor.content
            let resArr = []
            for (let i = 0; i < mainTableData.length; i++) {
                let rowContent = mainTableData[i]
                let obj = {}
                for (let j = 0; j < mainSelecArr.length; j++) {
                    let rowLine = mainSelecArr[j]
                    if (rowLine && keysArr.includes(rowLine.key)) { //判断该字段是否是数组类型，是数组类型需要重新选表
                        let selectKeyDataName = chartOption.configProcessorTabs.find(row => row.name == rowLine.filed)
                        if (selectKeyDataName && selectKeyDataName.globalData && selectKeyDataName.globalProcessor) {
                            let rowData = (result.find(row => row.name == selectKeyDataName.globalData)) ? JSON.parse((result.find(row => row.name == selectKeyDataName.globalData)).rawData) : undefined
                            if (rowData) {
                                let rowProcessor = rowData.find(row => row.title == selectKeyDataName.globalProcessor)
                                if (rowProcessor.content && rowProcessor.content.length > 0) {
                                    tableObj[rowLine.key] = rowProcessor.content
                                } else {
                                    tableObj[rowLine.key] = []
                                }
                                if (tableObj[rowLine.key] && tableObj[rowLine.key].length > 0) {
                                    obj[rowLine.key] = []
                                    for (let l = 0; l < tableObj[rowLine.key].length; l++) {
                                        let childrenRowContent = tableObj[rowLine.key][l]
                                        let childrenRowObj = {}
                                        for (let k = 0; k < chartOption.tableSelectLine[rowLine.key].length; k++) {
                                            let rowChildrenLine = chartOption.tableSelectLine[rowLine.key][k]
                                            childrenRowObj[rowChildrenLine.key] = childrenRowContent[rowChildrenLine.filed]
                                        }
                                        obj[rowLine.key].push(childrenRowObj)
                                    }

                                } else {
                                    obj[rowLine.key] = []
                                    let childrenRowObj = {}
                                    for (let k = 0; k < chartOption.tableSelectLine[rowLine.key].length; k++) {
                                        let rowChildrenLine = chartOption.tableSelectLine[rowLine.key][k]
                                        childrenRowObj[rowChildrenLine.key] = ''
                                    }
                                    obj[rowLine.key].push(childrenRowObj)
                                }
                            }

                        }
                    } else {
                        if (rowLine.parentIdKey) {
                            if (obj[rowLine.parentIdKey]) {
                                obj[rowLine.parentIdKey][rowLine.key] = rowContent[rowLine.filed]
                            } else {
                                obj[rowLine.parentIdKey] = {}
                                obj[rowLine.parentIdKey][rowLine.key] = rowContent[rowLine.filed]
                            }
                        } else {
                            obj[rowLine.key] = rowContent[rowLine.filed]
                        }
                    }
                }
                resArr.push(obj)
            }
            return resArr

        } else {
            return []
        }
    } else {
        return []
    }
}
export function childrenArrayDataHandle(result, chartOption) { //展示数据子集包含数组的处理
    if (chartOption && chartOption.dataSourceType == "gobal" && chartOption.globalData && chartOption.globalProcessor) {
        let rawData = result.rawData ? JSON.parse(result.rawData) : []
        if (rawData && rawData.length > 0) {
            let sameProcessor = rawData.find(row => row.title == chartOption.globalProcessor)
            if (sameProcessor) {
                if (sameProcessor.content && sameProcessor.content.length > 0) {
                    if (chartOption.tableSelectLine && chartOption.tableSelectLine.length > 0 && sameProcessor.content.length > 0) {
                        let resArr = getTreeDatafunc(sameProcessor.content, chartOption.tableSelectLine)
                        return resArr
                    } else {
                        return []
                    }
                } else {
                    return []
                }
            }
        }
    }
}

function getTreeDatafunc(orginData, tableSelectLine) { //子集数据处理
    let resultData = []
    for (let i = 0; i < orginData.length; i++) {
        let rowContent = orginData[i]
        let obj = {}
        for (let j = 0; j < tableSelectLine.length; j++) {
            let rowLine = tableSelectLine[j]
            let rowval = rowContent[rowLine.filed]
            if (Array.isArray(rowval)) {
                if (rowContent[rowLine.filed].length > 0) {
                    obj[rowLine.key] = getTreeDatafunc(rowContent[rowLine.filed], tableSelectLine)
                } else {
                    obj[rowLine.key] = null
                }
            } else {
                obj[rowLine.key] = rowContent[rowLine.filed]
            }

        }
        resultData.push(obj)
    }

    return resultData
}
export function heatDataHandle(result, chartOption) { //热力图展示数据的处理
    if (chartOption && chartOption.dataSourceType == "gobal" && chartOption.globalData && chartOption.globalProcessor) {
        let rawData = result.rawData ? JSON.parse(result.rawData) : []
        if (rawData && rawData.length > 0) {
            let sameProcessor = rawData.find(row => row.title == chartOption.globalProcessor)
            if (sameProcessor) {
                if (sameProcessor.content && sameProcessor.content.length > 0) {
                    if (chartOption.tableSelectLine && typeof chartOption.tableSelectLine == 'object' && sameProcessor.content.length > 0) {
                        let resArr = []
                        for (let key in chartOption.tableSelectLine) {
                            if (key !== 'data') {
                                if (resArr && resArr.length > 0) {
                                    for (const i in resArr) {
                                        resArr[i][key] = sameProcessor.content.map(row => row[chartOption.tableSelectLine[key]]).filter((item, index, arr) => arr.indexOf(item) === index);
                                    }
                                } else {
                                    resArr = [{ xAxisData: [], yAxisData: [], data: [] }]
                                    for (const i in resArr) {
                                        resArr[i][key] = sameProcessor.content.map(row => row[chartOption.tableSelectLine[key]]).filter((item, index, arr) => arr.indexOf(item) === index);
                                    }
                                }
                            }
                        }
                        if (resArr && resArr.length > 0 && chartOption.tableSelectLine.data) { //处理data的数据
                            for (const i in resArr) {
                                if (resArr[i].xAxisData.length > 0 && resArr[i].yAxisData.length > 0) {
                                    let arr = []
                                    sameProcessor.content.map(rs => {
                                        let arr1 = []
                                        let num1 = resArr[i].yAxisData.indexOf(rs[chartOption.tableSelectLine.yAxisData])
                                        let num2 = resArr[i].xAxisData.indexOf(rs[chartOption.tableSelectLine.xAxisData])
                                        arr1.push(num1)
                                        arr1.push(num2)
                                        let dataval = rs[chartOption.tableSelectLine.data] ? rs[chartOption.tableSelectLine.data] : 0
                                        arr1.push(dataval)
                                        arr.push(arr1)
                                    })
                                    resArr[i].data = arr
                                }
                            }
                        }
                        return resArr
                    } else {
                        return []
                    }
                } else {
                    return []
                }
            }
        }
    }
}