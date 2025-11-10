<template>
	<view style="width: 100%;height: 100vh;overflow-y: scroll;position: absolute;top: 0;line-height: 64rpx;">
		<top :title="topTitle" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" :rightText="allSelect"
			rightWidth="157rpx" :isleftBack="false" @clickLeft="clickLeft" backgroundColor="#161A26"
			@clickRight="handleCheckAllChange"></top>
		<search-compt :isOnlySearch="true" pal="Please enter the customer name" @searching="searchUser" :zIndex="997"
			v-if="type=='user'"></search-compt>
		<view class="employee_con">
			<view class="employee_ul">
				<view class="employee_li" v-if="nowDeptId&&nowDeptId!=null&&!search" @click.stop="beforeNode">
					<view class="li_left">
						<view class="bumenicons_con t-icon-gongsi" v-if="deptStack.length==0"></view>
						<view class="bumenicons_con t-icon-bumenguanli-fanhuishangji" v-if="deptStack.length>0"></view>
						<view class="bumenicons_con t-icon-bumenguanli-bumentubiao" v-if="deptStack.length>0"></view>
						<view class="li_text">
							{{deptStackStr}}
						</view>
					</view>
				</view>
				<!-- <view class="employee_li">
					<view class="li_left">
						<view class="bumenicons_con t-icon-bumenguanli-fanhuishangji"></view>
						<view class="li_text">
							Fujian Huade Group
						</view>
					</view>
				</view> -->
				<view class="employee_li" v-for="row in orgs" @click.stop="nextNode(row)">
					<view class="li_left">
						<view class="icons_con t-icon-gouxuan" v-if="setSelectIcon(row)&&row.type=='user'"></view>
						<view class="icons_con t-icon-gouxuan" v-if="setSelectIcon(row)&&type=='dept'"
							@click.stop="selectChange(row)"></view>
						<view class="icons_con t-icon-weixuanzhong" v-if="!setSelectIcon(row)&&row.type=='user'"></view>
						<view class="icons_con t-icon-weixuanzhong" v-if="!setSelectIcon(row)&&type=='dept'"
							@click.stop="selectChange(row)"></view>
						<!-- <view class="icons_con t-icon-zuneiyixuan"></view> -->
						<image class="ava_image" :src="row.avatar" mode=""
							v-if="row.type=='user'&&$isNotEmpty(row.avatar)"></image>
						<view class="ava_image" v-if="row.type=='user'&&!$isNotEmpty(row.avatar)">
							{{getShortName(org.name)}}
						</view>
						<view class="bumenicons_con t-icon-bumenguanli-bumentubiao" v-if="row.type=='dept'"></view>
						<view class="li_text">
							{{row.name}}
						</view>
					</view>
					<view class="li_right" v-if="row.type=='dept'">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"></custom-icons>
					</view>
				</view>
			</view>
			<view class="alselect_con_zhanwei" v-if="multiple">

			</view>
			<view class="alselect_con_con" v-if="multiple&&type=='user'">
				<view class="alselect_con">
					<scroll-view class="alselect_ul" scroll-x="true">
						<!-- <view class="alselect_ul"> -->
						<view class="select_li" v-for="item in select">
							<image class="avatar_image" :src="item.avatar" mode=""
								v-if="item.type=='user'&&$isNotEmpty(item.avatar)"></image>
							<view class="avatar_image" v-if="item.type=='user'&&!$isNotEmpty(item.avatar)">
								{{getShortName(item.name)}}
							</view>
						</view>
						<view class="btn_zhanwei">

						</view>
						<!-- </view> -->
					</scroll-view>
					<view class="com_btn_con">
						<view class="com_btn" @click="finishMulSelect">
							Confirm({{select.length}})
						</view>
					</view>
				</view>
			</view>
			<view class="alselect_con_con" v-if="multiple&&type=='dept'">
				<button class="submit_button" @click="finishMulSelect">
					Confirm({{select.length}})
				</button>
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		getOrgTree,
		getUserByName
	} from "@/api/user.js";
	export default {
		name: "employee_select",
		props: {
			topTitle: {
				type: String,
				default: "Select personnel",
			},
			multiple: {
				type: Boolean,
				default: false,
			},
			type: {
				default: "user",
				type: String,
			},
			selected: {
				type: Array,
				default: () => {
					return []
				}
			},
		},
		data() {
			return {
				// topTitle: 'Select personnel', //标题
				// multiple: false, //是否是多选
				visible: false,
				loading: false,
				checkAll: false,
				nowDeptId: null,
				isIndeterminate: false,
				searchUsers: [],
				nodes: [],
				select: [],
				search: "",
				deptStack: [],
				// type: "user", //org选择部门/人员  user-选人  dept-选部门 role-选角色
				deptUserInfo: {

				}
			};
		},
		mounted() {

			this.$nextTick(() => {
				console.log(this.select, '选中的');
				this.selectToLeft()
				this.getOrgList()
			})
		},
		computed: {
			deptStackStr() {
				let str = ''
				let arr = this.deptStack.map((v) => v.name)
				// console.log(arr,'arr');
				str = arr.join('>')
				// return String(this.deptStack.map((v) => v.name)).replaceAll(",", " > ");
				return str
			},
			orgs() {
				return !this.search || this.search.trim() === "" ?
					this.nodes :
					this.searchUsers;
			},
			showUsers() {
				return this.search || this.search.trim() !== "";
			},
			allSelect() {
				console.log("multiple", this.multiple);
				if (this.multiple) {
					if (this.checkAll) {
						return 'empty'
					} else {
						return 'Select all'
					}
				} else {
					return ''
				}
			}
		},
		methods: {
			setSelected(val){
				this.select=JSON.parse(JSON.stringify(val))
			},
			clickLeft() {
				this.$emit('closeSelect')
			},
			setSelectIcon(node) {
				//设置选中，未选中，有子项选中的图标样式
				if (this.type == 'user') {
					if (node.type == 'user') {
						return node.selected
					}
				} else {
					return node.selected
				}

			},
			handleCheckAllChange() {
				//全选事件
				this.checkAll = !this.checkAll
				this.nodes.map((node) => {
					if (this.checkAll) {
						if (!node.selected && !this.disableDept(node)) {
							node.selected = true;
							this.select.push(node);
						}
					} else {
						node.selected = false;
						for (let i = 0; i < this.select.length; i++) {
							if (this.select[i].id === node.id) {
								this.select.splice(i, 1);
								break;
							}
						}
					}
				});
				this.$forceUpdate()
				console.log("列表", this.nodes);
			},
			show(selected, t) {

				this.getOrgList();
			},
			init(selected) {

			},
			disableDept(node) {
				return this.type === "user" && "dept" === node.type;
			},
			nextNode(node) {
				//进入下一级
				if (this.type == 'user' && node.type == 'dept') {
					this.nowDeptId = node.id;
					this.deptStack.push(node);
					this.getOrgList('next');
				} else if (this.type == 'dept' && node.type == 'dept') {
					this.nowDeptId = node.id;
					this.deptStack.push(node);
					this.getOrgList('next');
				} else {
					this.selectChange(node)
				}
			},
			selectChange(node) {
				if (node.selected) {
					this.checkAll = false;
					for (let i = 0; i < this.select.length; i++) {
						if (this.select[i].id === node.id) {
							this.select.splice(i, 1);
							break;
						}
					}
					node.selected = false;
				} else if (!this.disableDept(node)) {
					node.selected = true;
					let nodes = this.search.trim() === "" ? this.nodes : this.searchUsers;
					if (!this.multiple) {
						nodes.map((nd) => {
							if (node.id !== nd.id) {
								nd.selected = false;
							}
						});
					}
					if (node.type === "dept") {
						if (!this.multiple) {
							this.select = [node];
							this.$nextTick(() => {
								let selsee = JSON.parse(JSON.stringify(this.select))
								console.log(selsee, 'selseeselsee');
								// setPagesParam('selectDept', selsee, 1)
								this.$emit('selectDept',selsee)

							})
						} else {
							this.select.unshift(node);
						}
					} else {
						if (!this.multiple) {
							this.select = [node];
							this.$nextTick(() => {
								let selsee = JSON.parse(JSON.stringify(this.select))
								// setPagesParam('selectEmplee', selsee, 1)
								this.$emit('selectEmplee',selsee)
							})
						} else {
							this.select.push(node);
						}
					}
				}
				this.getCheckVal()
			},
			finishMulSelect() {
				//完成多选
				console.log("是这里吗？",this.type,this.type == 'user',this.type == 'dept');
				if (this.type == 'user') {
					// setPagesParam('selectEmplee', this.select, 1)
					this.$emit('selectEmplee',this.select)
				} else if (this.type == 'dept') {
					// setPagesParam('selectDept', this.select, 1)
					this.$emit('selectDept',this.select)
				}
			},
			getOrgList() {
				this.$refs.promptMsg.loadingOpen()
				getOrgTree({
						deptId: this.nowDeptId,
						type: this.type
					})
					.then((rsp) => {
						console.log("企业人员列表", rsp);
						this.$refs.promptMsg.loadingColse()
						this.nodes = rsp.data;
						// for(let i=0;i<5;i++){
						// 	this.nodes=[...this.nodes,...rsp.data]
						// }
						this.selectToLeft()
					})
					.catch((err) => {
						this.$refs.promptMsg.loadingColse()
						this.setMsgTop(err)
					});
			},
			selectToLeft() {
				let nodes = this.search.trim() === "" ? this.nodes : this.searchUsers;
				nodes.map((node) => {
					for (let i = 0; i < this.select.length; i++) {
						if (this.select[i].id === node.id) {
							node.selected = true;
							break;
						} else {
							node.selected = false;
						}
					}
				});
				this.getCheckVal()
			},
			getCheckVal() {
				//设置全选框的值
				console.log();
				if (this.type == 'user') { //员工是否全选
					let noselect = this.orgs.find(row => row.selected === false && row.type === 'user')
					let user = this.orgs.find(row => row.type === 'user')
					if (noselect === undefined && user !== undefined) {
						this.checkAll = true
					} else {
						this.checkAll = false
					}
				} else if (this.type == 'dept') { //部门是否全选
					let noselect = this.orgs.find(row => row.selected === false && row.type === 'dept')
					let dept = this.orgs.find(row => row.type === 'dept')
					if (noselect === undefined && dept !== undefined) {
						this.checkAll = true
					} else {
						this.checkAll = false
					}
				}
			},
			getShortName(name) {
				if (name) {
					return name.length > 2 ? name.substring(1, 3) : name;
				}
				return "**";
			},
			searchUser(val) {
				this.search = val
				let userName = this.search.trim();
				this.searchUsers = [];
				this.loading = true;
				getUserByName({
						realName: userName
					})
					.then((rsp) => {
						this.loading = false;
						this.searchUsers = rsp.data;
						this.selectToLeft();
					})
					.catch((err) => {
						this.loading = false;
						this.$message.error("接口异常");
					});
			},
			beforeNode() {
				if (this.deptStack.length === 0) {
					return;
				}
				if (this.deptStack.length < 2) {
					this.nowDeptId = null;
				} else {
					this.nowDeptId = this.deptStack[this.deptStack.length - 2].id;
				}
				this.deptStack.splice(this.deptStack.length - 1, 1);
				this.getOrgList();
			},
			getEmployeList() {
				//获取员工列表

			},
		}
	}
</script>

<style lang="less">
	.employee_con {
		.alselect_con_zhanwei {
			width: 100%;
			height: 98rpx;
		}

		.alselect_con_con {
			position: fixed;
			bottom: 0;
			left: 0;
			width: 100%;
			height: 98rpx;

			.submit_button {
				border-radius: 0;
			}
		}

		.alselect_con {
			position: relative;
			// width: 100%;
			height: 98rpx;
			background: rgba(28, 34, 50, 1);
			padding: 20rpx;
			box-sizing: border-box;
			display: flex;
			justify-content: space-between;
			align-items: center;

			.alselect_ul {
				white-space: nowrap;
				width: 100%;

				.select_li {
					display: inline-block;
					margin-right: 10rpx;
					width: 60rpx;
					height: 60rpx;

					.avatar_image {
						width: 60rpx;
						height: 60rpx;
						border-radius: 50%;
						background: #fff;
					}
				}

				.btn_zhanwei {
					display: inline-block;
					width: 236rpx;
					height: 68rpx;
				}
			}

			.com_btn_con {
				position: absolute;
				right: 0;
				top: center;
				// width: 100%;
				padding: 15rpx 22rpx 15rpx 22rpx;
				background: rgba(28, 34, 50, 1);
			}

			.com_btn {

				display: flex;
				align-items: center;
				justify-content: center;
				font-size: 28rpx;
				font-weight: 500;
				color: #fff;
				padding: 0 20rpx;
				height: 68rpx;
				background: linear-gradient(180deg, #FF3535 0%, #FF613D 100%);
				border-radius: 10px;
			}
		}

		.employee_ul {
			.employee_li {
				display: flex;
				justify-content: space-between;
				align-items: center;
				color: #fff;
				min-height: 100rpx;
				padding: 20rpx;
				width: 100%;
				box-sizing: border-box;

				.li_left {
					display: flex;
					justify-content: flex-start;
					align-items: center;

					.ava_image {
						width: 60rpx;
						height: 60rpx;
						border-radius: 50%;
						margin-right: 20rpx;
						// background: #fff;
					}

					.icons_con {
						min-width: 32rpx;
						height: 32rpx;
						margin-right: 20rpx;
					}

					.bumenicons_con {
						min-width: 28rpx;
						height: 28rpx;
						margin-right: 16rpx;
					}

					.li_text {
						max-width: calc(100% - 32rpx -20rpx -28rpx -16rpx - 20rpx);
					}
				}
			}
		}
	}
</style>