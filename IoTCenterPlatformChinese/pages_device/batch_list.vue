<template>
	<view class="pages_con pages_bgcon">
		<top :title="topTitle" leftWidth="157rpx" backgroundColor="#fff" :isleftBack="true" leftIcon="icon-fanhui"
			rightWidth="157rpx"></top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入设备名称或者设备编码"
			inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="device_list_con">
			<view class="device_list_flex">
				<view class="list_li" @click="selectDeviceFun(row, inx)" v-for="(row, inx) in deviceTableData" :style="{
                        opacity:
                            (isDisable && !selectDeviceId) ||
                            (isDisable && !selectDeviceId.includes(row.Id))
                                ? 0.5
                                : 1,
                    }">
					<view class="li_top">
						<image class="image" :src="row.PhotoUrl + '?wh=500x500'" mode="aspectFill" v-if="row.PhotoUrl">
						</image>
						<image v-if="getSerVerUrl()" class="image" :src="getSerVerUrl()+'/appimg/device_default.png'" mode="aspectFill" v-else></image>
						<!-- <view class="device_state">
							<view class="online_status" :class="{ offline: row.Online == 0, unKnow: row.Online == 2 }">
								<view class="dot"></view>
								<view class="text" v-if="row.Online == 0">离线</view>
								<view class="text" v-if="row.Online == 1">在线</view>
								<view class="text" v-if="row.Online == 2">未知</view>
							</view>
							<view class="alarm_status" v-if="row.HavWarn">
								<custom-icons iconsName="icon-baojing" iconsSize="24rpx"
									iconsColor="rgba(255, 53, 53, 1)"></custom-icons>
							</view>
						</view> -->
					</view>
					<view class="li_name_group">
						<view class="dev_name">
							{{ row.BatchName }}
						</view>
						<view class="group" v-if="row.Number">
							<text class="group_name">
								{{ row.ProductLabel === 'U' ? '半成品' : '成品' }}
							</text>
							<view class="line" v-if="row.Number"></view>
							<text class="group_name" v-if="row.Number">
								{{ row.Number }}
							</text>
						</view>
					</view>
					<view class="select_icon t-icon-xuanzhongshebeihaocai1"
						v-if="selectDeviceId && selectDeviceId.includes(row.Id)"></view>
				</view>
			</view>
			<view v-if="isMulSelect">
				<view class="btn_zhanwei" style="width: 100%; height: 98rpx"></view>
				<button class="submit_button" @click="confirmSelectdev">
					确定({{ selectDevice.length }})
				</button>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>

		<!-- 提示信息组件 -->
	</view>
</template>

<script>
	import {
		setPagesParam
	} from '@/common/utillib.js';
	import {
		crmDeviceList,
		addUseDevice
	} from '@/api/device.js';
	import {
		enterDevList
	} from '@/api/stock';
	export default {
		data() {
			return {
				topTitle: '选择产品批次',
				selectListParam: [],
				querydata: {
					pageNum: 1,
					pageSize: 30,
					Name: '',
				}, //过滤参数
				timeQuery: {
					name: '创建时间',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				statusList: [{
						text: '离线',
						value: 0,
					},
					{
						text: '在线',
						value: 1,
					},
				],
				deviceTableData: [],
				key: '', //搜索关键词
				status: 'loading',
				selectDevice: [], //已经选择的
				selectDeviceId: [], //已经选择的
				isMulSelect: false,
				isEnterDev: false,
				isNotReturn: false, //控制选择后是需要返回页面还是关闭弹窗
				isDisable: false, //是否禁用
				isOnlyRead: false, //是否只读
				dataType: '',
				rulesAdForm: {},
				infoIndex: null,
				allInx: 0,
			};
		},
		onLoad(options) {
			if (options.ProductId) {
				this.querydata.ProductId = options.ProductId;
			}
			if (options.ProductList) {
				this.querydata.ProductList = JSON.parse(options.ProductList);
			}
			if (options.isMulSelect) {
				this.isMulSelect = !!options.isMulSelect;
			}
			if (options.selectDevice) {
				this.selectDevice = JSON.parse(options.selectDevice);
				this.selectDeviceId = this.selectDevice.map((item) => {
					if (item.id) {
						return item.id;
					} else if (item.TargetId) {
						return item.TargetId;
					}
				});
			}
			this.getDeviceList();
		},
		onReachBottom() {
			//上拉触底
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = 'loading';
				this.getDeviceList();
			}
		},
		methods: {
			confirmSelectdev() {
				//确定选择设备结果
				setPagesParam('selectDevice', this.selectDevice, 1);
			},
			loadList() {
				//加载列表的方法
				this.querydata.pageNum = 1;
				this.status = 'loading';
				this.getDeviceList();
			},
			searching(val) {
				this.key = val;
				if (this.key) {
					this.querydata.Key = this.key;
					this.querydata.pageNum = 1;
					this.status = 'loading';
					this.deviceTableData = [];
					this.getDeviceList();
				} else {
					delete this.querydata.Key;
					this.querydata.pageNum = 1;
					this.status = 'loading';
					this.deviceTableData = [];
					this.getDeviceList();
				}
			},
			getDeviceList(query) {
				//获取设备列表
				if (query && query == 'unbind') {
					this.querydata.pageNum = 1;
				}

				if (!this.querydata.Key) {
					delete this.querydata.Key;
				}
				enterDevList(this.querydata).then((res) => {
					if (this.querydata.pageNum == 1) {
						this.deviceTableData = [];
					}
					this.deviceTableData = [...this.deviceTableData, ...res.data.List];
					this.topTitle = '选择产品（' + res.data.Total + '台）';
					if (res.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				})
				.catch((err) => {
					this.status = 'noMore';
					this.setMsgTop(err);
				});
			},
			selectDeviceFun(row, inx) {
				//规则中选择设备
				if (this.selectDeviceId && this.selectDeviceId.includes(row.Id)) {
					let inxO = this.selectDeviceId.indexOf(row.Id)
					this.selectDeviceId.splice(inxO, 1)
					this.selectDevice.splice(inxO, 1)
				} else {
					this.selectDevice.push(row)
					this.selectDeviceId.push(row.Id)
				
				}
				if (this.isMulSelect) {
				} else {
					let arr = [];
					arr.push(row);
					setPagesParam('selectDevice', arr, 1);
				}
			},
			openSelect() {
				this.$refs.selectCompt.openSelect();
			},
			selectFinsh(query) {
				this.querydata = JSON.parse(JSON.stringify(query));
				this.querydata.pageNum = 1;
				this.status = 'loading';
				this.deviceTableData = [];
				this.getDeviceList();
			},
		},
	};
</script>

<style lang="less" scoped>
	.pages_con {
		// background-color: rgba(22, 26, 38, 1);
	}
</style>