<template>
	<view>
		<view class="device_iot_list">
			<view class="group_con" v-for="(val,key) in liveInfoListObj">
				<view class="line_title">
					<view class="left_text" style="font-weight: normal;">
						<text>{{key}}</text>
					</view>
					<view class="line"></view>
				</view>
				<view class="iot_li no_padding" v-for="it in liveInfoListObj[key]">
					<view class="li_info">
						<view class="state_name">{{it.Name}}</view>
						<view class="state_val">{{it.Value}}
							<view class="unit">{{it.Unit}}</view>
						</view>
						<view class="state_uptime" v-if="it.UpdatedOn">Update time: {{it.UpdatedOn}}</view>
					</view>
					<view class="li_icon" @click.stop="toHistoryIot(it)" v-if="it.OptionType == 'date' ||it.OptionType == 'float' ||it.OptionType == 'int'">
						<custom-icons iconsName="icon-lishishuju" iconsSize="36rpx"></custom-icons>
					</view>
				</view>
			</view>
			<view class="iot_li" v-for="it in liveInfoList">
				<view class="li_info">
					<view class="state_name">{{it.Name}}</view>
					<view class="state_val">{{it.Value}}
						<view class="unit">{{it.Unit}}</view>
					</view>
					<view class="state_uptime" v-if="it.UpdatedOn">Update time: {{it.UpdatedOn}}</view>
				</view>
				<view class="li_icon" @click.stop="toHistoryIot(it)"
					v-if="it.OptionType == 'date' ||it.OptionType == 'float' ||it.OptionType == 'int'">
					<custom-icons iconsName="icon-lishishuju" iconsSize="36rpx"></custom-icons>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		getClientId,
		getClientId2,
		getWebIp,
		asyncClientIdOperation
	} from "@/api/mqtt";
	import {
		DeviceLiveInfo
	} from '@/api/device.js'
	import {
		Time2Local
	} from "@/common/utillib.js";
	// #ifdef H5
	import mqtt from 'mqtt'
	// #endif
	// #ifdef MP-WEIXIN||APP-PLUS
	import mqtt from 'mqtt/dist/mqtt.js'
	// #endif
	export default {
		name: "device-iot",
		props: {
			deviceId: { //传通讯id
				type: String,
				default: ""
			},
			deviceBasic: {
				type: Object,
				default: ''
			}
		},
		data() {
			return {
				status: 'loading',
				liveInfoList: [], //没有分组的iot数据
				isFirst: true,
				liveInfoListObj: {}, //有分组的设备iot数据
				client: null
			};
		},
		computed: {
			userId() {
				return this.$store.state.user.uid; //添加节点时父级节点
			}
		},
		watch: {
			deviceBasic: {
				handler(newName, oldName) {
					this.$nextTick(()=>{
						this.getDeviceLiveInfo();
						if(this.deviceId){
							this.initMQTT();
						}
						
					})
				},
				// 代表在wacth里声明了firstName这个方法之后立即先去执行handler方法
				immediate: false,
				deep: true,
			},
		},
		mounted() {

			// this.getDeviceLiveInfo();
			// this.initMQTT();
			// this.getDeviceLiveInfo(true)
		},
		destroyed() {
			this.$store.dispatch("mqttclient/getClient").then((client) => {
				let tkey = "newprop/" + this.deviceId;
				client.unsubscribe(tkey, (error) => {
					console.info("取消订阅dddd22", error)
					this.$store.commit("mqttclient/Del_Handler", tkey);
				});
			});
			// let tkey = "newprop/" + this.deviceId;
			// this.client.unsubscribe(tkey, (error) => {
			// 	console.info("取消订阅dddd22",error)
			// });
		},
		methods: {
			toHistoryIot(item) {
				//跳转至历史数据
				// console.log("item", item);
				if (item.OptionType == 'date' ||
					item.OptionType == 'float' ||
					item.OptionType == 'int') {
					uni.navigateTo({
						url: '/pages_device/device_info/iot_history?deviceId=' + this.deviceBasic.Id +
							'&activeAttr=' +
							JSON.stringify(item)
					})
				}

			},
			getDeviceLiveInfo(isSubscribe) {
				// console.log("this.deviceBasic.DeviceId", this.deviceId);
				if (this.isFirst) {
					this.status = 'loading'
				}
				// this.liveInfoListObj={}
				this.liveInfoList = []
				let objQuery = {
					needsend: !isSubscribe,
					id: this.deviceId,
					needTag: false
				}
				DeviceLiveInfo(objQuery).then((res) => {
					// console.log("设备实时数据", res);
					if (isSubscribe) {
						//判断是否是订阅后数据变化时执行的
						let online = 1
						if (res.data == "") {
							online = 0;
						} else {
							online = 1;
						}
					}
					// console.log("设备实时数据", res.data);
					if (res.data && res.data.length > 0) {
						res.data.map(row => {
							if (row.Description) {
								if (this.liveInfoListObj[row.Description]) {
									this.liveInfoListObj[row.Description].push(row)
								} else {
									this.liveInfoListObj[row.Description] = []
									this.liveInfoListObj[row.Description].push(row)
								}
							} else {
								this.liveInfoList.push(row)
							}

						})
					}

					this.liveInfoListObj = JSON.parse(JSON.stringify(this.liveInfoListObj))
					// console.log("设备实时数据分组", this.liveInfoListObj);
					// this.$forceUpdate()
					// this.liveInfoList = res.data;
					if (this.isFirst) {
						this.status = 'noMore'
						this.isFirst = false
					}

				}).catch(err => {
					if (this.isFirst) {
						this.status = 'noMore'
					}
					this.setMsgTop(err)
				});
			},
			setDevLiveInfo(itemList) {
				if (this.liveInfoList == null) {
					this.liveInfoList = [];
				}
				if (this.liveInfoListObj == null) {
					this.liveInfoListObj = {}
				}
				itemList.map(async (item) => {
					if (item.Description) {
						if (this.liveInfoListObj[item.Description]) {
							let curitem1 = null
							this.liveInfoListObj[item.Description].find((x, inx) => {
								if(x.Code == item.Code){
									curitem1= inx
								}
							});
							if (curitem1 != null) {
								this.liveInfoListObj[item.Description][curitem1].Name = item.Name;
								this.liveInfoListObj[item.Description][curitem1].Value = item.Value;
								this.liveInfoListObj[item.Description][curitem1].Unit = item.Unit;
								this.liveInfoListObj[item.Description][curitem1].OptionType = item.OptionType;
								this.liveInfoListObj[item.Description][curitem1].UpdatedOn = await Time2Local(item.UpdatedOn);
								this.liveInfoListObj[item.Description][curitem1].Description = item.Description;
							} else {
								this.liveInfoListObj[item.Description].push(item);
							}
						} else {
							this.liveInfoListObj[item.Description] = []
							this.liveInfoListObj[item.Description].push(item)
						}
					} else {
						let curitem = null
						this.liveInfoList.find((x,inx) => {
							if(x.Code == item.Code){
								curitem=inx
							}
						});
						if (curitem != null) {
							this.liveInfoList[curitem].Name = item.Name;
							this.liveInfoList[curitem].Value = item.Value;
							this.liveInfoList[curitem].Unit = item.Unit;
							this.liveInfoList[curitem].OptionType = item.OptionType;
							this.liveInfoList[curitem].UpdatedOn = await Time2Local(item.UpdatedOn);
							this.liveInfoList[curitem].Description = item.Description;
						} else {
							this.liveInfoList.push(item);
						}
					}
			
				});
			},
			initMQTT() {
				let that = this;
				let relUrl = "";
				this.$store.dispatch("mqttclient/getClient").then((client) => {
					let tkey = "newprop/" + this.deviceId;
					if(client){
						client.subscribe(tkey, (error) => {
							if (!error) {
								that.$store.commit("mqttclient/Add_Handler", {
									key: tkey,
									func: function(message) {
										// console.log("监听mqtt信息",message);
										let msgtxt = message.toString();
										if (msgtxt == "online") {
											that.deviceBasic.Online = 1;
										} else if (msgtxt == "Offline") {
											that.deviceBasic.Online = 0;
										} else {
											that.setDevLiveInfo(JSON.parse(msgtxt));
										}
									},
								});
							}
						});
					}else{
						this.initMQTT()
					}
					
				});
			}
		}
	}
</script>

<style lang="scss" scoped>
	.device_iot_list {
		width: 100%;

		.group_con {
			margin-top: 20rpx;
			position: relative;
			background-color: rgba(28, 34, 50, 1);
			border-radius: 10rpx;
			width: 100%;
			padding: 0 30rpx;
			box-sizing: border-box;

			.line_title {
				height: 92rpx;
				padding: 20rpx;
				width: 100%;
				box-sizing: border-box;
			}

			.iot_li {
				margin-top: 0;
				border-top: 1rpx solid rgba(255, 255, 255, 0.2);
				border-radius: 0;
				width: 100%;

				&.no_padding {
					padding-left: 0;
					padding-right: 0;
				}
			}
		}

		.iot_li {
			padding: 24rpx 30rpx 25rpx;
			width: 100%;
			box-sizing: border-box;
			display: flex;
			justify-content: space-between;
			align-items: center;
			background-color: rgba(28, 34, 50, 1);
			border-radius: 10rpx;
			margin-top: 20rpx;

			.li_info {
				.state_name {
					font-size: 32rpx;
					line-height: 44rpx;
					color: rgba(255, 255, 255, 0.5);
				}

				.state_val {
					margin-top: 9rpx; //24-6-9
					font-size: 44rpx;
					color: rgba(255, 255, 255, 1);
					display: flex;
					justify-content: flex-start;
					align-items: flex-end;
					line-height: 62rpx;

					.unit {
						font-size: 36rpx;
						line-height: 62rpx;
						margin-left: 10rpx;
					}
				}

				.state_uptime {
					margin-top: 10rpx; //24-5-9
					font-size: 24rpx;
					line-height: 34rpx;
					color: rgba(255, 255, 255, 0.5);
				}
			}
		}
	}
</style>