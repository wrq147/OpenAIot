<template>
	<view>
		<view class="basic_info_list" v-for="(val,key) in deviceBasicInfoObj" v-if="val.value&&val.value.length>0">
			<view class="line_title">
				<view class="left_text">
					<text>{{val.name}}</text>
				</view>
				<view class="line"></view>
			</view>
			<view class="basic_info_li basic_flex" v-for="row in val.value"
				v-if="val.value&&val.value.length>0&&row.DisplayValue!==null&&row.DisplayValue!==''">
				<view class="flex_left">
					<view class="li_label">{{row.Name}}</view>
					<view class="li_val">{{row.DisplayValue}}</view>
				</view>
				<view class="flex_right"
					v-if="val.type==2&&row.MapCode&&row.Option.type == 'date' ||val.type==2&&row.MapCode&&row.Option.type == 'float' ||val.type==2&&row.MapCode&&row.Option.type == 'int'" @click.stop="toHistoryIot(row)">
					<custom-icons iconsName="icon-lishishuju" iconsSize="36rpx" iconsColor="#999999"></custom-icons>
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

			};
		},
		watch: {},
		mounted() {},
		methods: {
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
		}
	}
</style>