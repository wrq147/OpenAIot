// 引入API
import { chartApi } from "@/api/report/chartApi";
import { chartBIanalysis } from "@/api/report/sourse";


export async function getUrlData(rowGlobal, searchTableData) {//运行接口源的数据
    if (rowGlobal.apiInterfaceUrl == "") {
      return null;
    }
    let rawData = rowGlobal.rawData ? JSON.parse(rowGlobal.rawData) : []
    let dataMap = new Map()
    let changeOption = {
      interfaceURL: rowGlobal.interfaceURL,
      requestMethod: rowGlobal.requestMethod,
      requestHeader: rowGlobal.requestHeader,
      requestParamType: rowGlobal.requestParamType,
      timeout: rowGlobal.timeout,
      preprocess: rowGlobal.preprocess,
    };
    if (searchTableData !== '') {
      searchTableData.forEach(element => {
        let key = element.fieldName;
        rowGlobal.requestParameters[key] = element.formatDefault
      });
    }
    changeOption["requestParameters"] = rowGlobal.requestParameters;
    // let newOption = replaceLinkParam(drawingList, changeOption);
    let res = await chartApi(changeOption);
    let resInfo = {};
  if (typeof res !== "string") {
    resInfo = res;
  } else {
    if (res) {
      resInfo = JSON.parse(res);
    } else {
      return []
    }

  }
  let apiTableRes = []
  if (resInfo.data && resInfo.data.List) {
    apiTableRes = JSON.parse(JSON.stringify(resInfo.data.List))
  } else if (resInfo.data) {
    apiTableRes = JSON.parse(JSON.stringify(resInfo.data))
  } else {
    apiTableRes = JSON.parse(JSON.stringify(resInfo))
  }
  if (apiTableRes && apiTableRes.length > 0) {
    dataMap = getTreeParams(apiTableRes, '', dataMap)
  }
  if (rawData && rawData.length) {
    for (let i = 0; i < rawData.length; i++) {
      let rowOption = rawData[i]
      console.log(rowOption)
      if (rowOption.name == '0') {
        
        if(rawData[i].selectKeyFiled){
          // rawData[i].content =this.changeFilterFiled(tab.selectKeyFiled)
          let keyData=JSON.parse(JSON.stringify(dataMap.get(rawData[i].selectKeyFiled)))
          if(keyData){
            if(typeof keyData=='string'){
              rawData[i].content = JSON.parse(dataMap.get(rawData[i].selectKeyFiled))
            }else{
              rawData[i].content = dataMap.get(rawData[i].selectKeyFiled)
            }
            
          }
          
        }else{
          rawData[i].content = apiTableRes
        }
      } else {
        if (rawData[i].resultPreCode) {
          if (rawData[i].requestType == "0") {
            let initResult = null
            if (rawData[i].resultPreCode) {
              try {
                let callFunction = eval(rawData[i].resultPreCode);
                initResult = callFunction(JSON.parse(JSON.stringify(apiTableRes)),[]);
              } catch (error) {
                initResult=[]
              }
            } else {
              initResult = JSON.parse(JSON.stringify(apiTableRes))
            }
            rawData[i].content = initResult.length === undefined ? [initResult] : initResult;
          } else {
            rawData[i].content = dataMap.get(rawData[i].resultPrePath)
          }
        }
      }
    }
  }
  return rawData[rowGlobal.filtrationId].content;

}
// 获取数据库数据
export async function getbaseData(rowGlobal, searchTableData) {
  if (rowGlobal.database.executeSql != undefined && rowGlobal.database.executeSql != '') {
      let rawData = JSON.parse(rowGlobal.rawData)
      let database = rowGlobal.database;
      if (searchTableData !== '') {
        searchTableData.forEach(element => {
          let key = element.fieldName;
          database[key] = element.formatDefault
        });
      }
      let functionData = await chartBIanalysis(database);
      
      rawData.forEach(element => {
        if (element.name === '0') {
          element.content = functionData.data
        } else {
          let initResult = null
          if (element.resultPreCode) {
            try {
              let callFunction = eval(element.resultPreCode);
              initResult = callFunction(JSON.parse(JSON.stringify(functionData.data)));
            } catch (error) {
              initResult=[]
            }
          } else {
            initResult = JSON.parse(JSON.stringify(functionData.data))
          }
          element.content = initResult.length === undefined ? [initResult] : initResult;
        }
      })
      rowGlobal.rawData = [...rawData];
    }
    return rowGlobal.rawData[rowGlobal.filtrationId].content
}


function getTreeParams(val, orgPath, dataMap = new Map()) {//将结果字段按层级路径展示
  for(let i=0;i<val.length;i++){
    let row=val[i]
    for(let key in row){
      let keyPath=orgPath+key
      if(row[key]&&typeof(row[key])=='object'&&row[key]&&row[key].length==undefined){//对象类型数据的处理
        for(let keyName in row[key]){
          let keyPath2=keyPath+'_'+keyName
          let valObj={}
          if(dataMap.get(keyPath2)){
            let hsval=dataMap.get(keyPath2)
            valObj[key+'_'+keyName]=row[key][keyName]
            hsval.push(valObj)
            dataMap.set(keyPath2,hsval)
          }else{
            valObj[key+'_'+keyName]=row[key][keyName]
            dataMap.set(keyPath2,[valObj])
          }
          
        }
      }else{
        if(dataMap.get(keyPath)){//已经有设置过值
          let hsval=dataMap.get(keyPath)
          const value = row[key];
          let valObj={}
          if(Array.isArray(value)){//数组类型数据的处理
            let hsval2=JSON.parse(hsval)
            let afterArr=[...Array.from(hsval2),...value]
            dataMap.set(keyPath,JSON.stringify(afterArr))
            getTreeParams(value,keyPath+'/',dataMap)
          }else{
            if(value!=null&&value!=undefined){
                valObj[key]=value
                hsval.push(valObj)
                dataMap.set(keyPath,hsval)
              }
          }
        }else{//初始设置值
          const value = row[key];
          let valObj={}
          if(value&&Array.isArray(value)){//数组类型数据的处理
            dataMap.set(keyPath,JSON.stringify(value))
            getTreeParams(value,keyPath+'/',dataMap)
          }else{
            if(value!=null&&value!=undefined){
              valObj[key]=value
              dataMap.set(keyPath,[valObj])
              
            }
          }
        }
      }
      
    }
  }
  return dataMap
}

export async function combinationTableColum(tmpoption, globalData1, drawingList) {//获取组合数据集中分别组合的数据集的相关参数
  let globalData=JSON.parse(JSON.stringify(globalData1))
  let tableColum = []
  if (tmpoption.combinationTable && tmpoption.combinationTable.length > 0) {
    for (let i = 0; i < tmpoption.combinationTable.length; i++) {
      let columObj = tmpoption.combinationTable[i]
      if (columObj.globalData) {
        let resArr = globalData.find(row => row.name == columObj.globalData)
        // if (resArr && resArr.rawData) {
        //   let rawData = JSON.parse(resArr.rawData)
        // let content = (rawData.find(row => row.name == columObj.globalProcessor))
        let content = []
        // if (content && content.content && content.content.length > 0) {
        //   columObj.resProcessData = content.content
        //   columObj.resData = content.content
        // } else {
          if(resArr){
            if (resArr.dataSourceType == "url") {
              let resArrrawData = [];
              if(resArr.rawData){
                if(typeof resArr.rawData=='string'){
                  let resArrArray=JSON.parse(resArr.rawData)
                  if(resArrArray[0]&&resArrArray[0].content&&resArrArray[0].content.length>0){
                    resArrrawData=JSON.parse(resArr.rawData)
                  }else{
                    resArrrawData = await confirmValue(resArr, drawingList,globalData);
                  }
                }else{
                  let resArrArray=JSON.parse(JSON.stringify(resArr.rawData))
                  if(resArrArray[0]&&resArrArray[0].content&&resArrArray[0].content.length>0){
                    resArrrawData=JSON.parse(JSON.stringify(resArr.rawData))
                  }else{
                    resArrrawData = await confirmValue(resArr, drawingList,globalData);
                  }
                }
              }else{
                resArrrawData = await confirmValue(resArr, drawingList,globalData);
                
              }
              if(resArrrawData){
                content = (resArrrawData.find(row => row.name == columObj.globalProcessor))
                columObj.resProcessData = content.content
                columObj.resData = content.content
              }
              
            } else if (resArr.dataSourceType === "combination") {
              resArr.combinationTable = await combinationTableColum(resArr, globalData, drawingList);
              let resArrrawData = combinationConfirmValue(resArr,globalData)
              if(resArrrawData){
                content = (resArrrawData.find(row => row.name == columObj.globalProcessor))
                columObj.resProcessData = content.content
                columObj.resData = content.content
              }
              
            } else if (resArr.dataSourceType === "database") {
              let resArrrawData = [];
              if(resArr.rawData){
                if(typeof resArr.rawData=='string'){
                  let resArrArray=JSON.parse(resArr.rawData)
                  if(resArrArray[0]&&resArrArray[0].content&&resArrArray[0].content.length>0){
                    resArrrawData=JSON.parse(resArr.rawData)
                  }else{
                    let rowGlobal = await getbaseData(resArr,globalData);
                    if(typeof rowGlobal.rawData=='string'){
                      resArrrawData=JSON.parse(rowGlobal.rawData)
                    }else{
                      resArrrawData=JSON.parse(JSON.stringify(rowGlobal.rawData))
                    }
                  }
                }else{
                  let resArrArray=JSON.parse(JSON.stringify(resArr.rawData))
                  if(resArrArray[0]&&resArrArray[0].content&&resArrArray[0].content.length>0){
                    resArrrawData=JSON.parse(JSON.stringify(resArr.rawData))
                  }else{
                    let rowGlobal = await getbaseData(resArr,globalData);
                    if(typeof rowGlobal.rawData=='string'){
                      resArrrawData=JSON.parse(rowGlobal.rawData)
                    }else{
                      resArrrawData=JSON.parse(JSON.stringify(rowGlobal.rawData))
                    }
                  }
                }
              }else{
                let rowGlobal = await getbaseData(resArr,globalData);
                if(typeof rowGlobal.rawData=='string'){
                  resArrrawData=JSON.parse(rowGlobal.rawData)
                }else{
                  resArrrawData=JSON.parse(JSON.stringify(rowGlobal.rawData))
                }
              }
              
              if(resArrrawData){
                content = (resArrrawData.find(row => row.name == columObj.globalProcessor))
                columObj.resProcessData = content.content
                columObj.resData = content.content
              }
              
            } else {
              columObj.resProcessData = []
              columObj.resData = []
            }
          } else {
            columObj.resProcessData = []
            columObj.resData = []
          }
        

        // }
        // } else {
        //   columObj.resProcessData = []
        //   columObj.resData = []
        // }
      } else {
        columObj.resProcessData = []
        columObj.resData = []
      }
      tableColum[i] = columObj
    }
  }
  return tableColum
}

export async function everyOngetData(tmpoption, globalData, drawingList) {//获取每一个单独的数据集的方法
  if (Array.isArray(tmpoption)) {
    for (let tm = 0; tm < tmpoption.length; tm++) {
      if(tmpoption[tm]){
        let ddtype = tmpoption[tm].dataSourceType;
        if (ddtype === "url") {
          let rawData = await confirmValue(tmpoption[tm], drawingList,globalData);
          tmpoption[tm].rawData = JSON.stringify(rawData);
        } else if (ddtype === "database") {
          let rowGlobal = await getbaseData(tmpoption[tm],globalData);
          rowGlobal[tm].rawData = JSON.stringify(rowGlobal.rawData);
        } else if (ddtype === "combination") {
          let tableColum = await combinationTableColum(
            tmpoption[tm],
            globalData,
            drawingList
          );
          tmpoption[tm].combinationTable = tableColum;
          let rawData = combinationConfirmValue(tmpoption[tm],globalData);
          tmpoption[tm].rawData = JSON.stringify(rawData);
        }
      }
      
    }
  } else {
    let ddtype = tmpoption.dataSourceType;
    if (ddtype === "url") {
      let rawData = await confirmValue(tmpoption, drawingList,globalData);
      tmpoption.rawData = JSON.stringify(rawData);
    } else if (ddtype === "database") {
      let rowGlobal = await getbaseData(tmpoption,globalData);
      rowGlobal.rawData = JSON.stringify(rowGlobal.rawData);
    } else if (ddtype === "combination") {
      let tableColum = await combinationTableColum(
        tmpoption,
        globalData,
        drawingList
      );
      tmpoption.combinationTable = tableColum;
      let rawData = combinationConfirmValue(tmpoption,globalData);
      tmpoption.rawData = JSON.stringify(rawData);
    }
  }

  return tmpoption
}

export function combinationConfirmValue(tmpoption,GlobalApiData) {//组合数据集运行
  let rawData = tmpoption.rawData ? JSON.parse(tmpoption.rawData) : []
  let arr1 = {}
  let dataMap=new Map()
  if (tmpoption.combType == "1") {
    for (let i = 0; i < tmpoption.combinationTable.length; i++) {
      if (tmpoption.combinationTable[i].globalField && tmpoption.combinationTable[i].resData) {
        if (!arr1.resData && !arr1.globalField) {
          arr1 = JSON.parse(JSON.stringify(tmpoption.combinationTable[i]))
        } else {
          let arr2 = JSON.parse(JSON.stringify(tmpoption.combinationTable[i].resData))
          const combinedArray = arr2.map(item1 => {
            const item2 = arr1.resData.find(item => item[arr1.globalField] === item1[tmpoption.combinationTable[i].globalField]);
            let resObj = Object.assign(item1, item2)
            return resObj;
          });
          arr1.resData = combinedArray
        }
      }

    }
  } else if (tmpoption.combType == "0") {
    for (let i = 0; i < tmpoption.combinationTable.length; i++) {
      if (i == 0) {
        arr1.resData = []
      }
      if (tmpoption.combinationTable[i] && tmpoption.combinationTable[i].resProcessData && tmpoption.combinationTable[i].resProcessData.length > 0) {
        arr1.resData = [...arr1.resData, ...tmpoption.combinationTable[i].resProcessData]
      }

    }
  }
  dataMap = getTreeParams(arr1.resData, '', dataMap)
  if (rawData && rawData.length) {
    for (let i = 0; i < rawData.length; i++) {
      let rowOption = rawData[i]
      if (rowOption.name == '0') {
        // rawData[i].content = arr1.resData
        if(rawData[i].selectKeyFiled){
          // rawData[i].content =this.changeFilterFiled(tab.selectKeyFiled)
          let keyData=dataMap.get(rawData[i].selectKeyFiled)?JSON.parse(JSON.stringify(dataMap.get(rawData[i].selectKeyFiled))):[]
          if(keyData){
            if(typeof keyData=='string'){
              rawData[i].content = JSON.parse(dataMap.get(rawData[i].selectKeyFiled))
            }else{
              rawData[i].content = dataMap.get(rawData[i].selectKeyFiled)
            }
            
          }else{
            rawData[i].content = keyData
          }
          
        }else{
          rawData[i].content = arr1.resData
        }
      } else {
        if (rawData[i].resultPreCode) {
          if (rawData[i].requestType == "0") {
            // let callFunction = eval(rawData[i].resultPreCode);
            // let initResult = callFunction(JSON.parse(JSON.stringify(arr1.resData)));
            let initResult = null
            if (rawData[i].resultPreCode) {
              try {
                let callFunction = eval(rawData[i].resultPreCode);
                initResult = callFunction(JSON.parse(JSON.stringify(arr1.resData)),GlobalApiData);
              } catch (error) {
                initResult=[]
              }
            } else {
              initResult = JSON.parse(JSON.stringify(arr1.resData))
            }
            rawData[i].content = initResult.length === undefined ? [initResult] : initResult;
          } else {
            let keyData=dataMap.get(rawData[i].resultPrePath)?JSON.parse(JSON.stringify(dataMap.get(rawData[i].resultPrePath))):[]
            if(keyData){
              if(typeof keyData=='string'){
                rawData[i].content = JSON.parse(dataMap.get(rawData[i].resultPrePath))
              }else{
                rawData[i].content = dataMap.get(rawData[i].resultPrePath)
              }
              
            }else{
              rawData[i].content = keyData
            }
          }
        }
      }
    }
  }
  return rawData[tmpoption.filtrationId].content

}