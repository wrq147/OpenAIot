<template>
	<view>
		<view class="flow_name">
			<view class="icon_con" :style="{'background':flowInfo.Background}">
				<custom-icons :iconsName="flowInfo.Icon?iconSubStr(flowInfo.Icon):''"
					iconsSize="40rpx"></custom-icons>
			</view>
			<view class="name">
				{{flowInfo.Name}}
			</view>
		</view>
		<form-render ref="form" :forms="formConf" v-model="formValues" />
		<view class="flow_con">
			<view class="title" v-if="node_list&&node_list.length>0">
				Process records
			</view>
			<node-info-tree :nodeList="node_list"></node-info-tree>
		</view>
		<view class="padding_con" v-if="istoDoProcess&&!loading">
			<view class="btn_con">
				<button :disabled="formloading" :loading="formloading" @click="onHandle(op.action)" :class="[op.type=='primary'?'submit_button':'jump_button']" v-for="op in handleOptions">
					{{op.action=='同意'?'Agree':(op.action=='驳回'?'Reject':op.action)}}
				</button>
				<!-- <button class="submit_button" @click="successAddDevice">
					REQUIRE
				</button> -->
			</view>
		</view>
		<view class="zhanwei"></view>
		<msg-prompt ref="promptMsg"></msg-prompt>
		<jp-signature-popup ref="signature" popup v-model="signBase" @input="getSignImg($event)" @closeSign="closeSign"/>
	</view>
</template>

<script>
	import FormRender from "@/components/flow-form/form-render.vue";
	import {
		getFormDetail,
		flowRootRecord,
		flowRecord,
		deployStart,
		getQuery,
		excuteTask
	} from '@/api/process.js'
	// import "./utlity.js";
	require('./utlity.js')
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import NodeInfoTree from "@/components/flow-form/NodeInfoTree.vue";
	import JpSignaturePopup from '@/uni_modules/jp-signature/components/jp-signature-popup/jp-signature-popup.vue'
	export default {
		components: {
			FormRender,
			NodeInfoTree,
			JpSignaturePopup
		},
		data() {
			return {
				loading:true,
				formloading:false,//按钮状态
				formConf: [], // 默认表单数据
				formValues: {},
				node_list: [],
				assign: {},
				flowId: 0,
				nodeId:'',//节点
				flowInfo: '', //流程信息
				isRootProcess: false,
				istoDoProcess: false,
				handleOptions: {},
				needSign: false,
				signBase:'',//签名图片
				ac:'',//现在执行的操作
			};
		},
		onLoad(options) {
			if (options.id) {
				this.$nextTick(async () => {
					this.$refs.promptMsg.loadingOpen('Loading...')
					this.loading=true
					try {
						// this.flowId = options.id;
						await this.handleFlowRecord(options.id)
						this.nodeId=options.id
						this.$refs.promptMsg.loadingColse()
						this.loading=false
					} catch (e) {
						//TODO handle the exception
						this.setMsgTop(e)
						this.$refs.promptMsg.loadingColse()
						this.loading=false
					}
				})
			}
			if (options.isRoot) {
				this.isRootProcess = !!options.isRoot
			}
			if (options.toDo) {
				this.istoDoProcess = !!options.toDo
			}

		},
		methods: {
			filterPerm(formItems, commitOperates) {
				return formItems.filter((it) => {
					if (it.name === "SpanLayout") {
						it.items = this.filterPerm(it.props.items, commitOperates);
						it.props.disabled = false;
						return it.items.length > 0;
					} else {
						let opval = commitOperates.get(it.id);
						if (opval != null) {
							if (opval.perm == "H") {
								return false;
							}
						}
						it.props.disabled = true;
						return true;
					}
				});
			},
			/** 流程流转记录 */
			async handleFlowRecord(id) {

				try {
					// 初始化表单
					let rsp = {}
					if (this.isRootProcess) {
						rsp = await flowRootRecord(id);
					} else {
						rsp = await flowRecord(id);
					}
					if (this.istoDoProcess) {
						this.signBase = "";
						this.needSign = false;
						this.handleOptions = rsp.data.Step.Options
						console.log("this.handleOptions",this.handleOptions);
					}
					console.log("rsp流程", rsp,rsp.data.Step.FormPerms);
					await this.loadFormDetail(rsp.data.TemplateId)
					// let commitOperates = rsp.data.Step.FormPerms.toMap("id");
					let commitOperates = new Map()
					rsp.data.Step.FormPerms.map(v => commitOperates.set(v["id"], v))
					let jsondata = rsp.data.NodeField;
					if (this.istoDoProcess) {
						//初始化参数
						let queryrsp = await getQuery({
							FlowId: rsp.data.FlowId
						});
						let tmpqueryObj = {};
						queryrsp.data.forEach(x => {
							tmpqueryObj[x.Name] = x.Value;
						})
						this.formValues = Object.assign(rsp.data.Model, tmpqueryObj);
						this.flowId = rsp.data.FlowId
						//判断是否需要签名
						if (rsp.data.Step.Type == "FlowService.FlowNode.Builder.ApprovalTask") {
							this.needSign = rsp.data.Step.Sign;
						}
					} else {
						this.formValues = rsp.data.Model;
					}
					this.detailtitle = rsp.data.FormName;
					this.node_list = rsp.data.NodeList;
					console.log(this.node_list, 'this.node_list');
					this.formConf = this.filterPerm(jsondata, commitOperates);
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}

			},
			async loadFormDetail(templateId) {
				if (templateId != null) {
					try {
						let rsp = await getFormDetail(templateId);
						this.flowInfo = rsp.data
					} catch (e) {
						//TODO handle the exception
						this.setMsgTop(e)
					}
				}
			},
			getSignImg(val){
				//签名确定
				// console.log("签名",val);
				// this.formloading =false
				// console.log("签名1111",this.signBase);
				this.onHandle(this.ac)
			},
			closeSign(){
				//取消签名
				this.formloading=false
			},
			/** 执行动作 */
			onHandle(ac) {
				this.formloading = true;
				let subdatas = {};
				this.formConf.forEach((it) => {
					subdatas[it.id] = it.value;
				});
				if (this.needSign) {
					if(!this.signBase){
						this.ac=ac//操作名称
						this.$refs.signature.toPop()
						return
					}
					this.formValues["$ApprovalSign"] = this.signBase;
				}
				excuteTask({
						flowId: this.flowId,
						model: this.formValues,
						key: this.nodeId,
						action: ac,
					})
					.then((res) => {
						this.$store.commit('SET_UPDATE_REPORT', true)
						this.$refs.promptMsg.open('Operation successful', 1500)
						setTimeout(() => {
							setPagesParam('loadData', 'load', 1)
						}, 1500);
					}).catch((err)=>{
						console.log("excuteTask",err);
						this.signBase=''
						this.setMsgTop(err)
						this.formloading = false;
					})
					.finally(() => {
					});
			},
		}
	}
</script>

<style lang="less" scoped>
	.padding_con{
		width: 100%;
		padding:  0 20rpx;
		box-sizing: border-box;
		.btn_con{
			.jump_button{
				margin-right: 0;
			}
		}
	}
	.flow_name {
		width: calc(100% - 40rpx);
		color: #fff;
		display: flex;
		justify-content: flex-start;
		align-items: center;
		padding: 20rpx 30rpx 40rpx;
		box-sizing: border-box;
		background-color: rgba(28, 34, 50, 1);
		margin: 0 20rpx;
		border-radius: 10rpx 10rpx 0 0;
		.icon_con {
			width: 60rpx;
			height: 60rpx;
			display: flex;
			justify-content: center;
			align-items: center;
			border-radius: 10rpx;
			margin-right: 20rpx;
			font-size: 36rpx;
		}

	}

	.sub_save_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		margin-top: 20rpx;
	}

	.zhanwei {
		width: 100%;
		height: 40rpx;
	}
</style>