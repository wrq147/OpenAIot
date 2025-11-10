<template>
	<view v-if="templateId != null" :style="{'margin':isNotPadding?'0 -20rpx':'0'}">
		<view class="flow-title" style="font-size:16px;color:#999;">
			<slot></slot>
		</view>
		<form-render ref="form" :forms="formConf" @update:value="updateval" :value="formValues" @setActiveItem="setActiveItem"
			:disabled="disabled" />
		<view class="flow_con full_con" v-if="nodelist&&nodelist.length>0">
			<view class="title">
				流程记录
			</view>
			<view class="flow_li" v-for="(ite,inx) in nodelist" :class="{'noborder':inx==nodelist.length-1}">
				<view class="name">
					<view class="text">审批人</view>
					<!-- <view class="tips" :class="{'counter_tips':ite.Tip=='会签'}">
						{{ite.Tip}}
					</view> -->
				</view>
				<!-- <view class="user_info" v-for="item in ite.Value">
					<view class="avatar">
						<image class="image" :src="item.Avatar" mode=""></image>
					</view>
					<view class="info_right">
						<view class="user_name">{{item.RealName}}</view>
					</view>
				</view> -->
				<view class="user_sign" v-for="item in ite.Value">
					<view class="user_info_icon">
						<view class="user_info">
							<view class="avatar">
								<image class="image" :src="item.Avatar" mode=""></image>
							</view>
							<view class="info_right">
								<view class="user_name">{{item.RealName}}</view>
							</view>
						</view>
						<view class="type_text" v-if="item.ActionName&&item.ActionName=='同意'">
							同意
						</view>
						<view class="type_text rej" v-if="item.ActionName&&item.ActionName=='驳回'">
							驳回
						</view>
					</view>
					<view class="sign" v-if="item.SignImg">
						<text>签名:</text>
						<image @click="previewImg(item.SignImg)" class="image" :src="item.SignImg" mode=""></image>
					</view>
				</view>
				<view class="dot">
					<custom-icons iconsName="icon-shenpi" iconsSize="20rpx" iconsColor="#C1C1C1"
						v-if="ite.Type=='Approval'"></custom-icons>
					<custom-icons iconsName="icon-chaosong" iconsSize="20rpx" iconsColor="#C1C1C1"
						v-else-if="ite.Type=='CS'"></custom-icons>
					<custom-icons iconsName="icon-faqi" iconsSize="20rpx" iconsColor="#C1C1C1" v-else></custom-icons>
				</view>
				<view class="add_icon" v-if="ite.CanAdd" @click.stop="choiceManage(ite)">
					<custom-icons iconsName="icon-tianjia" iconsSize="24rpx" iconsColor="#C1C1C1"></custom-icons>
				</view>
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import FormRender from "@/pages_flow/flow-form/form-render.vue";
	import {
		getFormDetail,
		flowRootRecord,
		deployStart,
		getQuery
	} from '@/api/process.js'
	import "@/pages_flow/utlity.js";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		name: "processCompot",
		components: {
			FormRender,
		},
		props: {
			disabled: {
				default: false,
				type: Boolean,
			},
			isNotPadding: {
				default: false,
				type: Boolean,
			},
		},
		data() {
			return {
				formName: "",
				formConf: [], // 默认表单数据
				formValues: {},
				templateId: null,
				nodelist: [],
				assign: {},
				flowId: 0,
				refreshNode: true,
				curassignId: null,
				isbeforedestroy: false
			};
		},
		watch: {
			formValues: {
				handler: function() {
					this.refreshNode = true;
				},
				deep: true,
			},
		},

		mounted() {},
		beforeDestroy() {
			this.isbeforedestroy = true
		},
		methods: {
			updateval(val){
				this.formValues=val
			},
			selectDept(val, acitveId) {
				//选择部门
				this.$refs.form.selectDept(val, acitveId)
			},
			selectEmplee(isChoiceFlowUser, val, acitveId) {
				//选择人员
				// this.$refs.form.selectEmplee(val, acitveId)
				if (isChoiceFlowUser) {
					if (val) {
						let curnode = this.nodelist.find((x) => x.Id == this.curassignId);
						let oldarr = curnode.Value.filter((x) => x.IsNew == null);
						let newarr = [];
						val.forEach((x) => {
							if (oldarr.some((i) => i.Id == x.id)) {
								return;
							}
							newarr.push({
								Id: x.id,
								RealName: x.name,
								Avatar: x.avatar,
								IsNew: true,
							});
						});
						curnode.Value = oldarr.concat(newarr);
						this.assign[this.curassignId] = newarr;
					}
					this.$emit('isChoiceFlowUserFun', false)
				} else {
					this.$refs.form.selectEmplee(val, acitveId)
				}
			},
			selectDevice(val, acitveId) {
				//选择设备
				this.$refs.form.selectDept(val, acitveId)
			},
			setActiveItem(val) {
				this.$emit('setActiveItem', val)
			},
			choiceManage(item) {
				//选择员工
				// this.isChoiceFlowUser = true
				this.$emit('isChoiceFlowUserFun', true)
				this.curassignId = item.Id;
				let uitems = item.Value.filter((x) => x.IsNew != null);
				let selected = uitems.map((x) => {
					return {
						avatar: x.Avatar,
						id: x.Id,
						name: x.RealName,
						selected: false,
						type: "user",
					};
				});
				uni.navigateTo({
					url: '/pages_flow/inventory/employee_select?selected=' + JSON.stringify(selected) + '&multiple=true'
				})
			},
			async InitData(procDefId, fromParams, fromVal, defaultFromval) {
				// 初始化表单
				try{
					this.templateId = procDefId;
					if (fromVal != null) {
						let sdddx = await getQuery({
							Name: "@from",
							Value: fromVal
						});
						if (sdddx.data.length > 0) {
							this.flowId = sdddx.data[0].FlowId;
						}
					}
					if (this.templateId != null) {
						let rsp = await getFormDetail(this.templateId);
						this.formName = rsp.data.Form.FormName;
						let rootNode = JSON.parse(rsp.data.FlowJson);
						let commitOperates = rootNode.props.formPerms.toMap("id");
						let jsondata = JSON.parse(rsp.data.Form.FormFields);
					
						let valuesModel = Object.assign({}, fromParams);
						this.formConf = jsondata.filter((it) => {
							let opval = commitOperates.get(it.id);
							if (opval != null) {
								if (opval.perm == "H") {
									return false;
								} else if (opval.perm == "R") {
									it.props.disabled = true;
									return true;
								}
							}
							valuesModel[it.id.toString()] = null;
							return true;
						});
						this.formValues = valuesModel;
						if (defaultFromval) {
							this.formValues = {
								...valuesModel,
								...defaultFromval
							}
						}
						if (this.flowId != null && this.flowId > 0) {
							// 初始化表单
							let xrsp = await flowRootRecord(this.flowId);
							this.formValues = xrsp.data.Model;
							this.assign = xrsp.data.Assign;
						}
					
						this.prebuild();
					}
				}catch(e){
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			prebuild() {
				if (this.refreshNode) {
					this.refreshNode = false;
					deployStart({
						templateId: this.templateId,
						model: this.formValues,
						state: 0,
						assign: this.assign,
						isEmbed: true
					}).then((res) => {
						res.data.forEach((element) => {
							if (this.assign[element.Id] != null) {
								element.Value.concat(this.assign[element.Id]);
							}
						});
						this.nodelist = res.data;
					}).catch(err=>{
						this.setMsgTop(err)
					});
				}
				if (this.isbeforedestroy) {} else {
					setTimeout(this.prebuild, 2000);
				}

			},
			getModel() {
				return {
					"model": this.formValues,
					"assign": this.assign,
					"flowId": this.flowId
				};
			},
			/** 申请流程表单数据提交 */
			async submitForm(st, callback) {
				if (this.templateId == null) return 0;
				if (st == 2) {
					this.$refs.form.validate(async res => {
						if (res) {
							try {
								let rs = await deployStart({
									templateId: this.templateId,
									model: this.formValues,
									assign: this.assign,
									state: st,
									flowId: this.flowId,
									isEmbed: true
								});
								// return rs.data;
								callback(rs.data)
							} catch (e) {
								//TODO handle the exception
								callback(false)
								this.setMsgTop(e)
							}
						} else {
							callback(false)
						}
					})
				} else {
					try {
						let rs = await deployStart({
							templateId: this.templateId,
							model: this.formValues,
							assign: this.assign,
							state: st,
							flowId: this.flowId,
							isEmbed: true
						});
						// return rs.data;
						callback(rs.data)
					} catch (e) {
						//TODO handle the exception
						callback(false)
						this.setMsgTop(e)
					}
				}

			},
			setTaskFlow(callback) {
				this.$refs.form.validate(async res => {
					if (res) {
						try {
							callback(this.formValues)
						} catch (e) {
							//TODO handle the exception
							callback(false)
							this.setMsgTop(e)
						}
					} else {
						callback(false)
					}
				})
			}
		},
	}
</script>

<style lang="less" scoped>
	.flow-title {
		font-size: 36rpx;
		line-height: 48rpx;
		padding-bottom: 20rpx;
		padding-left: 10rpx;
		background-color: #fff;
	}
</style>