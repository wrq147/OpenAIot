<template>
	<view style="min-height: 100vh;background-color: #fff;">
		<top :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui"
			rightWidth="157rpx" :isleftBack="true" backgroundColor="#ffffff"></top>
		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="120rpx" backgroundColor="#ffffff">
			<template v-slot:allslot>
				<view class="flow_name">
					<view class="icon_con" :style="{'background':flowInfo.Background}">
						<custom-icons :iconsName="flowInfo.Icon?iconSubStr(flowInfo.Icon):''"
							iconsSize="40rpx" iconsColor="#ffffff"></custom-icons>
					</view>
					<view class="name">
						{{flowInfo.Name}}
					</view>
				</view>
			</template>
		</uni-nav-bar>
		<form-render @update:value="updateval" ref="form" :forms="formConf" :value="formValues" @setActiveItem="setActiveItem" />
		<view class="flow_con full_con">
			<view class="title">
				流程记录
			</view>
			<view class="flow_li" v-for="(ite,inx) in nodelist" :class="{'noborder':inx==nodelist.length-1}">
				<view class="name">
					<view class="text">发起人</view>
					<!-- <view class="tips" :class="{'counter_tips':ite.Tip=='会签'}" v-if="ite.Tip">
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
					<custom-icons iconsName="icon-faqi" iconsSize="20rpx" iconsColor="#C1C1C1"
						v-else></custom-icons>
				</view>
				<view class="add_icon" v-if="ite.CanAdd" @click.stop="choiceManage(ite)">
					<custom-icons iconsName="icon-tianjia" iconsSize="24rpx" iconsColor="#C1C1C1"></custom-icons>
				</view>
			</view>
		</view>
		<view class="sub_save_con" v-if="flowInfo">
			<button class="submit_button" @click="submitForm(1)" :disabled="isLoading"
				:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
				保存
			</button>
			<button class="jump_button" @click="submitForm(2)" :disabled="isLoading"
				:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
				提交
			</button>
		</view>
		<view class="zhanwei"></view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import FormRender from "@/pages_flow/flow-form/form-render.vue";
	import {
		getFormDetail,
		flowRootRecord,
		deployStart
	} from '@/api/process.js'
	import "./utlity.js";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		components: {
			FormRender,
		},
		data() {
			return {
				topTitle:'创建流程',
				isLoading: false,
				ormName: "",
				formConf: [], // 默认表单数据
				formValues: {},
				formloading: false,
				templateId: null,
				nodelist: [],
				assign: {},
				flowId: 0,
				refreshNode: true,
				curassignId: null,
				activeItemId: '', //执行跳转页面的组件id
				isChoiceUser: false,
				isChoiceDept: false,
				isChoiceDevice: false,
				choiceUser: '',
				choiceDept: '',
				choiceDevice: '',
				flowInfo: '', //流程信息
				isChoiceFlowUser: false, //是否是流程中的选择人员
			};
		},
		watch: {
			formValues: {
				handler(to) {
					this.refreshNode = true;
				},
				deep: true,
			},
		},
		onLoad(options) {
			if (options.procDefId) {
				this.$nextTick(async () => {
					this.$refs.promptMsg.loadingOpen('加载中...')
					try {
						this.templateId = options.procDefId
						if (options.id) {
							this.flowId = options.id;
						}
						await this.loadFormDetail(options)
						this.prebuild();
						this.$refs.promptMsg.loadingColse()
					} catch (e) {
						//TODO handle the exception
						this.setMsgTop(e)
						this.$refs.promptMsg.loadingColse()
					}
				})
			}

		},
		onShow() {
			if (this.isChoiceUser) {
				this.$refs.form.selectEmplee(this.choiceUser, this.activeItemId)
				this.isChoiceUser = false
			}
			if (this.isChoiceDept) {
				this.$refs.form.selectDept(this.choiceDept, this.activeItemId)
				this.isChoiceDept = false
			}
			if (this.isChoiceDevice) {
				this.$refs.form.selectDevice(this.choiceDevice, this.activeItemId)
				this.isChoiceDevice = false
			}
		},
		methods: {
			updateval(val){
				this.formValues=val
			},
			setActiveItem(val) {
				this.activeItemId = val
			},
			selectDept(val) {
				//选择部门
				// this.$refs.form.selectDept(val,this.activeItemId)
				this.isChoiceDept = true
				this.choiceDept = val
			},
			selectEmplee(val) {
				//选择人员
				// this.$refs.form.selectEmplee(val,this.activeItemId)
				if (this.isChoiceFlowUser) {
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
					this.isChoiceFlowUser = false
				} else {
					this.isChoiceUser = true
					this.choiceUser = val
				}

			},
			selectDevice(val) {
				//选择设备
				// this.$refs.form.selectDept(val,this.activeItemId)
				this.isChoiceDevice = true
				this.choiceDevice = val
			},
			choiceManage(item) {
				//选择员工
				this.isChoiceFlowUser = true
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
			async loadFormDetail(options) {
				if (this.templateId != null) {
					try {
						let tmpfrom = {};
						for (var key in options) {
							if (key == "procDefId") {} else if (key == "id") {} else {
								tmpfrom["@" + key] = options[key];
							}
						}
						
						let rsp = await getFormDetail(this.templateId);
						this.flowInfo = rsp.data
						this.formName = rsp.data.Form.FormName;
						let rootNode = JSON.parse(rsp.data.FlowJson);
						// let commitOperates = rootNode.props.formPerms.toMap("id");
						let commitOperates = new Map()
						rootNode.props.formPerms.map(v => commitOperates.set(v["id"], v))
						let jsondata = JSON.parse(rsp.data.Form.FormFields);
						let valuesModel = Object.assign({}, tmpfrom);
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

						if (this.flowId != null && this.flowId > 0) {
							// 初始化表单
							let xrsp = await flowRootRecord(this.flowId);
							this.formValues = xrsp.data.Model;
							this.assign = xrsp.data.Assign;
						}
						// console.log(this.formConf, "yyyyyy", this.formValues);
						this.prebuild();
					} catch (e) {
						//TODO handle the exception
						this.setMsgTop(e)
					}
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
					}).then((res) => {
						res.data.forEach((element) => {
							if (this.assign[element.Id] != null) {
								element.Value.concat(this.assign[element.Id]);
							}
						});
						this.nodelist = res.data;
						// console.log("this.nodelist", this.nodelist);
					});
				}
				setTimeout(this.prebuild, 2000);
			},
			/** 申请流程表单数据提交 */
			submitForm(st) {
				if (st == 2) {
					this.$refs.form.validate(res => {
						if (res) {
							console.log("提交保存的数据显示",this.templateId,this.formValues,this.assign,this.flowId);
							this.isLoading = true;
							deployStart({
									templateId: this.templateId,
									model: this.formValues,
									assign: this.assign,
									state: st,
									flowId: this.flowId,
								})
								.then((res) => {
									this.$refs.promptMsg.open('保存成功', 2000) //提示信息组件
									setTimeout(() => {
										setPagesParam('loadData', 'load', 1)
									}, 2000);
								})
								.catch((err) => {
									this.setMsgTop(err)
									this.isLoading = false;
									// setTimeout(() => {
									// 	uni.navigateBack()
									// }, 2000);

								});
						}
					})
				} else {
					// console.log(this.formValues,'最后的数据');
					// return
					this.isLoading = true;
					deployStart({
							templateId: this.templateId,
							model: this.formValues,
							assign: this.assign,
							state: st,
							flowId: this.flowId,
						})
						.then((res) => {
							this.$refs.promptMsg.open('保存成功', 2000) //提示信息组件
							setTimeout(() => {
								setPagesParam('loadData', 'load', 1)
							}, 2000);
						})
						.catch((err) => {
							this.setMsgTop(err)
							this.isLoading = false;
							// setTimeout(() => {
							// 	uni.navigateBack()
							// }, 2000);

						});
				}

			},
		}
	}
</script>

<style lang="less" scoped>
	.flow_name {
		width: 100%;
		color: #333;
		display: flex;
		justify-content: flex-start;
		align-items: center;
		padding: 0 20rpx;
		box-sizing: border-box;
		margin: 20rpx 0 40rpx 0;

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
		padding-top: 20rpx;
	}

	.zhanwei {
		width: 100%;
		height: 40rpx;
		background-color: #fff;
	}
</style>