<template>
	<view style="min-height: 100vh;width: 100%;background-color: #F5F8F9;">
		<top title="详情" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx" :isleftBack="true"
			backgroundColor="#ffffff" :rightText="returnRightText()" @clickRight="cancelWare"></top>
		<view class="detail_con" style="padding-bottom: 0;">
			<view class="status_jumpcon">
				<view class="detail_status" v-if="form.Status==1">
					<custom-icons iconsName="icon-daishenpi" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						待审批
					</view>
				</view>
				<view class="detail_status" v-if="form.Status==2">
					<custom-icons iconsName="icon-rukuchenggong" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						入库成功
					</view>
				</view>
				<view class="detail_status" v-if="form.Status==0">
					<custom-icons iconsName="icon-daitijiao" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						待提交
					</view>
				</view>
				<view class="detail_status" v-if="form.Status==4">
					<custom-icons iconsName="icon-yituihuo" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						已退货
					</view>
				</view>
				<view class="detail_status" v-if="form.Status==3">
					<custom-icons iconsName="icon-daituihuo" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
					<view class="status_text">
						待退货
					</view>
				</view>
				<view class="jump_other" v-if="form.Status == 4 && aboutid != null" @click="jumpReturn">
					<view class="text">
						查看退货单
					</view>
					<custom-icons iconsName="icon-jinru" iconsSize="16rpx"
						iconsColor="rgba(35, 113, 255, 1)"></custom-icons>
				</view>
			</view>
			<view class="basic_info_list" v-if="form.List&&form.List.length>0" style="margin-top: 0;">
				<view class="line_title">
					<view class="left_text">
						<text>入库物品</text>
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
									{{item.TargetNumber}}
									<text style="margin-left: 10rpx;"
										v-if="item.Quantity!=null">×{{item.Quantity}}</text>
								</view>
								<view class="text2" v-if="item.Price!=null">
									Price: ￥{{item.Price}}
								</view>
							</view>
						</view>
					</view>
				</view>
			</view>
			<view class="basic_info_list">
				<view class="line_title">
					<view class="left_text">
						<text>基本信息</text>
					</view>
					<view class="line"></view>
				</view>
				<view class="basic_info_li" v-for="(val,inx) in basicInfoObj" v-if="val.Value">
					<view class="li_label">{{val.Name}}</view>
					<view class="li_val" v-if="val.Key!='Tabulator'&&val.Key!='Warehousing method'">{{val.Value}}
					</view>
					<view class="li_val" v-if="val.Key=='Warehousing method'&&val.Key!='Tabulator'">
						{{EnterMethodName(val.Value)}}
					</view>
					<view class="li_val" v-if="val.Key=='Tabulator'&&val.Key!='Warehousing method'">
						<view class="icon_avatar">
							<image class="image" :src="val.Value.Avatar" mode=""></image>
						</view>
						<view class="val_text">
							{{val.Value.RealName}}
						</view>
					</view>
					<view class="url_jump" v-if="val.Name=='Logistics tracking number'" @click="jumpToLogistics">
						点击查看物流信息
					</view>
				</view>
			</view>
			<!-- <view class="jump_con" v-if="form.Status == 4 && aboutid != null" @click="jumpReturn">
				<view class="text">
					查看退货单
				</view>
				<custom-icons iconsName="icon-jinru" iconsSize="16rpx"
					iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
			</view> -->
		</view>
		<view class="flow_con" v-if="node_list&&node_list.length>0" style="margin-top: 20rpx;border-radius: 10rpx;">
			<view class="title">
				入库审核
			</view>
			<node-info-tree :nodeList="node_list" :key="form.Id"></node-info-tree>
		</view>
		<view style="width: 100%;height: 20rpx;"></view>

		<msg-prompt ref="promptMsg" @confirm="confirmCancel"></msg-prompt>
		<JpSignaturePopup ref="signature" popup v-model="signBase" @input="getSignImg($event)"
			@closeSign="closeSign" />
	</view>
</template>

<script>
	import {
		getEnterInfo,
		getLeaveBySource,
		cancelEnter
	} from "@/api/stock";
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
			jumpReturn() {
				//跳转至退货单
				uni.navigateTo({
					url: '/pages_flow/inventory/outbound_records_detail?id=' + this.aboutid
				})
			},
			jumpToLogistics() {
				//跳转外部查快递的链接
				uni.navigateTo({
					url: '/pages/webview/webview?url=https://www.kuaidi100.com/?nu=' + this.form.ExpressNumber
				})
			},
			returnRightText() {
				if (this.form.Status == 0 || this.form.Status == 1) {
					return '撤销'
				} else if (this.form.Status == 2 || this.form.Status == 3) {
					return '退货'
				}
			},
			async confirmCancel() {
				//确认撤销动作
				await cancelEnter({
					Id: this.form.Id
				});
				await this.getDetailInfo();
				setPagesParam('loadData', 'load', 1, false)
				if (this.form.Status == 1 || this.form.EnterMethod != 3) {
					uni.redirectTo({
						url: '/pages_flow/inventory/warehouse_records_cancel?id=' + this.form.Id
					})
				}
			},
			async cancelWare() {
				try {
					let row = this.form
					if (row.Status == 0 || row.Status == 1) {
						// await this.$modal.confirm("确定撤销入库单'" + row.StockNumber + "'？（此操作不可逆）");
						this.$refs.promptMsg.noticeOpen(
							'是否确定撤销入库单 ' + row.StockNumber +
							'? (确定撤销入库单)?'
						)
					} else {
						uni.redirectTo({
							url: '/pages_flow/inventory/warehouse_records_cancel?id=' + row.Id
						})
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
					case 3:
						return "手动";
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

			},
			async getDetailInfo() {
				//获取信息
				uni.showLoading({
					title:'加载中'
				})
				try {
					let enterInfo = await getEnterInfo(this.recordsId);
					let createrInfo = await getUser(enterInfo.data.createId)
					this.form = enterInfo.data;
					this.form.ExpressCompanyName = await this.$store.dispatch("data/kuaiName", this.form
						.ExpressCompany);
					this.basicInfoObj = [{
							Value: this.form.StockNumber,
							Name: '入库编号',
							Key: 'Warehouse entry number'
						},
						{
							Value: {
								'RealName': createrInfo.data.user.RealName,
								'Avatar': createrInfo.data.user.Avatar
							},
							Name: '操作人',
							Key: 'Tabulator',
						},
						{
							Value: this.form.FromName,
							Name: '来源企业名称',
							Key: 'Source company name',
						},
						{
							Value: this.form.EnterMethod,
							Name: '入库方式',
							Key: 'Warehousing method',
						},
						{
							Value: this.form.ToHouseName,
							Name: '入库仓库',
							Key: 'Incoming warehouse',
						},
						{
							Value: this.form.InDate,
							Name: '入库时间',
							Key: 'Warehousing time',
						},
						{
							Value: this.form.ExpressNumber,
							Name: '物流编号',
							Key: 'Logistics tracking number',
						},
						{
							Value: this.form.ExpressCompanyName,
							Name: '物流公司',
							Key: 'logistics company',
						},
						{
							Value: this.form.ExpressPhone,
							Name: '物流联系手机',
							Key: 'contact phone number',
						},
						{
							Value: this.form.Remark,
							Name: '备注说明',
							Key: 'Notes',
						}
					]
					if (this.isCheckPermi(['/FlowService/Flow'])) {
						await this.initNodes(this.form.FlowId);
					}
					if (this.form.Status == 4) {

						let rkdd = (await getLeaveBySource(this.form.Id)).data;
						if (rkdd != null) {
							this.aboutid = rkdd.Id;
						}
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