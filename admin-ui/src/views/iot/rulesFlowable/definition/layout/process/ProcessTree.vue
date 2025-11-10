<script>
//导入所有节点组件
import Pid from '../../../common/process/nodes/PIDNode.vue'
import Timescheduler from '../../../common/process/nodes/TimeSchedulerNode.vue'
import Cleardelta from '../../../common/process/nodes/ClearDeltaNode.vue'
import Concurrent from '../../../common/process/nodes/ConcurrentNode.vue'
import Condition from '../../../common/process/nodes/ConditionNode.vue'
import Trigger from '../../../common/process/nodes/TriggerNode.vue'
import Conversion from '../../../common/process/nodes/ConversionNode.vue'
import Datawrite from '../../../common/process/nodes/DataWriteNode.vue'
import Metronome from '../../../common/process/nodes/MetronomeNode.vue'
import Warn from '../../../common/process/nodes/WarnMessageNode.vue'
import Func from '../../../common/process/nodes/FuncNode.vue'
import Delay from '../../../common/process/nodes/DelayNode.vue'
import Empty from '../../../common/process/nodes/EmptyNode.vue'
import Except from '../../../common/process/nodes/ExceptNode.vue'
import Redirect from '../../../common/process/nodes/RedirectNode.vue'
import Root from '../../../common/process/nodes/RootNode.vue'
import Tag from '../../../common/process/nodes/TagNode.vue'
import Setprop from '../../../common/process/nodes/SetPropNode.vue'
import Node from '../../../common/process/nodes/Node.vue'
import DefaultProps from "./DefaultNodeProps"
import {deepCopy} from '../../../common/utlity.js'

export default {
  name: "ProcessTree",
  components: {Node, Root, Trigger,Conversion,Datawrite,Metronome,Warn,Func, Concurrent,Cleardelta,Timescheduler,Pid, Condition, Delay, Empty,Except,Redirect,Setprop,Tag},
  data() {
    return {
      valid: true
    }
  },
  computed:{
    nodeMap(){
      return this.$store.state.rulesFlowable.rulesNodeMap;
    },
    dom(){
      return this.$store.state.rulesFlowable.rulesDesign.Process;
    }
  },
  render(h, ctx) {
    this.nodeMap.clear()
    let processTrees = this.getDomTree(h, this.dom)
    //插入末端节点
    processTrees.push(h('div', {style:{'text-align': 'center'}}, [
      h('div', {class:{'process-end': true}, domProps: {innerHTML:'结束'}})
    ]))
    return h('div', {class:{'_root': true}, ref:'_root'}, processTrees)
  },
  methods: {
    getDomTree(h, node) {
      this.toMapping(node);
      if (this.isPrimaryNode(node)){
        //普通业务节点
        let childDoms = this.getDomTree(h, node.children)
        this.decodeAppendDom(h, node, childDoms)
        return [h('div', {'class':{'primary-node': true}}, childDoms)];
      }else if (this.isBranchNode(node)){
        let index = 0;
        //遍历分支节点，包含并行及条件节点
        let branchItems = node.branchs.map(branchNode => {
          //处理每个分支内子节点
          this.toMapping(branchNode);
          let childDoms = this.getDomTree(h, branchNode.children)
          this.decodeAppendDom(h, branchNode, childDoms, {level: index + 1, size: node.branchs.length})
          //插入4条横线，遮挡掉条件节点左右半边线条
          this.insertCoverLine(h, index, childDoms, node.branchs)
          //遍历子分支尾部分支
          index++;
          return h('div', {'class':{'branch-node-item': true,'hori':node.branchs.length>1}}, childDoms);
        })

        //插入添加分支/条件的按钮
        branchItems.unshift(h('div', { 'class': { 'add-branch-btn': true } }, [
          h('el-button', {
            'class': { 'add-branch-btn-el': true },
            props: { size: 'small', round: true },
            on: { click: () => this.addBranchNode(node) },
            domProps: { innerHTML: `添加${this.isConditionNode(node) ? '条件' : '分支'}` },
          }, [])
        ]));

        let bchDom = [h('div', {'class':{'branch-node': true}}, branchItems)]
        //继续遍历分支后的节点
        let afterChildDoms = this.getDomTree(h, node.children)
        return [h('div', {}, [bchDom, afterChildDoms])]
      }else if (this.isEmptyNode(node)){
        //空节点，存在于分支尾部
        let childDoms = this.getDomTree(h, node.children)
        this.decodeAppendDom(h, node, childDoms)
        return [h('div', {'class':{'empty-node': true},props:{config:node}}, childDoms)];
      }else {
        //遍历到了末端，无子节点
        return [];
      }
    },
    //解码渲染的时候插入dom到同级
    decodeAppendDom(h, node, dom, props = {}){
      props.config = node
      dom.unshift(h(node.type.toLowerCase(), {
        props: props,
        ref: node.id,
        key: node.id,
        //定义事件，插入节点，删除节点，选中节点，复制/移动
        on:{
          insertNode: type => this.insertNode(type, node),
          delNode: () => this.delNode(node),
          selected: () => this.selectNode(node),
          copy:() => this.copyBranch(node),
          leftMove: () => this.branchMove(node, -1),
          rightMove: () => this.branchMove(node, 1)
        }
      }, []))
    },
    //id映射到map，用来向上遍历
    toMapping(node){
      if (node && node.id){
        //console.log("node=> " + node.id + " name:" + node.name + " type:" + node.type)
        this.nodeMap.set(node.id, node)
      }
    },
    insertCoverLine(h, index, doms, branchs){
      if (index === 0){
        //最左侧分支
        doms.unshift(h('div', {'class':{'line-top-left': true}}, []))
        doms.unshift(h('div', {'class':{'line-bot-left': true}}, []))
      }else if (index === branchs.length - 1){
        //最右侧分支
        doms.unshift(h('div', {'class':{'line-top-right': true}}, []))
        doms.unshift(h('div', {'class':{'line-bot-right': true}}, []))
      }
    },
    copyBranch(node){
      let parentNode = this.nodeMap.get(node.parentId)
      let branchNode = deepCopy(node)
      branchNode.name = branchNode.name + '-copy'
      this.forEachNode(parentNode, branchNode, (parent, node) => {
        let id = this.getRandomId()
        console.log(node, '新id =>'+ id, '老nodeId:' + node.id )
        node.id = id
        node.parentId = parent.id
      })
      parentNode.branchs.splice(parentNode.branchs.indexOf(node), 0, branchNode)
      this.$forceUpdate()
    },
    branchMove(node, offset){
      let parentNode = this.nodeMap.get(node.parentId)
      let index = parentNode.branchs.indexOf(node)
      let branch = parentNode.branchs[index + offset]
      parentNode.branchs[index + offset] = parentNode.branchs[index]
      parentNode.branchs[index] = branch
      this.$forceUpdate()
    },
    //判断是否为主要业务节点
    isPrimaryNode(node){
      return node &&
          (node.type === 'ROOT' || node.type === 'DELAY'
              || node.type === 'TRIGGER' || node.type==='CONVERSION' || node.type==='DATAWRITE' 
              || node.type==='METRONOME'|| node.type==='WARN' ||node.type==='FUNC'||node.type==='EXCEPT'||node.type==='REDIRECT'||node.type==='CLEARDELTA'||node.type==='TIMESCHEDULER'||node.type==='PID'||node.type==='TAG'||node.type==='SETPROP');
    },
    isBranchNode(node){
      return node && (node.type === 'CONDITIONS' || node.type === 'CONCURRENTS');
    },
    isEmptyNode(node){
      return node && (node.type === 'EMPTY')
    },
    //是分支节点
    isConditionNode(node){
      return node.type === 'CONDITIONS';
    },
    //是分支节点
    isBranchSubNode(node){
      return node && (node.type === 'CONDITION' || node.type === 'CONCURRENT');
    },
    isConcurrentNode(node){
      return node.type === 'CONCURRENTS'
    },
    getRandomId(){
      return `node_${new Date().getTime().toString().substring(5)}${Math.round(Math.random()*9000+1000)}`
    },
    //选中一个节点
    selectNode(node){
      // console.log("选中一个节点",node);
      
      this.$store.commit('rulesSelectedNode', node)
      this.$emit('selectedNode', node)
    },
    //处理节点插入逻辑
    insertNode(type, parentNode){
      this.$refs['_root'].click()
      //缓存一下后面的节点
      // console.log("添加节点",type, parentNode);
      
      let afterNode = parentNode.children
      //插入新节点
      parentNode.children = {
        id: this.getRandomId(),
        parentId: parentNode.id,
        props: {},
        type: type,
      }

      switch (type){
        case 'DELAY': this.insertDelayNode(parentNode); break;
        case 'TRIGGER': this.insertTriggerNode(parentNode); break;
        case 'CONVERSION': this.insertConversionNode(parentNode); break;//数据转换
        case 'DATAWRITE': this.insertDataWriteNode(parentNode); break;//设备数据
        case 'METRONOME': this.insertMetronomeNode(parentNode); break;//聚合数据
        case 'FUNC': this.insertFuncNode(parentNode); break;//功能节点
        case 'WARN': this.insertWarnNode(parentNode); break;//消息节点
        case 'CONDITIONS': this.insertConditionsNode(parentNode); break;
        case 'CONCURRENTS': this.insertConcurrentsNode(parentNode); break;
        case 'EXCEPT': this.insertExceptNode(parentNode); break;
        case 'REDIRECT': this.insertRedirectNode(parentNode); break;
        case 'CLEARDELTA':this.insertClearDeltaNode(parentNode); break;
        case 'TIMESCHEDULER': this.insertTimeSchedulerNode(parentNode); break;
        case 'PID': this.insertPidNode(parentNode); break;
        case 'TAG': this.insertTagNode(parentNode); break;
        case 'SETPROP': this.insertSetPropNode(parentNode); break;
        default: break;
      }
      //拼接后续节点
      if (this.isBranchNode({type: type})){
        if (afterNode && afterNode.id){
          afterNode.parentId = parentNode.children.children.id
        }
        this.$set(parentNode.children.children, 'children', afterNode)
      }else {
        if (afterNode && afterNode.id){
          afterNode.parentId = parentNode.children.id
        }
        this.$set(parentNode.children, 'children', afterNode)
      }
      this.$forceUpdate()
      
    },


    insertDelayNode(parentNode){
      this.$set(parentNode.children, "name", "延时处理")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.DELAY_PROPS))
    },
    insertTriggerNode(parentNode){
      this.$set(parentNode.children, "name", "触发器")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.TRIGGER_PROPS))
    },
    insertConversionNode(parentNode){
      this.$set(parentNode.children, "name", "数据转换")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.CONVERSION_PROPS))
    },
    insertDataWriteNode(parentNode){
      this.$set(parentNode.children, "name", "参数赋值")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.DATAWRITE_PROPS))
    },
    insertMetronomeNode(parentNode){
      this.$set(parentNode.children, "name", "聚合数据")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.METRONOME_PROPS))
    },
    insertFuncNode(parentNode){
      this.$set(parentNode.children, "name", "功能节点")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.FUNC_PROPS))
    },
    insertTagNode(parentNode){
      this.$set(parentNode.children, "name", "标签赋值")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.TAG_PROPS))
    },
    insertSetPropNode(parentNode){
      this.$set(parentNode.children, "name", "属性赋值")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.SETPROP_PROPS))
    },
    insertWarnNode(parentNode){
      this.$set(parentNode.children, "name", "触发事件")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.MESSAGE_PROPS))
    },
    insertExceptNode(parentNode){
      this.$set(parentNode.children, "name", "异常检测")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.EXCEPT_PROPS))
    },
    insertRedirectNode(parentNode){
      this.$set(parentNode.children, "name", "转发节点")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.REDIRECT_PROPS))
    },
    insertClearDeltaNode(parentNode){
      this.$set(parentNode.children, "name", "数据清除")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.CLEARDELTA_PROPS))
    },
    insertTimeSchedulerNode(parentNode){
      this.$set(parentNode.children, "name", "设备调度")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.SCHEDULER_PROPS))
    },
    insertPidNode(parentNode){
      this.$set(parentNode.children, "name", "PID控制")
      this.$set(parentNode.children, "props", deepCopy(DefaultProps.PID_PROPS))
    },
    insertConditionsNode(parentNode){
      this.$set(parentNode.children, "name", "条件分支")
      this.$set(parentNode.children, 'children', {
        id: this.getRandomId(),
        parentId: parentNode.children.id,
        type: "EMPTY"
      })
      this.$set(parentNode.children, "branchs", [
        {
          id: this.getRandomId(),
          parentId: parentNode.children.id,
          type: "CONDITION",
          props: deepCopy(DefaultProps.CONDITION_PROPS),
          name: "条件1",
          children:{}
        },{
          id: this.getRandomId(),
          parentId: parentNode.children.id,
          type: "CONDITION",
          props: deepCopy(DefaultProps.CONDITION_PROPS),
          name: "条件2",
          children:{}
        }
      ])
    },
    insertConcurrentsNode(parentNode){
      this.$set(parentNode.children, "name", "并行分支")
      this.$set(parentNode.children, 'children',{
        id: this.getRandomId(),
        parentId: parentNode.children.id,
        type: "EMPTY"
      })
      this.$set(parentNode.children, "branchs", [
        {
          id: this.getRandomId(),
          name: "分支1",
          parentId: parentNode.children.id,
          type: "CONCURRENT",
          props: {},
          children:{}
        },{
          id: this.getRandomId(),
          name: "分支2",
          parentId: parentNode.children.id,
          type: "CONCURRENT",
          props: {},
          children:{}
        }
      ])
    },
    getBranchEndNode(conditionNode){
      if (!conditionNode.children || !conditionNode.children.id){
        return conditionNode;
      }
      return this.getBranchEndNode(conditionNode.children);
    },
    addBranchNode(node){
      if (node.branchs.length < 100){
        node.branchs.push({
          id: this.getRandomId(),
          parentId: node.id,
          name: (this.isConditionNode(node) ? '条件':'分支') + (node.branchs.length + 1),
          props: this.isConditionNode(node) ? deepCopy(DefaultProps.CONDITION_PROPS):{},
          type: this.isConditionNode(node) ? "CONDITION":"CONCURRENT",
          children:{}
        })
      }else {
        this.$message.warning("最多只能添加 100 项😥")
      }
    },
    //删除当前节点
    delNode(node){
      //获取该节点的父节点
      let parentNode = this.nodeMap.get(node.parentId)
      if (parentNode){
        //判断该节点的父节点是不是分支节点
        if (this.isBranchNode(parentNode)){

          let afterdel=false;
          if(parentNode.branchs.length>1){
            //移除该分支
            parentNode.branchs.splice(parentNode.branchs.indexOf(node), 1)
          }
          else{
            afterdel=true;
          }

          //处理只剩1个分支的情况
          if (parentNode.branchs.length < 2){
            let enabledel=parentNode.type === 'CONCURRENTS'||afterdel;
            if(enabledel){
              //获取条件组的父节点
              let ppNode = this.nodeMap.get(parentNode.parentId)
              //判断唯一分支是否存在业务节点
              if (parentNode.branchs[0].children && parentNode.branchs[0].children.id) {
                //将剩下的唯一分支头部合并到主干
                ppNode.children = parentNode.branchs[0].children
                ppNode.children.parentId = ppNode.id
                //搜索唯一分支末端最后一个节点
                let endNode = this.getBranchEndNode(parentNode.branchs[0])
                //后续节点进行拼接, 这里要取EMPTY后的节点
                endNode.children = parentNode.children.children
                if (endNode.children && endNode.children.id) {
                  endNode.children.parentId = endNode.id
                }
              } else {
                //直接合并分支后面的节点，这里要取EMPTY后的节点
                ppNode.children = parentNode.children.children
                if (ppNode.children && ppNode.children.id) {
                  ppNode.children.parentId = ppNode.id
                }
              }
            }
            if(!afterdel){
              parentNode.branchs[0].children={};
            }
          }

        }else {
          //不是的话就直接删除
          if (node.children && node.children.id) {
            node.children.parentId = parentNode.id
          }
          parentNode.children = node.children
        }
        this.$forceUpdate()
      }else {
        this.$message.warning("出现错误，找不到上级节点😥")
      }
    },
    validateProcess(){
      this.valid = true
      let err = []
      this.validate(err, this.dom)
      return err
    },
    validateNode(err, node){
      if (this.$refs[node.id].validate){
        this.valid = this.$refs[node.id].validate(err)
      }
    },
    //更新指定节点的dom
    nodeDomUpdate(node){
      this.$refs[node.id].$forceUpdate()
    },
    //给定一个起始节点，遍历内部所有节点
    forEachNode(parent, node, callback){
      if (this.isBranchNode(node)){
        callback(parent, node)
        this.forEachNode(node, node.children, callback)
        node.branchs.map(branchNode => {
          callback(node, branchNode)
          this.forEachNode(branchNode, branchNode.children, callback)
        })
      }else if (this.isPrimaryNode(node) || this.isEmptyNode(node) || this.isBranchSubNode(node)){
        callback(parent, node)
        this.forEachNode(node, node.children, callback)
      }
    },
    //校验所有节点设置
    validate(err, node){
      if (this.isPrimaryNode(node)){
        this.validateNode(err, node)
        this.validate(err, node.children)
      }else if (this.isBranchNode(node)){
        //校验每个分支
        node.branchs.map(branchNode => {
          //校验条件节点
          this.validateNode(err, branchNode)
          //校验条件节点后面的节点
          this.validate(err, branchNode.children)
        })
        this.validate(err, node.children)
      }else if (this.isEmptyNode(node)){
        this.validate(err, node.children)
      }

    }
  },
  watch:{

  }
}
</script>

<style lang="less" scoped>
._root{
 margin: 0 auto;
 box-sizing:content-box;
}
.process-end{
  width: 60px;
  margin: 0 auto;
  margin-bottom: 20px;
  border-radius: 15px;
  padding: 5px 10px;
  font-size: small;
  color: #747474;
  background-color: #f2f2f2;
  box-shadow: 0 0 10px 0 #bcbcbc;
}
.primary-node{
  display: flex;
  align-items: center;
  flex-direction: column;
}
.branch-node{
  display: flex;
  justify-content: center;
  /*border-top: 2px solid #cccccc;
  border-bottom: 2px solid #cccccc;*/
  position: relative;
}
.hori{
    border-top: 2px solid #cccccc;
    border-bottom: 2px solid #cccccc;
  }
.branch-node-item{
  position: relative;
  display: flex;
  background: #f5f6f6;
  flex-direction: column;
  align-items: center;

  &:before{
    content: "";
    position: absolute;
    top: 0;
    left: calc(50% - 1px);
    margin: auto;
    width: 2px;
    height: 100%;
    background-color: #CACACA;
  }
  .line-top-left, .line-top-right, .line-bot-left, .line-bot-right{
    position: absolute;
    width: 50%;
    height: 4px;
    background-color: #f5f6f6;
  }
  .line-top-left{
    top: -2px;
    left: -1px;
  }
  .line-top-right{
    top: -2px;
    right: -1px;
  }
  .line-bot-left{
    bottom: -2px;
    left: -1px;
  }
  .line-bot-right{
    bottom: -2px;
    right: -1px;
  }
}
.add-branch-btn{
  position: absolute;
  width: 80px;
  .add-branch-btn-el{
    z-index: 999;
    position: absolute;
    top: -15px;
  }
}

.empty-node{
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
}
</style>
