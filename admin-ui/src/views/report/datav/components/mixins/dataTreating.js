// import Vue from 'vue'
let resultData = [];
let tableData = [];
let columnData = [];
let optionData = [];
let modelValue = [];
export async function initResultStart (dataOption, res) {
    let resData = JSON.parse(JSON.stringify(res))
    optionData = JSON.parse(JSON.stringify(dataOption))
    modelValue = optionData.modelValue;
    resultData = resData.rawData !== undefined ? JSON.parse(resData.rawData) : [];
    let showData =  await filterData(optionData.globalProcessor);
    // new Vue().$message({
    //     message: "这是图表数据加载开始111"+JSON.stringify(showData),
    //     type: 'waring',
    //     duration: 3 * 1000
    // })
   return showData
}

export async function filterData(event) {
   
    for (let i = 0; i < resultData.length; i++) {
        if (resultData[i].title === event) {
            tableData = resultData[i].content
            // new Vue().$message({
            //     message:"这是图表数据加载开始222"+JSON.stringify(await disposeData()),
            //     type: 'waring',
            //     duration: 3 * 1000
            // })
            return await disposeData()
            break;
        }
    }
}

export async function disposeData() {
    columnData = [];
    for (const key in tableData[0]) {
        columnData.push(key) 
    }
   
    if (typeof optionData.staticDataValue === "string") {
        let showData = await dataProcessingString()
        return showData
    } else if (typeof optionData.staticDataValue === "object") {
        if (optionData.staticDataValue.length > 0) {
            let showData = await dataProcessingArray()
            return showData
        } else {
            if (optionData.staticDataValue.nodes === undefined) {
                let showData = await dataProcessingObject()
                return showData
            } else {
                let showData = await dataProcessingKnowledge()
                return showData
            }
        } 
    }
}

function getTableData(data) {
    let clickedColumn = columnData[data[1]-1];
    let showData = tableData[data[0]][clickedColumn];
    return showData
}

function getTableDataMore(data){
    let showData = []
    tableData.forEach(v => {
        let array = {}
        for (const key in data) { 
            array[key] = v[columnData[data[key]-1]]
        }
        showData.push(array)
    })
    return showData
}

function getTableDataOnece(data){
    let showData = {}
    for (const key in data) { 
        if(typeof data[key] === 'array') {
            showData[key] = getTableData(data[key])
        } else {
            showData[key] = data[key]
        }
    }
    return showData
}

export async function dataProcessingString() {
    if (modelValue === '') return ''
    let showData = getTableData(modelValue)
   return showData
}

export async function dataProcessingObject() {
    if (modelValue) return ''
    let showData = getTableDataOnece(modelValue)
   return showData
}

export async function dataProcessingArray() {
    if (Object.keys(modelValue).length === 0) return ''
    let showData = getTableDataMore(modelValue)
    return showData
}

export async function dataProcessingKnowledge () {
    if (Object.keys(modelValue.nodes).length === 0) return ''
    let showData = getTableDataMore(modelValue.nodes)
    let array = [];
    array.nodes = [...showData]
    array.links = modelValue.links
    return array
}