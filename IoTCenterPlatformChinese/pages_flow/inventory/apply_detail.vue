<template>
	<view>
		<top title="出库记录详情" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx" :isleftBack="true"
			backgroundColor="#ffffff" :rightText="returnRightText()" @clickRight="cancelWare"></top>
		<view class="detail_con" style="background-color: rgba(245, 248, 249, 1);">
			<view class="status_jumpcon">
				<view class="detail_status" v-if="form.Status==1">
					<custom-icons iconsName="icon-daishenpi" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						待审批
					</view>
				</view>
				<view class="detail_status" v-if="form.Status==2">
					<custom-icons iconsName="icon-chukuchenggong" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						已完成
					</view>
				</view>
				<view class="detail_status" v-if="form.Status==0">
					<custom-icons iconsName="icon-daitijiao" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						待提交
					</view>
				</view>
				<view class="detail_status" v-if="form.Status==4">
					<custom-icons iconsName="icon-yiquxiao" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						已取消
					</view>
				</view>
				<view class="detail_status" v-if="form.Status==3">
					<custom-icons iconsName="icon-chukushibai" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						出库失败
					</view>
				</view>
				<view class="jump_other" v-if="form.LeaveMethod == 1" @click="jumpWarehouse">
					<view class="text">
						查看入库单
					</view>
					<custom-icons iconsName="icon-jinru" iconsSize="16rpx"
						iconsColor="rgba(35, 113, 255, 1)"></custom-icons>
				</view>
			</view>
			<view class="basic_info_list" v-if="form.List&&form.List.length>0" style="margin-top: 0;">
				<view class="line_title">
					<view class="left_text">
						<text>出库物品</text>
					</view>
					<view class="line"></view>
				</view>
				<view class="li_pro_con first_con" v-for="(item,inx) in form.List">
					<view class="li_pro">
						<view class="pro_left">
							<image class="image" :src="item.PhotoUrl+'?wh=500x500'" mode=""></image>
						</view>
						<view class="pro_right">
							<view class="pro_title">{{item.TargetName}}</view>
							<view class="pro_num_price">
								<view class="text1">
									{{item.TargetId}}
									<text style="margin-left: 10rpx;"
										v-if="item.Quantity!=null">×{{item.Quantity}}</text>
								</view>
								<view class="text2" v-if="item.Price!=null">
									价格: ￥{{item.Price}}
								</view>
							</view>
						</view>
					</view>
				</view>
			</view>
			<view class="basic_info_list" :style="{'margin-top':form.List&&form.List.length>0?'20rpx':'0'}">
				<view class="line_title">
					<view class="left_text">
						<text>基本信息</text>
					</view>
					<view class="line"></view>
				</view>
				<view class="basic_info_li" v-for="(val,inx) in basicInfoObj" v-if="val.Value">
					<view class="li_label">{{val.Name}}</view>
					<view class="li_val" v-if="val.Key!='ApplyUser'">{{val.Value}}</view>
					<view class="li_val" v-if="val.Key=='ApplyUser'">
						<view class="icon_avatar">
							<image class="image" :src="val.Value.Avatar" mode=""></image>
						</view>
						<view class="val_text">
							{{val.Value.RealName}}
						</view>
					</view>
				</view>
			</view>
			<!-- <view class="jump_con">
				<view class="text">
					查看入库单
				</view>
				<custom-icons iconsName="icon-jinru" iconsSize="16rpx"
					iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
			</view> -->
			<view class="flow_con" v-if="node_list&&node_list.length>0" style="margin: 0;width: 100%;margin-top: 20rpx;border-radius: 10rpx;">
				<view class="title">
					出库审核
				</view>
				<node-info-tree :nodeList="node_list"></node-info-tree>
			</view>

		</view>

		<msg-prompt ref="promptMsg" @confirm="confirmCancel"></msg-prompt>
		<JpSignaturePopup ref="signature" popup v-model="signBase" @input="getSignImg($event)"
			@closeSign="closeSign" />
	</view>
</template>

<script>
	import {
		ApplyInfo,
		cancelApply,
		deleteApply
	} from "@/api/apply";
	import {
		getUser
	} from '@/api/user.js'
	import {
		flowRootRecord
	} from '@/api/process.js'
	import NodeInfoTree from "@/pages_flow/flow-form/NodeInfoTree.vue";
	import JpSignaturePopup from '@/pages_flow/components/jp-signature/components/jp-signature-popup/jp-signature-popup.vue'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		components: {
			NodeInfoTree,
			JpSignaturePopup
		},
		data() {
			return {
				signBase: '',
				basicInfoObj: {},
				allloading: false,
				open: false,
				form: {},
				aboutid: undefined,
				recordsId: '',
				node_list: []
			};
		},
		async onLoad(options) {
			if (options.id) {
				this.recordsId = options.id
				await this.getDetailInfo()
			}
		},
		methods: {
			closeSign() {}, //清除签名后执行的函数
			jumpWarehouse() {
				//跳转至入库单
				uni.navigateTo({
					url: '/pages_flow/inventory/warehouse_records_detail?id=' + this.form.SourceEnterId
				})

			},
			returnRightText() {
				if (this.form.Status == 1) {
					return '撤销'
				}
				if (this.form.Status == 0 || this.form.Status == 4) {
					return '删除'
				}
			},
			async confirmCancel() {
				//确认撤销动作
				if(this.form.Status == 1){
					await cancelApply({
						id: this.form.Id
					});
				}else if (this.form.Status == 0 || this.form.Status == 4){
					await deleteApply({
						id: this.form.Id
					});
				}
				
				setTimeout(() => {
					setPagesParam('loadData', 'load', 1)
				}, 1500)
			},
			async cancelWare() {
				try {
					let row = this.form
					if (row.Status == 1) {
						// await this.$modal.confirm("确定撤销入库单'" + row.StockNumber + "'？（此操作不可逆）");
						this.$refs.promptMsg.noticeOpen(
							'你确定要撤销编号为 ' + row.ApplyNumber + '的出库申请单吗? (此操作是不可逆的)?'
						)
					}else if (row.Status == 0 || row.Status == 4) {
						this.$refs.promptMsg.noticeOpen('你确定要删除编号为 ' + row.ApplyNumber + '的出库申请单吗? (此操作是不可逆的)?')
					}
				} catch (e) {
					console.log(e);
				}
			},
			EnterMethodName(way) { //入库方式
				switch (way) {
					case 0:
						return "出库";
					case 1:
						return "退货";
					case 2:
						return "调拨";
						// case 3:
						// 	return "hand movement";
				}
				return "";
			},
			async initNodes(flowId) {
				if (flowId != null && flowId > 0) {
					let rsp = await flowRootRecord(flowId);
					this.node_list = rsp.data.NodeList;
				} else {
					this.node_list = [];
				}
				this.$forceUpdate()
			},
			async applyMethodName(way) {
			  // let wayName=this.dict.getName("apply_type", way);
			  let wayName = await this.$store.dispatch("data/dictName", {name:'apply_type',value:way});
			  return wayName;
			},
			async getDetailInfo() {
				//获取信息
				try {
					uni.showLoading({
						title: '加载中'
					})
					let enterInfo = await ApplyInfo(this.recordsId);
					// let createrInfo = await getUser(enterInfo.data.createId)
					this.form = enterInfo.data;
					this.form.ApplyTypeText=await this.applyMethodName(this.form.ApplyType)
					this.basicInfoObj = [{
							Value: this.form.ApplyNumber,
							Name: '申请编号',
							Key: 'ApplyNumber',
						},
						{
							Value: {
								'RealName': this.form.ApplyUserInfo.RealName,
								'Avatar': this.form.ApplyUserInfo.Avatar
							},
							Name: '申请人',
							Key: 'ApplyUser',
						},
						{
							Value: this.form.ApplyTypeText,
							Name: '申请类型',
							Key: 'ApplyType',
						},
						{
							Value: this.form.HouseName,
							Name: '仓库',
							Key: 'Incoming warehouse',
						},
						{
							Value: this.form.ApplyOn,
							Name: '申请时间',
							Key: 'Warehousing time',
						},
						{
							Value: this.form.Reason,
							Name: '备注',
							Key: 'Notes'
						}
					]
					if (this.isCheckPermi(['/FlowService/Flow'])) {
						await this.initNodes(this.form.FlowId);
					}
					uni.hideLoading()
				} catch (e) {
					//TODO handle the exception
					uni.hideLoading()
					this.setMsgTop(e)
				}
			}
		}
	}
</script>

<style lang="less" scoped>
</style>