<template>
	<view class="index avatar_con">
		<view class="avatar-box" :style="{ 'background-color': bgColor }">
			<canvas class="avatar-canvas" :style="{ width: `${aWidth}px`, height: `${aHeight}px` }" canvas-id="avatar"
				id="avatar"></canvas>
			<view class="avatar-btn-box">
				<text class="iconface avatar-icon" @click="randAvatarDelay">&#xe630;</text>
				<text class="iconface avatar-icon" @click="saveImg">&#xe61c;</text>
			</view>
		</view>

		<scroll-view scroll-y="true" class="part-box" :style="{ height: `${sys.windowHeight - 300}px` }">
			<view class="part-title"><text>颜色</text></view>
			<view class="color-box">
				<template v-for="(color, index) in colors">
					<view :key="index" class="color-item" @click="colorOpen(index)">
						<text class="color-item-name">{{ color.name }}</text>
						<view class="color-item-point"
							:style="{ 'background-color': colors[index]['list'][group[index]] }"></view>
					</view>
				</template>
				<view class="color-pop" v-if="selectColor.name">
					<view class="color-pop-bar">
						<text class="color-pop-title">{{ selectColor.name }}</text>
						<text class="iconface close-icon" @click="colorClose()">&#xe600;</text>
					</view>
					<view class="color-pop-list">
						<template v-for="(item, index) in selectColor.list">
							<view :key="index" class="color-item-point" :style="{ 'background-color': item }"
								@click="changeColor(index)"></view>
						</template>
					</view>
				</view>
			</view>

			<template v-for="(part, index) in parts">
				<view :key="index" v-if="!part.hide">
					<view class="part-title">
						<text>{{ part.name }}</text>
						<view class="flex-row align-items-center">
							<text class="iconface part-icon" @click="randOne(index)">&#xe630;</text>
							<text class="iconface part-icon" @click="setLock(index)" v-if="part.lock">&#xe613;</text>
							<text class="iconface part-icon" @click="setLock(index)" v-else>&#xe704;</text>
						</view>
					</view>
					<scroll-view scroll-x="true" class="part-imgs">
						<template v-for="(img, index1) in part.imgs">
							<image v-if="img !== 'x'" :key="index1" mode="aspectFit" class="part-imgs-item"
								:src="requireImg(`${part.path}${img}`)" @click="changePart(index, index1)"></image>
							<image v-else :key="index1" class="part-imgs-item" @click="changePart(index, index1)">
							</image>
						</template>
					</scroll-view>
				</view>
			</template>
		</scroll-view>
	</view>
</template>
<script>
	import {
		parts,
		colors,
		shape
	} from './conf.js';
	//import { base64ToPath,pathToBase64 } from './img64.js';
	import func from './func.js';

	export default {
		props: {
			avatar: {
				type: Array,
				default: () => [],
			},
		},
		data() {
			return {
				timer: null,
				sys: {
					windowHeight: 0
				},
				parts: [],
				colors: [],
				ctx: null,
				bgColor: '#ffffff',
				aWidth: 280,
				aHeight: 280,
				group: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
				selectColor: {
					index: 0,
					name: '',
					list: []
				},
				skinData: [],
				colorIndex: 0,
				sn: 0,
				saveFile: ''
			};
		},
		computed: {
			pointColor() {
				// return function(index) {
				// 	if (!this.group[index]) return '';
				// 	const arr = this.colors[index]['list'];
				// 	return `background-color:${arr[this.group[index]]}`;
				// };
				return function(index1, index2) {
					if (!index2) return '';
					const arr = this.colors[index1]['list'];
					return `background-color:${arr[index2]}`;
				};
			}
		},
		async created() {
			// #ifdef MP-WEIXIN
			this.fsm = uni.getFileSystemManager();
			await this.removeSave();
			// #endif
			this.sys = uni.getSystemInfoSync();
			this.parts = parts;
			this.colors = colors;
			this.ctx = uni.createCanvasContext('avatar', this);
			if (this.avatar.length > 0) {
				this.group = JSON.parse(JSON.stringify(this.avatar));
				await this.createAvatar();
			} else {
				this.randAvatar();
			}


		},
		async destroyed() {
			// #ifdef MP-WEIXIN
			await this.removeSave();
			// #endif
		},
		methods: {
			//保存图片
			saveImg() {
				const that = this;
				uni.canvasToTempFilePath({
						x: 0,
						y: 0,
						width: that.aWidth,
						height: that.aHeight,
						canvasId: 'avatar',
						success: function(res) {
							that.$emit('getAvatar', {
								img: res.tempFilePath,
								group: that.group
							});
						},
						fail: res => {
							console.log(res);
						}
					},
					this
				);
			},
			//获取图片像素
			getImgData(name, x, y, width, height) {
				return new Promise(function(resolve) {
					uni.canvasGetImageData({
						canvasId: name,
						x: 0,
						y: 0,
						width,
						height,
						success(res) {
							resolve(res.data);
						}
					});
				});
			},
			getImgInfo(img) {
				const that = this;
				return new Promise(function(resolve) {
					// #ifdef MP-WEIXIN
					const filePath = `${wx.env.USER_DATA_PATH}/sharp_${that.sn}.png`;
					that.sn += 1;
					that.fsm.writeFile({
						filePath: filePath, // 要写入的文件路径 (本地路径)
						data: img, // 要写入的文本或二进制数据
						encoding: 'base64', // 指定写入文件的字符编码
						success() {
							resolve(filePath);
						}
					});

					// #endif
					// #ifndef MP-WEIXIN
					uni.getImageInfo({
						src: img,
						success: function(image) {
							resolve(image.path);
						},
						fail: e => {
							console.log(e);
						}
					});
					// #endif
				});
			},
			removeSave() {
				return new Promise(resolve => {
					const fsm = uni.getFileSystemManager();
					fsm.readdir({
						dirPath: wx.env.USER_DATA_PATH, // 当时写入的文件夹
						success(res) {
							res.files.forEach(el => {
								if (el !== 'miniprogramLog') {
									// 过滤掉miniprogramLog
									fsm.unlink({
										filePath: `${wx.env.USER_DATA_PATH}/${el}`, // 文件夹也要加上，如果直接文件名会无法找到这个文件
										fail(e) {
											console.log('readdir文件删除失败：', e);
										}
									});
								}
							});
							resolve();
						}
					});
				});
			},
			//延迟随机头像
			randAvatarDelay() {
				// #ifdef MP-WEIXIN
				this.randAvatar();
				// #endif
				// #ifndef MP-WEIXIN
				let limit = 5;
				clearInterval(this.timer);
				this.timer = setInterval(() => {
					limit -= 1;
					this.randAvatar();
					if (limit <= 0) {
						clearInterval(this.timer);
					}
				}, 80);
				// #endif
			},
			//改变背景
			async changeBg(n) {
				let color = this.colors[shape.bg_color]['list'][n];
				this.bgColor = color;
				this.ctx.setFillStyle(color);
				this.ctx.fillRect(0, 0, this.aWidth, this.aHeight);
			},
			//改变肤色
			async changeSkin(n) {
				console.log(n,'肤色');
				let color = this.colors[shape.skin_color]['list'][n];
				this.skinData = await this.getColorChangeData(color);
				await this.putImageData(this.skinData);
			},
			//随机头像
			async randAvatar() {
				for (let i = 0, len = this.colors.length; i < len; i++) {
					this.group[i] = func.randArrIndex(this.colors[i]['list']);
				}

				for (let j = 0, len = this.parts.length; j < len; j++) {
					if (this.parts[j]['lock'] !== true) {
						this.group[j + 2] = func.randArrIndex(this.parts[j]['imgs']);
					}
				}
				await this.createAvatar();
			},
			//随机改变一个
			async randOne(index) {
				this.group[index + 2] = func.randArrIndex(this.parts[index]['imgs']);
				await this.createAvatar();
			},
			//推入图片数据
			putImageData(imgData) {
				return new Promise(() => {
					uni.canvasPutImageData({
						canvasId: 'avatar',
						x: 0,
						y: 0,
						width: this.aWidth,
						height: this.aHeight,
						data: imgData
					});
				});
			},
			async createAvatar() {
				//背景
				let color = this.colors[shape.bg_color]['list'][this.group[0]];
				this.bgColor = color;
				this.ctx.setFillStyle(color);
				this.ctx.fillRect(0, 0, this.aWidth, this.aHeight);
				//身体
				// #ifdef MP-WEIXIN
				let img = await this.getImgInfo(await this.importImg(`body/body${this.group[1]}.png`));
				// #endif
				// #ifndef MP-WEIXIN
				let img = await this.getImgInfo(this.requireImg(`body/body${this.group[1]}.png`));
				// #endif
				this.ctx.drawImage(img, 0, 0, this.aWidth, this.aHeight);
				for (let j = 0, len = this.parts.length; j < len; j++) {
					let src = this.parts[j]['imgs'][this.group[j + 2]];
					if (src !== 'x') {
						// #ifdef MP-WEIXIN
						img = await this.getImgInfo(await this.importImg(`${this.parts[j].path}${src}`));
						// #endif
						// #ifndef MP-WEIXIN
						img = await this.getImgInfo(this.requireImg(`${this.parts[j].path}${src}`));
						// #endif

						if (!img) return;
						this.ctx.drawImage(img, this.parts[j].x, this.parts[j].y, this.parts[j].width, this.parts[j]
							.height);
					}
				}
				this.ctx.draw();
				// const that = this;
				// this.ctx.draw(false, () => {
				// 	uni.canvasToTempFilePath(
				// 		{
				// 			canvasId: 'avatar',
				// 			success: function(res) {
				// 				console.log(res);
				// 			},
				// 			fail: function(res) {
				// 				console.log(res);
				// 			}
				// 		},
				// 		this
				// 	);
				// });
			},
			//获取图片
			requireImg(img) {
				return require(`../../static/img/${img}`);
			},
			//获取图片
			importImg(img) {
				const that = this;
				return new Promise(resolve => {
					that.fsm.readFile({
						filePath: `/uni_modules/butong-avatar/static/img/${img}`,
						encoding: 'base64',
						success(res) {
							resolve(res.data);
						}
					});
				});
			},
			//打开颜色列表
			colorOpen(index) {
				this.colorClose();
				this.selectColor.index = index;
				this.selectColor.name = this.colors[index].name;
				for (let i = 0; i < this.colors[index].list.length; i++) {
					this.selectColor.list.push(this.colors[index].list[i]);
				}
			},
			//关闭颜色列表
			colorClose() {
				this.selectColor.name = '';
				this.selectColor.list.length = 0;
			},
			//改变颜色
			async changeColor(index) {
				this.$set(this.group, this.selectColor.index, index);
				await this.createAvatar();
			},
			//改变形状
			async changePart(index, index1) {
				this.$set(this.group, index + 2, index1);
				await this.createAvatar();
			},
			//关闭
			setLock(index) {
				this.$set(this.parts[index], 'lock', !this.parts[index]['lock']);
			}
		}
	};
</script>
<style lang="scss" scoped>
	@font-face {
		font-family: 'iconface';
		src: url('../../static/css/iconfont.ttf');
	}

	.avatar_con {
		position: fixed;
		top: 100%;
		left: 100%;
	}

	@import '../../static/css/common.scss';
</style>