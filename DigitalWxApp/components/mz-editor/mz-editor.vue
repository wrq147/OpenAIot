<template>
	<view>
		<view class="edtitle border-bottom border-top">
			<navigator url="" @click="addWord" class="editem border-right">
				<uni-icons custom-prefix="my-icon" type="my-icon-wenzi" size="14" color="#333333">
				</uni-icons>
				<text class="edtxt">添加文字</text>
			</navigator>
			<navigator url="" @click="addImage" class="editem border-right">
				<uni-icons custom-prefix="my-icon" type="my-icon-KHCFDC_tupian" size="14" color="#333333">
				</uni-icons>
				<text class="edtxt">添加图片</text>
			</navigator>
			<navigator v-if="urlType?false:true" url="" @click="addVideo" class="editem">
				<uni-icons custom-prefix="my-icon" type="my-icon-shipin" size="14" color="#333333">
				</uni-icons>
				<text class="edtxt">添加视频</text>
			</navigator>
		</view>
		<view class="edcont">
			<template v-for="(it,idx) in list">
				<view v-if="it.type=='text'" class="bkwrap">
					<textarea :maxlength="500" @focus="closeTanchu"
						:style="{'line-height': '36rpx','color':it.attr?JSON.parse(it.attr).colors:'#000000','font-weight':it.attr?(JSON.parse(it.attr).bolds?'bold':'normal'):'normal','font-size':it.attr?JSON.parse(it.attr).zihao*2+'rpx':'28rpx','line-height':it.attr?JSON.parse(it.attr).hanggao*36+'rpx':'36rpx','text-align':it.attr?(JSON.parse(it.attr).duiqi==1?'left':(JSON.parse(it.attr).duiqi==2?'center':'right')):'left'}"
						:auto-height="true" v-model="it.data" @input="delWord($event,idx)" placeholder="请输入文字(最多500字)"
						class="contitem wenzi" />
					<view class="xcontrol">
						<view class="xbtn tanchu" @click="selectSize(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-zihao" size="16" color="#333333">
							</uni-icons>
							<view class="sanjian" style="border-right-color:#333333"></view>
							<view class="sizeList" :style="{'display': activeSize==idx?'flex':'none'}">
								<view class="topSanjiao"></view>
								<view class="sizeLi_con active_con"
									style="background-color: #fff !important;width: 90rpx !important;">
									<view class="sizeLi" style="">字号</view>
								</view>
								<view @click.stop="choiceSize(idx,it,li)"
									:class="['sizeLi_con',it.attr?(JSON.parse(it.attr).zihao==li?'active_con':''):'']"
									v-for="li in zihaoLis">
									<view class="sizeLi" style="">{{li}}</view>
								</view>
							</view>
						</view>
						<view class="xbtn tanchu" @click="setBold(idx,it)"
							:style="{'background': it.attr?(JSON.parse(it.attr).bolds?'#F0F0F0':'#ffffff'):'#FFFFFF'}">
							<uni-icons custom-prefix="my-icon" type="my-icon-a-jiacu105334" size="16" color="#333333">
							</uni-icons>
						</view>
						<view class="xbtn tanchu" @click="selectColor(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-a-zitiyanse105334" size="16"
								color="#333333">
							</uni-icons>
							<view class="sanjian"
								:style="{'border-right-color': it.attr?JSON.parse(it.attr).colors:'#000000'}"></view>
							<view class="colorList"
								:style="{'display': activeColor==idx?'flex':'none','left':-(leftPos+169)+'rpx'}">
								<view class="topSanjiao" :style="{'left':(leftPos+195)+'rpx'}"></view>
								<view class="colorLi_con active_con twoLine"
									style="background-color: #fff !important;width: 90rpx !important;">
									<view style="">颜色</view>
								</view>
								<view
									:class="['colorLi_con',it.attr?(JSON.parse(it.attr).colors==li?'active_con':''):'']"
									:style="{'width':inx==6?'90rpx !important':''}" @click.stop="choiceColor(idx,it,li)"
									v-for="li,inx in colorLis">
									<view class="colorLi" :style="{'background':li}"></view>
								</view>
							</view>
						</view>
						<view class="xbtn tanchu" @click="selectRowHeight(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-a-hangjianju105334" size="16"
								color="#333333">
							</uni-icons>
							<view class="sanjian" style="border-right-color:#333333"></view>
							<view class="sizeList hanggaoList"
								:style="{'display': activeRowHeight==idx?'flex':'none','left':-(leftPos+159+10)+'rpx'}">
								<view class="topSanjiao" :style="{'left':(leftPos+185+10)+'rpx'}"></view>
								<view class="sizeLi_con active_con"
									style="background-color: #fff !important;width: 120rpx !important;">
									<view style="">行间距</view>
								</view>
								<view
									:class="['sizeLi_con',it.attr?(JSON.parse(it.attr).hanggao==li?'active_con':''):'']"
									@click.stop="choiceRowHeight(idx,it,li)" v-for="li in hanggaoLis">
									<view class="sizeLi" style="">{{li}}</view>
								</view>
							</view>
						</view>
						<view class="xbtn tanchu" @click="selectAlign(idx)">
							<uni-icons custom-prefix="my-icon"
								:type="it.attr?(JSON.parse(it.attr).duiqi==1?'my-icon-zuoduiqi':(JSON.parse(it.attr).duiqi==2?'my-icon-juzhongduiqi':'my-icon-youduiqi')):'my-icon-zuoduiqi'"
								size="16" color="#333333">
							</uni-icons>
							<view class="sanjian" style="border-right-color:#333333"></view>
							<view class="alignList"
								:style="{'display': activeAlign==idx?'flex':'none','left':-(leftPos+79)+'rpx'}">
								<view class="topSanjiao" :style="{'left':(leftPos+105)+'rpx'}"></view>
								<view class="alignLi_con active_con"
									style="background-color: #fff !important;width: 90rpx !important;">
									<view style="">对齐</view>
								</view>
								<view class="alignLi_con" @click.stop="choiceAlign(idx,it,li)"
									:style="{'background':it.attr?(JSON.parse(it.attr).duiqi==li?'#F0F0F0':'#ffffff'):(duiqiLis[0]==li?'#F0F0F0':'#ffffff')}"
									v-for="li in duiqiLis">
									<view class="alignLi" style="">
										<uni-icons custom-prefix="my-icon"
											:type="li==1?'my-icon-zuoduiqi':(li==2?'my-icon-juzhongduiqi':'my-icon-youduiqi')"
											size="16" color="#333333">
										</uni-icons>
									</view>
								</view>
							</view>
						</view>
						<navigator url="" class="xbtn" v-if="idx!=0" @click="topItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-huaban" size="16" color="#333333">
							</uni-icons>
						</navigator>
						<navigator url="" class="xbtn" v-if="idx!=0" @click="upItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-huabanbeifen" size="16" color="#333333">
							</uni-icons>
						</navigator>
						<navigator url="" class="xbtn" v-if="(idx+1)!=list.length" @click="downItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-a-huabanbeifen4" size="16" color="#333333">
							</uni-icons>
						</navigator>
						<navigator url="" class="xbtn" style="margin-right: -30rpx;" @click="delItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-a-huabanbeifen5" size="16" color="#333333">
							</uni-icons>
						</navigator>
					</view>
				</view>
				<view v-else-if="it.type=='image'" class="bkwrap" style="text-align: right;">
					<image class="contitem" mode="widthFix" :src="it.data" @click="prevImage(it)"></image>
					<view class="xcontrol">
						<navigator url="" class="xbtn" v-if="idx!=0" @click="topItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-huaban" size="16" color="#333333">
							</uni-icons>
						</navigator>
						<navigator url="" class="xbtn" v-if="idx!=0" @click="upItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-huabanbeifen" size="16" color="#333333">
							</uni-icons>
						</navigator>
						<navigator url="" class="xbtn" v-if="(idx+1)!=list.length" @click="downItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-a-huabanbeifen4" size="16" color="#333333">
							</uni-icons>
						</navigator>
						<navigator url="" class="xbtn" style="margin-right: -30rpx;" @click="delItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-a-huabanbeifen5" size="16" color="#333333">
							</uni-icons>
						</navigator>
					</view>
				</view>
				<view v-else class="bkwrap">
					<video :autoplay="true" :src="it.data" class="contitem"></video>
					<view class="xcontrol">
						<navigator url="" class="xbtn" v-if="idx!=0" @click="topItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-huaban" size="16" color="#333333">
							</uni-icons>
						</navigator>
						<navigator url="" class="xbtn" v-if="idx!=0" @click="upItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-huabanbeifen" size="16" color="#333333">
							</uni-icons>
						</navigator>
						<navigator url="" class="xbtn" v-if="(idx+1)!=list.length" @click="downItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-a-huabanbeifen4" size="16" color="#333333">
							</uni-icons>
						</navigator>
						<navigator url="" class="xbtn" style="margin-right: -30rpx;" @click="delItem(idx)">
							<uni-icons custom-prefix="my-icon" type="my-icon-a-huabanbeifen5" size="16" color="#333333">
							</uni-icons>
						</navigator>
					</view>
				</view>
			</template>
		</view>
		<view class="edbtn">
			<button :class="{'btn':true,'dis':curLen}" @click="saveClick" :disabled="curLen">保存</button>
		</view>
	</view>
</template>

<script>
	import {
		uploadFile
	} from '@/api/file.js'
	import request from '@/common/request.js'

	export default {
		name: "mz-editor",
		props: {
			datalist: {
				type: String,
				default: "",
			},
			urlType: {
				type: String,
				default: ","
			}
		},
		data() {
			return {
				list: [],
				colorLis: ['#000000', '#FF0000', '#FFC000', '#FFFF00', '#92D050', '#00B050', '', '#385723', '#00B0F0',
					'#0070C0', '#002060',
					'#7030A0', '#C00000'
				],
				zihaoLis: [14, 12, 10, 16, 18],
				hanggaoLis: [1, 1.5, 2, 2.5, 3],
				duiqiLis: [1, 2, 3], //1表示左对齐,2表示居中对齐,3表示右对齐
				activeColor: -1,
				activeSize: -1,
				activeRowHeight: -1,
				activeAlign: -1,
				leftPos: 0
			}
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
							if (it.type == "image" && it.data.indexOf("/") == 0) {
								it.data = request.config.baseURL + it.data;
							}
						})
					}
				}
			}
		},
		computed: {
			curLen() {
				return (this.list.length === 0 || (this.list.length === 1 && !this.list[0].data));
			}
		},
		methods: {
			closeTanchu() {
				//获取到输入框的焦点,其他的任意弹出的列表都隐藏
				this.activeColor = -1;
				this.activeSize = -1;
				this.activeRowHeight = -1;
				this.activeAlign = -1;
			},
			selectAlign(idx) {
				//打开选择对齐方式的模块
				if (this.list.length == 1) {
					this.leftPos = 64
				} else {
					this.leftPos = 0
				}
				this.activeColor = -1;
				this.activeSize = -1;
				this.activeRowHeight = -1;
				if (this.activeAlign == idx) {
					this.activeAlign = -1
				} else {
					this.activeAlign = idx
				}
			},
			choiceAlign(idx, item, alignli) {
				//选择对齐方式
				// console.log("选中的对齐方式",idx,item,alignli);
				let attrs1 = {}
				if (item.attr) {
					let q1 = item.attr
					attrs1 = JSON.parse(q1)
				}
				attrs1.duiqi = alignli
				this.list[idx].attr = JSON.stringify(attrs1)
				this.activeAlign = -1
			},
			selectRowHeight(idx) {
				//打开选择行高的模块
				if (this.list.length == 1) {
					this.leftPos = 64
				} else {
					this.leftPos = 0
				}
				this.activeColor = -1;
				this.activeSize = -1;
				this.activeAlign = -1;
				if (this.activeRowHeight == idx) {
					this.activeRowHeight = -1
				} else {
					this.activeRowHeight = idx
				}
			},
			choiceRowHeight(idx, item, rowHeightli) {
				//选择行高
				// console.log("选中的行高大小",idx,item,rowHeightli);
				let attrs1 = {}
				if (item.attr) {
					let q1 = item.attr
					attrs1 = JSON.parse(q1)
				}
				attrs1.hanggao = rowHeightli
				this.list[idx].attr = JSON.stringify(attrs1)
				this.activeRowHeight = -1
			},
			selectSize(idx) {
				//打开选择字号的模块
				this.activeColor = -1;
				this.activeRowHeight = -1;
				this.activeAlign = -1;
				if (this.activeSize == idx) {
					this.activeSize = -1
				} else {
					this.activeSize = idx
				}
			},
			choiceSize(idx, item, sizeli) {
				//选择字体大小
				// console.log("选中的字体大小",idx,item,sizeli);
				let attrs1 = {}
				if (item.attr) {
					let q1 = item.attr
					attrs1 = JSON.parse(q1)
				}
				attrs1.zihao = sizeli
				this.list[idx].attr = JSON.stringify(attrs1)
				this.activeSize = -1
			},
			setBold(idx, item) {
				//设置字体加粗
				this.closeTanchu()
				let attrs = {}
				if (item.attr) {
					let q = item.attr
					attrs = JSON.parse(q)
				}
				attrs.bolds = !attrs.bolds
				this.list[idx].attr = JSON.stringify(attrs)

			},
			selectColor(idx) {
				//打开选择字号的模块
				if (this.list.length == 1) {
					this.leftPos = 64
				} else {
					this.leftPos = 0
				}
				this.activeSize = -1;
				this.activeRowHeight = -1;
				this.activeAlign = -1;
				if (this.activeColor == idx) {
					this.activeColor = -1
				} else {
					this.activeColor = idx
				}
			},
			choiceColor(idx, item, colorli) {
				//选择字体颜色
				// console.log("选中的字体颜色",idx,item,colorli);
				if (colorli == '') {
					return
				}
				let attrs = {}
				if (item.attr) {
					let q = item.attr
					attrs = JSON.parse(q)
				}
				attrs.colors = colorli
				this.list[idx].attr = JSON.stringify(attrs)
				setTimeout(() => {
					this.activeColor = -1
				}, 200)
			},
			delWord(e, idx) {
				this.closeTanchu()
				if (e.detail.keyCode == 8 && e.detail.value.length == 0) {
					delItem(idx);
				}
			},
			addWord() {
				this.closeTanchu()
				let attr = {
					zihao: 14, //字号
					bolds: false, //是否加粗
					colors: '#000000', //字体颜色
					hanggao: 1, //行高
					duiqi: 1 //对齐方式
				}
				this.list.push({
					type: "text",
					data: "",
					attr: JSON.stringify(attr)
				});
				this.$nextTick(() => {
					uni.pageScrollTo({
						scrollTop: 2000000,
						duration: 0
					});
				});
			},
			addImage() {
				this.closeTanchu()
				uni.chooseImage({
					count: 6,
					sizeType: ['compressed'],
					success: async res => {
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFilePaths.length; i++) {

							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								uni.showToast({
									title: '上传图片不能超过10M',
									icon: "none",
									duration: 2000
								});
								return;
							}
							this.list.push({
								type: "image",
								data: res.tempFilePaths[i],
								attr: ''
							});
						}
						this.$nextTick(() => {
							uni.pageScrollTo({
								scrollTop: 2000000,
								duration: 0
							});
						});
					}
				});
			},
			addVideo() {
				this.closeTanchu()
				let _this = this;
				uni.chooseVideo({
					sourceType: ['camera', 'album'],
					success: function(res) {
						_this.list.push({
							type: "video",
							data: res.tempFilePath,
							attr: ''
						});
						_this.$nextTick(() => {
							uni.pageScrollTo({
								scrollTop: 2000000,
								duration: 0
							});
						});
					}
				});
			},
			delItem(idx) {
				this.closeTanchu()
				this.list.splice(idx, 1);
			},
			swapArray(index1, index2) {
				this.list[index1] = this.list.splice(index2, 1, this.list[index1])[0];
			},
			downItem(idx) {
				this.closeTanchu()
				this.swapArray(idx, idx + 1);
			},
			upItem(idx) {
				this.closeTanchu()
				this.swapArray(idx - 1, idx);
			},
			topItem(idx) {
				this.closeTanchu()
				let tmpitem = this.list[idx];
				this.list.splice(idx, 1);
				this.list.unshift(tmpitem);
			},
			prevImage(it) {
				this.closeTanchu()
				uni.previewImage({
					urls: [it.data]
				});
			},
			async uploadFiles(localFiles) {
				let fileList = [];
				for (let i = 0; i < localFiles.length; i++) {
					let rsp = await uploadFile(localFiles[i].value);
					fileList.push({
						id: localFiles[i].id,
						value: rsp
					});
				}
				return fileList;
			},
			async saveClick() {
				this.closeTanchu()
				uni.showLoading({
					title: '数据保存中...'
				});
				try {
					let tmpfiles = [];
					this.list.forEach((x, idx) => {
						if (x.type == 'image' || x.type == 'video') {
							if (x.data.indexOf(request.config.baseURL) != 0) {
								tmpfiles.push({
									id: idx,
									value: x.data
								});
							}

						}
					});

					let files = await this.uploadFiles(tmpfiles);
					// console.log("图片",files);
					files.forEach(x => {
						this.list[x.id].data = x.value;
					});
					this.$emit("Save", JSON.stringify(this.list), () => {
						// uni.hideLoading();
					});
				} catch (e) {
					// uni.hideLoading();
				} finally {
					uni.hideLoading()
				}
			}
		}
	}
</script>

<style lang="scss">
	.edtitle {
		position: fixed;
		top: var(--window-top);
		z-index: 999;
		display: flex;
		background-color: #fff;
		width: 100vw;
		box-sizing: border-box;

		.editem {
			flex: 1;
			display: flex;
			align-items: center;
			justify-content: center;
			height: 100rpx;
			box-sizing: border-box;

			.edtxt {
				margin-left: 6rpx;
			}
		}
	}

	.edcont {
		padding-top: 100rpx;
		padding-bottom: 232rpx;
		z-index: 1;

		.bkwrap {
			display: flex;
			flex-direction: column;
			background-color: #ffffff;
			padding-left: 25rpx;
			padding-right: 25rpx;
			padding-top: 20rpx;

			.contitem {
				width: 100%;
				font-size: 28rpx;

				&.wenzi {
					// line-height: 36rpx;
					min-height: 36rpx;
					padding: 10px 0;
				}
			}

			.xcontrol {
				display: flex;
				justify-content: flex-end;

				.xbtn {
					display: flex;
					height: 60rpx;
					align-items: center;
					padding-left: 25rpx;
					padding-right: 25rpx;

					&.tanchu {
						position: relative;
					}

					.alignList {
						position: absolute;
						// left: -79rpx;
						top: 70rpx;
						width: 240rpx;
						// height: 110rpx;
						background-color: #fff;
						border: 1rpx solid #F9F9F9;
						// padding: 1.5rpx 6.25rpx;
						display: flex;
						flex-direction: row;
						justify-content: flex-start;
						padding: 10rpx 20rpx 10rpx 10rpx;
						box-shadow: 0 -10rpx 10rpx rgba(0, 44, 63, 0.1000);
						border-radius: 10rpx;
						z-index: 999;
						background-color: #fff;

						.topSanjiao {
							// left: 105rpx !important;
						}

						.alignLi_con {
							// width: 25%;
							// height: 33.33%;
							// padding: 2.5rpx 5rpx;
							width: 60rpx;
							height: 60rpx;
							line-height: 60rpx;
							text-align: center;
							box-sizing: border-box;
							z-index: 99;
							opacity: 1;
							border-radius: 10rpx;
						}

						.alignLi {
							z-index: 999;
							opacity: 1;


						}
					}

					.hanggaoList {

						// left: -159rpx !important;
						.topSanjiao {
							// left: 185rpx !important;
						}
					}

					.topSanjiao {
						position: absolute;
						left: 26rpx;
						top: -18rpx;
						width: 0;
						height: 0;
						border-left: 10rpx solid transparent;
						border-right: 20rpx solid transparent;
						border-bottom: 20rpx solid #ffffff;
					}

					.sizeList {
						position: absolute;
						left: 0rpx;
						top: 70rpx;
						// width: 170rpx;
						// height: 110rpx;
						padding: 10rpx 20rpx 10rpx 0;
						background-color: #fff;
						border: 1rpx solid #F9F9F9;
						// padding: 1.5rpx 6.25rpx;
						display: flex;
						flex-direction: row;
						justify-content: flex-start;
						z-index: 99;
						box-shadow: 0 -10rpx 10rpx rgba(0, 44, 63, 0.1000);
						border-radius: 10rpx;



						.sizeLi_con {
							// width: 25%;
							// height: 33.33%;
							// padding: 2.5rpx 5rpx;
							width: 60rpx;
							height: 60rpx;
							line-height: 60rpx;
							text-align: center;
							box-sizing: border-box;
							// padding-left: 10rpx;
							box-sizing: border-box;
							z-index: 999;
							opacity: 1;
							border-radius: 10rpx;
							background-color: #fff;
							color: rgba(153, 153, 153, 1);

						}



						.sizeLi {
							// width: 30rpx;
							// height: 30rpx;
							z-index: 999;
							opacity: 1;

						}
					}

					.colorList {
						position: absolute;
						// left: -169rpx;
						top: 70rpx;
						width: 450rpx;
						height: 120rpx;
						background-color: #fff;
						border: 1rpx solid #F9F9F9;
						// box-shadow: 1px 1px 1px 1px #F9F9F9;
						// padding: 1.5rpx 6.25rpx;
						display: flex;
						flex-direction: row;
						justify-content: flex-start;
						align-items: center;
						flex-wrap: wrap;
						z-index: 99;
						padding: 10rpx 20rpx 10rpx 0;
						box-shadow: 0 -10rpx 10rpx rgba(0, 44, 63, 0.1000);
						border-radius: 10rpx;
						opacity: 1;

						.topSanjiao {
							// left: 195rpx !important;
						}

						.colorLi_con {
							width: 60rpx;
							height: 60rpx;
							// padding:5rpx;
							display: flex;
							justify-content: center;
							align-items: center;
							box-sizing: border-box;
							z-index: 999;
							opacity: 1;
							border-radius: 10rpx;

							.colorLi {
								width: 36rpx;
								height: 36rpx;
								border-radius: 18rpx;
								z-index: 999;
								opacity: 1;
							}
						}


					}

					.sanjian {
						position: absolute;
						left: 55rpx;
						bottom: 16rpx;
						width: 0;
						height: 0;
						border-right: 10rpx solid #50A6FA;
						border-top: 10rpx solid transparent;
						// border-bottom: 10rpx solid transparent;

					}

					.active_con {
						background-color: #f0f0f0 !important;
						color: rgba(51, 51, 51, 1) !important;
						font-weight: 700;
					}
				}

			}
		}
	}

	.edbtn {
		position: fixed;
		bottom: 0;
		background-color: #fff;
		width: 100vw;
		z-index: 9;

		.btn {
			margin: 20rpx 30rpx;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 8rpx;
			color: #ffffff;

			&.dis {
				background: #C1E1FF !important;
			}
		}
	}
</style>
