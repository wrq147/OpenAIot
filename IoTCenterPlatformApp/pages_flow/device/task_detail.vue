<template>
	<view>
		<top :title="toptitle" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx" :isleftBack="true"
			backgroundColor="#ffffff"></top>
		<view class="detail_con" style="background-color: rgba(245, 248, 249, 1);">
			<view class="basic_info_list" :style="{'margin-top':'20rpx'}">
				<view class="line_title">
					<view class="left_text">
						<text>基本信息</text>
					</view>
					<view class="line"></view>
				</view>
				<view class="basic_info_li" v-for="(val,inx) in basicInfoObj" v-if="val.Value">
					<view class="li_label">{{val.Name}}</view>
					<view class="li_val">{{val.Value}}</view>
				</view>
				
			</view>
			<view style="padding:0 5px;background-color: #fff;">
				<form-render ref="form" :forms="formConf" v-model="formValues" :disabled="true" />
			</view>
			<view class="flow_con full_con" v-if="nodelist&&nodelist.length>0">
				<view class="title">
					流程记录
				</view>
				<node-info-tree :nodeList="nodelist"></node-info-tree>
			</view>
			
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		flowRootRecord
	} from '@/api/process.js'
	import {devPlaneTaskInfo} from '@/api/devplane.js'
	import FormRender from "@/pages_flow/flow-form/form-render.vue";
	import NodeInfoTree from "@/pages_flow/flow-form/NodeInfoTree.vue";
	export default {
		components: {
			FormRender,
			NodeInfoTree
		},
		data() {
			return {
				formConf:[],
				formValues:{},
				nodelist:[],
				isViewInfo:true,
				basicInfoObj:[],
				toptitle:'任务详情'
			}
		},
		async onLoad(options) {
			if(options.id){
				try{
					let res=await devPlaneTaskInfo({id:options.id})
					console.log("任务详情",res);
					await this.loadInfo(res.data)
				}catch(e){
					//TODO handle the exception
					console.log(e);
					this.setMsgTop(e)
				}
				
			}
			
		},
		methods: {
			async loadInfo(taskInfo){
				// this.taskForm=JSON.parse(JSON.stringify(taskInfo))
				this.basicInfoObj=[{
					Name:'计划名称',
					Value:taskInfo.PlanName
				},{
					Name:'任务单号',
					Value:taskInfo.PlaneNumber
				},{
					Name:'目标设备',
					Value:taskInfo.TargetDevice.Name
				},{
					Name:'任务备注',
					Value:taskInfo.Remark
				}]
				this.taskForm={
					planeNumber: taskInfo.PlaneNumber,
					planName: taskInfo.PlanName,
					planTypeId: taskInfo.PlanTypeId,
					targetId: taskInfo.TargetId,
					targetName: taskInfo.TargetDevice.Name,
					flowId:taskInfo.FlowId,
					remark: taskInfo.Remark
				}
				let rsp=await flowRootRecord(taskInfo.FlowId)
				console.log("记录结果",rsp);
				let jsondata = rsp.data.NodeField;
				let commitOperates = rsp.data.Step.FormPerms.toMap("id");
				this.formValues = rsp.data.Model;
				this.nodelist = rsp.data.NodeList;
							
				this.formConf = this.filterPerm(jsondata,commitOperates);
			},
			filterPerm(formItems,commitOperates){
			  return formItems.filter((it) => {
				if (it.name === "SpanLayout") {
				  it.items=this.filterPerm(it.props.items,commitOperates);
				  it.props.disabled = false;
				  return it.items.length>0;
				}
				else{
				  let opval = commitOperates.get(it.id);
				  if (opval != null) {
					if (opval.perm == "H") {
					  return false;
					} else if (opval.perm == "R") {
					  it.props.disabled = true;
					  return true;
					}
				  }
				  it.props.disabled = true;
				  return true;
				}
			  });
			},
		}
	}
</script>

<style>

</style>
