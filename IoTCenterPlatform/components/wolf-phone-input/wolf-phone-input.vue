<template>
	<view>
		<!-- 输入框带边框 -->
		<view class="wolf-input-main-v" v-if="inputType == 1">
			<view 
				class="item-input-v"  
				:class="[currCursorIndex == index ? 'input-focus-v' : '']"
				v-for="(item, index) in inputList" :key="index" 
				:style="{'margin-right': (index == inputList.length-1 ? 0 : rowSpaceWidth) + 'px', 
				'height': inputHeight + 'px', 'border-width': inputCircleWidth + 'px !important', 
				'border-radius': inputCircleRadius + 'px', 'font-size': numFontSize + 'px',
				'border-color': (currCursorIndex == index ? inputCircleSelectedCol : inputCircleDefaultCol),
				'color': inputContentCol}"
				@click="onClick(index)" 
			>
				{{item}}
			</view>
		</view>
		
		<!-- 输入框只有底部带边框 -->
		<view class="wolf-input-main-v" v-if="inputType == 2">
			<view 
				class="item-input-v-type-2"  
				:class="[currCursorIndex == index ? 'input-focus-v-type-2' : '']"
				v-for="(item, index) in inputList" :key="index" 
				:style="{'margin-right': (index == inputList.length-1 ? 0 : rowSpaceWidth) + 'px', 
				'height': inputHeight + 'px', 'border-width': inputCircleWidth + 'px !important', 
				'font-size': numFontSize + 'px', 'color': inputContentCol,
				'border-bottom-color': (currCursorIndex == index ? inputCircleSelectedCol : inputCircleDefaultCol),}"
				@click="onClick(index)" 
			>
				{{item}}
			</view>
		</view>
		
		<!-- 输入框中间横线 -->
		<view class="wolf-input-main-v" v-if="inputType == 3">
			<view 
				class="item-input-v-type-3"  
				:class="[currCursorIndex == index ? 'input-focus-v-type-3' : '']"
				v-for="(item, index) in inputList" :key="index" 
				:style="{'margin-right': (index == inputList.length-1 ? 0 : rowSpaceWidth) + 'px', 
				'height': inputHeight + 'px', 'border-width': inputCircleWidth + 'px !important', 
				'font-size': numFontSize + 'px', 'color': inputContentCol}"
				@click="onClick(index)" 
			>
				{{item}}
				<view v-if="!item" class="middle-line" :style="{'height': inputCircleWidth + 'px !important',
				'background': (currCursorIndex == index ? inputCircleSelectedCol : inputCircleDefaultCol)}"></view>
			</view>
		</view>
		
		
		<!-- 数字键盘 -->
		<view class="number-keybord-v" :class="[isShowKeyBoard ? 'max-height' : '']">
			<view class="collaspe-v" hover-stay-time="200" hover-start-time="0" hover-class="press-cls" @click="collaspeKeyBoard">
				<image src="../../static/small-arrow-down-o.png"></image>
			</view>
			<view class="number-key-row-v">
				<view class="number-key-item-v right-border" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(1);">1</view>
				<view class="number-key-item-v right-border" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(2);">2</view>
				<view class="number-key-item-v" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(3);">3</view>
			</view>
			<view class="number-key-row-v">
				<view class="number-key-item-v right-border" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(4);">4</view>
				<view class="number-key-item-v right-border" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(5);">5</view>
				<view class="number-key-item-v" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(6);">6</view>
			</view>
			<view class="number-key-row-v">
				<view class="number-key-item-v right-border" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(7);">7</view>
				<view class="number-key-item-v right-border" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(8);">8</view>
				<view class="number-key-item-v" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(9);">9</view>
			</view>
			<view class="number-key-row-v">
				<view class="number-key-item-v right-border fun-cls confirm-txt"  hover-start-time="0" hover-stay-time="200" hover-class="image-press-cls" @click="onInput(-2);">完成</view>
				<view class="number-key-item-v right-border" hover-start-time="0" hover-stay-time="200" hover-class="press-cls" @click="onInput(0);">0</view>
				<view class="number-key-item-v fun-cls" hover-start-time="0" hover-stay-time="200" hover-class="image-press-cls" @click="onInput(-1);">
					<image src="../../static/delete.png"></image>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	export default{
		props:{
			/* 输入框展示类型   1-带边框   2-底部边框  3-无边框，中间横线*/
			inputType: {
				type: Number,
				default: 1
			},
			/*输入框数量*/
			inputCount: {
				type: Number,
				default: 11
			},
			/*输入框间距，单位px*/
			rowSpaceWidth: {
				type: Number,
				default: 0
			},
			inputHeight: {
				type: Number,
				default: 42
			},
			/*输入框边框宽度*/
			inputCircleWidth: {
				type: Number,
				default: 1
			},
			/*输入框圆角*/
			inputCircleRadius: {
				type: Number,
				default: 4
			},
			/*输入内容字体大小*/
			numFontSize: {
				type: Number,
				default: 20
			},
			/*输入框默认边框颜色*/
			inputCircleDefaultCol: {
				type: String,
				default: "#999999"
			},
			/*输入框选中输入时边框颜色*/
			inputCircleSelectedCol: {
				type: String,
				default: "#2E9DFF"
			},
			/*输入框内容颜色*/
			inputContentCol: {
				type: String,
				default: "#333333"
			}
		},
		data(){
			return{
				inputList:[],
				currCursorIndex: 0,//输入光标位置记录
				isShowKeyBoard: false
			}
		},
		created() {
			for(var i = 0;i < this.inputCount; i++){
				this.inputList.push("")
			}
			this.currCursorIndex = 0;
			if(this.inputList.length > 0){
				
			}
		},
		methods:{
			/**
			 * 监听输入框输入
			 * @param {Object} e
			 */
			onInput(_str){
				var _val = _str;
				if(_val!=null&&_val!=undefined){
					if(_val == -1){//删除
						this.inputList[this.currCursorIndex] = "";
						if(this.currCursorIndex > 0){
							this.currCursorIndex --;
						}else if(this.currCursorIndex == 0){
							this.$forceUpdate()
						}
					}else if(_val == -2){
						//完成输入，触发事件
						//收起键盘
						this.isShowKeyBoard = false
						//完成最终输入，判断输入内容完整性
						var _tempStr = this.inputList.join("")
						//触发事件
						this.completeInput(_tempStr);
					}else{
						this.inputList[this.currCursorIndex] = _str;
						//切换光标到下一个
						if(this.currCursorIndex < this.inputList.length - 1){
							//可以切换
							//切换光标
							this.currCursorIndex ++;
						}else{
							if(this.currCursorIndex == this.inputList.length - 1){
								this.$forceUpdate()
							}
							//完成最终输入，判断输入内容完整性
							var _tempStr = this.inputList.join("")
							if(_tempStr.length == this.inputCount){
								//内容完整
								//收起键盘
								this.isShowKeyBoard = false
								//触发事件
								this.completeInput(_tempStr);
							}
						}
					}
				}
			}, 
			/**
			 * 点击切换输入框
			 * @param {Object} e
			 */
			onClick(_index){
				this.currCursorIndex = _index;
				if(!this.isShowKeyBoard){
					this.isShowKeyBoard = true;
				}
			},
			/**
			 * 收起键盘
			 */
			collaspeKeyBoard(){
				this.isShowKeyBoard = false;
			},
			openKeyBoard(){
				this.isShowKeyBoard = true;
			},
			/**
			 * 输入完成触发
			 * @param {Object} _str
			 */
			completeInput(_str){
				this.$emit("completeInput", _str);
			}
		}
	}
</script>

<style scoped lang="scss">
	.wolf-input-main-v{
		display: flex;
		flex-direction: row;
	}
	.item-input-v{
		// background: #FFFFFF;
		border-style: solid;
		display: flex;
		flex: 1;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		font-family: PingFang SC-Bold, PingFang SC;
		font-weight: 520;
	}
	.input-focus-v{
	}
	
	.number-keybord-v{
		position: fixed;
		bottom: 0;
		background-color: #f5f5f5;
		left: 0;
		width: 100%;
		height: 0px;
		transition: height 0.25s ease-in-out;
	}
	.max-height{
		height: 470rpx;
	}
	.number-key-row-v{
		display: flex;
		flex-direction: row;
		border-bottom: 2rpx solid #F1F1F1;
	}
	.number-key-item-v{
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		flex: 1;
		height: 100rpx;
		color: #000000;
		font-size: 48rpx;
		font-weight: 520;
	}
	.right-border{
		border-right: 2rpx solid #F1F1F1;
	}
	.press-cls{
		background-color: #eeeeee;
	}
	.fun-cls{
		background-color: #eeeeee;
	}
	.fun-cls image{
		width: 54rpx;
		height: 54rpx;
	}
	.image-press-cls{
		background-color: #dddddd;
	}
	.collaspe-v{
		border-bottom: 2rpx solid #F1F1F1;
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: center;
	}
	.collaspe-v image{
		width: 60rpx;
		height: 60rpx;
	}
	.confirm-txt{
		font-size: 34rpx !important;
		font-weight: 540 !important;
	}
	
	
	.item-input-v-type-2{
		background: transparent;
		/* border-bottom: 1px solid $wp-input-circle-default-col; */
		border-bottom-style: solid;
		display: flex;
		flex: 1;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		font-family: PingFang SC-Bold, PingFang SC;
		font-weight: 520;
		/* color: $wp-input-content-col; */
	}
	.input-focus-v-type-2{
		/* border-bottom: 1px solid $wp-input-circle-selected-col !important; */
	}
	
	.item-input-v-type-3{
		background: transparent;
		/* border-bottom: 1px solid $wp-input-circle-default-col; */
		display: flex;
		flex: 1;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		font-family: PingFang SC-Bold, PingFang SC;
		font-weight: 520;
		/* color: $wp-input-content-col; */
	}
	.input-focus-v-type-3 .middle-line{
	}
	.middle-line{
		height: 1px;
		width: 50%;
	}
</style>