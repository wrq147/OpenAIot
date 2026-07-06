<template>
	<view>
		<view class="basic_info_list" v-for="(val,key) in deviceBasicInfoObj" v-if="val.value&&val.value.length>0">
			<view class="line_title">
				<view class="left_text">
					<text>{{val.name}}</text>
				</view>
				<view class="line"></view>
			</view>
			<view class="basic_info_li basic_flex" v-for="(row,ix) in val.value"
				v-if="val.value&&val.value.length>0&&row.DisplayValue!==null&&row.DisplayValue!==''">
				<view class="flex_left">
					<view class="li_label">{{row.Name}}</view>
					<view class="li_val" v-if="val.type!=2||val.type==2&&!row.isEdit||!val.canEdit">{{row.DisplayValue}}</view>
					<view class="value input_val" v-else-if="val.type==2&&val.canEdit&&row.isEdit">

						<uni-number-box :step="0.1" v-model="row.DisplayValue" style="width: 90%;height: 68rpx;"
							v-if="row.Option.type=='int'" :min="row.Option.min" :max="row.Option.max"
							:precision="row.Option.decimals" />

						<uni-number-box v-model="row.DisplayValue" style="width: 90%;height: 68rpx;"
							v-else-if="row.Option.type=='float'" :min="row.Option.min" :max="row.Option.max" />
						<uni-datetime-picker ref="dateChoice2" class="date" type="date" :clear-icon="false"
							v-model="row.DisplayValue" placeholder='请选择' :isCustom="true"
							v-else-if="row.Option.type=='date'">
							<view class="date_slot" :class="{'has_val':row.DisplayValue}">
								<custom-icons iconsName="icon-xuanzeshijian" iconsSize="36rpx"
									iconsColor="#999999"></custom-icons>
								<view class="text">
									{{row.DisplayValue?row.DisplayValue:'请选择'}}
								</view>
							</view>
						</uni-datetime-picker>
						<view class="switch_con" v-else-if="row.Option.type=='boolean'">
							<view class="switch_text" :class="row.DisplayValue===row.Option.falseText?'active_text':''">
								{{row.Option.falseText}}
							</view>
							<switch @change="switch2Change(key, ix,row)" :checked="row.DisplayValue===row.Option.trueText"
								style="transform:scale(0.9);width: 120rpx;" />
							<view class="switch_text" :class="row.DisplayValue===row.Option.trueText?'active_text':''">
								{{row.Option.trueText}}
							</view>
						</view>
						<uni-data-select v-else-if="row.Option.type=='enum'" v-model="row.DisplayValue"
							:localdata="row.Option.elements" style="width: 100%;min-width: 400rpx;" placeholder="请选择"
							borderColor="rgba(255, 255, 255, 0.20)" palColor="rgba(193, 193, 193, 1)" :isCustom="true"
							:isDark="false"></uni-data-select>
						<uni-easyinput type="text" placeholder="请输入" v-model="row.DisplayValue" clearSize="18"
							placeholderStyle="color:#C1C1C1;font-size:32rpx" :styles="customstyles"
							primaryColor="rgba(255, 255, 255, 0.5)" inputHeight="68rpx" :isCustom="true"
							contentFontSize="32rpx" :inputBorder="false" v-else>
						</uni-easyinput>
						<view class="icon_con">
							<view style="margin-left: 40rpx;" class="flex_right" @click.stop="setEditSave(row,key, ix)"
								v-if="row.isEdit">
								<custom-icons iconsName="icon-wancheng" iconsSize="36rpx"
									iconsColor="rgba(0, 127, 255, 1)"></custom-icons>
							</view>
							<view style="margin-left: 40rpx;" class="flex_right" @click.stop="setEdit(key,ix)"
								v-if="row.isEdit">
								<custom-icons iconsName="icon-crmtianjiaguanbi" iconsSize="36rpx"
									iconsColor="rgba(0, 127, 255, 1)"></custom-icons>
							</view>
						</view>
					</view>
				</view>
				<view class="flex_right_con" v-if="val.type==2">

					<view class="flex_right" @click.stop="setEdit(key,ix)" v-if="!row.isEdit&&val.canEdit">
						<custom-icons iconsName="icon-bianji" iconsSize="36rpx" iconsColor="#999999"></custom-icons>
					</view>
					<view style="margin-left: 30rpx;" class="flex_right" @click.stop="toHistoryIot(row)"
						v-if="!row.isEdit&&row.MapCode&&row.Option.type == 'date' ||!row.isEdit&&row.MapCode&&row.Option.type == 'float' ||!row.isEdit&&row.MapCode&&row.Option.type == 'int'">
						<custom-icons iconsName="icon-lishishuju" iconsSize="36rpx" iconsColor="#999999"></custom-icons>
					</view>
				</view>

			</view>
		</view>
		<view class="empty_con" v-if="!deviceBasicInfoObj.basic.name&&!deviceBasicInfoObj.Tag.name">
			<image class="image" :src="getSerVerUrl()+'/appimg/no_data.png'" mode=""></image>
			<view class="text">目前没有可用的数据！</view>
		</view>
	</view>
</template>

<script>
	import {
		saveDeviceTag
	} from '@/api/device.js'
	export default {
		name: "device-basic",
		props: {
			deviceBasicInfoObj: {
				type: Object,
				default: () => {
					return {}
				}
			},
			deviceBasic: {
				type: Object,
				default: ''
			}
		},
		data() {
			return {
				customstyles: {
					color: '#333333',
					backgroundColor: '#F8F8F8',
					disableColor: '#F7F6F6',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
			};
		},
		watch: {},
		mounted() {},
		methods: {
			switch2Change(key, ix,row) {
				if(row.DisplayValue==row.Option.trueText){
					this.deviceBasicInfoObj[key].value[ix].DisplayValue=row.Option.falseText
				}else{
					this.deviceBasicInfoObj[key].value[ix].DisplayValue=row.Option.trueText
				}
				this.$forceUpdate()
				// console.log(this.paramsForm,'uuuuuuu');
			},
			setEdit(key, ix) {
				this.deviceBasicInfoObj[key].value[ix].isEdit = !this.deviceBasicInfoObj[key].value[ix].isEdit
				this.$forceUpdate()
			},
			setEditSave(row,key, ix) {
				let arr = [];
				let obj = {
					Code: row.Code,
					Value: row.DisplayValue,
				};
				arr.push(obj);
				let subQuery = {
					id: this.deviceBasic.Id,
					list: arr
				}
				saveDeviceTag(subQuery).then(res=>{
					uni.showToast({
						title: '保存成功',
						icon: 'none',
						duration: 2000,
					});
					this.setEdit(key, ix)
				})
			},
			toHistoryIot(item) {
				//跳转至历史数据
				let itemObj = {
					Code: item.MapCode,
					Name: item.Name,
					OptionType: item.Option.type,
					Unit: item.Option.unit
				}
				if (itemObj.OptionType == 'date' || itemObj.OptionType == 'float' || itemObj.OptionType == 'int') {
					uni.navigateTo({
						url: '/pages_device/device_info/iot_history?deviceId=' + this.deviceBasic.Id +
							'&deviceNu=' + this.deviceBasic.DeviceId + '&activeAttr=' + JSON.stringify(itemObj)
					})
				} else if (itemObj.OptionType == 'geo') {
					uni.navigateTo({
						url: '/pages_device/map_track?deviceId=' + this.deviceBasic.Id + '&deviceNu=' + this
							.deviceBasic.DeviceId + '&activeAttr=' + JSON.stringify(itemObj),
						fail: (err) => {
							console.log("跳转失败", err);
						}
					})
				}
			},
		}
	}
</script>

<style lang="scss" scoped>
	.basic_info_list {
		.basic_info_li.basic_flex {
			display: flex;
			justify-content: space-between;
			align-items: center;

			.flex_right_con {
				display: flex;
				justify-content: flex-end;
			}

			.flex_left {
				.input_val {
					display: flex;
					align-items: center;
					width: 100%;
					.switch_con {
						display: flex;
						justify-content: flex-start;
						align-items: center;
						width: 90%;
					
						.switch_text {
							font-size: 28rpx;
							margin-left: 10rpx;
							margin-right: 10rpx;
					
							&.active_text {
								color: #2371FF;
							}
						}
					
						&.disable_switch {
							opacity: 0.6;
						}
					
						::v-deep .uni-switch-input {
							width: 120rpx;
					
						}
					
						::v-deep uni-switch .uni-switch-input:before {
							width: 0;
						}
					
						::v-deep uni-switch .uni-switch-input.uni-switch-input-checked:after {
							-webkit-transform: translateX(60rpx);
							transform: translateX(60rpx);
						}
					}

					.icon_con {
						display: flex;
						justify-content: flex-end;
						align-items: center;
					}

					.uni-numbox {
						width: 100%;

						::v-deep .uni-numbox__value {
							width: calc(100% - 128rpx) !important;
							height: 68rpx !important;
							margin: 0 10rpx;
						}

						::v-deep .uni-numbox-btns {
							padding: 0 30rpx;
							font-size: 14rpx;
						}
					}
				}
			}
			.date_slot {
				display: flex;
				justify-content: flex-start;
				align-items: center;
				height: 88rpx;
				width: 100%;
				padding: 0 20rpx;
				box-sizing: border-box;
				color: #C1C1C1;
				background: #f8f8f8;
				border-radius: 5rpx;
			
				&.has_val {
					color: #333333;
				}
			
				.text {
					margin-left: 20rpx;
				}
			
				&.dis_val {
					background-color: #f8f8f8;
					color: #C1C1C1;
					// border: 1rpx solid rgba(193, 193, 193,1);
				}
			}
		}
	}
</style>