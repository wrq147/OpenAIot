<template>
	<view>
		<top title="Outbound details" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#161A26" :rightText="returnRightText()" @clickRight="cancelWare"></top>
		<view class="detail_con">
			<view class="detail_status" v-if="form.Status==1">
				<custom-icons iconsName="icon-daishenpi" iconsSize="36rpx"
					iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
					<view class="status_text">
						Pending approval
					</view>
			</view>
			<view class="detail_status" v-if="form.Status==2">
				<custom-icons iconsName="icon-chukuchenggong" iconsSize="36rpx"
					iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
					<view class="status_text">
						Successful
					</view>
			</view>
			<view class="detail_status" v-if="form.Status==0">
				<custom-icons iconsName="icon-daitijiao" iconsSize="36rpx"
					iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
					<view class="status_text">
						Pending submission
					</view>
			</view>
			<view class="detail_status" v-if="form.Status==4">
				<custom-icons iconsName="icon-yiquxiao" iconsSize="36rpx"
					iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
					<view class="status_text">
						Canceled
					</view>
			</view>
			<view class="detail_status" v-if="form.Status==3">
				<custom-icons iconsName="icon-chukushibai" iconsSize="36rpx"
					iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
					<view class="status_text">
						Failed
					</view>
			</view>
			<view class="basic_info_list" v-if="form.List&&form.List.length>0">
				<view class="line_title">
					<view class="left_text">
						<text>Outbound items</text>
					</view>
					<view class="line"></view>
				</view>
				<view class="li_pro_con" v-for="(item,inx) in form.List" :class="{'first_con':inx==0}">
					<view class="li_pro">
						<view class="pro_left">
							<image class="image" :src="item.PhotoUrl+'?wh=500x500'" mode=""></image>
						</view>
						<view class="pro_right">
							<view class="pro_title">{{item.TargetName}}</view>
							<view class="pro_num_price">
								<view class="text1">
									{{item.TargetNumber}}
									<text style="margin-left: 10rpx;" v-if="item.Quantity!=null">×{{item.Quantity}}</text>
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
						<text>Basic information</text>
					</view>
					<view class="line"></view>
				</view>
				<view class="basic_info_li" v-for="(val,inx) in basicInfoObj"
					v-if="val.Value">
					<view class="li_label">{{val.Name}}</view>
					<view class="li_val" v-if="val.Name!='Tabulator'&&val.Name!='Warehousing method'">{{val.Value}}</view>
					<view class="li_val" v-if="val.Name=='Warehousing method'&&val.Name!='Tabulator'">{{EnterMethodName(val.Value)}}</view>
					<view class="li_val" v-if="val.Name=='Tabulator'&&val.Name!='Warehousing method'">
						<view class="icon_avatar">
							<image class="image" :src="val.Value.Avatar" mode=""></image>
						</view>
						<view class="val_text">
							{{val.Value.RealName}}
						</view>
					</view>
					<view class="url_jump" v-if="val.Name=='Logistics tracking number'" @click="jumpToLogistics">
						Click to view logistics information
					</view>
				</view>
			</view>
			<view class="jump_con" v-if="form.LeaveMethod == 1" @click="jumpWarehouse">
				<view class="text">
					View receipt form
				</view>
				<custom-icons iconsName="icon-jinru" iconsSize="16rpx"
					iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
			</view>
			<view class="flow_con" v-if="node_list&&node_list.length>0">
				<view class="title">
					Outbound audit
				</view>
				<node-info-tree :nodeList="node_list"></node-info-tree>
			</view>
			
		</view>
		
		<msg-prompt ref="promptMsg" @confirm="confirmCancel"></msg-prompt>
		<jp-signature-popup ref="signature" popup v-model="signBase" @input="getSignImg($event)"
			@closeSign="closeSign" />
	</view>
</template>

<script>
	import {
		getLeaveInfo,
		cancelLeave
	} from "@/api/stock";
	import {
		getUser
	} from '@/api/user.js'
	import {
		flowRootRecord
	} from '@/api/process.js'
	import NodeInfoTree from "@/components/flow-form/NodeInfoTree.vue";
	import JpSignaturePopup from '@/uni_modules/jp-signature/components/jp-signature-popup/jp-signature-popup.vue'
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
			closeSign(){},//清除签名后执行的函数
			jumpWarehouse(){
				//跳转至入库单
				uni.navigateTo({
					url:'/pages_Inventory/warehouse_records_detail?id='+this.form.SourceEnterId
				})
				
			},
			jumpToLogistics(){
				//跳转外部查快递的链接
				uni.navigateTo({ url: '/pages/webview/webview?url=https://www.kuaidi100.com/?nu='+this.form.ExpressNumber })
			},
			returnRightText() {
				if (this.form.Status == 0 || this.form.Status == 1) {
					return 'revoke'
				}
			},
			async confirmCancel(){
				//确认撤销动作
				await cancelLeave({ id: this.form.Id });
				setTimeout(() => {
					setPagesParam('loadData', 'load', 1)
				}, 1500)
			},
			async cancelWare() {
				try {
					let row=this.form
				  if (row.Status == 0 || row.Status == 1) {
				    // await this.$modal.confirm("确定撤销入库单'" + row.StockNumber + "'？（此操作不可逆）");
					this.$refs.promptMsg.noticeOpen(
						'Are you sure you want to cancel the outbound order '+row.StockNumber+'? (This operation is irreversible)?'
						)
				  }
				}
				catch (e){
					console.log(e);
				}
			},
			EnterMethodName(way) { //入库方式
				switch (way) {
					case 0:
						return "Outbound";
					case 1:
						return "return of goods";
					case 2:
						return "allocate and transfer";
					case 3:
						return "hand movement";
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
			async getDetailInfo() {
				//获取信息
				try {
					uni.showLoading({
						title:'loading'
					})
					let enterInfo = await getLeaveInfo(this.recordsId);
					console.log("enterInfo.data详情信息", enterInfo.data);
					let createrInfo = await getUser(enterInfo.data.createId)
					console.log(createrInfo, '创建者信息');
					this.form = enterInfo.data;
					this.form.ExpressCompanyName = await this.$store.dispatch("data/kuaiName", this.form
						.ExpressCompany);
					this.basicInfoObj = [{
							Value: this.form.StockNumber,
							Name: 'Warehouse entry number'
						},
						{
							Value: {
								'RealName': createrInfo.data.user.RealName,
								'Avatar': createrInfo.data.user.Avatar
							},
							Name: 'Tabulator'
						},
						{
							Value: this.form.CustomerName,
							Name: 'Target customer name'
						},
						{
							Value: this.form.LeaveMethod,
							Name: 'Warehousing method'
						},
						{
							Value: this.form.ToHouseName,
							Name: 'Incoming warehouse'
						},
						{
							Value: this.form.ToName,
							Name: 'Target company name'
						},
						{
							Value: this.form.OutDate,
							Name: 'Warehousing time'
						},
						{
							Value: this.form.ExpressNumber,
							Name: 'Logistics tracking number'
						},
						{
							Value: this.form.ExpressCompanyName,
							Name: 'logistics company'
						},
						{
							Value: this.form.ExpressPhone,
							Name: 'contact phone number'
						},
						{
							Value: this.form.Remark,
							Name: 'Notes'
						}
					]
					
					await this.initNodes(this.form.FlowId);
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