<template>
	<view>
		<template v-for="(it,idx) in list">
			<view v-if="it.type=='text'" class="bkwrap"
				:style="{'line-height': '36rpx','color':it.attr?JSON.parse(it.attr).colors:'#000000','font-weight':it.attr?(JSON.parse(it.attr).bolds?'bold':'normal'):'normal','font-size':it.attr?JSON.parse(it.attr).zihao*2+'rpx':'28rpx','line-height':it.attr?JSON.parse(it.attr).hanggao*36+'rpx':'36rpx','text-align':it.attr?(JSON.parse(it.attr).duiqi==1?'left':(JSON.parse(it.attr).duiqi==2?'center':'right')):'left'}">
				<text class="contitem">{{it.data}}</text>
			</view>
			<view v-else-if="it.type=='image'" class="bkwrap">
				<image style="" class="contitem" mode="widthFix" :src="it.data" @click.stop="prevImage(it)"></image>
			</view>
			<view v-else class="bkwrap">
				<video :autoplay="true" :src="it.data" class="contitem"></video>
			</view>
		</template>
	</view>
</template>

<script>
	import request from '@/common/request.js'
	export default {
		name: "mz-editor-parser",
		props: {
			datalist: {
				type: String,
				default: "",
			}
		},
		data() {
			return {
				list: []
			};
		},
		watch: {
			datalist: {
				immediate: true,
				handler: function(newV) {
					if (newV === '') {
						this.list = [];
					} else {
						this.list = JSON.parse(this.datalist);
						this.list.forEach(it => {
							// console.log("初始图片名",it.data);
							if (it.type == "image" && it.data.indexOf("/") == 0) {
								it.data = request.config.baseURL + it.data;
							}
						})
					}
				}
			}
		},
		methods: {
			prevImage(it) {
				uni.previewImage({
					urls: [it.data]
				});
			}
		}
	}
</script>

<style lang="scss">
	.bkwrap {
		width: 100%;
		display: flex;
		flex-direction: column;
		background-color: #ffffff;
		padding-top: 20rpx;
		flex-wrap: wrap;
		align-content: flex-start;

		.contitem {
			display: inline-block;
			width: 100%;

		}

		text.contitem {
			//文本类型的数据要强制换行
			word-break: break-all
		}
	}
</style>
