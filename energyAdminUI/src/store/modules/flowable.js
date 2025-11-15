const flowable = {
    state: {
        nodeMap: new Map(),
        isEdit: null,
        selectedNode: {},
        selectFormItem: null,
        design: {},
    },
    mutations: {
        selectedNode(state, val) {
            // console.log("全局变量storeflowable", val);
            state.selectedNode = val
        },
        loadForm(state, val) {
            state.design = val
        },
        setIsEdit(state, val) {
            state.isEdit = val
        }
    },
    getters: {},
    actions: {}
}

export default flowable