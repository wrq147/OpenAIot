<template>
	<view style="width: 100%;">
		<view style="width: 100%;" v-if="disabled&&_value&&_value!=null">
			<view class="dis_text">
				<text>{{_value}}</text>
			</view>
		</view>
		<view style="width: 100%;" v-else>
			
			<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx" inputHeight="70rpx"
				:styles="styles" type="textarea" v-model="_value" :placeholder="placeholder?placeholder:'请输入内容'"
				contentFontSize="32rpx" primaryColor="rgba(255, 255, 255, 0.5)" autoHeight :maxlength="255"
				:disabled="disabled" :inputBorder="disabled" :isCustom="true" />
			<!--#ifdef H5 -->
			<view class="record_btn unselectable" @click="openRecord">语音输入</view>
			<!--#endif -->
		</view>
		<uni-popup :ref="'recordPop'+keyId" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="record_con">
				<view class="cancel" @click="closeRecord">取消</view>
				<view class="remaining_time" v-if="60-luyinTime<=10&&60-luyinTime>0"><text class="num">{{60-luyinTime}}s</text> 后结束语音输入</view>
				<view class="center_info">
					<image v-if="!isStart" class="image" :src="getSerVerUrl()+'/appimg/luyin.png'" mode=""></image>
					<view class="seconds" v-if="isStart">
						<view class="num">{{luyinTime}}</view>
						<view class="unit">s</view>
					</view>
				</view>
				<view class="btn unselectable" :class="{'start':isStart}" @longpress="startRecord" @touchstart="touchStart" @touchmove="touchMove" @touchend="endRecord">{{isStart?'松开结束语音输入':'长按开始语音输入'}}</view>
			</view>
		</uni-popup>
	</view>
</template>

<script>
	import componentMinxins from '../ComponentMinxins'
	// #ifdef H5
	import wechatUtils from '@/common/wxCommon.js'
	// #endif
	export default {
		mixins: [componentMinxins],
		name: "TextareaInput",
		components: {},
		props: {
			value: {
				type: String,
				default: null
			},
			placeholder: {
				type: String,
				default: '请输入内容'
			},
			disabled: {
				default: false,
				type: Boolean,
			},
			keyId: {
				type: [String, Number],
				default: ''
			},
		},
		data() {
			return {
				// #ifdef H5
				wechatUtils: new wechatUtils(),
				// #endif
				isStart: false,
				isTranslateErr:false,
				luyinTime:0,
				luyinSetInterval:undefined,
				jsShowLuyin:false,
				recording:false,
				startTouchData:{}
			}
		},
		mounted() {
			
		},
		destroyed() {
			if (this.isStart) {
				this.stopRecord()
			}
			if(this.luyinSetInterval){
				clearInterval(this.luyinSetInterval)
			}

		},
		methods: {
			touchStart(e){
				// 无论你是否绑定longpress，只要手指触摸这个view，就触发start
				// 这里我们需要记录此时的坐标
				this.startTouchData.clientX = e.changedTouches[0].clientX; //手指按下时的X坐标
				this.startTouchData.clientY = e.changedTouches[0].clientY; //手指按下时的Y坐标
			},
			touchMove(e){
				// 触摸后继续移动手指时，将触发这个事件
				// 这里我们根据每次移动的坐标变化判断是向上还是向下滑动
				let touchData = e.touches[0]; //滑动过程中，手指滑动的坐标信息 返回的是Objcet对象
				let moveX = touchData.clientX - this.startTouchData.clientX;
				let moveY = touchData.clientY - this.startTouchData.clientY;
				if (moveY < -50) {
					// 向上滑动
					this.recording = false;
				} else {
					// 向下滑动
					this.recording = true;
				}
			},
			endRecord(){
				// 最后就是松手、结束触摸
				// 这里分为两种情况
				// 也就是当已经监听到你向上滑动时，松手则取消录音
				// 当监听到没有移动、或是向下滑动时，则完成录音
				if (this.recording) {
					// 完成录音
				} else {
					// 此时松手后响应的是取消录音
					this.stopRecord()
				}
			},
			openRecord(){
				this.$refs['recordPop'+this.keyId].open()
			},
			closeRecord(){
				this.$refs['recordPop'+this.keyId].close()
			},
			handleRecord() {
				if (!this.isStart) {
					this.startRecord()
				}else{
					this.stopRecord()
				}
			},
			initRecord(){
				
			},
			//开启录音接口  
			startRecord() {
				if(this.isStart){
					return
				}
				this.wechatUtils.startRecord((res) => {
					this.isTranslateErr = false
					this.isStart = true
					this.recordEnd()
					this.luyinTime=0
					this.luyinSetInterval=setInterval(()=>{
						this.luyinTime=this.luyinTime+1
					},1000)
				}, (err) => {
					this.isTranslateErr = true
					this.isStart = false
					console.log("startRecord--err", err);
					
				})

			},
			//停止录音接口  
			stopRecord() {
				this.wechatUtils.stopRecord((res) => {
					this.isStart = false
					let localId = res.localId;
					this.translateVoice(localId)

				}, (err) => {
					this.isTranslateErr = true
					this.isStart = true
					console.log("stopRecord--err", err);
				})
			},
			//监听 60秒停止录音
			recordEnd() {
				this.wechatUtils.onVoiceRecordEnd((res) => {
					let localId = res.localId;
					this.translateVoice(localId)

				})

			},
			//录音文字识别
			translateVoice(localId) {
				if(this.luyinTime<1){
					uni.showToast({
						title:'录音时间太短',
						icon:'none',
						duration:3000
					})
				}
				this.wechatUtils.translateVoice(localId, (res) => {
					this.luyinTime=0
					this.isStart = false
					clearInterval(this.luyinSetInterval)
					this.closeRecord()
					let result = res.translateResult;
					//去掉最后一个句号
					result = result.substring(0, result.length - 1);
					this._value=this._value?this._value+result:result
				}, (err) => {
					this.isTranslateErr = true
					console.log("translateVoice--err", err);
					uni.showToast({
						title:'没有识别到语音内容',
						icon:'none',
						duration:3000
					})
					this.luyinTime=0
					this.isStart = false
					clearInterval(this.luyinSetInterval)
				})

			},

		}
	}
</script>

<style lang="less" scoped>
	.unselectable {
	  -webkit-user-select: none; /* Safari 3.1+ */
	  -moz-user-select: none;    /* Firefox 2+ */
	  -ms-user-select: none;     /* IE 10+ */
	  user-select: none;         /* 标准语法 */
	}

	.record_btn {
		width: 128rpx;
		height: 54rpx;
		border-radius: 6rpx;
		background-color: rgba(35, 113, 255, 1);
		text-align: center;
		line-height: 54rpx;
		color: #ffffff;
		font-size: 24rpx;
		margin-top: 20rpx;
		display: flex;
		justify-content: center;
		align-items: center;
	}
	.record_con{
		background-color: #ffffff;
		border-radius: 20rpx 20rpx 0 0;
		display: flex;
		// justify-content: center;
		flex-direction: column;
		align-items: center;
		padding-bottom:46rpx ;
		.remaining_time{
			font-size: 28rpx;
			line-height: 28rpx;
			margin-top: -48rpx;
			color: rgba(153, 153, 153, 1);
			.num{
				color: rgba(255, 53, 53, 1);
			}
		}
		.cancel{
			color: rgba(153, 153, 153, 1);
			font-size: 28rpx;
			line-height: 88rpx;
			text-align: right;
			width: 100%;
			padding: 0 30rpx;
			box-sizing: border-box;
		}
		.center_info{
			margin-top: 32rpx;
			width: 300rpx;
			height: 300rpx;
			border: 55rpx solid rgba(236, 244, 255, 1);
			border-radius: 50%;
			display: flex;
			flex-direction: column;
			justify-content: center;
			align-items: center;
			box-sizing: border-box;
			.image{
				width: 64rpx;
				height: 88rpx;
			}
			.seconds{
				.num{
					color: rgba(35, 113, 255, 1);
					font-size: 48rpx;
					line-height: 48rpx;
				}
				.unit{
					color: rgba(153, 153, 153, 1);
					font-size: 24rpx;
					line-height: 24rpx;
					margin-top: 16px;
					text-align: center;
				}
			}
		}
		.btn{
			width: 320rpx;
			height: 80rpx;
			background-color: rgba(35, 113, 255, 1);
			color: #ffffff;
			font-size: 28rpx;
			line-height: 80rpx;
			text-align: center;
			border-radius: 45rpx;
			margin-top: 80rpx;
			display: flex;
			justify-content: center;
			align-items: center;
			&.start{
				background-color: rgba(71, 226, 241, 1);
			}
		}
	}
</style>