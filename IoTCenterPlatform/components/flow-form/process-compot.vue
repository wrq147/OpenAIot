<template>
	<view v-if="templateId != null">
		<view class="flow-title" style="font-size:16px;color:#FFF;"><slot></slot></view>
		<form-render ref="form" :forms="formConf" v-model="formValues" @setActiveItem="setActiveItem" />
		<view class="flow_con">
			<view class="title">
				Process records
			</view>
			<view class="flow_li" v-for="(ite,inx) in nodelist" :class="{'noborder':inx==nodelist.length-1}">
				<view class="name">
					<view class="text">Promoter</view>
					<!-- <view class="tips">
						{{ite.Tip=='会签'?'Countersign':'eithersign'}}
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
							Agreed
						</view>
						<view class="type_text rej" v-if="item.ActionName&&item.ActionName=='驳回'">
							Rejected
						</view>
					</view>
					<view class="sign" v-if="item.SignImg">
						<text>Signature:</text>
						<image @click="previewImg(item.SignImg)" class="image" :src="item.SignImg" mode=""></image>
					</view>
				</view>
				<view class="dot">
					<custom-icons iconsName="icon-shenpi" iconsSize="20rpx" iconsColor="rgba(255, 255, 255, 0.5)"
						v-if="ite.Type=='Approval'"></custom-icons>
					<custom-icons iconsName="icon-chaosong" iconsSize="20rpx" iconsColor="rgba(255, 255, 255, 0.5)"
						v-else-if="ite.Type=='CS'"></custom-icons>
					<custom-icons iconsName="icon-faqi" iconsSize="20rpx" iconsColor="rgba(255, 255, 255, 0.5)"
						v-else></custom-icons>
				</view>
				<view class="add_icon" v-if="ite.CanAdd" @click.stop="choiceManage(ite)">
					<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"></custom-icons>
				</view>
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import FormRender from "@/components/flow-form/form-render.vue";
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
		name:"processCompot",
		components: {
			FormRender,
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
		
		methods: {
			selectDept(val, acitveId) {
				//选择部门
				this.$refs.form.selectDept(val, acitveId)
			},
			selectEmplee(isChoiceFlowUser,val, acitveId) {
				//选择人员
				// this.$refs.form.selectEmplee(val, acitveId)
				if (isChoiceFlowUser) {
					console.log("负责人", val);
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
					this.$emit('isChoiceFlowUserFun',false)
				} else {
					this.$refs.form.selectEmplee(val, acitveId)
				}
			},
			selectDevice(val, acitveId) {
				//选择设备
				this.$refs.form.selectDept(val, acitveId)
			},
			setActiveItem(val) {
				console.log("valform-render", val);
				this.$emit('setActiveItem', val)
			},
			choiceManage(item) {
				//选择员工
				// this.isChoiceFlowUser = true
				this.$emit('isChoiceFlowUserFun',true)
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
					url: '/pages_Inventory/employee_select?selected=' + JSON.stringify(selected) + '&multiple=true'
				})
			},
			async InitData(procDefId, fromParams, fromVal) {

				// 初始化表单
				this.templateId = procDefId;
				if (fromVal != null) {
					let sdddx = await getQuery({
						Name: "@from",
						Value: fromVal
					});
					console.log('yyyyyy',sdddx);
					if (sdddx.data.length > 0) {
						this.flowId = sdddx.data[0].FlowId;
					}
				}


				if (this.templateId != null) {
					let rsp = await getFormDetail(this.templateId);
					this.formName = rsp.data.Form.FormName;
					let rootNode = JSON.parse(rsp.data.FlowJson);
					// let commitOperates = rootNode.props.formPerms.toMap("id");
					let commitOperates = new Map()
					rootNode.props.formPerms.map(v => commitOperates.set(v["id"], v))
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

					if (this.flowId != null && this.flowId > 0) {
						// 初始化表单
						let xrsp = await flowRootRecord(this.flowId);
						this.formValues = xrsp.data.Model;
						console.log(this.formValues,'this.formValues');
						this.assign = xrsp.data.Assign;
					}

					this.prebuild();
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
					});
				}
				setTimeout(this.prebuild, 2000);
			},
			/** 申请流程表单数据提交 */
			async submitForm(st,callback) {
				if (this.templateId == null) return 0;
				if(st==2){
					this.$refs.form.validate(async res => {
						if (res) {
							try{
								let rs = await deployStart({
									templateId: this.templateId,
									model: this.formValues,
									assign: this.assign,
									state: st,
									flowId: this.flowId,
									isEmbed: true
								});
								this.$store.commit('SET_UPDATE_REPORT', true)
								// return rs.data;
								callback(rs.data)
							}catch(e){
								//TODO handle the exception
								callback(false)
								this.setMsgTop(e)
							}
						}else{
							callback(false)
						}
					})
				}else{
					try{
						let rs = await deployStart({
							templateId: this.templateId,
							model: this.formValues,
							assign: this.assign,
							state: st,
							flowId: this.flowId,
							isEmbed: true
						});
						this.$store.commit('SET_UPDATE_REPORT', true)
						// return rs.data;
						callback(rs.data)
					}catch(e){
						//TODO handle the exception
						callback(false)
						this.setMsgTop(e)
					}
				}
				
			},
		},
	}
</script>

<style lang="less" scoped>
	.flow-title{
		font-size: 36rpx;
		line-height: 48rpx;
		margin-bottom: 20rpx;
		padding-left: 10rpx;
	}
</style>