const rulesFlowable = {
    state: {
        rulesNodeMap: new Map(),
        rulesIsEdit: null,
        rulesSelectedNode: {},
        rulesSelectedParentNode: {},
        rulesDesign: {},
        rulesProductAttr: [],
        rulesProductFunc: [],
        rulesProductEvent: [],
        rulesProductTags: [],
        isShowNodeList: false,
        selectProductInfo: {}
    },
    mutations: {
        rulesSelectedNode(state, val) {
            state.rulesSelectedNode = val
        },
        rulesSelectedParentNode(state, val) {
            state.rulesSelectedParentNode = val
        },
        rulesloadForm(state, val) {
            state.rulesDesign = val
        },
        setIsEdit(state, val) {
            state.rulesIsEdit = val
        },
        setShowNodeList(state, val) {
            state.isShowNodeList = val
        },
        setProductAttrList(state, val) {
            state.rulesProductAttr = val
        },
        setProductFuncList(state, val) {
            state.rulesProductFunc = val
        },
        setProductEventList(state, val) {
            state.rulesProductEvent = val
        },
        setProductTagList(state, val) {
            state.rulesProductTags = val
        },
        setSelectProductInfo(state, val) {
            state.selectProductInfo = val
        },
    },
    getters: {},
    actions: {}
}

export default rulesFlowable