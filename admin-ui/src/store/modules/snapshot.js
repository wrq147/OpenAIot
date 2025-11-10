function deepCopy(obj) {
    return JSON.parse(JSON.stringify(obj))
}
const snapshot =  {
    state: {
        snapshotData: [], // 编辑器快照数据
        snapshotIndex: -1, // 快照索引
        componentData: [], // 画布组件数据
    },
    getters:{
        snapshotLength(state){
            return state.componentData.length
        }
    },
    mutations: {
        getComponentData(state,drawingList){
          state.componentData = drawingList;
        },
        resetSnapshotIndex(state){
            state.snapshotIndex = -1;
        },
        // setComponentData(state, componentData) {
        //  // console.log(componentData)
        //   Vue.set(state, 'componentData', componentData)
        // },
        // deleteComponent(state, index) {
        //   if (index === undefined) {
        //       index = state.curComponentIndex
        //   }
    
        //   state.componentData.splice(index, 1)
        // },
        undo(state) {

            if (state.snapshotIndex >= 0) {
                state.snapshotIndex--
               // store.commit('setComponentData', deepCopy(state.snapshotData[state.snapshotIndex]))

               let copyData = deepCopy(state.snapshotData[state.snapshotIndex]);
               //深copy后所有组件的地址将会变化 
               //所以判断如果是组合的话将组合里的组件重新赋值为对应drawingList的组件
                copyData.forEach((chart,i) => {
                   if(chart.chartType == 'group'){
                       chart.chartOption.forEach(element => {
                            copyData.some((data, j) => {
                                if (data.customId == element.customId ){
                                    copyData[i].chartOption[j]= data;
                                }
                            })
                        })       
                    }
                })
               
               state.componentData = copyData;
            }
        },

        redo(state) {
            if (state.snapshotIndex < state.snapshotData.length - 1) {
                state.snapshotIndex++
                // store.commit('setComponentData', deepCopy(state.snapshotData[state.snapshotIndex]))
                let copyData = deepCopy(state.snapshotData[state.snapshotIndex]);
                //深copy后所有组件的地址将会变化 
                //所以判断如果是组合的话将组合里的组件重新赋值为对应drawingList的组件
                copyData.forEach((chart,i) => {
                   if(chart.chartType == 'group'){
                       chart.chartOption.forEach(element => {
                            copyData.some((data, j) => {
                                if (data.customId == element.customId ){
                                    copyData[i].chartOption[j]= data;
                                }
                            })
                        })       
                    }
                })
               state.componentData = copyData;
            }
        },
        recordSnapshot(state,drawingList) {
            //判断是否新增或者删除组件
            if(state.snapshotIndex == -1 || state.snapshotData[state.snapshotIndex].length != drawingList.length ){
                state.componentData = drawingList;
                // 添加新的快照
                state.snapshotData[++state.snapshotIndex] = deepCopy(state.componentData);
                //仅保留20步操作
                if(state.snapshotIndex >= 20){
                    state.snapshotData.shift();
                    state.snapshotIndex = 19;
                }
                
                // 在 undo 过程中，添加新的快照时，要将它后面的快照清理掉
                if (state.snapshotIndex < state.snapshotData.length - 1) {
                    state.snapshotData = state.snapshotData.slice(0, state.snapshotIndex + 1)
                }

            }else{
                //判断组件是否更改了位置、大小，图层，如果更改则快照，否则不操作
                for(let index in drawingList){
                    if(drawingList[index].x != state.snapshotData[state.snapshotIndex][index].x){
                        state.componentData = drawingList;
                        state.snapshotData[++state.snapshotIndex] = deepCopy(state.componentData)
                        //仅保留20步操作
                        if(state.snapshotIndex >= 20){
                            state.snapshotData.shift();
                            state.snapshotIndex = 19;
                        }
                        // 在 undo 过程中，添加新的快照时，要将它后面的快照清理掉
                        if (state.snapshotIndex < state.snapshotData.length - 1) {
                            state.snapshotData = state.snapshotData.slice(0, state.snapshotIndex + 1)
                        }

                    }else if(drawingList[index].y != state.snapshotData[state.snapshotIndex][index].y){
                        state.componentData = drawingList;
                        state.snapshotData[++state.snapshotIndex] = deepCopy(state.componentData)
                        //仅保留20步操作
                        if(state.snapshotIndex >= 20){
                            state.snapshotData.shift();;
                            state.snapshotIndex = 19;
                        }
                        // 在 undo 过程中，添加新的快照时，要将它后面的快照清理掉
                        if (state.snapshotIndex < state.snapshotData.length - 1) {
                            state.snapshotData = state.snapshotData.slice(0, state.snapshotIndex + 1)
                        }

                    }else if(drawingList[index].width != state.snapshotData[state.snapshotIndex][index].width){
                        state.componentData = drawingList;
                        state.snapshotData[++state.snapshotIndex] = deepCopy(state.componentData)
                        //仅保留20步操作
                        if(state.snapshotIndex >= 20){
                            state.snapshotData.shift();
                            state.snapshotIndex = 19;
                        }
                        // 在 undo 过程中，添加新的快照时，要将它后面的快照清理掉
                        if (state.snapshotIndex < state.snapshotData.length - 1) {
                            state.snapshotData = state.snapshotData.slice(0, state.snapshotIndex + 1)
                        }

                    }else if(drawingList[index].height != state.snapshotData[state.snapshotIndex][index].height){
                        state.componentData = drawingList;
                        state.snapshotData[++state.snapshotIndex] = deepCopy(state.componentData)
                        //仅保留20步操作
                        if(state.snapshotIndex >= 20){
                            state.snapshotData.shift();
                            state.snapshotIndex = 19;
                        }
                        // 在 undo 过程中，添加新的快照时，要将它后面的快照清理掉
                        if (state.snapshotIndex < state.snapshotData.length - 1) {
                            state.snapshotData = state.snapshotData.slice(0, state.snapshotIndex + 1)
                        }

                    }else if(drawingList[index].zindex != state.snapshotData[state.snapshotIndex][index].zindex){
                        state.componentData = drawingList;
                        state.snapshotData[++state.snapshotIndex] = deepCopy(state.componentData)
                        //仅保留20步操作
                        if(state.snapshotIndex >= 20){
                            state.snapshotData.shift();
                            state.snapshotIndex = 19;
                        }
                        // 在 undo 过程中，添加新的快照时，要将它后面的快照清理掉
                        if (state.snapshotIndex < state.snapshotData.length - 1) {
                            state.snapshotData = state.snapshotData.slice(0, state.snapshotIndex + 1)
                        }
                    }
                }
            }
           
            //console.log(state.snapshotData)
        },
    },
}

export default snapshot